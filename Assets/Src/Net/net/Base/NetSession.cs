using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;

namespace NetBase
{
    public struct NetUserData
    {
        public UInt16 int_value;
        public Object obj_value;
    }

    public class NetSession
    {
        protected SessionState m_state;
        protected Socket m_socket;
        protected NetBuffer m_recv_buf;
        protected NetBuffer m_send_buf;
        protected EventData m_event_data;
        protected UInt32 m_user_data;
        protected ListNode<NetSession> m_lur_node;

        public NetSession()
        {
            m_state = SessionState.SESSION_STATE_NULL;
            m_socket = null;
            m_recv_buf = null;
            m_send_buf = null;
            m_event_data = null;
            m_user_data = 0;
            m_lur_node = null;
        }

        ~NetSession()
        {
        }

        public int Init(Socket sock, EventData event_data)
        {
            if (sock == null)
            {
                Log.Error("Session initial sock is null");
                goto Exit1;
            }
            if (event_data == null)
            {
                Log.Error("Session initial event_data is null");
                goto Exit1;
            }
            m_socket = sock;
            m_event_data = event_data;
            m_state = SessionState.SESSION_STATE_WORKING;
            m_lur_node = new ListNode<NetSession>(this);
            if (m_lur_node == null)
            {
                goto Exit1;
            }
            m_recv_buf = new NetBuffer();
            if (m_recv_buf == null)
            {
                Log.Error("new recv buffer is null");
                goto Exit1;
            }
            if (m_recv_buf.Init((int)NetBufferSize.RECV_BUFFER_SIZE) < 0)
            {
                Log.Error("init recv buffer is null");
                goto Exit1;
            }
            m_send_buf = new NetBuffer();
            if (m_send_buf == null)
            {
                Log.Error("new send buffer is null");
                goto Exit1;
            }
            if (m_send_buf.Init((int)NetBufferSize.SEND_BUFFER_SIZE) < 0)
            {
                Log.Error("init send buffer is null");
                goto Exit1;
            }
            return 0;

        Exit1:
            return -1;
        }

        public void Release()
        {
            CloseSocket();
            m_event_data = null;
            m_state = SessionState.SESSION_STATE_NULL;
            if (m_recv_buf != null)
            {
                m_recv_buf.Release();
                m_recv_buf = null;
            }
            if (m_send_buf != null)
            {
                m_send_buf.Release();
                m_send_buf = null;
            }
            m_lur_node = null;
        }

        public int Shutdown()
        {
            if (m_socket != null && IsSendBufferEmpty())
            {
                m_socket.Shutdown(SocketShutdown.Send);
            }
            m_state = SessionState.SESSION_STATE_SHUTDOWN;
            return 0;
        }

        public ListNode<NetSession> GetLRUNode()
        {
            return m_lur_node;
        }

        public int Send(byte[] buffer, int offset, int lenght)
        {
            int ret = 0;
            if (lenght > 0)
            {
                m_send_buf.Write(buffer, offset, lenght);
            }
            if (lenght == 0 || m_send_buf.GetOffset() > (int)NetThreshold.NET_SEND_THRESHOLD)
            {
                try
                {
                    ret = m_socket.Send(m_send_buf.GetBuffer(), 0, m_send_buf.GetOffset(), SocketFlags.None);
                    if (ret > 0)
                    {
                        m_send_buf.Read(ret);
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode != (int)SocketError.WouldBlock && e.ErrorCode != (int)SocketError.InProgress)
                    {
                        if (GameManager.GM.IsLogined)
                        {       
                            GameManager.GM.DS.Notic = GameManager.GM.PopUI(ResPath.Notic);
                            GameManager.GM.DS.Notic.GetComponent<UI_Notic>().SetMessage("客户端与服务端已断开连接,正在重新连接！超时6秒将关闭客户端！");
                            GameManager.GM.DS.Notic.GetComponent<UI_Notic>().QuitApp();
                            GameManager.GM.ReLogin();
                        }
                        //Log.Error("socket send error:{0}", e);
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        public int Recv()
        {
            // If keep reading while no data available, will show very low QPS statistic
            if (!m_socket.Connected)
            {
                return -1;
            }
            else
            {
                try
                {
                    int numAvail = m_socket.Available;
                    if (numAvail <= 0)
                    {
                        return 0;
                    }
                }
                catch
                {
                    return -2;
                }
            }

            int ret = 0;
            m_recv_buf.Resize();
            try
            {
                ret = m_socket.Receive(m_recv_buf.GetBuffer(), m_recv_buf.GetOffset(), m_recv_buf.GetSpace(), SocketFlags.None);
                if (ret > 0)
                {
                    m_recv_buf.Write(ret);   // success
                    return 0;
                }
                else
                {
                    if (ret == 0)
                    {
                        return 1;          // closed
                    }
                    else
                    {
                        return -1;         // error
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != (int)SocketError.WouldBlock && e.ErrorCode != (int)SocketError.InProgress)
                {
                    Log.Error("socket receive error:{0}", e);
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int CloseSocket()
        {
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
            return 0;
        }

        public bool IsWorking()
        {
            return m_state == SessionState.SESSION_STATE_WORKING;
        }

        public bool IsSendBufferEmpty()
        {
            return m_send_buf.GetOffset() == 0;
        }

        public Socket GetSocket()
        {
            return m_socket;
        }

        public EventData GetEventData()
        {
            return m_event_data;
        }

        public void GetRecvData(ref byte[] buf, ref int len)
        {
            buf = m_recv_buf.GetBuffer();
            len = m_recv_buf.GetOffset();
        }

        public void ReadRecvData(int len)
        {
            m_recv_buf.Read(len);
        }

        public UInt32 GetUserData()
        {
            return m_user_data;
        }

        public void SetUserData(UInt32 data)
        {
            m_user_data = data;
        }
    }
}
