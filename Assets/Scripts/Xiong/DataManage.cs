using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using System;


public class DataManage : MonoBehaviour
{
    #region Instance Define
    static DataManage inst;

    static public DataManage Instance
    {
        get
        {

            return inst;
        }
    }
    #endregion

    #region /*———DIC MJ Size And Name———*/
    public Dictionary<string, uint> dicMJSize = new Dictionary<string, uint>();
    public string[] mjName_Ary;
    public uint[] mjCardId_Ary;
    #endregion

    #region/*———麻将房间数据？———*/
    public EnterRoomRsp _roomEnterRsp;
    public CreateRoomRsp _roomCreateRsp;
    public bool isGameEnter_1 = false;
    public EnterRoomRsp SetRoomEnterRsp
    {
        set
        {
            _roomEnterRsp = value;
            isGameEnter_1 = true;
            Room_SetInfo();
        }
    }
    private int roomSyJuShu;//剩余局数
    public int roomJuShu_Max;//局数；
    /// <summary>剩余麻将牌数
    /// </summary>
    public int roomCardNumCount;//
    public uint roomBoosId;//房主ID
    private uint[] roomTouZi;
    public uint[] GetRoomTouZi
    {
        get
        {
            return roomTouZi;
        }
    }
    private string roomInfoNxStr;//房间规则
    public string RoomInfoNxStr
    {
        get
        {
            return roomInfoNxStr;
        }

        set
        {
            roomInfoNxStr = value;
        }
    }

    public int RoomSyJuShu
    {
        get { return roomSyJuShu; }

        set { roomSyJuShu = value; }
    }

    private uint mjXuanQue=0;
    public uint MjXuanQue
    {
        get { return mjXuanQue; }
        set { mjXuanQue = value; }
    }

    [SerializeField]
    private List<Mj_Sx_> all_Mj_ShuXing = new List<Mj_Sx_>();

    #endregion

    #region /*Player DATA*/
    [SerializeField]
    private Player_Data[] all_Player_Data = new Player_Data[4];//

    public Player_Data MyPlayer_Data
    {
        get
        {
            return all_Player_Data[0];
        }
    }

    public delegate void PlayerAry(Player_Data[] pDaArys);
    public PlayerAry onChangePlayerData;
    private Dictionary<uint, Sprite> dicPlayerHead = new Dictionary<uint, Sprite>();
    public uint all_PlayerIndexID;//用来记录进房间的
    #endregion

    #region/*———ChatPhiz———*/
    public Dictionary<string, string> dic_phizConfig = new Dictionary<string, string>();

    public Dictionary<string, string> dic_ChatConfig = new Dictionary<string, string>();

    private string cacheChatRecord = null;
    #endregion

    #region/*———系统数据———*/
    private string strSystemNotice1 = "请设置公告！请设置公告！请设置公告！请设置公告！请设置公告！";
    /// <summary>系统公告
    /// </summary>
    public string StrSystemNotice
    {
        set { strSystemNotice1 = value; }
        get { return strSystemNotice1; }
    }

    private string strSystemBuyDiaMsg = "请设置充值提示！请设置充值提示！请设置充值提示！";
    /// <summary>系统充值提示
    /// </summary>
    public string StrSystemBuyDiaMsg
    {
        set { strSystemBuyDiaMsg = value; }
        get { return strSystemBuyDiaMsg; }
    }

    #endregion
    // Use this for initialization
    void Awake()
    {

        inst = this;
        Init();

    }

    public uint Card_GetSize(string str_Name)
    {
        if (dicMJSize.ContainsKey(str_Name))
        {
            return dicMJSize[str_Name];
        }
        Debug.LogError("ERROR");
        return 0;
    }

    /*——————*/
    string ResPath_Assets = null;

    void Init()
    {

        #region/*MJ HS */
        mjName_Ary = new string[] {
            "1W","2W","3W","4W","5W","6W","7W","8W","9W",
            "1T","2T","3T","4T","5T","6T","7T","8T","9T",
            "1B","2B","3B","4B","5B","6B","7B","8B","9B",
            "1H","2H","3H","4H","5H","6H","7H"
        };
        mjCardId_Ary = new uint[] {
            1,2,3,4,5,6,7,8,9,
            17,18,19,20,21,22,23,24,25,
            33,34,35,36,37,38,39,40,41,
            48,49,50,51,52,53,54
        };
        for (int i = 0; i < mjName_Ary.Length; i++)
        {
            dicMJSize.Add(mjName_Ary[i], mjCardId_Ary[i]);
        }
        #endregion
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            all_Player_Data[i] = new Player_Data();
        }

