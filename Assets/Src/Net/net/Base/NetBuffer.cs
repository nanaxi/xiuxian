using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NetBase
{
    public class NetBuffer
    {
        private int m_nSize;
        private int m_nOffSet;
        private Byte[] m_pBuffer;

        public NetBuffer()
        {
            Release();
        }

        ~NetBuffer()
        {
            Release();
        }

        public int Init(int size) 
        {
            if (size <= 0)
            {
                Log.Error("net buffer init size {0} invalid", size);
                return -1;
            }
            m_nSize = size;
            m_pBuffer = new Byte[m_nSize];
            if (m_pBuffer == null)
            {
                return -1;
            }
            return 0;
        }

        public void Release() 
        {
            m_nOffSet = 0;
            m_nSize = 0;
            m_pBuffer = null;
        }

        public int Resize() 
        {
            if ((m_nSize - m_nOffSet) < (int)NetBufferSize.RESIZE_BUFFER_THRESHOLD)
            {
                if (Realloc(m_nSize * 2) < 0)
                {
                    return -1;
                }
            }
            return 0;
        }

        public Byte[] GetBuffer() 
        {
            return m_pBuffer;
        }

        public int GetOffset() 
        {
            return m_nOffSet;
        }

        public int GetSize() 
        {
            return m_nSize;
        }

        public int GetSpace() 
        {
            return m_nSize - m_nOffSet;
        }

        public int Read(int length) 
        {
            if (m_pBuffer == null)
            {
                Log.Error("m_pBuffer is null");
                return 0;
            }
            if (length > m_nOffSet)
            {
                Log.Error("read so longer {0} than remain {1}", length, m_nOffSet);
                return 0;
            }
            if (length < m_nOffSet)
            {
                System.Buffer.BlockCopy(m_pBuffer, length, m_pBuffer, 0, m_nOffSet - length);
            }
            m_nOffSet -= length;
            return 0;
        }

        public int Write(int length) 
        {
            if (m_nOffSet + length > m_nSize)
            {
                Log.Error("write too much longer {0} than space {1}", length, m_nOffSet);
                return 0;
            }
            m_nOffSet += length;
            return 0;
        }

        public int Write(Byte[] buf, int offset, int length) 
        {
            if (buf == null || length == 0) 
            {
                return 0;
            }
            if (length > (m_nSize - m_nOffSet))
            {
                if (Realloc(m_nOffSet + length) != 0)
                {
                    return -1;
                }
            }
            System.Buffer.BlockCopy(buf, offset, m_pBuffer, m_nOffSet, length);
            m_nOffSet += length;
            return 0;
        }

        protected int Realloc(int length) 
        {
            if (length <= m_nSize)
            {
                Log.Error("realloc length {0} short than m_nSize {1}", length, m_nSize);
                return -1;
            }
            Byte[] buf = new Byte[length];
            if (buf == null) {
                return -1;
            }
            if (m_pBuffer != null && m_nOffSet > 0)
            {
                System.Buffer.BlockCopy(m_pBuffer, 0, buf, 0, m_nOffSet);              
            }
            m_nSize = length;
            m_pBuffer = buf;
            return 0;
        }
    }
}
