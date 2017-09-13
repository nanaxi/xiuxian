using UnityEngine;
using System;
using System.Collections;
using NetBase;
using System.Diagnostics;
using System.Text;
using ProtoBuf;

public enum LOGIN_STATE
{
    LOGIN_STATE_NULL,
    LOGIN_STATE_CONNECT_SERVER,
    LOGIN_STATE_SERVER_LOGIN,           // 发送玩家账号，等待验证中
    LOGIN_STATE_SUCESS,                 // login server 返回验证成功
    LOGIN_STATE_FAIL
};

public class LoginProcessor : ISingleton<LoginProcessor>, IModule
{
    private LOGIN_STATE m_loginState = LOGIN_STATE.LOGIN_STATE_NULL;
    private Connection m_connection = null;

    public UInt64 m_serverStartMark = 0;
    private bool m_connectBreak = false;
    private bool m_reconnecting = false;
    private bool m_logoutGame = false;

    private float m_lastSendPingTime = 0;
    private float m_lastReconnectTime = 0.0f;
    private float m_breakConnectTime = 0.0f;
    static float m_reconnectInterval = 3.0f;
    static float m_maxReconnectTime = 5 * 60;

    public uint m_groupId = 0;
    public string m_ip = "";
    public ushort m_port = 0;
    public ulong m_token = 0;

    public bool Init()
    {
        m_serverStartMark = 0;
        m_connectBreak = false;
        m_reconnecting = false;
        m_logoutGame = false;

        m_loginState = LOGIN_STATE.LOGIN_STATE_NULL;
        m_connection = GameNetWork.Inst().GetLoginServerConnection();
        m_connection.RegisterConnectedEvent(this.OnConnectedLoginServer);
        m_connection.RegisterClosedEvent(this.OnClosedLoginServer);
        return true;
    }

    public bool UnInit()
    {
        return true;
    }

    public void Update()
    {
        if (m_loginState == LOGIN_STATE.LOGIN_STATE_NULL || m_loginState == LOGIN_STATE.LOGIN_STATE_FAIL)
        {
            return;
        }
        else if (m_loginState == LOGIN_STATE.LOGIN_STATE_CONNECT_SERVER)
        {
            if (!m_connection.IsConnected())
            {
                ConnectLoginServer();
                return;
            }
            CheckVersionRequest();
        }
        else if (m_loginState == LOGIN_STATE.LOGIN_STATE_SUCESS)
        {
            if (!m_connection.IsConnected())
            {
                if (m_serverStartMark != 0 && m_connectBreak && !m_reconnecting &&
                    Time.time - m_lastReconnectTime > m_reconnectInterval)
                {
                    if (Time.time - m_breakConnectTime > m_maxReconnectTime)
                    {
                        Log.Error("Connect Login Server Failed, Please ReLogin...");
                    }
                    else
                    {
                        m_lastReconnectTime = Time.time;
                        if (m_connection.Connect(m_ip, m_port))
                        {
                            m_reconnecting = true;
                        }
                    }
                }
            }
        }
        ProcessPingServer();
    }

    public void Login()
    {
        m_loginState = LOGIN_STATE.LOGIN_STATE_CONNECT_SERVER;
    }

    public void SetLoginState(LOGIN_STATE loginState)
    {
        m_loginState = loginState;
    }

    public LOGIN_STATE GetLoginState()
    {
        return m_loginState;
    }

    public void SetLogoutGame(bool flag)
    {
        m_logoutGame = flag;
        if (m_logoutGame)
        {
            m_connectBreak = false;
            m_reconnecting = false;
            m_serverStartMark = 0;
            m_breakConnectTime = 0.0f;
        }
    }

    public void LogoutReset()
    {
    }

    private void ProcessPingServer()
    {
        if (m_connection != null && m_connection.IsConnected())
        {
            if ((int)(Time.time - m_lastSendPingTime) > (int)PingConfig.CLIENT_PING_INTERVAL)
            {
                if (ApplyPingServer())
                {
                    m_lastSendPingTime = Time.time;
                }
            }
        }
    }

