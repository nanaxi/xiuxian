//using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetBase
{
    public class NetFrame 
    {
        private NetModule m_module;
        private NetCallback m_callback;
        private NetHandleLink m_link_handle;
        private NetHandleLinked m_linked_handle;
        private DoubleList<NetSession> m_session_lru;

        public NetFrame()
        {
            m_module = null;
            m_callback = null;
            m_link_handle = null;
            m_linked_handle = null;
            m_session_lru = null;
        }

        ~NetFrame()
        {
        }

        public int Init(NetCallback callback) 
        {
            if (callback == null) 
            {
                Log.Error("net frame initial callback is null");
                goto Exit1;
            }
            m_callback = callback;
            m_session_lru = new DoubleList<NetSession>();
            if (m_session_lru == null) 
            {
                goto Exit1;
            }
            m_module = new NetModule();
            if (m_module == null) 
            {
                Log.Error("net frame module is null");
                goto Exit1;
            }
            if (m_module.Init() < 0) 
            {
                Log.Error("net frame module initial failed");
                goto Exit1;
            }
            m_linked_handle = new NetHandleLinked();
            if (m_linked_handle == null) 
            {
                Log.Error("net frame linked handle is null");
                goto Exit1;
            }
            if (m_linked_handle.Init(m_module, m_callback, m_session_lru) < 0) 
            {
                Log.Error("net frame linked handle initial failed");
                goto Exit1;
            }
            m_link_handle = new NetHandleLink();
            if (m_link_handle == null) 
            {
                Log.Error("net frame link handle is null");
                goto Exit1;
            }
            if (m_link_handle.Init(m_module, m_callback, m_linked_handle, m_session_lru) < 0) 
            {
                Log.Error("net frame link handle initial failed");
                goto Exit1;
            }
            Log.Info("network initial success");
            return 0;

        Exit1:
            return -1;
        }

        public void Shutdown() 
        {
            if (m_module != null) {
                m_module.Shutdown();
                m_module = null;
            }
            if (m_linked_handle != null) 
            {
                m_linked_handle.Shutdown();
                m_linked_handle = null;
            }
            if (m_link_handle != null) 
            {
                m_link_handle.Shutdown();
                m_link_handle = null;
            }
            m_session_lru = null;
            m_callback = null;
        }

        public int Loop(int tm) 
        {
            m_module.Loop(tm);
            return 0;
        }

        public int Close(NetSession session) 
        {
            session.Shutdown();
            return 0;
        }

        public int Connect(string ip, UInt16 port, UInt16 flag) 
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (sock == null) 
            {
                goto Exit1;
            }
            sock.Blocking = false; 
            sock.NoDelay = true;
            IPAddress addr;
            if (IPAddress.TryParse(ip, out addr) == false) 
            {
                return -1;
            }
            try 
            {
                sock.Connect(addr, port);
            }
            catch (SocketException e) 
            {
                if (e.ErrorCode != (int)SocketError.WouldBlock && e.ErrorCode != (int)SocketError.InProgress)
                {
                    Log.Info("connect addr({0}:{1}) failed err:{2}", ip, port, e);
                    goto Exit1;
                }
            }
            EventData data = new EventData();
            data.m_flag = flag;
            data.m_handle = m_link_handle;
            data.m_socket_flag = SocketFlag.SOCKET_CONNECT;
            m_module.AddConnectingSock(sock, data);
            m_module.AddEvent(sock, 0, data);
            return 0;

        Exit1:
            m_callback.OnConnect(null, flag, false);
            if (sock != null) 
            {
                sock = null;
            }
            return -1;
        }
        
        public int Send(NetSession session, byte[] buffer, int lenght) 
        {
            if (session == null ||
                lenght > (int)NetPackLenght.MAX_NET_PACK_LENGTH) 
            {
                return -1;
            }
            NetSession s = (NetSession)session;
            if (!s.IsWorking()) 
            {
                return -1;
            }
            int ret = s.Send(buffer, 0, lenght);
            if (ret < 0) 
            {
                return -1;
            }
            EventFlag flag = s.IsSendBufferEmpty() ? EventFlag.EVENT_READ : EventFlag.EVENT_RANDW;
            m_module.ModEvent(s.GetSocket(), flag, s.GetEventData());
            return 0;
        }
    }
}
