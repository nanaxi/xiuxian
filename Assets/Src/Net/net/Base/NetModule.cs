using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;


namespace NetBase 
{
    public class NetModule 
    {
        protected List<Socket> m_read_event;
        protected List<Socket> m_write_event;
        protected Dictionary<Socket, EventData> m_event_map;
        protected Dictionary<Socket, EventData> m_connecting_map;

        public NetModule()
        {
            m_read_event = null;
            m_write_event = null;
            m_event_map = null;
        }
        
        ~NetModule()
        {
        }

        public int Init() 
        {
            m_read_event  = new List<Socket>();
            m_write_event = new List<Socket>();
            m_event_map   = new Dictionary<Socket, EventData>();
            m_connecting_map = new Dictionary<Socket, EventData>();
            return 0;
        }

        public void Shutdown() 
        {
            m_read_event  = null;
            m_write_event = null;
            m_event_map   = null;
        }

        public int  Loop(int wait_tm) 
        {
            if (m_connecting_map.Count > 0) 
            {
                HandleConnecting();
            }
            if (m_event_map.Count > 0) 
            {
                HandleIO(true, true);
            }
            return 0;
        }

        public int AddEvent(Socket sock, EventFlag flag, EventData data) 
        {
            if ((flag & EventFlag.EVENT_READ) != 0) 
            {
                m_read_event.Add(sock);
            }
            if ((flag & EventFlag.EVENT_WRITE) != 0) 
            {
                m_write_event.Add(sock);
            }
            m_event_map.Add(sock, data);
            data.m_event_flag = flag;
            return 0;
        }

        public int ModEvent(Socket sock, EventFlag flag, EventData data) 
        {
            m_read_event.Remove(sock);
            m_write_event.Remove(sock);
            if ((flag & EventFlag.EVENT_READ) != 0) 
            {
                m_read_event.Add(sock);
            }
            if ((flag & EventFlag.EVENT_WRITE) != 0) 
            {
                m_write_event.Add(sock);
            }
            data.m_event_flag = flag;            
            return 0;
        }

        public int DelEvent(Socket sock) 
        {
            m_event_map.Remove(sock);
            m_write_event.Remove(sock);
            m_read_event.Remove(sock);
            return 0;
        }

        public void AddConnectingSock(Socket sock, EventData ed) 
        {
            m_connecting_map.Add(sock, ed);
        }

        public void DelConnectingSock(Socket sock) 
        {
            m_connecting_map.Remove(sock);
        }

        protected int HandleIO(bool read, bool write) 
        {
            Socket    sock = null;
            EventData data = null;
            NetHandle handle = null;
            if (read && m_read_event.Count > 0) 
            {
                for (int i = 0; i < m_read_event.Count;) 
                {
                    sock = m_read_event[i];
                    data = m_event_map[sock];
                    handle = data.m_handle;
                    if (handle.Readable(sock, data) == 0) 
                    {
                        ++i;
                    }
                }
            }
            if (write && m_write_event.Count > 0) 
            {
                for (int i = 0; i < m_write_event.Count; ) 
                {
                    sock = m_write_event[i];
                    data = m_event_map[sock];
                    handle = data.m_handle;
                    if (handle.Writeable(sock, data) == 0) 
                    {
                        ++i;
                    }
                }
            }
            return 0;
        }

        protected void HandleConnecting()
        {
            List<Socket> toRemoveSock = new List<Socket>();
            foreach (KeyValuePair<Socket, EventData> kvp in m_connecting_map) 
            {
                Socket sock = kvp.Key;
                EventData ed = kvp.Value;
                try 
                {
                    bool ok = sock.Poll(1, SelectMode.SelectWrite);
                    if (ok) 
                    {
                        toRemoveSock.Add(sock);
                        ModEvent(sock, EventFlag.EVENT_WRITE, ed);
                    }
                } 
                catch 
                {
                    toRemoveSock.Add(sock);
                    ed.m_handle.Error(sock, ed);
                }
            }
            for (int i = 0; i < toRemoveSock.Count; i++) 
            {
                DelConnectingSock(toRemoveSock[i]);
            }
        }
    }
}
