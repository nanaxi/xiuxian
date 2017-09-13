using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NetBase;
using ProtoBuf;
using LZF.NET;

public enum NetConnectType
{
    NET_CONNECT_LOGIN_SERVER = 1,
    NET_CONNECT_GATE_SERVER
}

public class GameNetWork : ISingleton<GameNetWork>, NetCallback, IModule
{
    private int m_head_len;
    private int m_send_cache_len;
    private IntPtr m_pack_head_ptr;
    private byte[] m_serialize_buf;
    private byte[] m_send_cache_buf;

    private NetFrame m_net_frame;
    private NetPackHandle m_net_pack_handle;
    private NetPackHandleRegist m_net_handle_regist;

    private Connection m_con_login_server;
    
    private UInt64 m_upNetWorkFlow = 0;
    private UInt64 m_downNetWorkFlow = 0;

    public bool Init()
    {
        try
        {
            bool result = false;
            m_serialize_buf = new byte[(UInt16)NetPackLenght.MAX_NET_PACK_LENGTH];
            if (m_serialize_buf == null){ goto Exit0; }
            m_send_cache_len = 0;
            m_send_cache_buf = new byte[(UInt16)NetPackLenght.MAX_NET_PACK_LENGTH];
            if (m_send_cache_buf == null){ goto Exit0; }
            
            m_net_frame = new NetFrame();
            if (m_net_frame == null){ goto Exit0; }
            if (m_net_frame.Init(this) != 0){ goto Exit0; }

            m_net_pack_handle = new NetPackHandle();
            if (m_net_pack_handle == null){ goto Exit0; }
            m_net_pack_handle.Init((int)CLIToLGIProtocol.CLI_TO_LGI_PROTOCOL_COUNT);

            m_net_handle_regist = new NetPackHandleRegist();
            if (m_net_handle_regist == null){ goto Exit0; }
            m_net_handle_regist.Init(m_net_pack_handle);

            NetPack net_pack;
            net_pack.cmdAndFlag = 0;
            net_pack.length = 0;
            m_head_len = Marshal.SizeOf(net_pack);
            m_pack_head_ptr = Marshal.AllocHGlobal(m_head_len);
            if (m_pack_head_ptr == IntPtr.Zero)
            {
                goto Exit0;
            }

            m_con_login_server = new Connection(m_net_frame, NetConnectType.NET_CONNECT_LOGIN_SERVER);

            result = true;

        Exit0:
            if (!result)
            {
                Log.Info("GameNetWork Init Failed...");
            }
            return result;
        }
        catch (System.Exception ex)
        {
            Log.Exception(ex);
            return false;
        }
    }

    public bool UnInit()
    {
        try
        {
            m_send_cache_len = 0;
            if (m_con_login_server != null)
            {
                m_con_login_server.Close();
                m_con_login_server = null;
            }
            if (m_pack_head_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_pack_head_ptr);
            }
            if (m_net_handle_regist != null)
            {
                m_net_handle_regist.Shutdown();
                m_net_handle_regist = null;
            }
            if (m_net_pack_handle != null)
            {
                m_net_pack_handle.Shutdown();
                m_net_pack_handle = null;
            }
            if (m_net_frame != null)
            {
                m_net_frame.Shutdown();
                m_net_frame = null;
            }
            if (m_send_cache_buf != null)
            {
                m_send_cache_buf = null;
            }
            if (m_serialize_buf != null)
            {
                m_serialize_buf = null;
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Log.Exception(ex);
            return false;
        }
    }

    public void Update()
    {
        if (m_net_frame != null)
        {
            m_net_frame.Loop(0);
        }
    }

    public Connection GetLoginServerConnection()
    {
        return m_con_login_server;
    }

    public UInt64 UpNetWorkFlow
    {
        get{ return m_upNetWorkFlow; }
    }

    public UInt64 DownNetWorkFlow
    {
        get{ return m_downNetWorkFlow; }
    }

    static public bool prepareSerializer = false;
    static public System.Collections.IEnumerator PrepareSerializer()
    {
        yield return null;
        prepareSerializer = true;
    }