    public bool ConnectLoginServer()
    {
        if (m_connection.IsConnected() || m_connection.IsConnecting())
        {
            return false;
        }

        string ip = GlobalSettings.LoginServerIP;
        UInt16 port = GlobalSettings.LoginServerPort;
        m_logoutGame = false;
        m_connectBreak = false;
        m_reconnecting = false;

        bool result = m_connection.Connect(ip, port);
        if (!result)
        {
            Log.Info("Connect LoginServer failed!");
            return false;
        }

        Log.Info("Connect to login server  ... {0}:{1}", ip, port);
        return true;
    }

    public bool ConnectGameServer(string ip, UInt16 port)
    {
        m_ip = ip;
        m_port = port;
        m_logoutGame = false;
        m_connectBreak = false;
        m_reconnecting = false;

        bool result = m_connection.Connect(ip, port);
        if (!result)
        {
            Log.Info("Connect LoginServer Failed...");
            return false;
        }

        Log.Info("Connect to login server  ... {0}:{1}", ip, port);
        return true;
    }

    public bool CloseLoginServer()
    {
        Log.Info("Close login server...");
        return m_connection.Close();
    }

    public void OnConnectedLoginServer(object connection, bool success)
    {
        if (success)
        {
            m_lastSendPingTime = Time.time;
            m_lastReconnectTime = 0.0f;

            if (m_connectBreak)
            {
                Log.Info("Break ReConnect Success...");
                ApplyReconnectLoginServer();
                m_connectBreak = false;
            }
            else
            {
                Log.Info("Connect Login Success...");
            }
        }
        else
        {
            if (!m_connectBreak)
            {
                Log.Error("Connect Login Server Failed, Please ReLogin...");
            }
            else
            {
                Log.Info("Break ReConnect Failed...");
            }
            m_loginState = LOGIN_STATE.LOGIN_STATE_FAIL;
        }
        if (m_reconnecting)
        {
            m_reconnecting = false;
        }

        if (!m_connectBreak)
        { }
    }

    public void OnClosedLoginServer(object connection, SessionCloseFlag flag)
    {
        if (m_serverStartMark != 0)
        {
            if (!m_logoutGame)
            {
                m_connectBreak = true;
                m_breakConnectTime = Time.time;
            }
        }
        else
        {
            LoginProcessor.Inst().SetLogoutGame(true);
            Log.Error("DisConnect with Server, Please ReLogin...");
        }
        if (!m_connectBreak)
        { }
    }

    //----------------------------------------------------------------------------------------------------------------
    // 检查客户端版本及搜集玩家手机信息
    public bool CheckVersionRequest()
    {
        CheckVersionReq reqPack = new CheckVersionReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_CHECK_VERSION;
        reqPack.version = 1;
        reqPack.imei = "1234";
        bool retCode = GameNetWork.Inst().SendDataToLoginServer(command, reqPack);
        if (retCode)
        {
            m_loginState = LOGIN_STATE.LOGIN_STATE_SERVER_LOGIN;
        }
        else
        {
            m_loginState = LOGIN_STATE.LOGIN_STATE_FAIL;
        }
        return retCode;
    }

    // 检查客户端版本及搜集玩家手机信息 --> 服务器返回包
    public bool CheckVersionResponse(CheckVersionRsp rsp)
    {
        UnityEngine.Debug.Log("检查客户端版本及搜集玩家手机信息");
        if (rsp.result == CheckVersionRsp.Result.CHECK_VERSION_SUCCESS)
        {
            AccountLoginRequest();
        }
        else
        {
            SetLoginState(LOGIN_STATE.LOGIN_STATE_FAIL);
        }
        return true;
    }

    // 玩家账户登录，这里应该是接微信的
    public bool AccountLoginRequest()
    {
        AccountLoginReq reqPack = new AccountLoginReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_ACCOUNT_LOGIN;

        reqPack.channelId = GlobalSettings.LoginChannel;    // 渠道
        reqPack.userId = GlobalSettings.LoginUserId;        // 渠道用户id（微信用户id）
        reqPack.userName = GlobalSettings.LoginUserName;    // 渠道用户名（微信用户名）
        reqPack.deviceID = "";                              // 设备信息，留着以后扩展
        reqPack.portrait = GlobalSettings.avatarUrl;
        reqPack.sex = GlobalSettings.sex;
        bool retCode = GameNetWork.Inst().SendDataToLoginServer(command, reqPack);
        if (retCode)
        {
            m_loginState = LOGIN_STATE.LOGIN_STATE_SERVER_LOGIN;
        }
        else
        {
            m_loginState = LOGIN_STATE.LOGIN_STATE_FAIL;
        }
        UnityEngine.Debug.Log("申请登录");
        return retCode;
    }

