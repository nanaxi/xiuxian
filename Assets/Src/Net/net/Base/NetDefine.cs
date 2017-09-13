using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NetBase
{
    public enum NetPackLenght 
    {
        MAX_NET_PACK_LENGTH = 1024 * 48,
    }

    public enum NetThreshold 
    {
        NET_SEND_THRESHOLD = 128,
    };

    public enum SocketFlag : byte 
    {
        SOCKET_NULL,
        SOCKET_CONNECT,
        SOCKET_LINKED,
    }

    public enum SessionState 
    {
        SESSION_STATE_NULL = 0,
        SESSION_STATE_WORKING,
        SESSION_STATE_SHUTDOWN,   // app lay shutdown(grace close socket)
    }

    public enum EventFlag : byte 
    {
        EVENT_NULL  = 0,
        EVENT_READ  = 1,
        EVENT_WRITE = 1 << 1,
        EVENT_RANDW = EVENT_READ | EVENT_WRITE,
    }

    public enum NetBufferSize : int 
    {
        RECV_BUFFER_SIZE = 1024 * 56,
        SEND_BUFFER_SIZE = 1024 * 48,
        RESIZE_BUFFER_THRESHOLD = 128,
    }

    public delegate int NetPackHandleFunc(Stream stream, NetSession session);

    public class EventData
    {
        public UInt16 m_flag;
        public NetHandle m_handle;
        public NetSession m_session;
        public EventFlag m_event_flag;
        public SocketFlag m_socket_flag;  

        public EventData() 
        {
            m_flag = 0;
            m_handle = null;
            m_session = null;
            m_event_flag = EventFlag.EVENT_NULL;
            m_socket_flag = SocketFlag.SOCKET_NULL;
        }

        ~EventData() 
        {}
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NetPack
    {
        public UInt16 cmdAndFlag;
        public UInt32 length;

        public void SetLength(UInt32 len)
        {
            length = len;
        }

        public UInt32 GetLength()
        {
            return length;
        }

        public void SetCmd(UInt16 cmd)
        {
            cmdAndFlag = (UInt16)(cmd | (cmdAndFlag & (1 << 15)));
        }

        public UInt16 GetCmd()
        {
            return (UInt16)(cmdAndFlag & (~(1 << 15)));
        }

        public void SetCompress()
        {
            cmdAndFlag |= (1 << 15);
        }

        public void ClearCompress()
        {
            int tmp = ~(1 << 15);
            cmdAndFlag &= (UInt16)tmp;
        }

        public bool IsCompressed()
        {
            return 0 != (cmdAndFlag & (1 << 15));
        }
    }

    public enum SessionCloseFlag : byte
    {
        CLOSE_FLAG_ERROR = 1,   // error
        CLOSE_FLAG_TIMEOUT = 2,   // heart beat timeout
        CLOSE_FLAG_PEER_CLOSE = 3,   // remote peer close     
    }

    public interface NetCallback
    {
        int OnRecv(ref NetPack pack, byte[] data, int offset, int length, NetSession session);
        int OnConnect(NetSession session, UInt16 flag, bool success);
        int OnClose(NetSession session, SessionCloseFlag flag);
    }

    public interface INotifyConnect
    {
        void OnConnectedGameServer(object connection, bool success);
        void OnClosedGameServer(object connection, SessionCloseFlag flag);
    }

}