        //解析表情配置
        //string[] str_TMsgPhiz = Resources.Load<TextAsset>(ResPath_Assets).text.Split('\n');
        //for (int i = 0; i < str_TMsgPhiz.Length; i++)
        //{
        //    if (str_TMsgPhiz[i].IndexOf("|") >= 0)
        //    {
        //        string[] str_Split1 = str_TMsgPhiz[i].Split('|');
        //        if (str_Split1.Length == 2)
        //        {
        //            //all_Seed_Talk.Add(str_Split1[0], str_Split1[1]);
        //            dic_phizConfig.Add(str_Split1[0], str_Split1[1]);
        //        }
        //    }
        //}

        //快捷常用语会根据游戏类型 发生改变——》修改
        //string[] str_TMsgChat = Resources.Load<TextAsset>(ResPath_Assets.txt_T_MsgChat).text.Split('\n');
        //for (int i = 0; i < str_TMsgChat.Length; i++)
        //{
        //    if (str_TMsgChat[i].IndexOf("|") >= 0)
        //    {
        //        string[] str_Split1 = str_TMsgChat[i].Split('|');
        //        if (str_Split1.Length == 2)
        //        {
        //            dic_ChatConfig.Add(str_Split1[0], str_Split1[1]);
        //        }
        //    }
        //}
    }

    #region /*———关于玩家数据信息———*/

    /// <summary>获取玩家在数组中的座位号
    /// </summary>
    public int PData_GetIndex(uint p_ID)
    {
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i] != null && p_ID == all_Player_Data[i].p_ID)
            {
                return i;
            }
        }
        Debug.Log("<color=red>执行到这里，你传入的ID,并没有在当前房间玩家数据中找到！！！</color>" + p_ID);
        return -1;
    }

    public Player_Data PData_GetData(uint p_ID)
    {
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i] != null && p_ID == all_Player_Data[i].p_ID)
            {
                return all_Player_Data[i];
            }
        }
        Debug.Log("<color=red>执行到这里，你传入的ID,并没有在当前房间玩家数据中找到！！！</color>" + p_ID);
        return null;
    }
    public Player_Data[] PData_GetDataAry()
    {
        return all_Player_Data;

    }
    /// <summary>Get i_Seat==[AryIndex] Get Player ID
    /// </summary>
    public uint PData_GetSeatID(int i_Seat)
    {
        if (i_Seat < 0 || i_Seat > 3)
        {
            Debug.LogError("Error PData_GetSeatID SeatValue:" + i_Seat);
        }
        return all_Player_Data[i_Seat].p_ID;
    }
    public void PData_SetAdress(uint charid,Vector2 value)
    {
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i].p_ID == charid)
            {
                all_Player_Data[i].adress = value;
            }
        }
    }

    /// <summary>当进入房间，接收到数据数组的时候，批量加入
    /// </summary>
    public void PData_Update(Player_Data[] p_Data)
    {
        all_Player_Data = p_Data;
        onChangePlayerData(all_Player_Data);
    }

    public int PData_Add(Player_Data p_Data)
    {
        //for (int i = 0; i < all_Player_Data.Length; i++)
        //for (int i = (int) MyPlayer_Data.position; i >0; i--)
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i].p_ID == p_Data.p_ID)
            {
                return 0;
            }
        }
        for (int i = 0; i < all_Player_Data.Length; i++)
        {

            if (all_Player_Data[i].p_ID == 0)
            {
                all_Player_Data[i] = p_Data;
                break;
            }
        }
        onChangePlayerData(all_Player_Data);
        return 0;
    }

    public void PData_InitMyData(Player_Data p_Data)
    {
        all_Player_Data[0] = p_Data;
    }

    /// <summary>当玩家退出房间，
    /// </summary>
    public void PData_Reduce(uint p_Id)
    {
        if (p_Id != 0)
        {
            if (GameManager.GM.DS.InGameManage_3DMjUI != null)
                GameManager.GM.DS.InGameManage_3DMjUI.GetComponent<InGameManage_3DMjUI>().needMJ_Ui.Prep_OpenPrepStyle(DataManage.Instance.PData_GetIndex(p_Id), false);
        }
        for (int i = 1; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i].p_ID == p_Id)
            {
                all_Player_Data[i] = new Player_Data();
                break;
            }
        }
        onChangePlayerData(all_Player_Data);
    }

    /// <summary>清楚其他玩家在数组里面的数据
    /// </summary>
    public void PData_RemoveOtherPlayerData()
    {
        for (int i = 1; i < all_Player_Data.Length; i++)
        {
            all_Player_Data[i] = new Player_Data();
        }
    }

    /// <summary>玩家座位排序
    /// </summary>
    public Player_Data[] PData_Rank(Player_Data[] pDataAry)
    {
        for (int i = 0; i < pDataAry.Length; i++)
        {
            if (pDataAry[i].p_ID == BaseProto.playerInfo.m_id)
            {
                Player_Data[] pDaZL = new Player_Data[pDataAry.Length];
                int i_Index = 0;
                for (int i_1 = i; i_1 < pDataAry.Length; i_1++)
                {
                    pDaZL[i_Index] = pDataAry[i_1];
                    i_Index++;
                }
                for (int i_2 = 0; i_2 < i; i_2++)
                {
                    pDaZL[i_Index] = pDataAry[i_2];
                    i_Index++;
                }
                pDataAry = pDaZL;
                break;
            }
        }
        return pDataAry;
    }

    public void PData_SetReady(uint charID, bool isPrep)
    {
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            if (all_Player_Data[i] != null && all_Player_Data[i].p_ID == charID)
            {
                all_Player_Data[i].isReady = isPrep;
            }
        }
    }

    #endregion
    #region/*———关于玩家头像———*/

    public Sprite Head_GetSprite(uint charId)
    {
        if (dicPlayerHead.ContainsKey(charId))
        {
            return dicPlayerHead[charId];
        }
        return null;
    }

    /// <summary>头像 -增加头像缓存
    /// </summary>
    public void Head_AddSprite(uint charId, Sprite spriteHead)
    {
        if (!dicPlayerHead.ContainsKey(charId))
        {
            dicPlayerHead.Add(charId, spriteHead);
        }
    }

    /// <summary>销毁除开本地玩家的头像缓存
    /// </summary>
    public void Head_DeleteOtherPlayerSprite()
    {
        uint my_ID = all_Player_Data[0].p_ID;
        if (dicPlayerHead.ContainsKey(my_ID))
        {
            Sprite my_HeadSprite = dicPlayerHead[my_ID];
            dicPlayerHead.Clear();
            dicPlayerHead.Add(my_ID, my_HeadSprite);
        }
    }

    public IEnumerator W3_Request_Tx(uint p_ID, string url, Action<Sprite, int> act, int num)
    {
        //if (url.IndexOf("http") < 0)
        //{
        //    url = @url;
        //}
        if (dicPlayerHead.ContainsKey(p_ID))
        {
            act(Head_GetSprite(p_ID), num);
        }
        else
        {
            //Open_Prompt("加载玩家头像！ 头像地址：" + url);
            WWW w3 = new WWW(url);
            yield return w3;
            if (w3.error == null)
            {
                if (w3.texture != null)
                {
                    Sprite sprit_Tx = Sprite.Create(w3.texture, new Rect(0, 0, w3.texture.width, w3.texture.height), Vector2.one * 0.5f);
                    act(sprit_Tx, num);
                    Head_AddSprite(p_ID, sprit_Tx);
                }
            }
            else
            {
                //Open_Prompt("加载玩家头像失败！ 头像地址：" + url);
                // p_Tx.sprite = Resources.Load<Sprite>("tx_1");
                Head_AddSprite(p_ID, Resources.Load<Sprite>("Sprites/Test_Head"));
                act(Resources.Load<Sprite>("Sprites/Test_Head"), num);
                Debug.Log("<color=red>WWW Request Head Sprite Error!</color>" + url);
            }
        }

        yield return null;
    }

    #endregion

    #region/*———关于表情配置———*/


    public string Phiz_GetPath(string strPhizName)
    {
        if (dic_phizConfig.ContainsKey(strPhizName))
        {
            return dic_phizConfig[strPhizName];
        }
        return "";
    }
    public string[] Phiz_GetConfig()
    {
        string[] strAry = new string[dic_phizConfig.Count];
        int i_Index = 0;
        foreach (KeyValuePair<string, string> item in dic_phizConfig)
        {
            strAry[i_Index] = item.Key;
            i_Index++;
        }
        return strAry;
    }

    /// <summary>常用语配置初始化 !-该方法防止不同游戏房间的常用语不一样-!
    /// </summary>
    public void Chat_Init()
    {
        dic_ChatConfig.Clear();
        string audioConfigPath = audioConfigPath = "TxtConfigs/";

        if (audioConfigPath.Length < 1)
        {
            Debug.Log("<color=red>常用语路径为空</color>" + audioConfigPath);
            return;
        }
        string[] str_TMsgChat = Resources.Load<TextAsset>(audioConfigPath).text.Split('\n');

        for (int i = 0; i < str_TMsgChat.Length; i++)
        {
            if (str_TMsgChat[i].IndexOf("|") >= 0)
            {
                string[] str_Split1 = str_TMsgChat[i].Split('|');
                if (str_Split1.Length == 2)
                {
                    dic_ChatConfig.Add(str_Split1[0], str_Split1[1]);
                }
            }
        }
    }

    public string Chat_GetChat(string strPhizName)
    {
        if (dic_ChatConfig.ContainsKey(strPhizName))
        {

            return dic_ChatConfig[strPhizName];
        }
        return "";
    }
    public string Chat_GetChatPath(string strValue)
    {
        strValue = strValue.Replace("\t", "");
        foreach (KeyValuePair<string, string> item in dic_ChatConfig)
        {
            if (item.Value.IndexOf(strValue) >= 0 || strValue.IndexOf(item.Value) >= 0)
            {
                return item.Key;
            }
        }

        return "";
    }

    public List<string[]> Chat_GetConfig()
    {
        List<string[]> strAry = new List<string[]>();
        //int i_Index = 0;
        foreach (KeyValuePair<string, string> item in dic_ChatConfig)
        {
            strAry.Add(new string[] { item.Key, item.Value });
        }
        return strAry;
    }

    public void Chat_AddZDY(string str_)
    {

    }

    /// <summary>添加聊天记录缓存
    /// </summary>
    public void ChatRecord_Add(string senderName,string strChat)
    {
        cacheChatRecord += senderName+":"+strChat + "\n";
        if (GameManager.GM.DS.UsedChat!=null)
        {
            GameManager.GM.DS.UsedChat.GetComponent<UI_UsedChat>().SetHistoryContent(cacheChatRecord);
        }
    }
    /// <summary>聊天记录获取， 并清除缓存。
    /// </summary>
    public string ChatRecord_Get()
    {
        return cacheChatRecord;
    }
    public void ClearHistoryChat()
    {
        cacheChatRecord = "";
    }
    #endregion

    #region/*———MJ_SX———*/

    public void MJSX_Add(Mj_Sx_ mj_Sx)
    {
        if (all_Mj_ShuXing.Contains(mj_Sx))
        {//已经存储过该麻将属性
            return;
        }

        all_Mj_ShuXing.Add(mj_Sx);

    }

    public void MJSX_Clear()
    {
        all_Mj_ShuXing.Clear();
    }

    public Mj_Sx_ MJSX_Get(string mj_Name)
    {
        Mj_Sx_ mj_SX = new Mj_Sx_();
        for (int i = 0; i < all_Mj_ShuXing.Count; i++)
        {
            if (all_Mj_ShuXing[i].mj_SpriteName == mj_Name)
            {
                return all_Mj_ShuXing[i];
            }
        }
        Debug.LogError("似乎没有你想要的麻将属性？？" + mj_Name);
        return mj_SX;
    }

    public Mj_Sx_ MJSX_Get(uint mj_Uint)
    {
        Mj_Sx_ mj_SX = new Mj_Sx_();
        for (int i = 0; i < all_Mj_ShuXing.Count; i++)
        {
            if (all_Mj_ShuXing[i].mJCardID == mj_Uint)
            {
                return all_Mj_ShuXing[i];
            }
        }
        Debug.LogError("似乎没有你想要的麻将属性？？" + mj_Uint);
        return mj_SX;
    }
    #endregion

    #region/*———关于房间的数据———*/

    /// <summary>部分房间规则解析成文字，用UI显示
    /// </summary>
    public bool RuleH3Z = false;
    public bool RuleQue = true;
    public string rule = "";
    private void Room_SetInfo()
    {
        rule = "";
        for (int i = 0; i < _roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.flags.Count; i++)
        {
            switch (_roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.flags[i])
            {
                case XZDDRule.XZDD.ZMJD:
                    rule += " 自摸加底";
                    break;
                case XZDDRule.XZDD.ZMJF:
                    rule += " 自摸加番";
                    break;
                case XZDDRule.XZDD.DYJJD:
                    rule += " 带幺九将对";
                    break;
                case XZDDRule.XZDD.H3Z:
                    rule += " 换三张";
                    RuleH3Z = true;
                    break;
                case XZDDRule.XZDD.DGH_ZM:
                    rule += " 点杠胡自摸";
                    break;
                case XZDDRule.XZDD.DGH_FP:
                    rule += " 点杠胡放炮";
                    break;
                case XZDDRule.XZDD.TDH:
                    rule += " 天地胡";
                    break;
                case XZDDRule.XZDD.MQZZ:
                    rule += " 门清中张";
                    break;
                case XZDDRule.XZDD.HJZY:
                    rule += " 呼叫转移";
                    break;
                case XZDDRule.XZDD.ChampionPay:
                    rule += " 冠军房费";
                    break;
                default:
                    break;
            }
        }
        //Debug.LogError("当前的规则数量:"+ _roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.flags.Count);
        if (rule == "")
            roomInfoNxStr = "当前的规则数量:"+ _roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.flags.Count +" 规则没有传入";
        else
            roomInfoNxStr = "房间规则:" + rule;
        roomSyJuShu = (int)_roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.roundNum;
        roomJuShu_Max = (int)_roomEnterRsp.mjRoom.maxRoundNum;
    }

    public void SetRoomTouZi(List<uint> guiCards)
    {
        roomTouZi = guiCards.ToArray();
    }

    #endregion
}
