using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>战绩回放界面
/// </summary>
public class ZJHF_Nx3D : MonoBehaviour ,IFCMjGameNet_Update{

    ProtoBuf.MJRoundRecord MRR;
    bool isbofang = true;
    bool isjieshu = false;

    int Zhuangseat = -1;
    uint zhuangCharID = 0;

    int goseat = 0;  //第一次摸牌计数
    int Opnum = 0;
    public Transform t_ChuPaiZhiZhen;
    [SerializeField]
    private List<UnityAction> zjhf_Fun_List = new List<UnityAction>();
    public InGame_PlayerMJ[] allPlayerMJAry;//代表一个玩家在操作麻将    
    public bool isInitVar =false;

    //[SerializeField]
    //private Slider sld_TimeLine;//回放时间线
    [SerializeField]private bool yn_StartHuiFang;//是否开始回放
    [SerializeField]
    private bool yn_StopHuiFang;//是否停止回放
    [SerializeField]
    private int stop_TimeLinValue;//用来暂停回放

    /// <summary>//回放的速度， 越小越快Mathf.Clamp(playZhanJi_Speed, 0.5f, 5);
    /// </summary>
    [SerializeField]
    private float playZhanJi_Speed = 1.5f;
    [SerializeField]
    private UiWin_ZJHF uiW_ZJHF;

    [SerializeField]
    public MJGameC_Info mjGameInfo_M;//记录可以碰杠胡的值，以及OriCharId;

    public MJRoundRecord mjRoundR_;
    #region/*———特效———*/
    [SerializeField]
    private Transform t_PSPosition;// = new Transform[4];
    #endregion
    // Use this for initialization
    void Start()
    {
        if (!isInitVar)
        {
            Init_();
        }
        
    }

