using UnityEngine;
using System.Collections.Generic;
using anysdk;


/// <summary>
/// AnySDKLogin  --SDK登录响应
///  liuyu		2016.10.24
/// </summary>
public class AnySDKLogin
{

    //登录成功
    public static void LoginSuccess(string _msg)
    {
        AnySDK_C.GetIns().Login();
    }

    //登录网络错误
    public static void LoginNetworkError(string _msg)
    {
    }

    //取消登录
    public static void LoginCancel(string _msg)
    {
    }

    //登录失败
    public static void LoginFail(string _msg)
    {
    }

    //登出成功
    public static void LoginOutSuccess(string _msg)
    {
    }

    //登出失败
    public static void LoginOutFail(string _msg)
    {
    }

    //切换帐号成功回调
    public static void AccountSwitchSuccess(string _msg)
    {
    }

    //切换帐号失败回调
    public static void AccountSwitchFail(string _msg)
    {
    }


}
public class Root
{
    /// <summary>
    /// 
    /// </summary>
    //public int result { get; set; }
    ///// <summary>
    ///// 获取用户信息成功
    ///// </summary>
    //public string msg { get; set; }
    ///// <summary>
    /// 
    /// </summary>
    public string uid { get; set; }
    /// <summary>
    /// 嘉
    /// </summary>
    public string nickname { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string avatarUrl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int sex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string city { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int isVip { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string province { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string country { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<string> privilege { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string unionid { get; set; }
}