using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UI_PlayBack : MonoBehaviour
{
    public Text CenterTime = null;
    Transform ThisTrans = null;
    /// <summary>
    /// 标识器的上下左右
    /// </summary>
    //public GameObject up, left, down, right;
    public Button Quit, Setting;
    public GameObject XZDD, GAMJ;
    public Text RoomNum;
    public Button Play, Pause, RePlay;
    public Text RealtyTime;
    void Awake()
    {
        Init();
        Reg();
    }
    // Use this for initialization
    void Start()
    {
        if (GameManager.GM.DS.Combat != null)
        {
            Destroy(GameManager.GM.DS.Combat);
            GameManager.GM.DS.Combat = null;
        }
        if (GameManager.GM.DS.CombatItems != null)
        {
            Destroy(GameManager.GM.DS.CombatItems);
            GameManager.GM.DS.CombatItems = null;
        }
        GameManager.GM.DS.PlayBack = gameObject;
        GameManager.GM.ingame = true;
        Quit.onClick.AddListener(PressedQuit);
        Setting.onClick.AddListener(PressedSetting);
        Play.onClick.AddListener(PressedPlay);
        Pause.onClick.AddListener(PressedPause);
        //RePlay.onClick.AddListener(PressedRePlay);
        Default();
        ReFreshRealtyTime();
    }
    void Init()
    {
        ThisTrans = this.gameObject.transform;
    }
    void Default()
    {
        DefaultPlayerInformation();
        //SetNavigation(0);
    }
    /// <summary>
    /// 初始化玩家信息
    /// </summary>
    void DefaultPlayerInformation()
    {
        for (int i = 0; i < BaseProto.SeverPlayerNum; i++)
        {
            ThisTrans.Find("Players/Head" + i + "/Zhuang").gameObject.SetActive(false);
        }
    }
    Image HeadSprite = null;
    void SetPlyerInformation(string name, string gold, string head, int Pos = 0)
    {
        GameManager.GM.GetHead(head, SetHead, Pos);
        ThisTrans.Find("Players/Head" + Pos + "/Name").GetComponent<Text>().text = name;
        ThisTrans.Find("Players/Head" + Pos + "/Gold/Text").GetComponent<Text>().text = gold;
        ThisTrans.Find("Players/Head" + Pos + "/Zhuang").gameObject.SetActive(false);
    }

    void PlayerComing(string name, string gold, string head, int pos)
    {
        SetPlyerInformation(name, gold, head, pos);
    }
    void ShowZhuang(uint PlayerID)
    {
        int i = GameManager.GM.GetPlayerNum(PlayerID);
        Debug.Log("第" + i + "个玩家");
        ThisTrans.Find("Players/Head" + i + "/Zhuang").gameObject.SetActive(true);
    }

    void SetHead(Sprite sprite, int num = 0)
    {
        ThisTrans.Find("Players/Head" + num + "/Mask/HeadSprite").GetComponent<Image>().sprite = sprite;
    }

   
    void Reg()
    {
       
    }



    #region 游戏外按钮点击事件
    void PressedQuit()
    {
        ShowFace.Ins.DisAllAnim();
        ParticleManager.GetIns.SwitchSence(1);
        Debug.Log("退出到上一级");
        Invoke("Rest", 0.2f);
        //GameManager.GM.DS.IsVote = GameManager.GM.PopUI(ResPath.IsQuit);
    }
    void PressedSetting()
    {
        var temp = Resources.Load("PopUI/Setting") as GameObject;
        temp = Instantiate(temp);
        temp.transform.SetParent(transform.parent, false);

        temp.transform.SetAsLastSibling();
    }
    void PressedPlay()
    {
        isbofang = true;
    }
    void PressedPause()
    {
        isbofang = false;
    }
    #endregion
    /// <summary>
    /// 接受服务器发给我的信息,不包含游戏内玩家的顺序之类的信息，那些数据是通过PlayerUpdata来进行的
    /// </summary>
    /// <param name="rsp"></param>
    ProtoBuf.MJRoundRecord RoomRsp = null;

    public bool DingQue = false, HSZ = false;
    public void ReciveRoomData(ProtoBuf.MJRoundRecord rsp)
    {
        RoomRsp = rsp;
        RoomNum.text = "房间号:" + rsp.playBack.roomId.ToString();
        if (rsp.playBack.gameType == ProtoBuf.GameType.GT_Poker)
        {
            GameManager.GM.GameType = "ga";
            GAMJ.SetActive(true);
            XZDD.SetActive(false);
        }
        if (rsp.playBack.gameType == ProtoBuf.GameType.GT_MJ)
        {
            GameManager.GM.GameType = "xz";

            GAMJ.SetActive(false);
            XZDD.SetActive(true);

        }
        int[] te = new int[GameManager.PlayerNum];
        for (int i = 0; i < RoomRsp.players.Count; i++)
        {
            if (RoomRsp.players[i].charId == BaseProto.playerInfo.m_id)
            {
                //i是数据中玩家的位置 自己坐到0号座位
                te[i] = 0;
                int num = 3;
                //重排TE[] 
                for (int j = i + 1; j < GameManager.PlayerNum; j++)                  /*自身为0 0123    0321 */
                {                                                                   /*自身为1 3012    1032   2103*/
                    te[j] = num;
                    num--;
                }                for (int j = 0; j < i; j++)
                {
                    te[j] = num;
                    num--;
                }
                break;
            }
        }
        for (int i = 0; i < RoomRsp.players.Count; i++)
        {
            GameManager.GM._AllPlayerData[te[i]].Name = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(RoomRsp.players[i].name));
            GameManager.GM._AllPlayerData[te[i]].ID = RoomRsp.players[i].charId;
            GameManager.GM._AllPlayerData[te[i]].Head = RoomRsp.players[i].portrait;
            GameManager.GM._AllPlayerData[te[i]].IP = RoomRsp.players[i].ip;
            GameManager.GM._AllPlayerData[te[i]].Money = RoomRsp.players[i].gold;
            GameManager.GM._AllPlayerData[te[i]].sex = (int)RoomRsp.players[i].sex;
        }
        PlayerUpdata(rsp);
        StartGo();
    }
    /// <summary>
    /// 排序之后刷新首次进入当前房间内的玩家数据
    /// </summary>
    void PlayerUpdata(ProtoBuf.MJRoundRecord rsp)
    {
        Debug.Log("玩家数据更新！");
        for (int i = 0; i < 4; i++)
        {
            var t = ThisTrans.Find("Players/Head" + i).gameObject;
            if (rsp != null)
            {
                int pos = i;
                t.GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (GameManager.GM.DS.PlayerInfo != null)
                    {
                        Destroy(GameManager.GM.DS.PlayerInfo);
                        GameManager.GM.DS.PlayerInfo = null;
                    }
                    if (GameManager.GM.DS.PlayerInfo == null)
                    {
                        var temp = GameManager.GM.PopUI(ResPath.PlayerInfo);
                        GameManager.GM.DS.PlayerInfo = temp;
                        UI_PlayerInfo player = temp.GetComponent<UI_PlayerInfo>();

                        //player.SetInfo(GameManager.GM._AllPlayerData[pos].Name, GameManager.GM._AllPlayerData[pos].ID.ToString(), GameManager.GM._AllPlayerData[pos].IP.ToString(), GameManager.GM._AllPlayerData[pos].Diamond.ToString(), GameManager.GM._AllPlayerData[pos].Head);
                    }
                });
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }
        }
        StartCoroutine("loadPlayer");
    }
    IEnumerator loadPlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GameManager.GM._AllPlayerData[i].ID != 0)
            {
                yield return null;
                PlayerComing(GameManager.GM._AllPlayerData[i].Name, GameManager.GM._AllPlayerData[i].Money.ToString(), GameManager.GM._AllPlayerData[i].Head, i);
            }
        }
    }
    void SetPlayerDefault()
    {
        DefaultPlayerInformation();
    }
    void ReciveXQ(uint PlayerID, uint CardType)
    {
        // 注: 1是万，3是条，2是筒
        int Pos = 0;
        Pos = GameManager.GM.GetPlayerNum(PlayerID);
        Debug.Log("选缺的位置：" + Pos);
        var t = transform.Find("Players/Head" + Pos + "/WTT").gameObject;
        t.SetActive(true);
        t.GetComponent<Image>().sprite = GetWTT(CardType);
    }
    Sprite GetWTT(uint CardType)
    {
        if (CardType == 1)
            return Resources.Load<GameObject>("Prefabs/ChooseQue/Q_WAN").GetComponent<Image>().sprite;
        if (CardType == 3)
            return Resources.Load<GameObject>("Prefabs/ChooseQue/Q_TIAO").GetComponent<Image>().sprite;
        if (CardType == 2)
            return Resources.Load<GameObject>("Prefabs/ChooseQue/Q_TONG").GetComponent<Image>().sprite;

        return null;
    }
    #region 刘方舟
    List<List<uint>> handcards;
    bool isbofang = true;
    bool isjieshu = false;
    void Zhanting()
    {
        //点击声音
        //GameManager.GM.AudioGM.PlayAudio("PubicCommon", 0, 1);
        isbofang = false;
    }
    void Bofang()
    {
        //点击声音
        //GameManager.GM.AudioGM.PlayAudio("PubicCommon", 0, 1);
        isbofang = true;
    }
    void StartGo()
    {
        //MJui.RestMyHandCardList(true);
        handcards = new List<List<uint>>();
        for (int i = 0; i < RoomRsp.players.Count; i++)
        {
            List<uint> handcard = new List<uint>();
            for (int j = 0; j < RoomRsp.playBack.players[i].handCards.Count; j++)
            {
                handcard.Add(RoomRsp.playBack.players[i].handCards[j]);
            }
            handcards.Add(handcard);
        }
        StartCoroutine(GOGogo());
    }
    int Zhuangseat = -1;
    uint ZhuangID = 0;
    //int goseat = 0;  //第一次摸牌计数
    int Opnum = 0;
    int Queanum = 0;
    #endregion
    IEnumerator GOGogo()
    {
        //1.玩家初始牌更新
        for (int i = 0; i < RoomRsp.players.Count; i++)
        {
            Debug.Log(RoomRsp.players.Count);
            Debug.Log(handcards[i].Count + "xxxxxxxxx");/* + RoomRsp.playBack.players[i].handCards[0]);*/
            int seat = GameManager.GM.GetPlayerNum(RoomRsp.players[i].charId);
            PublicEvent.GetINS.Fun_reciveGetHandCards(handcards[i], seat);
            //PublicEvent.GetINS.Fun_UpdatePaishu(67);
            if (handcards[i].Count == 14)
            {
                //根据手牌数量确定庄家
                Zhuangseat = seat;
                ZhuangID = RoomRsp.players[i].charId;
                ShowZhuang(ZhuangID);
                //PublicEvent.GetINS.Fun_PlayerUpdata(seat);
            }
        }
        var wait = new WaitForSeconds(0.8f);
        while (!isjieshu)
        {
            while (isbofang)
            {
                if (Opnum < RoomRsp.playBack.opInfo.Count)
                {
                    //Debug.Log("出来的消息" + RoomRsp.playBack.opInfo[Opnum].op + "传来的玩家" + RoomRsp.playBack.opInfo[Opnum].charId + "传来的牌值" + RoomRsp.playBack.opInfo[Opnum].card);
                    //输入牌指令
                    uint Charid = RoomRsp.playBack.opInfo[Opnum].charId;
                    uint Cardid = RoomRsp.playBack.opInfo[Opnum].card;
                    uint Oricharid = RoomRsp.playBack.opInfo[Opnum].oricharId;
                    ProtoBuf.MJGameOP op = RoomRsp.playBack.opInfo[Opnum].op;
                    switch (op)
                    {
                        case ProtoBuf.MJGameOP.MJ_OP_PREP:
                            Debug.Log("xxMJ_OP_PREP");
                            wait = new WaitForSeconds(0.05f);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                            wait = new WaitForSeconds(0.05f);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_X3Z:
                            Debug.Log("xxMJ_OP_X3Z");
                            wait = new WaitForSeconds(0.05f);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_XQ:
                            Debug.Log("xxxMJ_OP_XQ");
                            wait = new WaitForSeconds(0.05f);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_MOPAI:
                            //if (ZhuangID == 0)
                            //{
                            //    ZhuangID = Charid;
                            //    int seat = GameManager.GM.GetPlayerNum(ZhuangID);
                            //    //for (int i = 0; i < RoomRsp.players.Count; i++)
                            //    //{
                            //    //    //PublicEvent.GetINS.Fun_reciveZhuang(ZhuangID);
                            //    //    //PublicEvent.GetINS.Fun_PlayerUpdata(seat);
                            //    //    //PublicEvent.GetINS.ReViewStart();//开始回放
                            //    //}
                            //}
                            //PublicEvent.GetINS.Fun_DirLight(Charid);
                            PublicEvent.GetINS.Fun_reciveGetCard(Charid, Cardid);
                           // PublicEvent.GetINS.Fun_UpdatePaishu(RoomRsp.playBack.opInfo[Opnum].rest);
                            wait = new WaitForSeconds(0.2f);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:
                            PublicEvent.GetINS.Fun_reciveOtherPopCard(Charid, Cardid);                            ReFreshRealtyTime();
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GUO:
                            PublicEvent.GetINS.Fun_ReciveOtherGuo(Charid);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_PENG:
                            //PublicEvent.GetINS.Fun_DirLight(Charid);
                            PublicEvent.GetINS.Fun_ReciveOtherPeng(Charid, Cardid, Oricharid);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG:
                            //PublicEvent.GetINS.Fun_DirLight(Charid);
                            PublicEvent.GetINS.Fun_ReciveOtherGang(Charid, Cardid, Oricharid);//*
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_HU:
                            PublicEvent.GetINS.Fun_ReciveOtherHu(Charid, Cardid, Oricharid);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_TING:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_X3Z_RESULT:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_ROUND_OVER:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_VOTE_JSROOM:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_VOTE_RESULT:
                            break;
                        default:
                            break;
                    }
                    Opnum++;
                }
                else
                {
                    isbofang = false;
                    isjieshu = true;
                }
                yield return wait;
                wait = new WaitForSeconds(1.3f);
            }
            yield return wait;
        }
        Debug.Log("end");
        //GetGameObject("Mask0").GetComponent<Image>().color = new Color(0, 0, 0, 70f / 255f);
    }
    DateTime _time;
    void ReFreshRealtyTime()
    {
        _time = DateTime.Now;
        RealtyTime.text = _time.Hour.ToString() + ":" + _time.Minute.ToString();
    }
    void Rest()
    {
        GameManager.GM.ingame = false;
        GameManager.GM.DS.PlayBack = null;
        for (int i = 1; i < 4; i++)
        {
            GameManager.GM.DeletePlayerData(GameManager.GM._AllPlayerData[i].ID);
            Debug.Log("删除玩家：" + GameManager.GM._AllPlayerData[i].Name);
        }
        Destroy(this.gameObject);
    }
    void OnDestory()
    {
        GameManager.GM.ingame = false;
        GameManager.GM.DS.PlayBack = null;
    }
}
