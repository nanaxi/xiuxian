using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public class GamePlayerInfo
{
    public string Name;
    public uint Id;
    public string HeadUrl;
    public int Diamonds;
    public string Ip;
    public int Goid;
    public int Position;

    public bool isZhuang;
    public uint XiaZhu;

    public int HuNum;
    public int ZiMoNum;
    public int FangPaoNum;
    public int FangGangNum;
    public int AnGangNum;
    public int MingGangNum;

    public void Reset()
    {
        HuNum = 0;
        ZiMoNum = 0;
        FangPaoNum = 0;
        FangGangNum = 0;
        AnGangNum = 0;
        MingGangNum = 0;
        XiaZhu = 0;
    }
    public void AllReset()
    {
        Reset();
        isZhuang = false;
        Position = 0;
        Goid = 0;
    }
}

//public class MJInfo
//{
//    /// <summary>
//    /// 手牌
//    /// </summary>
//    public List<uint> Hand = new List<uint>();
//    public List<uint> Peng = new List<uint>();
//    public List<uint> Gang = new List<uint>();
//    public List<uint> AnGang = new List<uint>();
//    public List<uint> Hu = new List<uint>();
//    public uint MoCard;
//    public uint NpcPass;


//    ///// <summary>
//    ///// 换三张出去的牌
//    ///// </summary>
//    //public uint[] HSZ = new uint[3];
//    ///// <summary>
//    ///// 换三张来的牌
//    ///// </summary>
//    //public uint[] HLDSZ = new uint[3];

//    //public List<GMEnum.MJType> Que = new List<GMEnum.MJType>();

//    //public void Reset()
//    //{
//    //    Hand = new List<uint>();
//    //    Peng = new List<uint>();
//    //    Gang = new List<uint>();
//    //    AnGang = new List<uint>();
//    //    Hu = new List<uint>();
//    //    HSZ = new uint[3];
//    //    HLDSZ = new uint[3];
//    //    Que = new List<GMEnum.MJType>();
//    //    MoCard = 0;
//    //    NpcPass = 0;
//    //}
//}
public class PKInfo
{
    public List<uint> Cards = new List<uint>();
    public int PosNum = 0;


    public void Reset()
    {
        Cards = new List<uint>();
        PosNum = 0;
    }
}



[Serializable]
public struct Mj_Sx_
{
    public string mj_SpriteName;
    public int value;
    public uint mJCardID;
    public Mj_Sx_(int value, uint CardID, string mjName)
    {
        this.value = value;
        this.mJCardID = CardID;
        this.mj_SpriteName = mjName;
    }
}
public enum CardsType
{
    Tiao,
    Tong,
    Wan,
    Null,//杠牌时暗杠显示的背牌
    Hua,//东南西北 中 发 白
}

public enum GameSceneType
{
    gm_Login,

    gm_Home,

    gm_MjGame,

    gm_MjZhanJiHuiFang

}

/// <summary>
/// </summary>
[Serializable]
public class Start_Mj_NeedUI
{
    public RectTransform img_Mj_Btn_Cz_Bg,
        btn_Peng_Pai,
        btn_Gang_Pai,
        btn_Hu_Pai,
        btn_Guo_Pai,
        img_ZhuangJia,
        btn_SetoutStart,
        btn_BaoPai,
        btn_FeiPai,
        btn_TiPai
        ;
    //public Image img_MoPai_Anima;
    public RectTransform[] img_ChuPai_Anima;
    public Vector2[] img_ChuPai_Anima_StartV2;
    public Text t_GameTime;
    //public Vector2[] jl_Start_V2;
    public bool[] jl_PointTog;
    /// <summary>作用于打开了碰杠胡等等按钮的时候不能出牌
    /// </summary>
    public bool IsShowBtn
    {
        get
        {
            for (int i = 0; i < img_Mj_Btn_Cz_Bg.childCount; i++)
            {
                if (img_Mj_Btn_Cz_Bg.GetChild(i).gameObject.activeInHierarchy)
                {
                    Debug.Log("作用于打开了碰杠胡等等按钮的时候不能出牌");
                    return false;
                }
            }
            return true;
        }
    }
    //public bool yn_PengGang;
    //public EventTrigger_ btn_V_Chat;
    public GameObject prepAllBg;