    public bool SendDataToLoginServer()
    {
        if (m_send_cache_len > 0)
        {
            if (SendData(m_con_login_server, m_send_cache_buf, m_send_cache_len))
            {
                m_send_cache_len = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public bool SendDataToLoginServer(UInt16 command)
    {
        if (!CanSendToLogic(command))
        {
            return false;
        }
        if ((m_send_cache_len + m_head_len) >= m_send_cache_buf.Length)
        {
            return false;
        }

        NetPack pack_head;
        pack_head.cmdAndFlag = 0;
        pack_head.length = 0;
        pack_head.SetCmd(command);
        pack_head.SetLength((UInt32)m_head_len);
        Marshal.StructureToPtr(pack_head, m_pack_head_ptr, false);
        Marshal.Copy(m_pack_head_ptr, m_send_cache_buf, m_send_cache_len, m_head_len);
        m_send_cache_len += m_head_len;
        return SendDataToLoginServer();
    }

    public bool SendDataToLoginServer<TYPE>(UInt16 command, TYPE pack) where TYPE : class
    {
        int writeSize = 0;
        if (!CanSendToLogic(command))
        {
            return false;
        }
        if (!MakeSendData(command, pack, m_send_cache_buf, m_send_cache_buf.Length, m_send_cache_len, out writeSize))
        {
            return false;
        }
        m_send_cache_len += writeSize;
        return SendDataToLoginServer();
    }

    public bool SendDataToLoginServer_CacheOnDisconnect<TYPE>(UInt16 command, TYPE pack) where TYPE : class
    {
        int writeSize = 0;
        if (!MakeSendData(command, pack, m_send_cache_buf, m_send_cache_buf.Length, m_send_cache_len, out writeSize))
        {
            return false;
        }
        m_send_cache_len += writeSize;
        return SendDataToLoginServer();
    }

    public bool SendDataToCenterServer<TYPE>(UInt16 command, TYPE pack) where TYPE : class
    {
        int writeSize = 0;
        int offset = (m_head_len * 2) + sizeof(UInt32);

        if (!CanSendToLogic(command))
        {
            return false;
        }
        // 多加一个包头以及预留一个characterId的位置，由Logic Server填充后续协议
        if (!MakeSendData(command, pack, m_send_cache_buf, m_send_cache_buf.Length, (m_send_cache_len + offset), out writeSize))
        {
            return false;
        }

        writeSize += offset;
        NetPack pack_head;
        pack_head.cmdAndFlag = 0;
        pack_head.length = 0;
        pack_head.SetCmd((UInt16)CLIToLGIProtocol.CLI_TO_LGI_REDIRECT_CENTERSERVER);
        pack_head.SetLength((UInt32)writeSize);
        Marshal.StructureToPtr(pack_head, m_pack_head_ptr, false);
        Marshal.Copy(m_pack_head_ptr, m_send_cache_buf, m_send_cache_len, m_head_len);
        m_send_cache_len += writeSize;
        return SendDataToLoginServer();
    }

    public bool SendCmdToCenterServer(UInt16 command)
    {
        if (!CanSendToLogic(command))
        {
            return false;
        }
        int offset = (m_head_len * 2) + sizeof(UInt32);
        int writeSize = offset + m_head_len;
        if ((m_send_cache_len + writeSize) >= m_send_cache_buf.Length)
        {
            return false;
        }

        NetPack pack_head_inner;
        pack_head_inner.length = 0;
        pack_head_inner.cmdAndFlag = 0;
        pack_head_inner.SetCmd(command);
        pack_head_inner.SetLength((UInt32)m_head_len);
        Marshal.StructureToPtr(pack_head_inner, m_pack_head_ptr, false);
        Marshal.Copy(m_pack_head_ptr, m_send_cache_buf, m_send_cache_len + offset, m_head_len);

        NetPack pack_head;
        pack_head.cmdAndFlag = 0;
        pack_head.length = 0;
        pack_head.SetCmd((UInt16)CLIToLGIProtocol.CLI_TO_LGI_REDIRECT_CENTERSERVER);
        pack_head.SetLength((UInt32)writeSize);
        Marshal.StructureToPtr(pack_head, m_pack_head_ptr, false);
        Marshal.Copy(m_pack_head_ptr, m_send_cache_buf, m_send_cache_len, m_head_len);
        m_send_cache_len += writeSize;
        return SendDataToLoginServer();
    }

    private bool CanSendToLogic(UInt16 command)
    {
        return m_con_login_server.IsConnected();
    }

    private void _ResetStream(Stream st)
    {
        st.Seek(0, SeekOrigin.Begin);
        st.SetLength(0);
    }

    private MemoryStream sendStream = new MemoryStream((UInt16)NetBufferSize.SEND_BUFFER_SIZE);
    private byte[] serial_buf = new byte[(UInt16)NetBufferSize.SEND_BUFFER_SIZE];
    private bool MakeSendData<TYPE>(UInt16 command, TYPE pack, byte[] buff, int buffSize, int offsetBuff, out int writeSize) where TYPE : class
    {
        _ResetStream(sendStream);
        bool bRetCode = ProtoBuf.Serializer.SafeSerialize<TYPE>(sendStream, pack);
        if (!bRetCode)
        {
            writeSize = 0;
            return false;
        }
        sendStream.Seek(0, SeekOrigin.Begin);
        sendStream.Read(serial_buf, 0, (int)sendStream.Length);

        int serial_len = (int)sendStream.Length;
        writeSize = serial_len + m_head_len;
        if ((offsetBuff + writeSize) >= buffSize)
        {
            Log.Error("MakeSendData Failed For Command{3}, offset:{0}, writeSize:{1}, buffSize:{2}", offsetBuff, writeSize, buffSize, command);
            writeSize = 0;
            return false;
        }

        NetPack pack_head;
        pack_head.length = 0;
        pack_head.cmdAndFlag = 0;
        pack_head.SetCmd(command);
        pack_head.SetLength((UInt32)writeSize);
        Marshal.StructureToPtr(pack_head, m_pack_head_ptr, false);
        Marshal.Copy(m_pack_head_ptr, buff, offsetBuff, m_head_len);
        System.Buffer.BlockCopy(serial_buf, 0, buff, offsetBuff + m_head_len, serial_len);

        return true;
    }

    public bool SendData<TYPE>(Connection connection, UInt16 command, TYPE pack) where TYPE : class
    {
        if (connection == null)
        {
            Trace.Assert(false);
            return false;
        }
        int writeSize = 0;
        if (!MakeSendData(command, pack, m_serialize_buf, m_serialize_buf.Length, 0, out writeSize))
        {
            return false;
        }
        return SendData(connection, m_serialize_buf, writeSize);
    }

    public bool SendData(Connection connection, byte[] buff, int len)
    {
        bool retCode = connection.SendData(buff, len);
        if (retCode)
        {
            m_upNetWorkFlow += (UInt64)len;
        }

        return retCode;
    }

    private CLZF compresser = new CLZF();
    private Stream recvStream = new MemoryStream((UInt16)NetBufferSize.RECV_BUFFER_SIZE);
    public int OnRecv(ref NetPack pack ,byte[] data ,int offset ,int length, NetSession session)
    {
        m_downNetWorkFlow += (UInt64)length;
        NetPackHandleFunc handle = m_net_pack_handle.GetNetPackHandle(pack.GetCmd());
        if (handle == null)
        {
            Log.Error("handle net pack command:{0} out off range", pack.GetCmd());
            return -1;
        }

        _ResetStream(recvStream);
        if (pack.IsCompressed())
        {
            int retLength = compresser.lzf_decompress(data, offset, length, serial_buf, serial_buf.Length);
            if (retLength <= 0)
            {
                Log.Error("handle net pack command:{0} decompress error", pack.GetCmd());
                return -1;
            }
            else
            {
                recvStream.Write(serial_buf, 0, retLength);
            }
        }
        else
        {
            recvStream.Write(data, offset, length);
        }

        recvStream.Seek(0, SeekOrigin.Begin);

#if UNITY_EDITOR
        handle(recvStream, session);
#else
        try 
        {
            handle(recvStream, session);
        } 
        catch (Exception e) 
        {
            Log.Error("handle net pack command:{0} Error:{1}", pack.GetCmd(), e.StackTrace);
        }
#endif
        return 0;
    }

    public int OnConnect(NetSession session, UInt16 flag, bool success)
    {
        if (flag == (UInt16)NetConnectType.NET_CONNECT_LOGIN_SERVER)
        {
            m_con_login_server.OnConnect(session, success);
        }
        else
        {
            Trace.Assert(false);
        }
        return 0;
    }

    public int OnClose(NetSession session, SessionCloseFlag flag)
    {
        UInt32 data = session.GetUserData();
        if (data == (UInt32)NetConnectType.NET_CONNECT_LOGIN_SERVER)
        {
            m_con_login_server.OnClose(session, flag);
        }
        else
        {
            Trace.Assert(false);
        }
        return 0;
    }

    public GameNetWork()
    {
        m_head_len = 0;
        m_send_cache_len = 0;
        m_pack_head_ptr = IntPtr.Zero;
        m_serialize_buf = null;
        m_send_cache_buf = null;
        m_net_frame = null;
        m_net_pack_handle = null;
        m_net_handle_regist = null;
        m_con_login_server = null;
    }

    ~GameNetWork()
    {}
}
