using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;
using ProtoBuf;
using System.IO;
using System;
using System.Threading;

public delegate void SetSprite(Sprite sprite, int num = 0);

public struct _PlayerInfo
{
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 玩家唯一ID
    /// </summary>
    public uint ID;
    /// <summary>
    /// 玩家IP
    /// </summary>
    public string IP;
    /// <summary>
    /// 玩家头像
    /// </summary>
    public string Head;
    /// <summary>
    /// 玩家钻石
    /// </summary>
    public uint Diamond;
    /// <summary>
    /// 钱
    /// </summary>
    public int Money;

    public int sex;


    public _PlayerInfo(string Name, uint ID, string IP, string Head, uint Diamond, int Money, int sex)
    {
        this.Name = Name;
        this.ID = ID;
        this.IP = IP;
        this.Head = Head;
        this.Diamond = Diamond;
        this.Money = Money;
        this.sex = sex;
    }
}


public class GameManager : MonoBehaviour
{
    static public WaitForEndOfFrame waitForEndOfFrame=new WaitForEndOfFrame();
    static public WaitForSeconds wait02=new WaitForSeconds(0.2f);
    static public WaitForSeconds wait01 = new WaitForSeconds(0.1f);
    static public WaitForSeconds wait1 = new WaitForSeconds(1.0f);
    public RectTransform Canvas;

