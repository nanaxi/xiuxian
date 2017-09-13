using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using anysdk;
using System.IO;

/// <summary>
/// Any SDK manager  --SDK初始化、管理类
///  liuyu		2016.11.17
/// </summary>
public class AnySDKManager
{

    private const string AppKey = "F96A33C1-1B95-3FC3-4458-19F2BA596544";
    private const string AppSecret = "fdc2ec5f7b541540ca16f53574ceb3e7";
    private const string PrivateKey = "95B6592CB45282A5984B00CB91A7069E";
    private const string OtherLoginServer = "http://120.132.71.105:860/login.aspx";
    private static bool isInit = false;
    private static MonoBehaviour _AnySDKGo;

    public static void AnySDKInit(MonoBehaviour gameObject)
    {
        Debug.Log("调用 ANY SDK 初始化");
        isInit = false;
        _AnySDKGo = AnySDK_C.GetIns();
        
        AnySDK.getInstance().init(AppKey, AppSecret, PrivateKey, OtherLoginServer);
        AnySDKUser.getInstance().setListener(AnySDK_C.GetIns(), "AnySDKCallBack");
    }

    //SDK是否初始化成功
    public static bool IsInit
    {
        get
        {
            return isInit;
        }
    }

    //SDK初始化 及 用户登录回调
    public static void UserExternalCall(string msg)
    {
        //        Debug.Log("UserExternalCall( "+ msg+" )");
        Dictionary<string, string> dic = AnySDKUtil.stringToDictionary(msg);
        int code = Convert.ToInt32(dic["code"]);
        string result = dic["msg"];

       // GameManager.GM.Testss.text += "\n AnySDK响应code" + code;
        Debug.Log("AnySDK响应 code: " + code);
        switch (code)
        {
            case (int)UserActionResultCode.kInitSuccess://初始化SDK成功回调
                isInit = true;
                Debug.Log("AnySDK 插件初始化成功！");
                break;
            case (int)UserActionResultCode.kInitFail://初始化SDK失败回调
                isInit = false;
                Debug.Log("AnySDK 插件初始化失败！");
                break;
            case (int)UserActionResultCode.kLoginSuccess://登陆成功回调
                Debug.Log("AnySDK 登陆成功！" + result);
                //GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("AnySDK登陆成功");
                AnySDKShare.getInstance().setListener(_AnySDKGo, "AnySDKShareCallBack");
                AnySDKLogin.LoginSuccess(result);
                break;
            case (int)UserActionResultCode.kLoginNetworkError://登陆网络出错回调
                AnySDKLogin.LoginNetworkError(result);
                Debug.Log("AnySDK 登陆网络出错！" + result);
                break;
            case (int)UserActionResultCode.kLoginCancel://登陆取消回调
                AnySDKLogin.LoginCancel(result);
                Debug.Log("AnySDK 登陆取消！" + result);
                break;
            case (int)UserActionResultCode.kLoginFail://登陆失败回调
                AnySDKLogin.LoginFail(result);
                Debug.Log("AnySDK 登陆失败！" + result);
                break;
            case (int)UserActionResultCode.kLogoutSuccess://登出成功回调
                AnySDKLogin.LoginOutSuccess(result);
                break;
            case (int)UserActionResultCode.kLogoutFail://登出失败回调
                AnySDKLogin.LoginFail(result);
                break;
            case (int)UserActionResultCode.kPlatformEnter://平台中心进入回调
                break;
            case (int)UserActionResultCode.kPlatformBack://平台中心退出回调
                break;
            case (int)UserActionResultCode.kPausePage://暂停界面回调
                break;
            case (int)UserActionResultCode.kExitPage://退出游戏回调
                break;
            case (int)UserActionResultCode.kAntiAddictionQuery://防沉迷查询回调
                break;
            case (int)UserActionResultCode.kRealNameRegister://实名注册回调
                break;
            case (int)UserActionResultCode.kAccountSwitchSuccess://切换账号成功回调
                AnySDKLogin.AccountSwitchSuccess(result);
                break;
            case (int)UserActionResultCode.kAccountSwitchFail://切换账号失败回调
                AnySDKLogin.AccountSwitchFail(result);
                break;
            default:
                break;
        }
    }

    //分享回调
    public static void ShareExternalCall(string msg)
    {
        Debug.Log("ShareExternalCall(" + msg + ")");
        Dictionary<string, string> dic = AnySDKUtil.stringToDictionary(msg);
        int code = Convert.ToInt32(dic["code"]);
        string result = dic["msg"];
        switch (code)
        {
            case (int)ShareResultCode.kShareSuccess://分享成功回调
                if (GameManager.GM.IsShareToQuan)
                {
                    BaseProto.Inst().MissionComplete();
                }
                GameManager.GM.IsShareToQuan = false;

                AnySDKShareFun.ShareSuccess(result);
                break;
            case (int)ShareResultCode.kShareFail://分享失败回调
                GameManager.GM.IsShareToQuan = false;
                AnySDKShareFun.ShareFail(result);
                break;
            case (int)ShareResultCode.kShareCancel://分享取消回调
                GameManager.GM.IsShareToQuan = false;
                break;
            case (int)ShareResultCode.kShareNetworkError://分享网络出错回调
                GameManager.GM.IsShareToQuan = false;
                AnySDKShareFun.ShareNetworkError(result);
                break;
            default:
                break;
        }
    }

    //发送登录请求
    public static void SendLogin()
    {
        if (!IsInit)
            return;
        // Dictionary<string, string> info = new Dictionary<string, string>();
        // info["server_id"] = "2";
        // info["server_url"] = "http://xxx.xxx.xxx";
        // info["key1"] = "value1";
        // info["key2"] = "value2";
        // AnySDKUser.getInstance().login(info);

        //if (AnySDKUser.getInstance().isLogined())
        //{
        //    return;
        //}
        AnySDKUser.getInstance().login();
    }

    //登出帐号
    public static void SendLogout()
    {
        if (!isLogin()) return;
        if (AnySDKUser.getInstance().isFunctionSupported("logout"))
        {
            AnySDKUser.getInstance().callFuncWithParam("logout");
        }
    }

    //是否登录成功
    public static bool isLogin()
    {
        if (!IsInit) return false;
        return AnySDKUser.getInstance().isLogined();
    }

    //帐号切换
    public static void SendAccountSwitch()
    {
        if (!isLogin()) return;
        if (AnySDKUser.getInstance().isFunctionSupported("accountSwitch"))
        {
            AnySDKUser.getInstance().callFuncWithParam("accountSwitch");
        }
    }

    //发送分享请求
    public static void SendShare(Dictionary<string, string> info)
    {
        if (!isLogin()) return;
        
        AnySDKShare.getInstance().share(info);
    }

    //得到用户信息
    public static string GetUserInfo()
    {
        if (!isLogin()) return null;
        if (AnySDKUser.getInstance().isFunctionSupported("getUserInfo"))
        {
            return AnySDKUser.getInstance().callStringFuncWithParam("getUserInfo");
        }
        else
        {
            return null;
        }
    }
}
