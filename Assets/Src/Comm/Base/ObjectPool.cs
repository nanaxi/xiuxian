using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IPoolObject : System.IComparable<IPoolObject>
{
    public virtual void Init() 
    {}
    // destroy
    public virtual void UnInit()
    {}
    // called when return back to pool
    public virtual void ReSet()
    {}
    // default compare method
    public virtual int CompareTo(IPoolObject obj)
    {
        if(this.Equals(obj))
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}

public class ObjectPool<T> 
    where T : IPoolObject, new()
{
    private List<T> m_listObject = null;
    private int m_nCapcity = 0;
    private int m_nFreeIndex = 0;

    public ObjectPool(int capcity)
    {
        m_nCapcity = capcity;
        m_nFreeIndex = 0;
        m_listObject = new List<T>(m_nCapcity);
        for (int i = 0; i < m_nCapcity; ++i)
        {
            m_listObject.Add(default(T));
        }
    }

    public T Malloc()
    {
        int tIndex = FreeIndex();
        T obj = m_listObject[tIndex];
        if (obj == null)
        {
            obj = new T();
            m_listObject[tIndex] = obj;
            obj.Init();
        }
        return obj;
    }

    public bool Free(T obj)
    {
        int tIndex = m_listObject.FindIndex(src => src.CompareTo(obj) == 0);
        if(tIndex < 0)
        {
            Log.Error("{0} not in object pool {1}", obj, this);
            return false;
        }
        obj.ReSet();
        if(tIndex < m_nFreeIndex)
        {
            T tofree = m_listObject[tIndex];
            m_listObject[tIndex] = m_listObject[m_nFreeIndex - 1];
            m_listObject[m_nFreeIndex - 1] = tofree;
        }
        else
        {
            Log.Error("{0} in wrong index {1}", obj, tIndex);
        }
        --m_nFreeIndex;
        return true;
    }

    // todo
    public void Slim()
    {
    }

    // destroy object pool
    public void Clear()
    {
        for (int i = 0; i < m_listObject.Count; ++i)
        {
            if(m_listObject[i] != null)
            {
                m_listObject[i].UnInit();
            }
        }
        m_listObject.Clear();
        m_listObject = null;
    }

    private int FreeIndex()
    {
        if (m_nFreeIndex < m_nCapcity - 1)
        {
            return m_nFreeIndex++;
        }
        m_listObject.Add(default(T));
        m_nCapcity++;
        return m_nFreeIndex++;
    }
}
