using System;
using System.Collections.Generic;
using System.Text;
using NetBase;

public class NetPackHandleRegist
{
    private LoginNetHandle m_login_handle;

    public int Init(NetPackHandle handle)
    {
        m_login_handle = new LoginNetHandle();
        if (m_login_handle == null)
        {
            return -1;
        }
        m_login_handle.Init(handle);

        return 0;
    }

    public void Shutdown()
    {
        if (m_login_handle != null)
        {
            m_login_handle.Shutdown();
            m_login_handle = null;
        }
    }

    public NetPackHandleRegist()
    {
        m_login_handle = null;
    }

    ~NetPackHandleRegist()
    { }

}