    public void Init_()
    {
        GameManager.GM.GmType = GameSceneType.gm_MjZhanJiHuiFang;
        GameManager.GM.DS.InGameManage_3DMjUI = GameManager.GM.PopUI(ResPath.InGameManage_3DMjUI);
        InGameManage_3DMjUI inGM_UI = GameManager.GM.DS.InGameManage_3DMjUI.GetComponent<InGameManage_3DMjUI>();
        inGM_UI.isInitSetActive = true;
        inGM_UI.Init_Var();
        inGM_UI.btn_YaoQing.gameObject.SetActive(false);
        //inGM_UI.uiWin_End1_MJ.gameObject.SetActive(false);

        GameManager.GM.DS.UiWin_ZJHF = GameManager.GM.PopUI(ResPath.UiWin_ZJHF);
        uiW_ZJHF = GameManager.GM.DS.UiWin_ZJHF.GetComponent<UiWin_ZJHF>();
        uiW_ZJHF.transform.SetAsLastSibling();
        Button[] btnAry = uiW_ZJHF.tBtnParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < btnAry.Length; i++)
        {
            Button btnNew = btnAry[i];
            btnNew.onClick.AddListener(delegate() {
                OnClick_ZhanJi(btnNew.gameObject);
            });
        }
        uiW_ZJHF.sld_PlaySpeed.onValueChanged.AddListener(delegate(float f) { OnSld_PlaySpeed(uiW_ZJHF.sld_PlaySpeed); });
        mjGameInfo_M = new MJGameC_Info();
        isInitVar = true;
        StartCoroutine(Start_());
    }
    IEnumerator Start_()
    {
        yield return new WaitForSeconds(2);
        GOGogo(mjRoundR_);
        Btn_Zj_Anima_Restart();
        yield return null;
    }

    public void OnSld_PlaySpeed(Slider sld_)
    {
        float f_Speed = sld_.value;
        f_Speed = 5.0f - f_Speed;
        f_Speed = Mathf.Clamp(f_Speed,1,5);
        playZhanJi_Speed = f_Speed;
    }

    public void OnClick_ZhanJi(GameObject btnGm)
    {
        //Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        switch (btnGm.name)
        {
            case "Btn_Play":
                Btn_Zj_Anima_Play();
                break;
            case "Btn_Stop":
                Btn_Zj_Anima_Stop();
                break;
            case "Btn_AnewPlay":
                Btn_Zj_Anima_Restart();
                break;
            default:
                break;
        }
    }
    public void Btn_Zj_Anima_Play()
    {
        stop_TimeLinValue = 1;
        yn_StopHuiFang = false;

        if (yn_StartHuiFang)
        {
            End_Recycle_All_Mj();
            StartCoroutine("Play_ZJ");
        }
    }

    public void Btn_Zj_Anima_Restart()
    {
        StopCoroutine("Play_ZJ");
        End_Recycle_All_Mj();
        yn_StartHuiFang = true;
        Btn_Zj_Anima_Play();
    }

    public void Btn_Zj_Anima_Stop()
    {
        yn_StopHuiFang = !yn_StopHuiFang;
        if (!yn_StopHuiFang)
        {
            stop_TimeLinValue = 1;
        }
        else
        {
            stop_TimeLinValue = 1000;
        }
    }

    /// <summary>回收所有麻将
    /// </summary>
    void End_Recycle_All_Mj()
    {
        MemoryPool_3D.Instance.MJ3D_RecycleALL();
        for (int i = 0; i < allPlayerMJAry.Length; i++)
        {
            allPlayerMJAry[i].Init_MjAll();
        }
    }
    IEnumerator Play_ZJ()
    {
        yn_StartHuiFang = false;
        playZhanJi_Speed = Mathf.Clamp(playZhanJi_Speed, 1f, 5);
        for (int i = 0; i < zjhf_Fun_List.Count; i++)
        {
            yield return new WaitForSeconds(playZhanJi_Speed);
            if (yn_StopHuiFang)
            {
                yield return StartCoroutine(Stop_HuiFang());
            }
            if (zjhf_Fun_List[i] != null)
            {
                zjhf_Fun_List[i].Invoke();
            }
            else
            {
                Debug.Log(zjhf_Fun_List[i] == null);
            }
        }
        yn_StartHuiFang = true;
        yield return null;
    }

    /// <summary>关于战绩回放的暂停
    /// </summary>
    /// <returns></returns>
    IEnumerator Stop_HuiFang()
    {
        while (stop_TimeLinValue > 0)
        {
            yield return GameManager.wait1;
            stop_TimeLinValue--;
        }
        yield return null;
    }

    public string MJHs_ToCN(string mjName)
    {
        string cnName = mjName;
        cnName = cnName.Replace("W", "万");
        cnName = cnName.Replace("T", "条");
        cnName = cnName.Replace("B", "筒");
        cnName = cnName.Replace("1H", "东");
        cnName = cnName.Replace("2H", "南");
        cnName = cnName.Replace("3H", "西");
        cnName = cnName.Replace("4H", "北");
        cnName = cnName.Replace("5H", "中");
        cnName = cnName.Replace("6H", "發");
        cnName = cnName.Replace("7H", "白板");
        return cnName;
    }
    public void UpdateStartHandCards(List<uint> all_SP, int seat)
    {
        all_SP.Sort();
        for (int i = 0; i < all_SP.Count; i++)
        {
            DataManage.Instance.MJSX_Add(all_SP[i].ToCard());
            if (i == 13)
            {
                allPlayerMJAry[seat].MoPai_Add_MY(DataManage.Instance.MJSX_Get(all_SP[i]));
            }
            else
            {
                allPlayerMJAry[seat].ShouPai_Add_My(DataManage.Instance.MJSX_Get(all_SP[i]));
            }
        }
    }
    /*
    1、更新玩家UI
    2、玩家初始手牌。
    3、庄家出牌， 庄家循环手牌，并出牌。玩家出牌，优先出摸牌位置的牌。然后再遍历手牌。 判断是否进行摸牌插牌动画
    4、碰牌，扣掉两张手牌，一张出牌
    5、明杠，扣掉3手牌+1被杠牌。
    6、胡牌，在胡牌位置添加牌面
        */
    public void UpdateChuPai(uint charid, uint cardId)
    {
        Mj_Sx_ mjSx = cardId.ToCard();
        DataManage.Instance.MJSX_Add(mjSx);
        //string strPath = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        //strPath = strPath + "MJ/" + DataManage.Instance.MJSX_Get(cardId).mj_SpriteName;
        //Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charid), strPath);
        mjGameInfo_M.mj_Cp_Transform = allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ZJHF_ChuPai(mjSx).transform;
        OpenOrClose_ChuPai_ZhiZhe(mjGameInfo_M.mj_Cp_Transform.parent);
    }

    void UpdateTiPai(uint charid, uint cardId)
    {
        Debug.Log("更新玩家 提牌");
        DataManage.Instance.MJSX_Add(cardId.ToCard());
        Mj_Sx_ mjSx = DataManage.Instance.MJSX_Get(cardId);
        /*提牌流程：1、销毁手牌1张。2、更新飞碰为碰。3、手牌添加红中
        */
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Destroy_My(1, mjSx, false);
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MJ_Op_Other(MJ_OpType.TiPai, mjSx);
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Add_My(DataManage.Instance.MJSX_Get(53));
        LocaHandCardsSort(charid);
    }

    //void UpdateFeiPeng(uint charid, uint cardId, uint oriCharid)
    //{
    //    Debug.Log("更新玩家 飞碰");
    //    DataManage.Instance.MJSX_Add(cardId.ToCard());
    //    Mj_Sx_ mjSx = DataManage.Instance.MJSX_Get(cardId);
    //    /*
    //    飞碰流程：1、去掉1张手牌被碰牌。2、去掉一张红中、3、去掉其他玩家的出牌
    //    */
    //    if (mjGameInfo_M.mj_Cp_Transform != null)
    //    {//删除出牌。
    //        MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
    //        mjGameInfo_M.mj_Cp_Transform = null;
    //        OpenOrClose_ChuPai_ZhiZhe();
    //    }
    //    allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Destroy_My(1, mjSx, false);
    //    allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Destroy_My(1, DataManage.Instance.MJSX_Get(53), false);
    //    allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MJ_Op_Other(MJ_OpType.FeiPeng, mjSx);
    //    LocaHandCardsSort(charid);
    //    //MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Peng, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));//特效
    //}

    public void UpdatePengPai(uint charid, uint cardId)
    {
        Mj_Sx_ mjSx = cardId.ToCard();
        DataManage.Instance.MJSX_Add(mjSx);
        //string strPath = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        //strPath = strPath + "" + AudioPathOp_TYPE.Op_Peng.ToString();
        //Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charid), strPath);
        MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Peng, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));//特效
        if (mjGameInfo_M.mj_Cp_Transform != null)
        {//删除出牌。
            MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
            mjGameInfo_M.mj_Cp_Transform = null;
        }
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Destroy_My(2,mjSx);
        if (DataManage.Instance.PData_GetIndex(charid) == 0)
        {//本地玩家
            allPlayerMJAry[0].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
        }
        else
        {//非本地玩家
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
        }
    }
    /// <summary>本地更新有玩家杠牌了
    /// </summary>
    public void UpdateGangPai(uint p_Id, uint cardId, uint oriCharid)
    {
        Mj_Sx_ mjSx = cardId.ToCard();
        DataManage.Instance.MJSX_Add(mjSx);
        MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Gang, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(p_Id))); //播放特效
        bool isMingAnGang = false;
        if (p_Id == DataManage.Instance.MyPlayer_Data.p_ID)
        {//本地玩家
            if (p_Id == oriCharid)
            {//玩家自己杠自己的牌
                int i_PengCount = allPlayerMJAry[0].Peng_IsExist(mjSx);//是否存在碰牌，并且將碰牌转为杠牌
                if (i_PengCount == 1)
                {//八杠 发现了想胡牌玩家的碰牌里面有过想碰的牌， 所以为明杠
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(1, mjSx, false);//回收手牌
                    Debug.Log(p_Id + "GangPai_Type(0)" + mjSx.mj_SpriteName);
                    goto AudioPlay;
                }
                else
                {////执行到这里，那么就是 暗杠。
                    Debug.Log(p_Id + "GangPai_Type(1)" + mjSx.mj_SpriteName);
                    int i_HuiShou_Sp = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(4, mjSx, false);//回收手牌
                    if (i_HuiShou_Sp == 3 || i_HuiShou_Sp == 4)
                    {
                        allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.AnGang, mjSx);//显示暗杠麻将
                        Debug.Log("AnGang OK");
                    }
                    else
                    {
                                          
                    }
                    isMingAnGang = true;
                    goto AudioPlay;
                }
            }
            else
            {//明杠
                if (mjGameInfo_M.mj_Cp_Transform != null )
                {//删除出牌。
                    MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
                    mjGameInfo_M.mj_Cp_Transform = null;
                }
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(3, mjSx, false);//回收手牌/*int i_HuiShou_Sp = */
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.MingGang, mjSx);//显示杠牌
                Debug.Log(p_Id + "GangPai_Type" + mjSx.mj_SpriteName);
                goto AudioPlay;
            }
        }
        else
        {//非本地玩家
            if (p_Id == oriCharid)
            {//玩家自己杠自己的牌
                int i_PengCount = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].Peng_IsExist(mjSx);//是否存在碰牌，并且將碰牌转为杠牌
                if (i_PengCount == 1)
                {//发现了想胡牌玩家的碰牌里面有过想碰的牌， 所以为明杠
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(1, mjSx,false);//回收手牌
                    Debug.Log(p_Id + "GangPai_Type(0)" + mjSx.mj_SpriteName);
                    goto AudioPlay;
                }
                else
                {////执行到这里，那么就是 暗杠。
                    Debug.Log(p_Id + "GangPai_Type(1)" + mjSx.mj_SpriteName);
                    int i_HuiShou_Sp = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(4, mjSx, false);//回收手牌
                    if (i_HuiShou_Sp == 3 || i_HuiShou_Sp == 4)
                    {
                        allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.AnGang, mjSx);//显示暗杠
                        Debug.Log("AnGang OK");
                    }
                    else
                    {

                    }
                    isMingAnGang = true;
                    goto AudioPlay;
                }
            }
            else
            {//明杠

                if (mjGameInfo_M.mj_Cp_Transform != null )
                {//删除出牌。
                    MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
                    mjGameInfo_M.mj_Cp_Transform = null;
                }
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(3, mjSx, false);//回收手牌
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.MingGang, mjSx);//显示杠牌
                goto AudioPlay;
            }
        }
        AudioPlay: AudioPlay_Gang(isMingAnGang, p_Id);
    }
    void AudioPlay_Gang(bool isMingAn, uint charId)
    {
        //string strPath = DataManage.Instance.PData_GetData(charId).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        //strPath = strPath + "" + (isMingAn ? AudioPathOp_TYPE.Op_AnGang.ToString() : AudioPathOp_TYPE.Op_Gang.ToString());
        //Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charId), strPath);
    }
    public void UpdateHule(uint charid, uint cardId, uint oriCharid)
    {
        Mj_Sx_ mjSx = cardId.ToCard();
        DataManage.Instance.MJSX_Add(mjSx);
        //自摸还是平胡的 特效
        //MemoryPool_3D.Instance.GetShowPS_Model(charid == oriCharid ? ResPath_Assets.se_ZiMo : ResPath_Assets.se_Hu, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));
        if (charid != oriCharid)
        {//显示点炮特效
            MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_DianPao, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(oriCharid)));
            MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Hu, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));
        }
        else {
            MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_ZiMo, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));
        }
       // string strPath = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
       // strPath = strPath + "" + AudioPathOp_TYPE.Op_Hu.ToString();
       // Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charid), strPath);
        Debug.Log("玩家胡牌！");
    }
    public void UpdateMopai(uint charid, uint cardId)
    {
        Mj_Sx_ mjSx = cardId.ToCard();
        DataManage.Instance.MJSX_Add(mjSx);
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MoPai_Add_MY(mjSx);
    }
    public void OpenOrClose_ChuPai_ZhiZhe(Transform t_ = null)
    {
        if (t_ != null)
        {
            t_ChuPaiZhiZhen.SetParent(t_);
            t_ChuPaiZhiZhen.localPosition = Vector3.zero;
            t_ChuPaiZhiZhen.localPosition = Vector3.zero;
            t_ChuPaiZhiZhen.localScale = Vector3.one;
            t_ChuPaiZhiZhen.gameObject.SetActive(true);
            waitTime_1 = Time.deltaTime;
            StartCoroutine(WaitTimeInveke_Event(
                delegate ()
                {
                    t_ChuPaiZhiZhen.SetParent(transform);
                    t_ChuPaiZhiZhen.localScale = Vector3.one;
                }
                ));
        }
        else
        {
            t_ChuPaiZhiZhen.SetParent(transform);
            t_ChuPaiZhiZhen.gameObject.SetActive(false);
        }
    }
    /// <summary>本地手牌排序
    /// </summary>
    void LocaHandCardsSort(uint charid)
    {
        allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_RankMYAll();
    }
    float waitTime_1 = 0;
    IEnumerator WaitTimeInveke_Event(UnityEngine.Events.UnityAction event_)
    {
        yield return GameManager.waitForEndOfFrame;
        yield return new WaitForSeconds(waitTime_1);
        event_.Invoke();
    }
    /// <summary>战绩解析，并添加到播放委托
    /// </summary>
    public string GOGogo(MJRoundRecord mrr_)
    {
        MRR = mrr_;
        string strZhanJi = "";
        //1.玩家初始牌更新
        Player_Data[] zjAllPlayerAry = new Player_Data[4];
        for (int i = 0; i < MRR.players.Count; i++)
        {
            zjAllPlayerAry[i] = new Player_Data(MRR.players[i].name, MRR.players[i].charId, MRR.players[i].ip, MRR.players[i].portrait, MRR.players[i].restDiamond, MRR.players[i].gold, (int)MRR.players[i].sex,Vector2.one);
        }
        int i_Count_HHH = 0;
        zjAllPlayerAry = DataManage.Instance.PData_Rank(zjAllPlayerAry);
        DataManage.Instance.PData_Update(zjAllPlayerAry);
        for (int i = 0; i < MRR.players.Count; i++)
        {
            Debug.Log(MRR.players.Count);
            Debug.Log("PlayerId:" + MRR.players[i].charId + "_ShouPai?__" + MRR.playBack.players[i].handCards.Count + "xxxxxxxxx");
            for (int i1 = 0; i1 < MRR.playBack.players[i].handCards.Count; i1++)
            {
                DataManage.Instance.MJSX_Add(MRR.playBack.players[i].handCards[i1].ToCard());
            }
            int i_CIndex = i;
            zjhf_Fun_List.Add(delegate () {
                List<uint> handCards_List = MRR.playBack.players[i_CIndex].handCards;//加入手牌
                handCards_List.Sort();
                UpdateStartHandCards(handCards_List, DataManage.Instance.PData_GetIndex(MRR.players[i_CIndex].charId));
                Debug.Log("[ZhanJiHuiFang] Start HandCards " + handCards_List.Count);
            });
        }
        string[] split_SP = strZhanJi.Split('\n');
        strZhanJi = "";
        split_SP = Player_Data_PaiXu.For_PaiXu(split_SP);
        for (int i = 0; i < split_SP.Length; i++)
        {
            strZhanJi += split_SP[i] + "\n";
        }
        string mjCNName = "";
        strZhanJi = "";//战绩播放1       
        while (!isjieshu)
        {
            while (isbofang)
            {
                if (Opnum < MRR.playBack.opInfo.Count)
                {
                    //输入牌指令
                    uint Charid = MRR.playBack.opInfo[Opnum].charId;
                    uint Cardid = MRR.playBack.opInfo[Opnum].card;
                    uint Oricharid = MRR.playBack.opInfo[Opnum].oricharId;
                    DataManage.Instance.MJSX_Add(Cardid.ToCard());
                    ProtoBuf.MJGameOP op = MRR.playBack.opInfo[Opnum].op;
                    switch (op)
                    {
                        case ProtoBuf.MJGameOP.MJ_OP_PREP:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                            break;
                        //case ProtoBuf.MJGameOP.MJ_OP_X3Z:
                        //    break;
                        case ProtoBuf.MJGameOP.MJ_OP_XQ:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_MOPAI:
                            if (Cardid != 0)
                            {
                                mjCNName = MJHs_ToCN(DataManage.Instance.MJSX_Get(Cardid).mj_SpriteName);
                                string str_1 = i_Count_HHH + "<color=#00ffffff>【玩家摸牌】</color>" + "名字：" + DataManage.Instance.PData_GetData(Charid).P_Name + "—摸牌：" + mjCNName + "\n";
                                strZhanJi += str_1;
                                zjhf_Fun_List.Add(delegate () {
                                    uiW_ZJHF.t_CLog.text = str_1;
                                    UpdateMopai(Charid, Cardid);
                                });
                                i_Count_HHH++;
                            }
                            break;
                        case MJGameOP.MJ_OP_CHUPAI:
                            mjCNName = MJHs_ToCN(DataManage.Instance.MJSX_Get(Cardid).mj_SpriteName);
                            string str_2 = i_Count_HHH + "<color=green>【玩家出牌】</color>" + "名字：" + DataManage.Instance.PData_GetData(Charid).P_Name + "—出牌：" + mjCNName + "\n";
                            strZhanJi += str_2;
                            zjhf_Fun_List.Add(delegate () {
                                uiW_ZJHF.t_CLog.text = str_2;
                                UpdateChuPai(Charid, Cardid);
                            });
                            i_Count_HHH++;
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GUO:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_PENG:
                            mjCNName = MJHs_ToCN(DataManage.Instance.MJSX_Get(Cardid).mj_SpriteName);
                            string str_3 = i_Count_HHH + "<color=blue>【玩家碰牌】</color>" + "名字：" + DataManage.Instance.PData_GetData(Charid).P_Name + "—碰牌：" + mjCNName + "\n";
                            strZhanJi += str_3;
                            zjhf_Fun_List.Add(delegate () {
                                uiW_ZJHF.t_CLog.text = str_3;
                                UpdatePengPai(Charid, Cardid);
                            });
                            i_Count_HHH++;
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG:
                            mjCNName = MJHs_ToCN(DataManage.Instance.MJSX_Get(Cardid).mj_SpriteName);
                            string str_4 = i_Count_HHH + "<color=yellow>【玩家杠牌】</color>" + "名字：" + DataManage.Instance.PData_GetData(Charid).P_Name + "—杠牌：" + mjCNName + "\n";
                            strZhanJi += str_4;
                            zjhf_Fun_List.Add(delegate () {
                                uiW_ZJHF.t_CLog.text = str_4;
                                UpdateGangPai(Charid, Cardid, Oricharid); 
                            });
                            i_Count_HHH++;
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_HU:
                            mjCNName = MJHs_ToCN(DataManage.Instance.MJSX_Get(Cardid).mj_SpriteName);
                            string str_5 = i_Count_HHH + "<color=red>【玩家胡牌】</color>" + "名字：" + DataManage.Instance.PData_GetData(Charid).P_Name + "—胡牌：" + mjCNName + "\n";
                            strZhanJi += str_5;
                            zjhf_Fun_List.Add(delegate () {
                                uiW_ZJHF.t_CLog.text = str_5;
                                UpdateHule(Charid, Cardid, Oricharid);
                            });
                            i_Count_HHH++;
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_TING:
                            Debug.Log("有玩家 听牌？？？？？？——");
                            break;
                        //case ProtoBuf.MJGameOP.MJ_OP_X3Z_RESULT:
                        //    break;
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
            }
        }
        isbofang = true;
        isjieshu = false;
        Opnum = 0;
        Debug.Log("LOOK ZhanJi?\n" + strZhanJi);
        uiW_ZJHF.t_Log.text = strZhanJi;
        return strZhanJi;
    }

    public void UpdateMopai(uint charid, uint cardId, List<uint> handCards)
    {
        throw new NotImplementedException();
    }
}
