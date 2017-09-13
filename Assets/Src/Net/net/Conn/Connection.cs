using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using NetBase;

public delegate void ProcessConnected(object connection, bool success);
public delegate void ProcessClosed(object connection, SessionCloseFlag flag);

public class Connection
{
    private NetFrame m_net_frame;
    private NetConnectType m_con_type;
    private NetSession m_net_session;
    private bool m_is_connecting;
    private event ProcessConnected m_connected_event;
    private event ProcessClosed m_closed_event;

    public Connection(NetFrame net_frame, NetConnectType con_type)
    {
        m_net_frame = net_frame;
        m_con_type = con_type;
        m_net_session = null;
        m_is_connecting = false;
    }

    public bool RegisterConnectedEvent(ProcessConnected connected_fn)
    {
        m_connected_event += new ProcessConnected(connected_fn);
        return true;
    }

    public bool RegisterClosedEvent(ProcessClosed closed_fn)
    {
        m_closed_event += new ProcessClosed(closed_fn);
        return true;
    }

    public bool Connect(string ip, UInt16 port) 
    {
        if (m_is_connecting) 
        {
            return false;
        }
        Close();
        int result = m_net_frame.Connect(ip, port, (UInt16)m_con_type);
        if (result == 0) 
        {
            m_is_connecting = true;
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public bool Close() 
    {
        m_is_connecting = false;
        if (m_net_session == null) 
        {
            return true;    
        }
        int result = m_net_frame.Close(m_net_session);
        m_net_session = null;
        return result == 0;
    }

    public bool OnConnect(NetSession net_session, bool success) 
    {
        m_is_connecting = false;
        if (success && net_session != null) 
        {
            m_net_session = net_session;
            m_net_session.SetUserData((UInt32)m_con_type);
        } 
        else 
        {
            m_net_session = null;
        }
        if (m_connected_event != null) 
        {
            m_connected_event(this, success);
        }
        return true;
    }

    public bool OnClose(NetSession net_session, SessionCloseFlag flag)
    {
        m_is_connecting = false;
        if (m_net_session == net_session) 
        {
            m_net_session = null;
        } 
        else 
        {
            Trace.Assert(false);
        }
        if (m_closed_event != null) 
        {
            m_closed_event(this, flag);
        }
        return true;
    }

    public bool IsConnected() 
    {
        return m_net_session != null;
    }

    public bool IsConnecting() 
    {
        return m_is_connecting;
    }

    public bool SendData(byte[] buffer, int lenght)
    {
        if (m_net_session == null) 
        {
            Trace.Assert(false);
            return false;
        }
        int result = m_net_frame.Send(m_net_session, buffer, lenght);
        return result == 0;
    }
}
