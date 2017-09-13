using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class InGameManage_3DMjUI : UiWin_Parent
{
    public Text Pop0, Pop1, Pop2, Pop3;
    public Start_Mj_NeedUI needMJ_Ui;
    public InGameManage_3DMJ _inGameM_3DMJ;
    public INGamePlayer_Ui[] allPlayerInfoUis;
    //public PhizAnima phizAnima_M;
    public SVMJDrag svMJDrag;//拖拽赋值
    public bool isInitSetActive = false;
    public Button btn_YaoQing;
    public List<MJInfos> H3ZCard = new List<MJInfos>();
    public Button H3Z_Btn;
    public bool IsH3Z = false;
    //public bool HasH3Z = false;
    public Toggle tog_ZDDQ, tog_FeiPeng;
    public List<Mj_ClickHight> H3ZDownCards = new List<Mj_ClickHight>();
    public List<Text> PlayerGolds = new List<Text>();
    public List<Text> Playerdistance = new List<Text>();
    public Button Btn_Mult_GangPai = null;
    public Toggle micToggle = null, listenerToggle = null;
    /// <summary>
    /// 换三张面板提示
    /// </summary>
    public GameObject NoticeH3z = null;
    /// <summary>
    /// 换三张中 已经换三张 选缺中 已经选缺
    /// </summary>
    public List<Sprite> h3zAndQue = new List<Sprite>();
    /// <summary>
    /// 换三张选缺的位置 用来替换
    /// </summary>
    public List<Image> h3zAndQueImage = new List<Image>();

    /// <summary>单局结束界面
    /// </summary>
    public UiWin_End1_MJ uiWin_End1_MJ;

    public GameObject gmXuanQue_Bg;

    public Sprite Sprite_ZiMo = null, Sprite_Hu = null;
    public Image[] playerHu = new Image[4];
    public Text Battery;
    string _battery = null;
    string roomid;
    // Use this for initialization
    void Start()
    {
        roomid = BaseProto.playerInfo.m_atRoomId.ToString();
        if (GameManager.GM.GmType != GameSceneType.gm_MjZhanJiHuiFang)
        {
            RealTimeVoice.Inst.JoinRoom(roomid);
            RealTimeVoice.Inst.onJoinRoomSuccess = () =>
            {
                RealTimeVoice.Inst.OpenSpeaker(true);
            };
        }

        if (!isInitVar)
        {
            Init_Var();
        }
        micToggle.GetComponent<Image>().color = Color.gray;
        micToggle.transform.GetChild(0).gameObject.SetActive(true);
        micToggle.onValueChanged.AddListener(delegate { ToggleVoiceSpeaker(); });
        listenerToggle.onValueChanged.AddListener(delegate { ToggleVoiceListen(); });

        GameManager.GM.DS.InGameManage_3DMjUI = gameObject;
        var T = GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>();
        T.SetMessage("休闲茶馆竭力提供一个休闲、健康的娱乐平台。如对本牌局有疑惑的玩家，可打完该局后在大厅的“战绩”里查看回放功能。");
        T.Dely3ToClose();
        H3Z_Btn.onClick.AddListener(delegate
        {
            if (H3ZCard.Count == 3)
            {
                List<uint> Tem = new List<uint>();
                for (int i = 0; i < H3ZCard.Count; i++)
                {
                    TempSetDown(H3ZCard[i].transform, 0);
                    H3ZCard[i].IsSelect = false;
                }
                for (int i = 0; i < 3; i++)
                {
                    Tem.Add(H3ZCard[i].MjINFO_.mJCardID);
                    MemoryPool_3D.Instance.MJ3D_Recycle(H3ZCard[i].transform.GetChild(0).gameObject);
                }
                _inGameM_3DMJ.allPlayerMJAry[0].ShouPai_RankMYAll();
                H3ZCard.Clear();
                H3Z_Btn.gameObject.SetActive(false);
                PublicEvent.GetINS.SentChange3Zhang(Tem);
                Tem.Clear();
                NoticeH3z.SetActive(false);
            }

        });
        Lang.GpsManager.Inst.GetGps(g =>
        {
            ProtoBuf.MJGameOpReq pack = new ProtoBuf.MJGameOpReq();
            pack.op = ProtoBuf.MJGameOP.MJ_OP_COORD;
            pack.charId = BaseProto.playerInfo.m_id;
            pack.latitude = g.x;
            pack.longitude = g.y;

            ushort command = (ushort)ProtoBuf.CLIToLGIProtocol.CLI_TO_LGI_MJ_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);

            //Lang.GpsManager.Inst.GetCity(g, c =>
            //{
            //    Debug.Log("C:"+c);
            //    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage(" Debug.Log C:"+c);
            //});
        });

        StartCoroutine("UpdataBattery");
    }
    /// <summary>
    /// 临时的一个方法
    /// </summary>
    void TempSetDown(Transform Value, float y)
    {
        Value.localPosition = new Vector3(Value.localPosition.x, y, 0);
    }
    /// <summary>
    /// 设定该玩家的gold
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="gold"></param>
    public void SetPlayerGold(uint charid, string gold)
    {
        PlayerGolds[DataManage.Instance.PData_GetIndex(charid)].text = gold;
    }
    public void SetPlayerDistance(uint charid)
    {
        if (BaseProto.playerInfo.m_id != charid)
            Playerdistance[DataManage.Instance.PData_GetIndex(charid)].text = "距离:" + Lang.Gps.CalcDistance(DataManage.Instance.PData_GetData(BaseProto.playerInfo.m_id).adress, DataManage.Instance.PData_GetData(charid).adress);
        //Lang.GpsManager.Inst.GetCity(value, delegate (string t)
        //{
        //    //mjRoom.charStates[i].latitude
        //    //mjRoom.charStates[i].longitude
        //    //t

        //});
    }
    public void Init_Var()
    {
        if (GameObject.Find("MjZhuoZi") != null)
        {
            _inGameM_3DMJ = GameObject.Find("MjZhuoZi").GetComponent<InGameManage_3DMJ>();
        }
        if (allPlayerInfoUis == null)
        {
            allPlayerInfoUis = transform.Find("All_PlayerUiBg").GetComponentsInChildren<INGamePlayer_Ui>();
        }
        svMJDrag = transform.Find("SVMJDrag").GetComponent<SVMJDrag>();
        needMJ_Ui = new Start_Mj_NeedUI(transform);
        if (GameManager.GM.GmType == GameSceneType.gm_MjZhanJiHuiFang)
        {
            needMJ_Ui.Prep_OpenOrClose_Btn(false);
        }
        Set_OnEventList<Button>(gmXuanQue_Bg.GetComponentsInChildren<Button>());
        Set_OnEventList<Button>(new Button[] {
            needMJ_Ui.btn_Peng_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Gang_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Hu_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Guo_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_SetoutStart.GetComponent<Button>(),
            needMJ_Ui.btn_FeiPai.GetComponent<Button>(),
            needMJ_Ui.btn_TiPai.GetComponent<Button>()
        });

        Button[] btnAry_Right = transform.Find("Img_BtnBg0").GetComponentsInChildren<Button>();
        Set_OnEventList<Button>(btnAry_Right);
        btn_YaoQing.onClick.RemoveAllListeners();
        btn_YaoQing.onClick.AddListener(delegate ()
        { //
            GameManager.GM.ShareLink("血战到底", DataManage.Instance.roomJuShu_Max.ToString(), DataManage.Instance.rule, DataManage.Instance._roomEnterRsp.roomId.ToString());
            //SdkEvent.Instance.OnClick_Btn_GameYaoQing();
        });

        tog_ZDDQ.onValueChanged.AddListener(delegate (bool bl) { this.OnTog_ZiDongDuiQi(tog_ZDDQ); });
        tog_FeiPeng.onValueChanged.AddListener(delegate (bool bl) { this.OnTog_FeiPeng(tog_FeiPeng); });

        needMJ_Ui.OpenOrClose_ZhuangJia(false);
        needMJ_Ui.Prep_CloseOKStyleAll();

        StartCoroutine(LoadChildScript());
        isInitVar = true;
    }

    void OnDisable()
    {
        for (int i = 0; i < allPlayerInfoUis.Length; i++)
        {
            if (allPlayerInfoUis[i] != null)
            {
                allPlayerInfoUis[i].Init_();
            }
        }
        tog_FeiPeng.isOn = true;
        tog_ZDDQ.isOn = true;
        tog_FeiPeng.interactable = true;
        tog_ZDDQ.interactable = true;

        needMJ_Ui.Prep_CloseOKStyleAll();
        //needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);

        Init_BaoTing();
    }
    void Awake()
    {
        PublicEvent.GetINS.Fun_reciveMessagePreDefine += ShowAnim;
        PublicEvent.GetINS.Event_reciveMessageText += ShowPop;
        PublicEvent.GetINS.PlaySound += ShowVoice;
        PublicEvent.GetINS.Event_DisHead += Showhead;
        PublicEvent.GetINS.Fun_SameIpTip += SameIpTip;

        DataManage.Instance.onChangePlayerData += this.UpdatePlayerUi;
        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
        PublicEvent.GetINS.Event_ReciveChange3ZhangResult += ReciveH3ZCards;
        PublicEvent.GetINS.Evnet_ReciveChange3ZhangOthercharid += ShowWhoHasH3Z;
        PublicEvent.GetINS.Event_TingCard += ShowTingCards;
        PublicEvent.GetINS.PlayerDistance += SetPlayerDistance;
        ParticleManager.GetIns.MainBg.SetActive(false);
    }
    void ReciveH3ZCards(List<uint> value)
    {
        IsH3Z = false;
        H3Z_Btn.gameObject.SetActive(false);
        List<uint> handCards = new List<uint>();
        for (int i = 0; i < value.Count; i++)
        {
            Debug.LogError("得到手牌:" + value[i]);

            handCards.Add(value[i]);
        }
        handCards.Sort();//手牌排序
        for (int i = 0; i < handCards.Count; i++)
        {
            Mj_Sx_ mjSx = handCards[i].ToCard();
            DataManage.Instance.MJSX_Add(mjSx);
            _inGameM_3DMJ.allPlayerMJAry[0].ShouPai_Add_My(mjSx);
        }
        _inGameM_3DMJ.allPlayerMJAry[0].ShouPai_RankMYAll();

        if (DataManage.Instance.RuleQue)
        {
            gmXuanQue_Bg.SetActive(true);
            h3zAndQueImage[0].gameObject.SetActive(true);
            h3zAndQueImage[0].sprite = h3zAndQue[2];
            h3zAndQueImage[0].SetNativeSize();

            h3zAndQueImage[1].gameObject.SetActive(true);
            h3zAndQueImage[1].sprite = h3zAndQue[2];
            h3zAndQueImage[1].SetNativeSize();

            h3zAndQueImage[2].gameObject.SetActive(true);
            h3zAndQueImage[2].sprite = h3zAndQue[2];
            h3zAndQueImage[2].SetNativeSize();

            h3zAndQueImage[3].gameObject.SetActive(true);
            h3zAndQueImage[3].sprite = h3zAndQue[2];
            h3zAndQueImage[3].SetNativeSize();

        }
        else
        {
            gmXuanQue_Bg.SetActive(false);

            h3zAndQueImage[0].gameObject.SetActive(false);
            h3zAndQueImage[1].gameObject.SetActive(false);
            h3zAndQueImage[2].gameObject.SetActive(false);
            h3zAndQueImage[3].gameObject.SetActive(false);
        }
        _inGameM_3DMJ.OnAbleAllCard();
        Debug.Log("换三张结束！");
    }
    public int h3znum = 0;
    void ShowWhoHasH3Z(uint charid)
    {
        switch (GameManager.GM.GetPlayerNum(charid))
        {
            case 0:
                IsH3Z = false;
                Debug.Log("我已经换三张了！");
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
        h3zAndQueImage[DataManage.Instance.PData_GetIndex(charid)].gameObject.SetActive(true);
        h3zAndQueImage[DataManage.Instance.PData_GetIndex(charid)].sprite = h3zAndQue[1];
        h3zAndQueImage[DataManage.Instance.PData_GetIndex(charid)].SetNativeSize();
        ++h3znum;
        if (h3znum > 3)
        {
            h3zAndQueImage[0].gameObject.SetActive(true);
            h3zAndQueImage[0].sprite = h3zAndQue[2];
            h3zAndQueImage[0].SetNativeSize();
            h3zAndQueImage[1].gameObject.SetActive(true);
            h3zAndQueImage[1].sprite = h3zAndQue[2];
            h3zAndQueImage[1].SetNativeSize();
            h3zAndQueImage[2].gameObject.SetActive(true);
            h3zAndQueImage[2].sprite = h3zAndQue[2];
            h3zAndQueImage[2].SetNativeSize();
            h3zAndQueImage[3].gameObject.SetActive(true);
            h3zAndQueImage[3].sprite = h3zAndQue[2];
            h3zAndQueImage[3].SetNativeSize();
            h3znum = 0;
        }
    }
    void OnDestroy()
    {
        if (GameManager.GM.GmType != GameSceneType.gm_MjZhanJiHuiFang)
        {
            RealTimeVoice.Inst.QuitRoom(roomid);
        }
        PublicEvent.GetINS.Fun_reciveMessagePreDefine -= ShowAnim;
        PublicEvent.GetINS.Event_reciveMessageText -= ShowPop;
        PublicEvent.GetINS.PlaySound -= ShowVoice;
        PublicEvent.GetINS.Event_DisHead -= Showhead;
        PublicEvent.GetINS.Fun_SameIpTip -= SameIpTip;
        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
        PublicEvent.GetINS.Event_ReciveChange3ZhangResult -= ReciveH3ZCards;
        DataManage.Instance.onChangePlayerData -= this.UpdatePlayerUi;
        PublicEvent.GetINS.Evnet_ReciveChange3ZhangOthercharid -= ShowWhoHasH3Z;
        PublicEvent.GetINS.Event_TingCard -= ShowTingCards;
        PublicEvent.GetINS.PlayerDistance -= SetPlayerDistance;
    }
    public void Rest()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
    IEnumerator LoadChildScript()
    {
        bool b_Stop = false;
        while (!b_Stop)
        {
            int i_Count1 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (allPlayerInfoUis[i].isInitVar)
                {//麻将牌面管理是否加载完成
                    i_Count1++;
                }
            }
            if (i_Count1 == 4)
            {
                b_Stop = true;
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return null;
    }
    List<uint> SamePlayers = new List<uint>();

    void SameIpTip(int playernum)
    {
        if (playernum == 4 && !GameManager.GM.ingame && DataManage.Instance.RoomSyJuShu == DataManage.Instance.roomJuShu_Max)
        {
            for (int P = 0; P < 4; P++)
            {
                for (int z = 3; z >= 0; z--)
                {
                    if (DataManage.Instance.PData_GetDataAry()[z].p_ID != 0 &&
                        DataManage.Instance.PData_GetDataAry()[P].p_ID != DataManage.Instance.PData_GetDataAry()[z].p_ID &&
                        DataManage.Instance.PData_GetDataAry()[P].p_Ip == DataManage.Instance.PData_GetDataAry()[z].p_Ip)
                    {
                        if (!SamePlayers.Contains(DataManage.Instance.PData_GetDataAry()[z].p_ID))//不存在则添加进去
                        {
                            Debug.Log("添加！！！！！");
                            SamePlayers.Add(DataManage.Instance.PData_GetDataAry()[z].p_ID);
                        }
                    }
                }
            }
            if (SamePlayers.Count > 0)
            {
                string te = "";
                for (int i = 0; i < SamePlayers.Count; i++)
                {
                    te += DataManage.Instance.PData_GetData(SamePlayers[i]).P_Name + "  ";
                }
                GameManager.GM.DS.Notic = GameManager.GM.PopUI(ResPath.Notic);
                if (SamePlayers.Count == 2)
                {
                    GameManager.GM.DS.Notic.GetComponent<UI_Notic>().SetMessage("\n系统检测到" + te + "在同一IP环境中游戏！继续游戏将会有" + SamePlayers.Count.ToString() + "打一的风险哦！");
                }
                else
                {
                    GameManager.GM.DS.Notic.GetComponent<UI_Notic>().SetMessage("\n系统检测到" + te + "在同一IP环境中游戏！继续游戏将会有" + (SamePlayers.Count - 1).ToString() + "打一的风险哦！");
                }

            }
        }
    }
    public void StartSetRoomTime()
    {
        StartCoroutine(Repeating_Event(30, delegate () { SetRoomTime(); }));
    }

    #region/*———关于Toggle事件———*/
    void OnTog_ZiDongDuiQi(Toggle tog)
    {
        _inGameM_3DMJ.IsAlign = tog.isOn;
    }
    void OnTog_FeiPeng(Toggle tog)
    {
        _inGameM_3DMJ.IsFeiPeng = tog.isOn;
    }
    #endregion
    #region/*———关于按钮事件———*/
    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        if (typeof(Button) == typeof(Component_))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                btn_.onClick.AddListener(delegate ()
                {
                    this.OnClick_(btn_.gameObject);
                });
            }
        }
    }

    void OnClick_(GameObject btn_)
    {
        Debug.Log("Click Button Name == " + btn_.gameObject.name);
        if (btn_.gameObject.name.IndexOf("Btn_XuanQue") >= 0)
        {
            OnClick_Btn_XuanQue(btn_);

            return;
        }

        switch (btn_.name)
        {
            case "Btn_OpenSetting":
                Debug.Log("打开设置");
                GameManager.GM.DS.Setting = GameManager.GM.PopUI(ResPath.Setting);
                break;
            case "Btn_RequestQuitRoom":
                if (GameManager.GM.GmType != GameSceneType.gm_MjZhanJiHuiFang)
                {
                    If_OpenWindowQuitRoom();
                }
                else
                {
                    ParticleManager.GetIns.SwitchSence(1);
                    StartCoroutine(DelyDestory());
                }
                break;
            case "Btn_OpenChatPhiz":
                if (GameManager.GM.GmType != GameSceneType.gm_MjZhanJiHuiFang)
                {
                    GameManager.GM.DS.UsedChat = GameManager.GM.PopUI(ResPath.UsedChat);
                }
                break;
            default:
                break;
        }
        switch (btn_.name)
        {
            case "Btn_Peng_Pai":
                OnClick_Btn_Peng();
                break;
            case "Btn_Gang_Pai":
                OnClick_Btn_Gang();
                break;
            case "Btn_Hu_Pai":
                OnClick_Btn_Hu();
                break;
            case "Btn_Guo_Pai":
                OnClick_Btn_Guo();
                break;
            case "Btn_SetoutStart":
                OnClick_Btn_SetoutStart();
                break;
            default:
                break;
        }
    }
    IEnumerator DelyDestory()
    {
        yield return new WaitForSeconds(0.4f);
        MemoryPool_3D.Instance.MJ3D_RecycleALL();
        DataManage.Instance.PData_RemoveOtherPlayerData();

        Destroy(GameManager.GM.DS.UiWin_ZJHF);
        GameManager.GM.DS.UiWin_ZJHF = null;

        Destroy(GameManager.GM.DS.MJZJHF);
        GameManager.GM.DS.MJZJHF = null;
        Rest();
    }
    void If_OpenWindowQuitRoom()
    {
        //GameManager.GM.DS.IsVote = GameManager.GM.PopUI(ResPath.IsQuit);


        GameManager.GM.DS.uiWin_Prompt_QuitRoom = GameManager.GM.PopUI(ResPath.uiWin_Prompt_QuitRoom);
        GameObject uiQuitPrompt = GameManager.GM.DS.uiWin_Prompt_QuitRoom;
        uiQuitPrompt.transform.Find("Img_Bg0/Btn_CloseWin").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            //this.Btn_DetermineOpenVote(4); 
            Destroy(GameManager.GM.DS.uiWin_Prompt_QuitRoom);
            GameManager.GM.DS.uiWin_Prompt_QuitRoom = null;
        });

        Button btn_ = uiQuitPrompt.transform.Find("Img_Bg0/Btn_Confirm").GetComponent<Button>();
        {
            if (DataManage.Instance.MyPlayer_Data.isgaming)
            {
                uiQuitPrompt.GetComponentInChildren<Text>().text = "游戏已开局，若现在退出游戏需要征求其他玩家同意。\n\n是否发起退出申请？";
                btn_.onClick.AddListener(delegate ()
                {
                    PublicEvent.GetINS.IsMyVote = true;
                    this.Btn_DetermineOpenVote(3);
                    Destroy(GameManager.GM.DS.uiWin_Prompt_QuitRoom);
                    GameManager.GM.DS.uiWin_Prompt_QuitRoom = null;
                });
            }
            else
            {
                if (DataManage.Instance.roomBoosId == BaseProto.playerInfo.m_id)
                {//是房主，
                    uiQuitPrompt.GetComponentInChildren<Text>().text = "房主游戏开始前退出游戏，房间即可解散。\n\n是否现在退出？";
                    btn_.onClick.AddListener(delegate ()
                    {
                        this.Btn_DetermineOpenVote(3);
                        Destroy(GameManager.GM.DS.uiWin_Prompt_QuitRoom);
                        GameManager.GM.DS.uiWin_Prompt_QuitRoom = null;
                    });
                }
                else
                {//不是房主
                    uiQuitPrompt.GetComponentInChildren<Text>().text = "您确认<color=red>退出</color>房间吗？";
                    btn_.onClick.AddListener(delegate ()
                    {
                        this.Btn_DetermineOpenVote(5);
                        Destroy(GameManager.GM.DS.uiWin_Prompt_QuitRoom);
                        GameManager.GM.DS.uiWin_Prompt_QuitRoom = null;
                    });
                }
            }
        }
    }

    /// <summary>退出房间
    /// 3:发起投票退出确定    4:发起投票退出拒绝    5：直接退出
    /// </summary>
    public void Btn_DetermineOpenVote(int i_)
    {
        // Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        switch (i_)
        {
            case 3:
                PublicEvent.GetINS.VoteRequest(true);
                break;
            case 4:
                PublicEvent.GetINS.VoteRequest(false);
                break;
            case 5:
                PublicEvent.GetINS.OnExitRoom();
                break;
            default:
                break;
        }
    }
    IEnumerator WaitTimeInveke_Event(float waitTime_1, UnityEngine.Events.UnityAction event_)
    {
        yield return GameManager.waitForEndOfFrame;
        yield return new WaitForSeconds(waitTime_1);
        event_.Invoke();
    }

    IEnumerator Repeating_Event(float waitTime_1, UnityEngine.Events.UnityAction event_)
    {

        while (gameObject.activeInHierarchy)
        {
            event_.Invoke();
            yield return new WaitForSeconds(waitTime_1);
        }

    }
    public void ToggleVoiceListen()
    {
        RealTimeVoice.Inst.OpenSpeaker(listenerToggle.isOn);
        listenerToggle.GetComponent<Image>().color = listenerToggle.isOn ? Color.white : Color.gray;
        listenerToggle.transform.GetChild(0).gameObject.SetActive(listenerToggle.isOn ? false : true);
    }
    public void ToggleVoiceSpeaker()
    {
        RealTimeVoice.Inst.OpenMic(micToggle.isOn);
        micToggle.GetComponent<Image>().color = micToggle.isOn ? Color.white : Color.gray;
        micToggle.transform.GetChild(0).gameObject.SetActive(micToggle.isOn ? false : true);
    }
    //float bgvalue = 0f;
    ///// <summary>录音按钮按下，拖拽赋值
    ///// </summary>
    //public void OnClickDown_MKF()
    //{
    //    Debug.Log("录音Start");
    //    //Event.Inst().StartMic();
    //    //gmMkfStyle = UIManager.Instance.ShowUI(AllPrefabName.Img_MkfStyle,transform);
    //    //gmMkfStyle.SetActive(true);

    //    Events.Inst().StartMic(true);
    //    bgvalue = SoundMag.BgValue;
    //    SoundMag.GetINS.ChangeBgValue(0);
    //}
    GameObject gmMkfStyle;
    bool MKF_IsOn = true;
    /// <summary>录音按钮抬起，拖拽赋值
    /// </summary>
    //public void OnClickUp_MKF()
    //{
    //    MicphoneTest.IsCancelMic = false;
    //    Events.Inst().StartMic(false);
    //    SoundMag.GetINS.ChangeBgValue(bgvalue);
    //    Debug.Log("录音END");
    //    //MicphoneTest.IsCancelMic = false;
    //    //Event.Inst().StartMic();
    //    //gmMkfStyle.SetActive(false);

    //    //if (MKF_IsOn)
    //    //{
    //    //    RealTimeVoice.Inst.OpenMic(MKF_IsOn);
    //    //    MKF_IsOn = !MKF_IsOn;
    //    //}
    //    //else {
    //    //    RealTimeVoice.Inst.OpenMic(MKF_IsOn);
    //    //    MKF_IsOn = !MKF_IsOn;
    //    //}
    //}
    public void OnClick_Btn_SetoutStart()
    {
        if (DataManage.Instance.RuleH3Z)
            IsH3Z = true;
        else
            IsH3Z = false;
        PublicEvent.GetINS.Fun_SentClientPre();
        Debug.Log("Click Setout Start");
    }

    void OnClick_Btn_Peng()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);
        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));

        PublicEvent.GetINS.Fun_SentPeng(_inGameM_3DMJ.mjGameInfo_M.keYi_Peng);
        _inGameM_3DMJ.mjGameInfo_M.keYi_Peng = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    void OnClick_Btn_Gang()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);
        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));

        PublicEvent.GetINS.Fun_SentGang(_inGameM_3DMJ.mjGameInfo_M.keYi_Gang, _inGameM_3DMJ.mjGameInfo_M.oriCharId);
        _inGameM_3DMJ.mjGameInfo_M.keYi_Gang = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    public void OnClick_Btn_Hu()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);

        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));
        Debug.Log("you HuPai？");
        PublicEvent.GetINS.Fun_SentHu(_inGameM_3DMJ.mjGameInfo_M.keYi_Hu, _inGameM_3DMJ.mjGameInfo_M.oriCharId);

        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }
    public Transform Mult_GangArea = null;
    public GameObject Mult_GangCards = null;
    public void Open_Mult_Gang(List<uint> value)
    {
        for (int z = 0; z < Mult_GangArea.childCount; z++)
        {
            Destroy(Mult_GangArea.GetChild(0).gameObject);
        }

        Btn_Mult_GangPai.gameObject.SetActive(true);
        Mult_GangArea.gameObject.SetActive(false);
        for (int i = 0; i < value.Count; i++)
        {
            GameObject temp = null;
            uint pe = value[i];
            temp = Instantiate(Mult_GangCards);
            temp.SetActive(true);
            temp.transform.SetParent(Mult_GangArea, false);
            temp.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<GameObject>("Prefabs/Cards/" + value[i].ToCard().mj_SpriteName).GetComponent<Image>().sprite;
            temp.GetComponent<Button>().onClick.AddListener(delegate
            {
                Debug.LogWarning("当前的杠牌:" + pe);
                PublicEvent.GetINS.Fun_SentGang(pe, BaseProto.playerInfo.m_id);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
                Btn_Mult_GangPai.gameObject.SetActive(false);
                Mult_GangArea.gameObject.SetActive(false);
            });
        }

        Btn_Mult_GangPai.onClick.AddListener(delegate
        {
            Mult_GangArea.gameObject.SetActive(true);

        });
    }

    public void OnClick_Btn_Guo()
    {
        //声音
        PublicEvent.GetINS.Fun_SentGuo(0);
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
        for (int z = 0; z < Mult_GangArea.childCount; z++)
        {
            Destroy(Mult_GangArea.GetChild(0).gameObject);
        }
    }

    void OnClick_Btn_XuanQue(GameObject btn_XuanQue)
    {
        uint i_Value = 0;
        switch (btn_XuanQue.name)
        {
            case "Btn_XuanQue_W":
                i_Value = 1;
                break;
            case "Btn_XuanQue_B":
                i_Value = 2;
                break;
            case "Btn_XuanQue_T":
                i_Value = 3;
                break;
        }
        PublicEvent.GetINS.Fun_SentSelectQue(i_Value);
        gmXuanQue_Bg.SetActive(false);
    }
    #endregion;

    #region/*———Player Ui Show———*/
    public void UpdatePlayerUi(Player_Data[] list_RoomPData)
    {
        if (gameObject.activeInHierarchy == false)
        {
            Debug.Log("<color=red>没有进行麻将游戏</color>");
            return;
        }
        if (list_RoomPData.Length > allPlayerInfoUis.Length)
        {
            Debug.LogError("<color=red>这里可能出现错误！</color>");
        }
        SetLeftTop_UI();
        for (int i_1 = 0; i_1 < list_RoomPData.Length; i_1++)
        {
            if (list_RoomPData[i_1] == null || list_RoomPData[i_1].p_ID == 0)
            {
                allPlayerInfoUis[i_1].Init_();
            }
            else
            {
                if (allPlayerInfoUis[i_1].playerCharID != list_RoomPData[i_1].p_ID)
                {
                    allPlayerInfoUis[i_1].gameObject.SetActive(true);
                    allPlayerInfoUis[i_1].btnHead.image.sprite = null;
                    if (allPlayerInfoUis[i_1].isInitVar == false)
                    {
                        allPlayerInfoUis[i_1].Init_Var();
                    }
                    //Debug.Log("__12_____" + allPlayerInfoUis[i_1].gameObject.activeInHierarchy);
                    allPlayerInfoUis[i_1].Set_PlayerInfoUI(list_RoomPData[i_1]);
                }
            }
        }
    }

    #endregion
    public void SetLeftTop_UI()
    {
        if (DataManage.Instance._roomEnterRsp != null && DataManage.Instance._roomEnterRsp.mjRoom != null)
        {
            needMJ_Ui.t_RoomID.text = "房间号：" + DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString();
            needMJ_Ui.t_ShengYuJuShu.text = "局数：" + DataManage.Instance._roomEnterRsp.mjRoom.roomRuleInfo.xzddRule.roundNum.ToString();

            needMJ_Ui.t_RoomInfo.text = DataManage.Instance.RoomInfoNxStr;
            needMJ_Ui.t_ShengYuPaiShu.text = "";
        }

    }
    IEnumerator UpdataBattery()
    {
        while (true)
        {
            //此处的battery是一个百分比数字，比如电量是93%，则这个数字是93
            _battery = GetBatteryLevel().ToString();
            Battery.text = _battery;//test.BatteryLevel().ToString();

            yield return new WaitForSeconds(60f);
        }
    }


    int GetBatteryLevel()
    {
        try
        {
            string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
            return int.Parse(CapacityString);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to read battery power; " + e.Message);
        }
        return 100;
    }
    public void Update_JuShu()
    {
        needMJ_Ui.t_ShengYuJuShu.text = "局数：" + DataManage.Instance.RoomSyJuShu.ToString();
    }

    public void Open_Player_ZhunBei(uint charID)
    {
        needMJ_Ui.Prep_OpenPrepStyle(DataManage.Instance.PData_GetIndex(charID), true);
        Debug.Log("玩家:" + DataManage.Instance.PData_GetIndex(charID) + "准备");
        DataManage.Instance.PData_SetReady(charID, true);
        if (charID == BaseProto.playerInfo.m_id)
        {
            needMJ_Ui.Prep_OpenOrClose_Btn(false);
            _inGameM_3DMJ.End_Init();
        }
    }
    public void Set_ShengYu_PaiShu(int shengyupai_)
    {
        if (DataManage.Instance.roomCardNumCount.ToString().Length >= 2 && shengyupai_ == 0)
        {
            return;
        }
        DataManage.Instance.roomCardNumCount = shengyupai_;
        needMJ_Ui.t_ShengYuPaiShu.text = "剩余牌数：" + shengyupai_.ToString();
    }
    public void Open_PengGangHuBtn(MJOpBtnName btnName)
    {
        switch (btnName)
        {
            case MJOpBtnName.BtnPeng:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.碰, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnGang:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.杠, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnHu:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.胡, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnGuo:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnFeiPai:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.飞碰, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            default:
                break;
        }
    }

    public void Set_ZhuangJia(uint charid)
    {
        Debug.Log("Set ZhuangJia UI???" + charid);
        int i_Index = DataManage.Instance.PData_GetIndex(charid);// Get_Player_Index(zhuangJia_Id);
        needMJ_Ui.img_ZhuangJia.SetParent(allPlayerInfoUis[i_Index].btnHead.transform.parent.parent);
        needMJ_Ui.OpenOrClose_ZhuangJia(true);
        needMJ_Ui.img_ZhuangJia.anchoredPosition = Vector2.one;

    }

    public void SetIsOnLine(uint charid, uint isOnLine)
    {
        allPlayerInfoUis[DataManage.Instance.PData_GetIndex(charid)].SetIsOnLine(isOnLine == 1);
    }


    public void SetRoomTime()
    {
        System.DateTime moment = System.DateTime.Now;
        // Year gets 1999.
        int year = moment.Year;
        // Month gets 1 (January).
        int month = moment.Month;
        // Day gets 13.
        int day = moment.Day;
    }
    public void Init_BaoTing()
    {
        for (int i = 0; i < allPlayerInfoUis.Length; i++)
        {
            allPlayerInfoUis[i].BaoTing_OpenOrClose(false);
        }
    }
    /// <summary>显示爆听
    /// </summary>
    /// <param name="charid"></param>
    //public void UpdateShowBaoTing(uint charid)
    //{
    //    allPlayerInfoUis[DataManage.Instance.PData_GetIndex(charid)].BaoTing_OpenOrClose(true);
    //}
    public GameObject TingCards = null;
    public Transform TingArea = null;
    public void ShowTingCards(uint charid, List<uint> cards)
    {
        TingArea.parent.gameObject.SetActive(true);
        for (int i = 0; i < TingArea.childCount; i++)
        {
            Destroy(TingArea.GetChild(i).gameObject);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject TempCard = null;
            TempCard = Instantiate(TingCards);
            if (TempCard != null)
                TempCard.transform.SetParent(TingArea, false);
            TempCard.gameObject.SetActive(true);
            TempCard.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<GameObject>("Prefabs/Cards/" + cards[i].ToCard().mj_SpriteName).GetComponent<Image>().sprite;
            Debug.Log(cards[i].ToCard().mj_SpriteName);
        }
    }
    public List<GameObject> voice = new List<GameObject>();
    void ShowVoice(uint SendId, string url)
    {
        int t = DataManage.Instance.PData_GetIndex(SendId);
        voice[t].gameObject.SetActive(true);
        StartCoroutine(DisShow(t));
    }
    IEnumerator DisShow(int SendId)
    {
        yield return new WaitForSeconds(3.0f);
        voice[SendId].gameObject.SetActive(false);
    }
    void Showhead(uint player, bool dispear)
    {
        if (dispear)
        {
            GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage(DataManage.Instance.PData_GetData(player).P_Name + "玩家掉线了！");
            transform.Find("All_PlayerUiBg/Player_UiBg_" + DataManage.Instance.PData_GetIndex(player) + "/Head/Image/Button").GetComponent<Image>().color = Color.grey;
        }
        else
        {
            GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage(DataManage.Instance.PData_GetData(player).P_Name + "玩家回到了游戏！");
            transform.Find("All_PlayerUiBg/Player_UiBg_" + DataManage.Instance.PData_GetIndex(player) + "/Head/Image/Button").GetComponent<Image>().color = Color.white;
        }
    }
    void ShowAnim(uint Sender, string Value)
    {
        var T = DataManage.Instance.PData_GetIndex(Sender);
        switch (Value)
        {
            case "x0xxd0":
                ShowFace.Ins.PlayAnim(Face.jiayou, T, 2);
                break;
            case "x0xxd1":
                ShowFace.Ins.PlayAnim(Face.keai, T, 2);
                break;
            case "x0xxd2":
                ShowFace.Ins.PlayAnim(Face.cry, T, 2);
                break;
            case "x0xxd3":
                ShowFace.Ins.PlayAnim(Face.huaixiao, T, 2);
                break;
            case "x0xxd4":
                ShowFace.Ins.PlayAnim(Face.weiqu, T, 2);
                break;
            case "x0xxd5":
                ShowFace.Ins.PlayAnim(Face.baochou, T);
                break;
            case "x0xxd6":
                ShowFace.Ins.PlayAnim(Face.fangle, T);
                break;
            case "x0xxd7":
                ShowFace.Ins.PlayAnim(Face.yun, T, 2);
                break;
            case "x0xxd8":
                ShowFace.Ins.PlayAnim(Face.han, T, 2);
                break;
            case "x0xxd9":
                ShowFace.Ins.PlayAnim(Face.meiqianle, T);
                break;
            case "x0xxd10":
                ShowFace.Ins.PlayAnim(Face.shengqi, T);
                break;
            case "x0xxd11":
                ShowFace.Ins.PlayAnim(Face.shuile, T);
                break;
            case "x0xxd12":
                ShowFace.Ins.PlayAnim(Face.zhuangbi, T, 2);
                break;
            case "x0xxd13":
                ShowFace.Ins.PlayAnim(Face.leipi, T);
                break;
            case "x0xxd14":
                ShowFace.Ins.PlayAnim(Face.ciya, T, 2);
                break;
            default:
                break;
        }
    }
    int showpopTime = 3;
    bool Pop0_bool = false, Pop1_bool = false, Pop2_bool = false, Pop3_bool = false;
    void ShowPop(uint Sender, string Value)
    {
        DataManage.Instance.ChatRecord_Add(DataManage.Instance.PData_GetData(Sender).P_Name, Value);
        int i = DataManage.Instance.PData_GetIndex(Sender);
        SoundMag.GetINS.ChatSound(Value, DataManage.Instance.PData_GetData(Sender).sex, i);
        switch (i)
        {
            case 0:
                Pop0.text = Value;
                Pop0_bool = true;
                Pop0.transform.parent.gameObject.SetActive(true);
                break;
            case 1:
                Pop1.text = Value;
                Pop1_bool = true;
                Pop1.transform.parent.gameObject.SetActive(true);
                break;
            case 2:
                Pop2.text = Value;
                Pop2_bool = true;
                Pop2.transform.parent.gameObject.SetActive(true);
                break;
            case 3:
                Pop3.text = Value;
                Pop3_bool = true;
                Pop3.transform.parent.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
    float t1 = 0, t2 = 0, t3 = 0, t0 = 0;
    void Update()
    {
        if (Pop0_bool)
        {
            if (t0 > showpopTime)
            {
                Pop0_bool = false;
                t0 = 0;
                Pop0.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                t0 += Time.deltaTime;
            }
        }
        if (Pop1_bool)
        {
            if (t1 > showpopTime)
            {
                Pop1_bool = false;
                t1 = 0;
                Pop1.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                t1 += Time.deltaTime;
            }
        }
        if (Pop2_bool)
        {
            if (t2 > showpopTime)
            {
                Pop2_bool = false;
                t2 = 0;
                Pop2.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                t2 += Time.deltaTime;
            }
        }
        if (Pop3_bool)
        {
            if (t3 > showpopTime)
            {
                Pop3_bool = false;
                t3 = 0;
                Pop3.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                t3 += Time.deltaTime;
            }
        }
    }


}

public enum MJOpBtnName
{
    BtnPeng, BtnGang, BtnHu, BtnGuo, BtnTiPai, BtnFeiPai, BtnBaoPai
}