    //public Image[] centerPointerAry;
    public Text t_RoomID, t_RoomInfo, t_ShengYuPaiShu, t_ShengYuJuShu, t_RoomTime;
    public Start_Mj_NeedUI(Transform window_MaJiang)
    {
        //t_ChuPai_ZhiZhen = window_MaJiang.FindChild("C_ChuPai_ZhiZhen").GetComponent<RectTransform>();
        //centerPointerAry = window_MaJiang.FindChild("Img_Center_Bg/Img_PlayPointer_Bg").GetComponentsInChildren<Image>();
        t_RoomID = window_MaJiang.Find("Top_Left/T_RoomID").GetComponent<Text>();
        t_RoomInfo = window_MaJiang.Find("Top_Left/T_RoomInfo").GetComponent<Text>();
        t_ShengYuPaiShu = window_MaJiang.Find("Top_Left/T_ShengYuPaiShu").GetComponent<Text>();
        t_ShengYuJuShu = window_MaJiang.Find("Top_Left/T_ShengYuJuShu").GetComponent<Text>();
        t_RoomTime = window_MaJiang.Find("Top_Left/T_RoomTime").GetComponent<Text>();
        prepAllBg = window_MaJiang.Find("PrepAllBg").gameObject;
        //t_GameTime = window_MaJiang.FindChild("T_GameTime").GetComponent<Text>();
        img_Mj_Btn_Cz_Bg = window_MaJiang.Find("Img_Mj_Btn_Cz_Bg").GetComponent<RectTransform>();
        btn_Peng_Pai = img_Mj_Btn_Cz_Bg.Find("Btn_Peng_Pai").GetComponent<RectTransform>();
        btn_Gang_Pai = img_Mj_Btn_Cz_Bg.Find("Btn_Gang_Pai").GetComponent<RectTransform>();
        btn_Hu_Pai = img_Mj_Btn_Cz_Bg.Find("Btn_Hu_Pai").GetComponent<RectTransform>();
        btn_Guo_Pai = img_Mj_Btn_Cz_Bg.Find("Btn_Guo_Pai").GetComponent<RectTransform>();
        btn_BaoPai = img_Mj_Btn_Cz_Bg.Find("Btn_BaoPai").GetComponent<RectTransform>();
        btn_FeiPai = img_Mj_Btn_Cz_Bg.Find("Btn_FeiPai").GetComponent<RectTransform>();
        btn_TiPai = img_Mj_Btn_Cz_Bg.Find("Btn_TiPai").GetComponent<RectTransform>();

        btn_SetoutStart = window_MaJiang.Find("Btn_SetoutStart").GetComponent<RectTransform>();
        img_ZhuangJia = window_MaJiang.Find("All_PlayerUiBg/Img_ZhuangJia").GetComponent<RectTransform>();

        Transform btn_V_Chat_T = window_MaJiang.Find("Btn_OpenSendVoice");
        //img_MoPai_Anima.enabled = false;
        //jl_Start_V2 = new Vector2[8];
        jl_PointTog = new bool[8];


        //yn_PengGang = false;

    }

    public void Open_Close_ChuPai_Anima(int i_Index, bool yn_Open = false, Sprite mj_Hs = null)
    {
        if (yn_Open)
        {
            img_ChuPai_Anima[i_Index].GetChild(0).GetComponent<Image>().sprite = mj_Hs;
            img_ChuPai_Anima[i_Index].anchoredPosition = img_ChuPai_Anima_StartV2[i_Index];
        }
        else
        {
            if (i_Index == -1)
            {
                for (int i = 0; i < 4; i++)
                {
                    img_ChuPai_Anima[i].gameObject.SetActive(false);/// = close_V2;
                }
                return;
            }
            img_ChuPai_Anima[i_Index].gameObject.SetActive(false);// = close_V2;
        }
    }