    public static GameManager GM;
    public DataStorage DS;
    public bool ingame = false;
    public bool GameEnd = false;
    public QueryInfoRsp combatGainRsp;
    public QueryInfoRsp qpcombatGainRsp;
    Dictionary<string, Sprite> HeadCaChe = new Dictionary<string, Sprite>();
    /// <summary>
    /// 屏幕高的一半
    /// </summary>
    public float SHeight;
    public float SWide;
    public GameSceneType GmType;
    public string JsonPath
    {
        get
        {
            string path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer)//判断平台
            {
                //path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);//ios 平台 就会获取documents路径
                //path = path.Substring(0, path.LastIndexOf('/')) + "/Documents/";
                path = Application.persistentDataPath;//安卓平台
            }
            else
            {
                path = Application.persistentDataPath;//安卓平台
            }
            return path;
        }
    }
    public string ToName(string base64Str)
    {
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
    }
    public List<GameObject> UIList = new List<GameObject>();
    public void DestoryUI()
    {
        int C = GameManager.GM.UIList.Count;
        for (int i = 0; i < C; i++)
        {
            if (UIList[i] != null)
                Destroy(UIList[i]);
        }
    }
    public bool IsLogined = false;
    public void ReLogin()
    {
        StartCoroutine(DestoryAllUI());
    }
    public int ReLoginTimes = 0;
    IEnumerator DestoryAllUI()
    {
        BaseProto.playerInfo.m_atRoomId = 0;
        BaseProto.playerInfo.m_cdRoomId = 0;
        Lang.NetReceiver.Inst.OnNetClosed();
        ParticleManager.GetIns.MainBg.SetActive(true);
        if (GameManager.GM.DS.MJGameController != null)
            Destroy(GameManager.GM.DS.MJGameController);
        if (GameManager.GM.DS.InGameManage_3DMjUI != null)
            Destroy(GameManager.GM.DS.InGameManage_3DMjUI);
        //PublicEvent.GetINS.AppLoginOut();
        GameManager.GM.IsLogined = false;
        while (!GameManager.GM.IsLogined)
        {
            //ReLoginTimes++;
            LoginProcessor.Inst().Login();
            Debug.Log("asd");
            yield return new WaitForSeconds(2.0f);
        }
    }
    public void UpDateDiamond(uint value)
    {
        _AllPlayerData[0].Diamond = value;
    }
    /// <summary>
    /// 当前是几人麻将模式就把这个数字改为几人
    /// </summary>
    public static uint PlayerNum = 4;
    public int LastGain0 = 0, LastGain1 = 0, LastGain2 = 0;
    public int ReplayRecordsNum = 0;
    public int ReplayRoundsNum = 0;
    public string MyHead;

    public void GetHead(string url, SetSprite Action, int num = 0)
    {
        //if (HeadCaChe.ContainsKey(url))
        //{
        //    Action(HeadCaChe[url], num);
        //}
        //else
        //{
        //if (PlayerPrefs.HasKey(url))
        //{
        //    var file = PlayerPrefs.GetString(url);
        //    StartCoroutine(LocalLoadSprite(file, url, Action, num));
        //}
        //else
        {
            StartCoroutine(LoadSprite(url, Action, num));
        }
        //}
    }
    IEnumerator LocalLoadSprite(string file, string url, SetSprite Action, int num = 0)
    {
#if UNITY_EDITOR
        WWW www = new WWW("file:///" + file);
#else
        WWW www = new WWW("file://" + file);
#endif
        yield return www;
        if (!www.isDone && www.texture == null)
        {
            PlayerPrefs.DeleteKey(file);
            yield return LoadSprite(url, Action, num);
        }
        else
        {
            var te = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            Action(te, num);
            HeadCaChe.Add(url, te);
        }
    }
    IEnumerator LoadSprite(string url, SetSprite Action, int num = 0)
    {
        if (HeadCaChe.ContainsKey(url))
        {
            Action(HeadCaChe[url], num);
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www != null)
                if (www.texture != null)
                {
                    var te = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 50);
                    string str = Application.persistentDataPath + GetTimeStr();
                    Action(te, num);
                    HeadCaChe.Add(url, te);
                }
        }
    }

    void SetHead(Sprite sprite)
    {
        // string url = "";
        //   GameManager.GM.GetHead(url, SetHead);
        Image head = null;

        head.sprite = sprite;
    }

    /// <summary>
    /// 当前游戏所有玩家的数据
    /// </summary>
    public _PlayerInfo[] _AllPlayerData = new _PlayerInfo[4];
    public QueryInfoRsp rsp_Save = null;
    public int GetPlayerNum(uint palyerID)
    {
        for (int i = 0; i < _AllPlayerData.Length; i++)
        {
            if (_AllPlayerData[i].ID == palyerID)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetPlayerSex(uint playerID)
    {
        for (int i = 0; i < _AllPlayerData.Length; i++)
        {
            if (_AllPlayerData[i].ID == playerID)
            {
                return _AllPlayerData[i].sex;
            }
        }
        return 1;
    }
    public string GetPlayerName(uint playerID)
    {
        for (int i = 0; i < _AllPlayerData.Length; i++)
        {
            if (_AllPlayerData[i].ID == playerID)
            {
                return _AllPlayerData[i].Name;
            }
        }
        return "缺省";
    }
    public string GetPlayerIp(uint playerID)
    {
        for (int i = 0; i < _AllPlayerData.Length; i++)
        {
            if (_AllPlayerData[i].ID == playerID)
            {
                return _AllPlayerData[i].IP;
            }
        }
        return "192";
    }
    public void MyPlayerData(AccountLoginRsp Rsp)
    {
        rsp_Save = null;

        _AllPlayerData[0] = new _PlayerInfo(ToName(Rsp.userName), Rsp.charId, Rsp.ip, MyHead, Rsp.diamond, (int)Rsp.gold, (int)GlobalSettings.sex);
        Debug.Log("名字" + _AllPlayerData[0].Name);
    }


    /// <summary>
    /// 客户端进入之后，有玩家进入
    /// </summary>
    /// <param name="info"></param>
    public void JoinPlayerData(ProtoBuf.CharacterInfo info)
    {
        bool none = true;
        for (int i = GameManager.GM._AllPlayerData.Length - 1; i > 0; i--)
        {
            if (_AllPlayerData[i].ID == info.charId)//如果这个人存在
            {
                none = false;
            }

        }
        if (none)
            for (int i = GameManager.GM._AllPlayerData.Length - 1; i > 0; i--)
            {
                if (_AllPlayerData[i].ID == 0)//如果没有人
                {
                    _AllPlayerData[i] = new _PlayerInfo(ToName(info.userName), info.charId, info.ip, info.portrait, info.diamond, (int)info.gold, (int)info.sex);
                    break;
                }

            }

        Player_Data new_ = new Player_Data();
        new_.p_ID = info.charId;
        new_.P_Name = GameManager.GM.ToName(info.userName);
        new_.p_TxPath = info.portrait;
        new_.p_gold = (int)info.gold;
        new_.p_Ip = info.ip;
        new_.sex = (int)info.sex;
        DataManage.Instance.PData_Add(new_);

        //PublicEvent.GetINS.Fun_JoinRoomPlayerUpdata();
        //Debug.Log("客户端进入之后，有玩家进入PlayerNum:" + BaseProto.SeverPlayerNum);
    }
    /// <summary>
    ///已经指定了位置的
    /// </summary>
    /// <param name="info"></param>
    /// <param name="num"></param>
    public void ReSetAllPlayerData(ProtoBuf.CharacterInfo info, int num)
    {
        Debug.Log("重新进入房间之后，之前玩家的状态" + "Player人数：" + BaseProto.SeverPlayerNum);
        _AllPlayerData[num] = new _PlayerInfo(ToName(info.userName), info.charId, info.ip, info.portrait, info.diamond, (int)info.gold, (int)info.sex);
    }
    public void DeletePlayerData(uint PlayerID)
    {
        if (PlayerID != BaseProto.playerInfo.m_id)
            for (int i = 0; i < _AllPlayerData.Length; i++)
            {
                if (_AllPlayerData[i].ID == PlayerID)
                {
                    _AllPlayerData[i].ID = 0;
                    _AllPlayerData[i].IP = i.ToString();
                    break;
                }
            }
    }
    public uint[] SortAllPlayerList(List<uint> player)
    {
        uint[] TempList = new uint[4];
        int TempNum = 0;
        for (int i = 0; i < player.Count; i++)
        {
            if (player[i] == BaseProto.playerInfo.m_id)
            {
                TempNum = i;
                break;
            }
        }
        int z = 0;
        for (int i = TempNum; i < 4; i++)
        {
            for (; z < TempList.Length;)
            {
                if (i < player.Count)
                {
                    TempList[z] = player[i];
                }
                else
                {
                    TempList[z] = 0;
                }
                z++;
                break;
            }
        }
        for (int i = 0; i < TempNum; i++)
        {
            for (; z < TempList.Length;)
            {
                if (i < player.Count)
                    TempList[z] = player[i];
                else
                    TempList[z] = 0;
                z++;
                break;
            }
        }
        string Paixu = "";
        for (int i = 0; i < TempList.Length; i++)
        {
            Paixu += TempList[i] + "  ";
        }
        Debug.Log("位置:" + Paixu);
        return TempList;
    }

    // Use this for initialization
    void Awake()
    {
        //NetUpdat.NetUpdate();
        GM = this;
        
    }
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Canvas = this.GetComponent<RectTransform>();
        Resources.Load<GameObject>(ResPath.PlayerInfo);
        GameNetWork.Inst().Init();
        LoginProcessor.Inst().Init();
        SHeight = GetComponent<RectTransform>().sizeDelta.y;
        SWide = GetComponent<RectTransform>().sizeDelta.x;
       
    }
   
    /// <summary>
    /// 输入地址然后返回预制件，记得注册这个，并且记得回收
    /// </summary>
    /// <param name="adress">ResPath</param>
    /// <returns></returns>
    public GameObject PopUI(string adress, bool SetParentToCanvas = true)
    {
        GameObject Temp = null;
        var t = Resources.Load<GameObject>(adress);
        Temp = Instantiate(t);
        Temp.SetActive(true);
        if (adress != ResPath.Main)
            UIList.Add(Temp);
        if (SetParentToCanvas)
            Temp.transform.SetParent(this.transform, false);
        Temp.transform.SetAsLastSibling();
        return Temp;
    }
    string GetTimeStr()
    {
        return System.DateTime.Now.Year + "" + System.DateTime.Now.Month + "" + System.DateTime.Now.Day + "" + System.DateTime.Now.Hour + "" + System.DateTime.Now.Minute + "" + System.DateTime.Now.Second + ".png";
    }
    IEnumerator open()
    {
        //GameObject Temp = null;
        yield return new WaitForSeconds(0.2f);

        //Temp = Instantiate(Resources.Load<GameObject>(ResPath.Main));
        DataStorage.GetIns.Main.SetActive(true);
        //Temp.transform.SetParent(this.transform, false);
        DataStorage.GetIns.Main.transform.SetAsLastSibling();

    }
    float i = 0;
    public string GameType;

    void Update()
    {
        i += Time.deltaTime;
        if (i > 0.3f)
        {
            i = 0;
            LoginProcessor.Inst().Update();
            GameNetWork.Inst().Update();
        }
    }
    //void FixedUpdate()
    //{

    //}

    void OnDestroy()
    {
        LoginProcessor.Inst().UnInit();
        GameNetWork.Inst().UnInit();
    }





    /// <summary>
    /// 是否分享成功
    /// </summary>
    public bool IsShareToQuan = false;
    /// <summary>
    /// 邀请分享
    /// </summary>
    /// <param name="num"></param>
    /// 
    //public void Share(int num)
    //{
    //    StartCoroutine(WorkToShare(num));
    //}

    public void Share(int num)
    {
        StartCoroutine(WorkToShare(num));
    }


    IEnumerator WorkToShare(int nums)
    {
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        Dictionary<string, string> info = new Dictionary<string, string>();

        // 读取屏幕像素信息并存储为纹理数据， 

        Debug.Log("开始截图1");
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        yield return GameManager.waitForEndOfFrame;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //yield return new WaitForEndOfFrame ();

        screenShot.Apply();
        screenShot.Compress(false);
        Debug.Log("截图1");
        // 然后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        var filename = GameManager.GM.JsonPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        //yield return new WaitForEndOfFrame ();
        info["mediaType"] = "1"; //分享类型： 0-文字 1-图片 2-网址  
        info["shareTo"] = nums.ToString(); //分享到：0-聊天 1-朋友圈 2-收藏  
        info["imagePath"] = filename;
        Debug.Log("截图2");
        if (Application.platform == RuntimePlatform.IPhonePlayer)//判断平台
        {
            info["thumbImage"] = GameManager.GM.JsonPath + "/icon.png";
        }
        else
        {
            info["thumbSize"] = "64";
        }
        AnySDKManager.SendShare(info);
        // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
        //		return filename;
    }



    /// <summary>
    /// 邀请，分享链接
    /// </summary>
    /// <param name="GameType">游戏类型</param>
    /// <param name="roundNum">回合</param>
    /// <param name="gameRule">游戏规则</param>
    public void ShareLink(string GameType, string roundNum, string gameRule, string roomnum)
    {
        Debug.Log(gameRule);
        Onyaoqing(GameType, roundNum, gameRule, roomnum);
    }

    void Onyaoqing(string GameType, string roundNum, string gamerule, string roomnum)
    {
        Dictionary<string, string> info = new Dictionary<string, string>();
        info["mediaType"] = "2"; //分享类型： 0-文字 1-图片 2-网址  
        info["shareTo"] = "0"; //分享到：0-聊天 1-朋友圈 2-收藏  

        info["title"] = "休闲茶馆-" + GameType + ",房间号:" + roomnum;
        info["imagePath"] = Application.persistentDataPath + "/icon.png";
        info["url"] = "xiuxianchaguan.com";
        info["text"] = roundNum + "局." + GameType + gamerule;

        if (Application.platform == RuntimePlatform.IPhonePlayer)//判断平台
        {
            info["thumbImage"] = Application.persistentDataPath + "/icon.png";
        }
        else
        {
            info["thumbSize"] = "64";
        }
        AnySDKManager.SendShare(info);
    }

    bool isPause = false;
    void OnApplicationFocus()//(bool isPause)
    {
        //if (isPause)
        //{
        //    Debug.Log("游戏暂停 一切停止");  // 缩到桌面的时候触发  
        //}
        //else
        //{

        Debug.Log("游戏开始  万物生机");  //回到游戏的时候触发 最晚  

        //GameManager.GM.DeletePlayerData(BaseProto.playerInfo.m_id);
        if (DS.Login == null)
        {
            StopCoroutine("ReloginOrQuit");
            StartCoroutine("ReloginOrQuit");
            MJProto.Inst().RequestIsDisConnect();
        }
        // }
    }
    public void StopRelogin()
    {
        Debug.LogWarning("停止~");
        IsRelogin = false;
        StopCoroutine("ReloginOrQuit");
        StopCoroutine("DestoryAllUI");
    }
    //public void SentRelogin()
    //{
    //    if (GameManager.GM.IsLogined)
    //    {
    //        //MJProto.Inst().RequestIsDisConnect();
    //        Debug.Log("请求");
    //        GameManager.GM.DS.Notic = GameManager.GM.PopUI(ResPath.Notic);
    //        GameManager.GM.DS.Notic.GetComponent<UI_Notic>().SetMessage("客户端与服务端已断开连接,正在重新连接！");
    //        //GameManager.GM.DS.Notic.GetComponent<UI_Notic>().QuitApp();
    //        GameManager.GM.ReLogin();
    //    }
    //}
    bool IsRelogin = true;
    IEnumerator ReloginOrQuit()
    {
        yield return new WaitForSeconds(3.0f);
        if (IsRelogin)
        {
            GameNetWork.Inst().Init();
            LoginProcessor.Inst().Init();
            Debug.Log("响应");
            GameManager.GM.DS.Notic = GameManager.GM.PopUI(ResPath.Notic);
            GameManager.GM.DS.Notic.GetComponent<UI_Notic>().SetMessage("客户端与服务端已断开连接,正在重新连接！");
            //GameManager.GM.DS.Notic.GetComponent<UI_Notic>().QuitApp();
            GameManager.GM.ReLogin();

            yield return new WaitForSeconds(5.0f);
            if (!GameManager.GM.IsLogined)
                Application.Quit();
        }
    }
}