    // 玩家账户登录，这里应该是接微信的 --> 服务器返回包
    public bool AccountLoginResponse(AccountLoginRsp rsp)
    {
        //UnityEngine.Debug.Log("登录返回");
        if (GetLoginState() != LOGIN_STATE.LOGIN_STATE_SERVER_LOGIN)
        {
            Log.Info("Login State Changed...");
            return false;
        }

        switch (rsp.result)
        {
            case AccountLoginRsp.Result.ACCOUNT_LOGIN_SUCCESS:
                if (rsp.token == 0)
                {
                    SetLoginState(LOGIN_STATE.LOGIN_STATE_FAIL);
                    return false;
                }

                SetLoginState(LOGIN_STATE.LOGIN_STATE_SUCESS);
                m_token = rsp.token;
                m_serverStartMark = rsp.startMark;
                m_groupId = rsp.groupId;

                BaseProto.playerInfo.m_id = rsp.charId;
                BaseProto.playerInfo.m_account = "";
                BaseProto.playerInfo.m_inGame = rsp.inGame;
                BaseProto.playerInfo.m_cdRoomId = rsp.crRoomId;
                BaseProto.playerInfo.m_atRoomId = rsp.atRoomId;
                BaseProto.playerInfo.m_level = 0;
                BaseProto.playerInfo.m_roomCard = 0;
                BaseProto.playerInfo.m_diamond = rsp.diamond;
                BaseProto.playerInfo.m_gold = rsp.gold;
                BaseProto.playerInfo.m_ip = rsp.ip;
                UnityEngine.Debug.Log("登录成功");

                GameManager.GM.IsLogined = true;
                //GameManager.GM.ReLoginTimes = 0;
                GameManager.GM.DestoryUI();
                PublicEvent.GetINS.Fun_LoginResult(rsp);
                Lang.NetReceiver.Inst.OnAccountLoginSuccess(rsp);

                break;
            default:
                SetLoginState(LOGIN_STATE.LOGIN_STATE_FAIL);
                //var wins = GameManager.GM.SearchEmpty().AddComponent<DisContact>();
                //wins.Ins("登陆失败,请稍后再试!");
                //wins.transform.SetAsLastSibling();
                //Log.Error("Account Fail...");
                break;
        }
        return true;
    }

    // 玩家退出游戏
    public bool ApplyLogout()
    {
        if (m_connection.IsConnected())
        {
            EmptyMessage pack = new EmptyMessage();
            UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_LOGOUT;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        m_token = 0;
        SetLogoutGame(true);
        CloseLoginServer();

        LoginProcessor.Inst().LogoutReset();
        return true;
    }

    // 断线重连接口，这个会自动检测网络情况，启动调用
    private bool ApplyReconnectLoginServer()
    {
        bool result = false;
        if (m_serverStartMark != 0)
        {
            UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_RECONNECT;
            ReconnectReq reqPack = new ReconnectReq();
            reqPack.charId = 0;
            reqPack.token = m_token;
            reqPack.channelId = GlobalSettings.LoginChannel;
            reqPack.userId = GlobalSettings.LoginUserId;
            reqPack.userName = GlobalSettings.LoginUserName;
            reqPack.startMark = m_serverStartMark;
            result = GameNetWork.Inst().SendData(GameNetWork.Inst().GetLoginServerConnection(), command, reqPack);
            if (result)
            {
                GameNetWork.Inst().SendDataToLoginServer();
            }
        }
        return result;
    }

    // 断线重连接口 --> 服务器返回包
    public bool ReconnectLoginResponse(ReconnectRsp rsp)
    {
        return true;
    }

    // 到服务器的心跳包
    public bool ApplyPingServer()
    {
        PingLoginReq reqPack = new PingLoginReq();
        return GameNetWork.Inst().SendDataToLoginServer((UInt16)CLIToLGIProtocol.CLI_TO_LGI_PING, reqPack);
    }
}