    public void Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name btn_name, bool openOrClose)
    {
        if (openOrClose)
        {
            img_Mj_Btn_Cz_Bg.gameObject.SetActive(openOrClose); //.anchoredPosition = jl_Start_V2[0];
        }
        switch (btn_name)
        {
            case Mj_NeedUi_Btn_Name.背景关闭所有按钮:

                if (!openOrClose)
                {
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.碰, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.杠, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.胡, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.爆, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.飞碰, false);
                    this.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.提, false);
                    img_Mj_Btn_Cz_Bg.gameObject.SetActive(openOrClose); //.anchoredPosition = close_V2;
                }
                return;
            case Mj_NeedUi_Btn_Name.碰:
                btn_Peng_Pai.gameObject.SetActive(openOrClose); //.anchoredPosition = openOrClose ? jl_Start_V2[1] : close_V2;
                //jl_PointTog[1] = openOrClose;
                //yn_PengGang = true;
                return;
            case Mj_NeedUi_Btn_Name.杠:
                btn_Gang_Pai.gameObject.SetActive(openOrClose); //.anchoredPosition = openOrClose ? jl_Start_V2[2] : close_V2;
                //jl_PointTog[2] = openOrClose;
                //yn_PengGang = true;
                return;
            case Mj_NeedUi_Btn_Name.胡:
                btn_Hu_Pai.gameObject.SetActive(openOrClose); //.anchoredPosition = openOrClose ? jl_Start_V2[3] : close_V2;
                //jl_PointTog[3] = openOrClose;
                //yn_PengGang = true;
                return;
            //case Mj_NeedUi_Btn_Name.报听:
            //    return;
            case Mj_NeedUi_Btn_Name.过:
                btn_Guo_Pai.gameObject.SetActive(openOrClose); //.anchoredPosition = openOrClose ? jl_Start_V2[5] : close_V2;
                return;

            case Mj_NeedUi_Btn_Name.爆:
                btn_BaoPai.gameObject.SetActive(openOrClose);
                break;
            case Mj_NeedUi_Btn_Name.飞碰:
                btn_FeiPai.gameObject.SetActive(openOrClose);
                break;
            case Mj_NeedUi_Btn_Name.提:
                btn_TiPai.gameObject.SetActive(openOrClose);
                break;

            default:
                break;
        }
    }
    public void OpenOrClose_ZhuangJia(bool yn_Open)
    {
        img_ZhuangJia.gameObject.SetActive(yn_Open); //.anchoredPosition = yn_Open ? jl_Start_V2[6] : close_V2;
    }

    public void Prep_OpenOrClose_Btn(bool oOrC)
    {
        btn_SetoutStart.gameObject.SetActive(oOrC); //btn_SetoutStart.anchoredPosition = oOrC ? jl_Start_V2[6] : close_V2;
    }

    public void Prep_CloseOKStyleAll()
    {
        for (int i = 0; i < prepAllBg.transform.childCount; i++)
        {
            prepAllBg.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Prep_OpenPrepStyle(int i_Index, bool b_)
    {
        if (i_Index >= 0 && i_Index < 4)
        {
            prepAllBg.transform.GetChild(i_Index).gameObject.SetActive(b_);
        }
        else
        {
            Debug.Log("可能超出索引上限");
        }
    }

}


public enum Mj_NeedUi_Btn_Name
{
    背景关闭所有按钮, 碰, 杠, 胡, 过, 飞碰, 提, 爆
}



/// <summary>麻将结束界面可能会用到的 UI。 
/// </summary>
[Serializable]
public class End_MaJiang_Player_Gm_UI
{
    public MjOver_1 mjOverManage;
    public Transform t_Parent;
    public Image img_TouXiang;
    public Text t_Name, t_DiFen, t_HuPai_Prompt, t_Gold_Change;
    public Transform img_Layout;
    public Image img_Yn_HuPai;
    public Image img_Title;
    public End_MaJiang_Player_Gm_UI()
    {

    }
    public void NewEnd_MaJiang_Player_Gm_UI(Transform img_Player_Bg_0)
    {
        t_Parent = img_Player_Bg_0;
        img_TouXiang = img_Player_Bg_0.GetComponentInChildren<Button>().image;//<Image>();
        t_Name = img_Player_Bg_0.Find("T_Name").GetComponent<Text>();
        t_DiFen = img_Player_Bg_0.Find("T_DiFen").GetComponent<Text>();
        t_HuPai_Prompt = img_Player_Bg_0.Find("T_HuPai_Prompt").GetComponent<Text>();
        t_Gold_Change = img_Player_Bg_0.Find("T_Gold_Change").GetComponent<Text>();
        img_Layout = img_Player_Bg_0.Find("Img_Layout");
        img_Yn_HuPai = img_Player_Bg_0.Find("Img_Yn_HuPai").GetComponent<Image>();
        //img_Title = img_Player_Bg_0.transform.parent.parent.parent.FindChild("Img_Title").GetComponent<Image>();

    }
    //public Player_Start_Gm_UI Get_P_Data(string p_ID)
    //{
    //    Player_Start_Gm_UI[] roomPlayerInfo = Gm_Manager.G_M.Start_Gm_Ui.Getall_Player_UIgm;
    //    for (int i = 0; i < roomPlayerInfo.Length; i++)
    //    {
    //        if (roomPlayerInfo[i].p_T_Id.text.IndexOf(p_ID) >= 0)
    //        {
    //            return roomPlayerInfo[i];
    //        }
    //    }
    //    return new Player_Start_Gm_UI();
    //}

    private const string
        fontPath_CaiSe = "Sprites/NingXiaHuaShui/QieTu/NumberKey/CaiSe/",
        fontPath_HuiSe = "Sprites/NingXiaHuaShui/QieTu/NumberKey/HuiSe/";

    #region /*——————结束界面整改——————*/
    public void Set_PlayerUIGai(ProtoBuf.EachMJPlayer player_Data, string p_Name, string str_ZJid = "")
    {

        img_TouXiang.sprite = DataManage.Instance.Head_GetSprite(player_Data.charId);// pl_.p_Img_TouXiang.sprite;// Resources.Load<Sprite>("tx_1");
        t_Gold_Change.text = player_Data.changeGold.ToString();

        Image[] imgAry = t_Gold_Change.GetComponentsInChildren<Image>();
        for (int i = 0; i < imgAry.Length; i++)
        {
            imgAry[i].enabled = false;
        }
        string strGold_1 = t_Gold_Change.text;
        string strNumberPath = player_Data.changeGold < 0 ? fontPath_HuiSe : fontPath_CaiSe;
        strGold_1 = strGold_1.Replace("-", "");
        strGold_1 = strGold_1.Replace("+", "");
        int i_ImgIndex = 1;
        imgAry[0].sprite = Resources.Load<Sprite>(strNumberPath + "FuHao");
        imgAry[0].enabled = true;

        for (int i = 0; i < strGold_1.Length; i++)
        {
            if (strGold_1[i] != '-' && strGold_1[i] != '+')
            {
                imgAry[i_ImgIndex].sprite = Resources.Load<Sprite>(strNumberPath + strGold_1[i]);
                imgAry[i_ImgIndex].enabled = true;
                i_ImgIndex++;
            }
        }
        t_Gold_Change.enabled = false;
        t_DiFen.text = "底分：" + (player_Data.restGold - player_Data.changeGold).ToString();
        t_HuPai_Prompt.text = "";
        if (player_Data.huInfos.Count > 0)
        {
            t_HuPai_Prompt.text = player_Data.huInfos[0].catag == 1 ? "接炮胡" : "自摸";
            if (player_Data.huInfos[0].oricharId != player_Data.charId)
            {
                mjOverManage.EndShow_Add("" + player_Data.huInfos[0].oricharId.ToString() + "|<color=black>[点炮" + player_Data.charId.ToString() + "]</color>");
            }
            switch (player_Data.huInfos[0].catag)
            {
                //    //平胡
                case 1:

                    t_HuPai_Prompt.text = player_Data.huInfos[0].oricharId != player_Data.charId ? "平胡" : "自摸胡";

                    break;


                ////七小对
                case 2:
                    t_HuPai_Prompt.text = "七小对";
                    break;

                ////清一色
                case 3:
                    t_HuPai_Prompt.text = "龙七对";
                    break;

                ////一条龙
                case 4:
                    t_HuPai_Prompt.text = "双龙七对";
                    break;

                ////豪华七小对
                case 5:
                    t_HuPai_Prompt.text = "三龙七对";
                    break;

                case 6:
                    t_HuPai_Prompt.text = "杠上花";
                    break;

                ////十三幺
                case 7:
                    t_HuPai_Prompt.text = "抢杠胡";
                    break;


                default:
                    break;
            }

        }
        else
        {
            t_HuPai_Prompt.text = "";// player_Data.huInfos[0].catag == 1 ? "自摸" : "接炮胡";
        }
        string str_StrTing = player_Data.hasTingPai == 1 ? "<听>" : " ";
        t_HuPai_Prompt.text = str_ZJid.IndexOf(player_Data.charId.ToString()) >= 0 ? "<color=red>【 庄家 】</color>" + t_HuPai_Prompt.text : t_HuPai_Prompt.text;
        t_HuPai_Prompt.text += str_StrTing;


        //t_ID.text = player_Data.charId.ToString();
        t_Name.text = "ID:" + player_Data.charId.ToString() + "\n" + p_Name;
        if (player_Data.charId == 0 /*BaseProto.playerInfo.m_id*/)
        {
            //img_Title.sprite = player_Data.changeGold < 0 ? Resources.Load<Sprite>(ResPath.end_UiImg_ShiBai) : Resources.Load<Sprite>(ResPath.end_UiImg_ShengLi);
        }

        img_Yn_HuPai.enabled = player_Data.huInfos.Count != 0 ? true : false;
        if (player_Data.restCards.Count == 0)
        {
            mjOverManage.btn_EndStart.onClick.RemoveAllListeners();
            mjOverManage.btn_EndStart.onClick.AddListener(delegate ()
            {
                //partic
            });
        }

        Add_EndMjGai(player_Data.restCards, player_Data.pengCards, player_Data.gangInfos, player_Data.huInfos, player_Data.charId);
    }
    public void Add_EndMjGai(List<uint> p_End_Mj_Data, List<uint> p_End_Mj_Peng, List<ProtoBuf.EachMJPlayerGang> gangList, List<ProtoBuf.EachMJPlayerHu> huPai_, uint p_ID)
    {

        p_End_Mj_Data.Sort();

        for (int i = 0; i < p_End_Mj_Peng.Count; i++)
        {
            DataManage.Instance.MJSX_Add(p_End_Mj_Peng[i].ToCard());
            for (int i1 = 0; i1 < 3; i1++)
            {
                Add_EndMjGai(DataManage.Instance.MJSX_Get(p_End_Mj_Peng[i]).mj_SpriteName, 1);
            }
            Add_EndMjGai(DataManage.Instance.MJSX_Get(p_End_Mj_Peng[i]).mj_SpriteName, 3);
        }
        int i_MingGang = 0;
        int i_AnGang = 0;
        for (int i = 0; i < gangList.Count; i++)
        {
            if (gangList[i].catag == 2)
            {
                i_AnGang++;
            }
            else
            {
                i_MingGang++;
                if (gangList[i].oricharId != p_ID)
                {
                    mjOverManage.EndShow_Add("" + gangList[i].oricharId.ToString() + "|[点杠" + p_ID.ToString() + "]");
                }
            }
            DataManage.Instance.MJSX_Add(gangList[i].card.ToCard());
            for (int i1 = 0; i1 < 4; i1++)
            {

                if (gangList[i].catag == 2)
                {//暗杠

                    if (i1 == 1)
                    {
                        Add_EndMjGai(DataManage.Instance.MJSX_Get(gangList[i].card).mj_SpriteName, 2);
                    }
                    else
                    {
                        Add_EndMjGai(DataManage.Instance.MJSX_Get(gangList[i].card).mj_SpriteName, 1);
                    }
                }
                else
                {//明杠
                    Add_EndMjGai(DataManage.Instance.MJSX_Get(gangList[i].card).mj_SpriteName, 1);
                }

            }
            Add_EndMjGai(DataManage.Instance.MJSX_Get(gangList[i].card).mj_SpriteName, 3);
        }

        t_HuPai_Prompt.text += i_AnGang >= 1 ? "[暗杠x" + i_AnGang + "]" : "";
        t_HuPai_Prompt.text += i_MingGang >= 1 ? "[明杠x" + i_MingGang + "]" : "";

        for (int i = 0; i < p_End_Mj_Data.Count && p_End_Mj_Data[i] != 0; i++)
        {
            DataManage.Instance.MJSX_Add(p_End_Mj_Data[i].ToCard());

            Add_EndMjGai(DataManage.Instance.MJSX_Get(p_End_Mj_Data[i]).mj_SpriteName, 1);
        }

        for (int i = 0; i < huPai_.Count; i++)
        {
            if (huPai_[i].card != 216)
            {
                DataManage.Instance.MJSX_Add(huPai_[i].card.ToCard());

                Add_EndMjGai(DataManage.Instance.MJSX_Get(huPai_[i].card).mj_SpriteName, 3);
                Add_EndMjGai(DataManage.Instance.MJSX_Get(huPai_[i].card).mj_SpriteName, 1);

            }
            else
            {
                //Debug.Log(t_ID.text + "ChaJiao???");
            }
        }

    }
    public void Add_EndMjGai(string p_End_Mj_Name, int i_)
    {

        Transform end_Mj = null;//

        //end_Mj.FindChild("Img_PaiMian").GetComponent<Image>().sprite = Gm_Manager.G_M.All_Resources.Get_Mj_PaiMian_Sprite(p_End_Mj_Data);

        switch (i_)
        {

            case 1:

                end_Mj = mjOverManage.GetMj_(1);
                end_Mj.Find("Img_PaiMian").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/NingXiaHuaShui/MJUI/paimian/" + p_End_Mj_Name);
                end_Mj.SetParent(this.img_Layout);
                break;

            case 2:

                end_Mj = mjOverManage.GetMj_(2);
                break;

            case 3:
                end_Mj = mjOverManage.GetMj_(3);
                break;

            default:
                break;
        }

        end_Mj.name = p_End_Mj_Name;
        end_Mj.SetParent(this.img_Layout);
        end_Mj.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    /// <summary>//将空格换成半格
    /// </summary>
    public void Set_Trim()
    {
        RectTransform[] endMJAry = new RectTransform[img_Layout.childCount];
        img_Layout.GetComponent<GridLayoutGroup>().enabled = false;

        for (int i = 0; i < img_Layout.childCount; i++)
        {
            if (img_Layout.GetChild(i).GetComponent<Image>().enabled == false)
            {
                for (int i_1 = i; i_1 < img_Layout.childCount; i_1++)
                {
                    RectTransform rectT_ = img_Layout.GetChild(i_1).GetComponent<RectTransform>();
                    Vector2 v2Jl = rectT_.anchoredPosition;
                    v2Jl.x -= img_Layout.GetChild(i).GetComponent<RectTransform>().sizeDelta.x * 0.5f;
                    rectT_.anchoredPosition = v2Jl;
                }
            }
        }
    }
    #endregion
}

public delegate void OnChangeDiamond(int dia);

[Serializable]
public class Player_Data
{
    public uint p_ID;
    public string P_Name;
    public int p_gold;
    public string p_TxPath;
    public string p_RoomNumber;
    public string p_Ip;
    public int p_ZuoWei_Number;
    public Vector2 adress;

    public int p_Diamond { get; private set; }
    public OnChangeDiamond onChangeDia;
    public int Set_Diamond
    {
        set
        {
            p_Diamond = value;
            if (onChangeDia != null)
            {
                onChangeDia(p_Diamond);
            }
        }
    }
    /// <summary>
    /// 玩家是在游戏过程中
    /// </summary>
    public bool isgaming;
    /// <summary>
    /// 玩家是在Playroom中是否准备
    /// </summary>
    public bool isReady;
    /// <summary>性别
    /// </summary>
    public int sex;

    /// <summary>
    /// 座位 （服务器中位置）
    /// </summary>
    public uint position;

    public Player_Data()
    {
        this.p_ID = 0;
    }
    /// <summary>0、name 。1、ID 。 2、头像Resources路径。 3、房间号码。 5、Ip 。 5: iGold
    /// </summary>
    public Player_Data(string[] str, int i_Gold)
    {
        P_Name = GameManager.GM.ToName(str[0]);
        p_ID = 0;
        p_TxPath = "http://localhost/Image/tx_1.png";// "file://" + @Application.dataPath + "/Resources/tx_1.png";// str[2];
        p_RoomNumber = str[3];
        p_ZuoWei_Number = 1;
        p_Ip = str[4];
        p_gold = i_Gold;

        p_Diamond = 0;
        this.isgaming = false;
        isReady = false;
        this.sex = 1;
    }
    public Player_Data(string Name, uint ID, string IP, string Head, int Diamond, int Money, int sex_1, Vector2 dizhi)
    {
        P_Name = GameManager.GM.ToName(Name);
        p_ID = ID;
        p_Ip = IP;
        p_TxPath = Head;//"http://localhost/Image/tx_1.png";// "file://" + @Application.dataPath + "/Resources/tx_1.png";// Head;
        p_Diamond = Diamond;
        p_gold = Money;
        p_ZuoWei_Number = 1;
        p_RoomNumber = "";
        isgaming = false;
        isReady = false;
        sex = sex_1;
        adress = dizhi; 
    }

    /// <summary>在加入一个房间的时候， 将会调用这个
    /// </summary>
    public Player_Data ReSetPlayerData(ProtoBuf.CharacterInfo info, ProtoBuf.CharacterState stateinfo, int num, uint yn_Zb)
    {
        P_Name = GameManager.GM.ToName(info.userName);
        p_ID = info.charId;
        p_Ip = info.ip;
        p_TxPath = info.portrait;// "http://localhost/Image/tx_1.png";// "file://" + @Application.dataPath + "/Resources/tx_1.png";// info.portrait;
        p_Diamond = (int)info.gold;
        p_gold = (int)info.gold;
        p_ZuoWei_Number = 1;
        p_RoomNumber = "";
        isgaming = false;
        isReady = yn_Zb > 0 ? true : false;
        sex = (int)info.sex;
        position = stateinfo.position;
        return this;
    }

}


/// <summary>玩家座位排序
/// </summary>
public class Player_Data_PaiXu
{
    /// <summary>玩家座位排序
    /// </summary>
    /// <param name="p_Room_P"></param>
    /// <returns></returns>
    public static Player_Data[] For_PaiXu(Player_Data[] p_Room_P)
    {
        Player_Data[] p_Room_P_ChuLi = new Player_Data[4];
        for (int i = 0; i < p_Room_P_ChuLi.Length; i++)
        {
            p_Room_P_ChuLi[i] = new Player_Data();
        }

        int my_Index = -1;
        for (int i = 0; i < p_Room_P.Length; i++)
        {
            //if (p_Room_P[i].p_ID == BaseProto.playerInfo.m_id)
            //{//MY ID
            //    my_Index = i;
            //}
        }
        switch (my_Index)
        {
            case 0:
                p_Room_P_ChuLi = p_Room_P;
                break;
            case 1:
                p_Room_P_ChuLi[0] = p_Room_P[1];
                p_Room_P_ChuLi[1] = p_Room_P[2];
                p_Room_P_ChuLi[2] = p_Room_P[3];
                p_Room_P_ChuLi[3] = p_Room_P[0];
                break;
            case 2:
                p_Room_P_ChuLi[0] = p_Room_P[2];
                p_Room_P_ChuLi[1] = p_Room_P[3];
                p_Room_P_ChuLi[2] = p_Room_P[0];
                p_Room_P_ChuLi[3] = p_Room_P[1];
                break;
            case 3:
                p_Room_P_ChuLi[0] = p_Room_P[3];
                p_Room_P_ChuLi[1] = p_Room_P[0];
                p_Room_P_ChuLi[2] = p_Room_P[1];
                p_Room_P_ChuLi[3] = p_Room_P[2];
                break;
            default:
                break;
        }
        return p_Room_P_ChuLi;
    }

    public static string[] For_PaiXu(string[] p_ID)
    {
        string[] p_Room_P_ChuLi = new string[4];
        //for (int i = 0; i < p_Room_P_ChuLi.Length; i++)
        //{//MY ID
        //    p_Room_P_ChuLi[i] = new string();
        //}

        int my_Index = -1;
        for (int i = 0; i < p_ID.Length; i++)
        {
            //if (p_ID[i].IndexOf(BaseProto.playerInfo.m_id.ToString()) >= 0)
            //{
            //    my_Index = i;
            //}
        }
        switch (my_Index)
        {
            case 0:
                p_Room_P_ChuLi = p_ID;
                break;
            case 1:
                p_Room_P_ChuLi[0] = p_ID[1];
                p_Room_P_ChuLi[1] = p_ID[2];
                p_Room_P_ChuLi[2] = p_ID[3];
                p_Room_P_ChuLi[3] = p_ID[0];
                break;
            case 2:
                p_Room_P_ChuLi[0] = p_ID[2];
                p_Room_P_ChuLi[1] = p_ID[3];
                p_Room_P_ChuLi[2] = p_ID[0];
                p_Room_P_ChuLi[3] = p_ID[1];
                break;
            case 3:
                p_Room_P_ChuLi[0] = p_ID[3];
                p_Room_P_ChuLi[1] = p_ID[0];
                p_Room_P_ChuLi[2] = p_ID[1];
                p_Room_P_ChuLi[3] = p_ID[2];
                break;
            default:
                break;
        }
        return p_Room_P_ChuLi;
    }
}
