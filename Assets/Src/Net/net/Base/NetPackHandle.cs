using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NetBase
{
    public class NetPackHandle 
    {
        int m_handle_count;
        NetPackHandleFunc[] m_handle_func;

        public NetPackHandle()
        {
            m_handle_count = 0;
            m_handle_func = null;
        }

        ~NetPackHandle()
        {
        }

        public int Init(int command_max_count)
        {
            m_handle_count = command_max_count;
            m_handle_func = new NetPackHandleFunc[m_handle_count];
            if (m_handle_func == null) 
            {
                return -1;
            }
            for (int i = 0; i < command_max_count; ++i) 
            {
                m_handle_func[i] = null;
            }
            return 0;
        }

        public void Shutdown()
        {
            m_handle_func = null;
        }

        public int RegistNetPackHandle(UInt16 command, NetPackHandleFunc func) 
        {
            if (command >= m_handle_count) 
            {
                Log.Error("command:{0} out of range {1}", command, m_handle_count);
                return -1;
            }
            if (m_handle_func[command] != null) 
            {
                Log.Error("command:{0} has regist", command);
                return -1;
            }
            m_handle_func[command] = func;
            return 0;
        }

        public NetPackHandleFunc GetNetPackHandle(UInt16 command) 
        {
            if (command >= m_handle_count) 
            {
                Log.Error("command:{0} out of range {1}", command, m_handle_count);
                return null;
            }
            if (command < m_handle_count)
            {
                return m_handle_func[command];
            } 
            else 
            {
                return null;
            }
        }
    }
}
