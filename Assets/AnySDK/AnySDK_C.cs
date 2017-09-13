using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnySDK_C : MonoBehaviour
{
    public Texture2D texture;
    static AnySDK_C _AnySDK_C;
    public static AnySDK_C GetIns()
    {
        return _AnySDK_C;
    }

    void Awake()
    {
        _AnySDK_C = this;
        AnySDKManager.AnySDKInit(this);
        DontDestroyOnLoad(gameObject);
        StartCoroutine(DownIcon());
    }


    IEnumerator DownIcon()
    {
        yield return null;

        string str = GameManager.GM.JsonPath + "/icon.png";
        ///读取屏幕像素点
        byte[] byt = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(str, byt);
    }




    private void AnySDKCallBack(string msg)
    {
        AnySDKManager.UserExternalCall(msg);
    }

    private void AnySDKShareCallBack(string msg)
    {
        AnySDKManager.ShareExternalCall(msg);
    }

    public void Login()
    {
        string _userInfo = AnySDKManager.GetUserInfo();
        Dictionary<string, object> _userInfoDic = MiniJSON.Json.Deserialize(_userInfo) as Dictionary<string, object>;

        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(_userInfoDic["nickName"].ToString());
        string base64Str = Convert.ToBase64String(byteArray);
        GlobalSettings.LoginUserName = base64Str;

        //GlobalSettings.LoginUserName = (jsonData2["nickname"]).ToString();
        GlobalSettings.LoginUserId = _userInfoDic["uid"].ToString(); ;
        GlobalSettings.LoginChannel = "wx";//(jsonData2["unionid"]).ToString();
        GlobalSettings.avatarUrl = _userInfoDic["avatarUrl"].ToString();
        //Debug.Log("头像" + GlobalSettings.avatarUrl);

        if (uint.Parse(_userInfoDic["sex"].ToString()) == 1)
        {
            GlobalSettings.sex = 1;
        }
        else
        {
            GlobalSettings.sex = 0;
        }
        LoginProcessor.Inst().Login();
    }
}
