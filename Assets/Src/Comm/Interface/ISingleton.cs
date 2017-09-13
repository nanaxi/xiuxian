using System;
using System.Diagnostics;

public class ISingleton<TYPE>
    where TYPE : new()
{
    public static TYPE Inst()
    {
        if (m_instance == null)
        {
            m_instance = new TYPE();
        }
        Trace.Assert(m_instance != null, "singleton is null");
        return m_instance;
    }

    protected ISingleton() {}
    private ISingleton(ref ISingleton<TYPE> instance) {}
    private ISingleton(ISingleton<TYPE> instance) {}

    private static TYPE m_instance;
}