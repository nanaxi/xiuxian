using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetBase
{
    public interface NetHandle 
    {
        int Readable(Socket sock, EventData data);
        int Writeable(Socket sock, EventData data);
        int Error(Socket sock, EventData data);
    }

    public class NetHandleLink : NetHandle 
    {
        protected NetModule m_module;
        protected NetHandle m_handle_linked;
        protected NetCallback m_callback;
        protected DoubleList<NetSession> m_session_lru;

        public NetHandleLink()
        {
            m_module = null;
            m_handle_linked = null;
            m_callback = null;
            m_session_lru = null;
        }

        ~NetHandleLink()
        {
        }

        public int Init(NetModule module, NetCallback callback, NetHandle handle_linked, DoubleList<NetSession> session_lru) 
        {
            if (module == null) 
            {
                Log.Error("module is null");
                return -1;
            }
            if (callback == null) 
            {
                Log.Error("callback is null");
                return -1;
            }
            if (handle_linked == null) 
            {
                Log.Error("handle_linked is null");
                return -1;
            }
            m_module = module;
            m_callback = callback;
            m_handle_linked = handle_linked;
            m_session_lru = session_lru;
            return 0;
        }

        public void Shutdown() 
        {
        }

        public int Readable(Socket sock, EventData data)
        {
            return 0;
        }

        public int Writeable(Socket sock, EventData data)
        {
            NetSession session = null;
            if (!sock.Connected) 
            {
                goto Exit1;
            }
            session = new NetSession();
            if (session == null) 
            {
                goto Exit1;
            }
            if (session.Init(sock, data) < 0) 
            {
                goto Exit1;
            }
            data.m_session = session;
            data.m_handle = m_handle_linked;
            data.m_socket_flag = SocketFlag.SOCKET_LINKED;            
            m_module.ModEvent(sock, EventFlag.EVENT_READ, data);
            if (m_callback != null) 
            {
                m_callback.OnConnect(session, data.m_flag, true);
            }
            return 0;

        Exit1:
            if (m_callback != null) 
            {
                m_callback.OnConnect(null, data.m_flag, false);
            }
            if (sock != null) 
            {
                m_module.DelEvent(sock);
                sock.Close();
                sock = null;
            }
            return -1;
        }

        public int Error(Socket sock, EventData data)
        {
            if (data.m_socket_flag == SocketFlag.SOCKET_CONNECT) 
            {
                if (m_callback != null) 
                {
                    m_callback.OnConnect(null, data.m_flag, false);
                }
                m_module.DelEvent(sock);
                sock.Close();
            }
            return 0;
        }
    }

    public class NetHandleLinked : NetHandle 
    {
        protected int m_head_len;
        protected IntPtr m_pack_head;
        protected NetModule m_module;
        protected NetCallback m_callback;
        protected DoubleList<NetSession> m_session_lru;

        public NetHandleLinked()
        {
            m_head_len = 0;
            m_pack_head = IntPtr.Zero;
            m_module = null;
            m_callback = null;
            m_session_lru = null;
        }

        ~NetHandleLinked()
        {
        }

        public int Init(NetModule module, NetCallback callback, DoubleList<NetSession> session_lru) 
        {
            if (module == null) 
            {
                Log.Error("module is null");
                return -1;
            }
            if (callback == null) 
            {
                Log.Error("callback is null");
                return -1;
            }
            m_module    = module;
            m_callback  = callback;
            m_session_lru = session_lru;

            NetPack net_pack;
            net_pack.length = 0;
            net_pack.cmdAndFlag = 0;
            m_head_len  = Marshal.SizeOf(net_pack);
            m_pack_head = Marshal.AllocHGlobal(m_head_len);
            if (m_pack_head == IntPtr.Zero) 
            {
                return -1;
            }
            return 0;
        }

        public void Shutdown() 
        {
            if (m_pack_head != IntPtr.Zero) 
            {
                Marshal.FreeHGlobal(m_pack_head);
            }
        }

        public int Readable(Socket sock, EventData data) 
        {
            NetSession session = data.m_session;
            int ret = session.Recv();
            if (ret != 0) 
            {
                SessionCloseFlag flag = (ret == 1) ?
                                        SessionCloseFlag.CLOSE_FLAG_PEER_CLOSE :
                                        SessionCloseFlag.CLOSE_FLAG_ERROR;
                ErrorSession(session, flag);
                return -1;
            }
            if (HandlePackage(session) < 0) 
            {
                ErrorSession(session, SessionCloseFlag.CLOSE_FLAG_ERROR);
                return -1;
            }
            return 0;
        }

        public int Writeable(Socket sock, EventData data) 
        {
            NetSession session = data.m_session;
            int ret = session.Send(null, 0, 0);
            if (ret < 0) 
            {
                ErrorSession(session, SessionCloseFlag.CLOSE_FLAG_ERROR);
                return -1;
            }
            if (session.IsSendBufferEmpty()) 
            {
                m_module.ModEvent(sock, EventFlag.EVENT_READ, data);
                if (!session.IsWorking()) 
                {
                    session.Shutdown();
                }
            }
            return 0;
        }

        public int Error(Socket sock, EventData data) 
        {
            NetSession session = data.m_session;
            ErrorSession(session, SessionCloseFlag.CLOSE_FLAG_ERROR);
            return 0;
        }

        public void CloseSession(NetSession session) 
        {
            m_module.DelEvent(session.GetSocket());
            session.Release();
            session = null;
        }

        protected int HandlePackage(NetSession session) 
        {
            byte[] data  = null;
            int data_len = 0;
            int pack_len = 0;
            int handle_len = 0;
            NetPack net_pack;
            session.GetRecvData(ref data, ref data_len);
            if (m_callback != null && session.IsWorking()) 
            {
                while (true) 
                {
                    if (data_len >= m_head_len) 
                    {
                        Marshal.Copy(data, handle_len, m_pack_head, m_head_len);
                        net_pack = (NetPack)Marshal.PtrToStructure(m_pack_head, typeof(NetPack));
                        pack_len = (int)net_pack.GetLength();
                        if (pack_len > (UInt16)NetPackLenght.MAX_NET_PACK_LENGTH) 
                        {
                            Log.Error("recv pack length {0} out of range {1}", pack_len, NetPackLenght.MAX_NET_PACK_LENGTH);
                            return -1;
                        }
                        if (data_len >= pack_len) 
                        {
                            m_callback.OnRecv(ref net_pack, data, (handle_len + m_head_len), pack_len - m_head_len, session);
                            data_len   -= pack_len;
                            handle_len += pack_len;
                        } 
                        else 
                        {
                            break;
                        }
                    } 
                    else 
                    {
                        break;
                    }
                    if (!session.IsWorking())
                    {
                        break;
                    }
                }
            } 
            else 
            {
                handle_len = data_len;
            }
            session.ReadRecvData(handle_len);
            return 0;
        }

        protected void ErrorSession(NetSession session, SessionCloseFlag flag) 
        {
            if (m_callback != null && session.IsWorking()) 
            {
                m_callback.OnClose(session, flag);
            }
            CloseSession(session);
        }
    }
}
