using System;
using System.Collections.Generic;
using System.Text;

public class ListNode<TYPE> 
{
    public TYPE m_host;
    public ListNode<TYPE> m_prev;
    public ListNode<TYPE> m_next;

    public ListNode(TYPE host) 
    {
        m_host = host;
        m_prev = null;
        m_next = null;
    }

    public void Reset() 
    {
        m_host = default(TYPE);
        m_prev = null;
        m_next = null;
    }

    public TYPE GetHost() 
    {
        return m_host;
    }

    public void SetHost(TYPE host) 
    {
        m_host = host;
    }

    public ListNode() 
    {
        Reset();
    }

    ~ListNode() {
        Reset();
    }
}

public class DoubleList<TYPE> 
{
    protected ListNode<TYPE> m_head;
    protected ListNode<TYPE> m_tail;

    public DoubleList()
    {
        m_head = null;
        m_tail = null;
    }

    ~DoubleList()
    {
    }

    public void InsertTail(ListNode<TYPE> node) 
    {
        if (m_tail != null) 
        {
            Insert(m_tail, node);
            m_tail = node;
        } 
        else 
        {
            m_head = m_tail = node;
            node.m_prev = node.m_next = null;
        }
    }

    public void InsertHead(ListNode<TYPE> node) 
    {
        if (m_head != null) 
        {
            node.m_next = m_head;
            m_head.m_prev = node;
            m_head = node;
        } 
        else 
        {
            m_head = m_tail = node;
            node.m_prev = node.m_next = null;
        }
    }

    public void Delete(ListNode<TYPE> node) 
    {
        if (m_head == node) 
        {
            m_head = m_head.m_next;
            if (m_head == null) 
            {
                m_tail = null;
            } 
            else 
            {
                m_head.m_prev = null;
            }
        } 
        else if (m_tail == node) 
        {
            m_tail = m_tail.m_prev;
            m_tail.m_next = null;
        } 
        else 
        {
            node.m_prev.m_next = node.m_next;
            node.m_next.m_prev = node.m_prev;
        }
        node.m_prev = null;
        node.m_next = null;
    }

    public bool IsEmpty() 
    {
        return m_head == null;
    }

    public ListNode<TYPE> Head() 
    {
        return m_head;
    }

    public ListNode<TYPE> Tail() 
    {
        return m_tail;
    }

    public ListNode<TYPE> Prev(ListNode<TYPE> node) 
    {
        if (node != null) 
        {
            return node.m_prev;
        } 
        else 
        {
            return null;
        }
    }

    public ListNode<TYPE> Next(ListNode<TYPE> node) 
    {
        if (node != null) 
        {
            return node.m_next;
        } 
        else 
        {
            return null;
        }
    }

    protected void Insert(ListNode<TYPE> prev, ListNode<TYPE> node) 
    {
        ListNode<TYPE> next = prev.m_next;
        prev.m_next = node;
        node.m_prev = prev;
        node.m_next = next;

        if (next != null) 
        {
            next.m_prev = node;
        }
    }
}
