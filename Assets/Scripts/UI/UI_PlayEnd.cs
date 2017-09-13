using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoBuf;
using System.Linq;

public struct ThePlayerInfo
{
    uint Name;
    string Gold;
    Sprite Head;
    string ThisGold;
    int[] Tag;

    public uint ID1
    {
        get
        {
            return Name;
        }

        set
        {
            Name = value;
        }
    }

    public string Gold1
    {
        get
        {
            return Gold;
        }

        set
        {
            Gold = value;
        }
    }

    public Sprite Head1
    {
        get
        {
            return Head;
        }

        set
        {
            Head = value;
        }
    }

    public string ThisGold1
    {
        get
        {
            return ThisGold;
        }

        set
        {
            ThisGold = value;
        }
    }
    /// <summary>
    /// 0胡  1自摸   2放炮   3没有胡
    /// </summary>
    public int[] Tag1
    {
        get
        {
            return Tag;
        }

        set
        {
            Tag = value;
        }
    }

    public ThePlayerInfo(uint name = 0, string gold = "缺省", Sprite head = null, string thisgold = "缺省", int[] tag = null)
    {
        Name = name;
        Gold = gold;
        Head = head;
        ThisGold = thisgold;
        Tag = tag;
    }

}

public class UI_PlayEnd : MonoBehaviour
{
    Transform ThisTrans = null;
    Button Share = null;
    Button Ready = null;
    GameObject Hu, ZiMo, DianPao;
    public GameObject Player0Cards;
    public GameObject Player1Cards;
    public GameObject Player2Cards;
    public GameObject Player3Cards;
    /// <summary>
    /// 手牌
    /// </summary>
    public GameObject card;
    public GameObject backCard;
    public GameObject gangcard;
    public GameObject nullCard;
    //List<PlayerInfo> Players = new List<PlayerInfo>();
    /// <summary>
    /// 放杠和放炮的次数
    /// </summary>
    public Text[] fgang = new Text[4];
    public Text[] Hutype = new Text[4];
    public Text Jushu = null;
    public Text RoomNum = null;
    void Awake()
    {
        GameManager.GM.GameEnd = true;
        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
        Init();
    }

    // Use this for initialization
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        Invoke("show", 0.0f);
    }
    void show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        if (GameManager.GM.DS.MJGameController != null)
            GameManager.GM.DS.MJGameController.GetComponent<InGameManage_3DMJ>().PlayEndInit();

        ShowFace.Ins.DistoryDelayAnim();
        ShowFace.Ins.DisAllAnim();
    }
    void Init()
    {
        ThisTrans = this.gameObject.transform;
        Share = ThisTrans.Find("BG/Share").GetComponent<Button>();
        Ready = ThisTrans.Find("BG/Ready").GetComponent<Button>();
        InvokeRepeating("OpenReady", 0, 1);
    }
    int temp = 0;
    void OpenReady()
    {
        if (int.Parse(ThisTrans.Find("BG/Ready/Text").GetComponent<Text>().text) <= 0)
        {
            ThisTrans.Find("BG/Ready/Text").gameObject.SetActive(false);
            ThisTrans.Find("BG/Ready/Image").GetComponent<Image>().raycastTarget = true;
            ThisTrans.Find("BG/Ready").GetComponent<Image>().raycastTarget = true;
        }
        else
        {
            temp = int.Parse(ThisTrans.Find("BG/Ready/Text").GetComponent<Text>().text);
            temp--;
            ThisTrans.Find("BG/Ready/Text").GetComponent<Text>().text = temp.ToString();
        }
    }
    public void Default(ThePlayerInfo[] players = null, bool ShowDaoJiShi = true)
    {
        if (!ShowDaoJiShi)
            ThisTrans.Find("BG/Ready/Text").GetComponent<Text>().text = "0";
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                ThisTrans.Find("BG/Player" + (1 + i) + "/Tag0").gameObject.SetActive(false);
                ThisTrans.Find("BG/Player" + (1 + i) + "/Tag1").gameObject.SetActive(false);
                ThisTrans.Find("BG/Player" + (1 + i) + "/Tag2").gameObject.SetActive(false);
            }

            for (int i = 0; i < players.Length; i++)
            {
                ThePlayerInfo Temp = new ThePlayerInfo(players[i].ID1, players[i].Gold1, players[i].Head1, players[i].ThisGold1, players[i].Tag1);
                ThisTrans.Find("BG/Player" + (1 + i) + "/Head/Mask/HeadSprite").GetComponent<Image>().sprite = Temp.Head1;
                ThisTrans.Find("BG/Player" + (1 + i) + "/NickName").GetComponent<Text>().text = "昵称：" + DataManage.Instance.PData_GetData(Temp.ID1).P_Name;
                ThisTrans.Find("BG/Player" + (1 + i) + "/AllGold").GetComponent<Text>().text = "积分：" + Temp.Gold1;
                ThisTrans.Find("BG/Player" + (1 + i) + "/ThisGold").GetComponent<Text>().text = Temp.ThisGold1;
                for (int p = 0; p < players[i].Tag1.Length; p++)
                {
                    if (players[i].Tag1[p] == 1)
                        ThisTrans.Find("BG/Player" + (1 + i) + "/Tag" + p).gameObject.SetActive(true);
                }
                if (players[i].ID1 == BaseProto.playerInfo.m_id)
                {
                    int jianindex = int.Parse(Temp.ThisGold1);
                    if (jianindex == 0)
                    {
                        ThisTrans.Find("BG2/PingjuImage").gameObject.SetActive(true);
                    }
                    else if (jianindex > 1)
                    {
                        ThisTrans.Find("BG2/SuccesImage").gameObject.SetActive(true);
                    }
                    else
                    {
                        ThisTrans.Find("BG2/LoseImage").gameObject.SetActive(true);

                    }
                }

            }
        }
        Share.onClick.AddListener(ShareToWX);
        Ready.onClick.AddListener(ReadyToPlay);
    }
    void ShareToWX()
    {
        Debug.Log("Share");
        GameManager.GM.Share(0);
    }
    void ReadyToPlay()
    {
        Debug.Log("!!!!!!!");
        if (PublicEvent.GetINS.VoteQuit)
        {
            GameManager.GM.DS.GameOver = GameManager.GM.PopUI(ResPath.GameOver);
            if (GameManager.GM.DS.GameOver != null)
            {
                Debug.Log("GameOver的UI不为空");
                GameManager.GM.DS.GameOver.GetComponent<UI_GameOver>().Default(PublicEvent.GetINS.GameOverRsp);
            }
            Rest();
        }
        if (MJProto.Inst().GameOver)
        {
            Debug.Log("GameManager.GM.DS.PlayRoom.GetComponent<UI_PlayRoom>().junum：" + DataManage.Instance.RoomSyJuShu);
            GameManager.GM.DS.GameOver = GameManager.GM.PopUI(ResPath.GameOver);
            if (GameManager.GM.DS.GameOver != null)
            {
                Debug.Log("GameOver的UI不为空");
                GameManager.GM.DS.GameOver.GetComponent<UI_GameOver>().Default(PublicEvent.GetINS.GameOverRsp);
            }
            Rest();
        }
        else
        {
            if (GameManager.GM.DS.InGameManage_3DMjUI != null)
                GameManager.GM.DS.InGameManage_3DMjUI.GetComponent<InGameManage_3DMjUI>().OnClick_Btn_SetoutStart();
            Debug.Log("Ready");
            Rest();
        }
    }
    void Rest()
    {
        Debug.Log("发生销毁！");
        Destroy(this.gameObject);
        Destroy(this);
    }
    void OnDestroy()
    {
        GameManager.GM.GameEnd = false;
        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
        GameManager.GM.DS.PlayEnd = null;
    }
    public void SetCard(ProtoBuf.MJGameOver rsp)
    {
        Jushu.text = (DataManage.Instance.roomJuShu_Max + 1 - DataManage.Instance.RoomSyJuShu).ToString();
        RoomNum.text = "房间号:" + rsp.roomId.ToString();
        for (int i = 0; i < rsp.players.Count; i++)
        {
            if (rsp.ownerId == rsp.players[i].charId)
            {
                ThisTrans.Find("BG/Player" + (i + 1).ToString() + "/fangzhuUI").GetComponent<Image>().enabled = true;
            }
            if (rsp.zjId == rsp.players[i].charId)
            {
                ThisTrans.Find("BG/Player" + (i + 1).ToString() + "/zhuangUI").GetComponent<Image>().enabled = true;
            }
            if (i == 0)
            {
                SetPlayerCard(Player0Cards, rsp, i);
            }
            if (i == 1)
            {
                SetPlayerCard(Player1Cards, rsp, i);
            }
            if (i == 2)
            {
                SetPlayerCard(Player2Cards, rsp, i);
            }
            if (i == 3)
            {
                SetPlayerCard(Player3Cards, rsp, i);
            }
        }
    }
    #region 牌面处理
    void SetPlayerCard(GameObject Player, ProtoBuf.MJGameOver rsp, int num)
    {
        for (int k = 0; k < rsp.players[num].pengCards.Count; k++)//加入碰的牌
        {
            SetPengCard(Player.transform, rsp.players[num].pengCards[k].ToCard().mj_SpriteName);
        }

        for (int l = 0; l < rsp.players[num].gangInfos.Count; l++)//加入杆的牌
        {
            if (rsp.players[num].gangInfos[l].catag == 2)//说明当前的这个牌是暗牌
            {
                SetAnGangCard(Player.transform, rsp.players[num].gangInfos[l].card.ToCard().mj_SpriteName);
            }
            else//说明这个是明牌
            {
                SetGangCard(Player.transform, rsp.players[num].gangInfos[l].card.ToCard().mj_SpriteName);
            }
        }
        int cards = rsp.players[num].restCards.Count;//手牌数量
        rsp.players[num].restCards.Sort();//排序
        for (int p = 0; p < cards; p++)
        {
            SetHandCard(Player.transform, rsp.players[num].restCards[p].ToCard().mj_SpriteName);

        }
        for (int z = 0; z < rsp.players[num].huInfos.Count; z++)//胡牌
        {
            if (rsp.players[num].huInfos[0].card != 216)
            {
                SetNullCard(Player.transform);
                SetHandCard(Player.transform, rsp.players[num].huInfos[0].card.ToCard().mj_SpriteName);
            }
        }

        List<string> Temp = new List<string>();

        for (int P = 0; P < 4; P++)
        {
            if (rsp.players[P].fanggangInfos.Count > 0)
                Temp.Add("放杠次数：" + rsp.players[P].fanggangInfos.Count + "    ");

            if (rsp.players[P].gangInfos.Count > 0)
            {
                for (int i = 0; i < rsp.players[P].gangInfos.Count; i++)
                {
                    if (rsp.players[P].gangInfos[i].oricharId != 0)
                    {
                        if (rsp.players[P].gangInfos[i].catag == 2)
                        {
                            //Temp.Add("暗杠");
                        }
                        else if (rsp.players[P].gangInfos[i].catag == 1)
                        {
                            //Temp.Add("明杠");
                        }
                        else
                        {
                            Temp.Add("接" + GameManager.GM.GetPlayerName(rsp.players[P].gangInfos[i].oricharId) + "杠牌" + " ");
                        }
                    }
                }
            }
            if (rsp.players[P].fanggangInfos.Count > 0)
            {
                for (int i = 0; i < rsp.players[P].fanggangInfos.Count; i++)
                {
                    if (rsp.players[P].fanggangInfos[i].catag == 1)
                    {
                        Temp.Add(GameManager.GM.GetPlayerName(rsp.players[P].fanggangInfos[i].ganger) + "明杠" + " ");
                    }
                    else if (rsp.players[P].fanggangInfos[i].catag == 2)
                    {
                        Temp.Add(GameManager.GM.GetPlayerName(rsp.players[P].fanggangInfos[i].ganger) + "被暗杠" + " ");
                    }
                }
            }

            if (rsp.players[P].fpInfos.Count > 0)
            {
                for (int i = 0; i < rsp.players[P].fpInfos.Count; i++)
                {
                    if (rsp.players[P].fpInfos[i].card == 216)
                        Temp.Add("被" + DataManage.Instance.PData_GetData(rsp.players[P].fpInfos[i].huer).P_Name + "查叫 ");
                    else if ((rsp.players[P].fpInfos[i].catag & 0x10000000) > 0)
                    {
                        Temp.Add("被" + GameManager.GM.GetPlayerName(rsp.players[P].fpInfos[i].huer) + "自摸" + " ");
                    }
                    else
                        Temp.Add("给" + DataManage.Instance.PData_GetData(rsp.players[P].fpInfos[i].huer).P_Name + "放炮 ");
                }
            }
            if (rsp.players[P].huInfos.Count > 0 && rsp.players[P].huInfos[0].card != 216)
            {
                Hutype[P].text = GetHuType((int)rsp.players[P].huInfos[0].catag);
            }
            if (rsp.players[P].huInfos.Count > 0)
            {
                uint gen;
                gen=rsp.players[P].huInfos[0].param1;
                //if (rsp.players[P].gangInfos.Count > 0)
                //{
                //    gen += rsp.players[P].gangInfos.Count;
                //}
                //if (rsp.players[P].pengCards.Count > 0)
                //{
                //    for (int i = 0; i < rsp.players[P].pengCards.Count; i++)
                //    {
                //        if (rsp.players[P].restCards.Contains(rsp.players[P].pengCards[i].ToCard().mJCardID))
                //        {
                //            gen += 1;
                //        }
                //    }
                //}

                //if (rsp.players[P].restCards.Count > 3)
                //{
                //    Dictionary<int, int> myDic = new Dictionary<int, int>();
                //    for (int i = 0; i < rsp.players[P].restCards.Count; i++)
                //    {
                //        if (myDic.Keys.Contains(rsp.players[P].restCards[i].ToCard().value))
                //        {
                //            myDic[rsp.players[P].restCards[i].ToCard().value]++;
                //        }
                //        else
                //        {
                //            myDic[rsp.players[P].restCards[i].ToCard().value] = 1;
                //        }
                //    }

                //    int[] numArr = myDic.Where(t => t.Value > 1).ToDictionary(t => t.Key, t => t.Value).Keys.ToArray();
                //    for (int i = 0; i < numArr.Length; i++)
                //    {
                //        if (numArr[i]==4)
                //        {
                //            gen += 1;
                //        }
                //    }
                //}
                if (gen != 0)
                {
                    Hutype[P].text += ",带" + gen + "根";
                }
                
            }
         

            if (Temp.Count > 1)
                for (int i = 1; i < Temp.Count; i++)
                {
                    Temp[0] += Temp[i];
                }
            if (Temp.Count > 0)
                fgang[P].text = Temp[0];
            Temp.Clear();
        }
    }
    /// <summary>
    /// 设定当前的空牌给指定的位置
    /// </summary>
    /// <param name="SetPostion"></param>
    void SetNullCard(Transform SetPostion)
    {
        var temp = Instantiate(nullCard);
        temp.SetActive(true);
        temp.transform.SetParent(SetPostion, false);
    }
    /// <summary>
    /// 放置当前暗刚的位置
    /// </summary>
    /// <param name="Setposetiom"></param>
    /// <param name="card"></param>
    void SetAnGangCard(Transform Setpostion, string thecard)

    {
        var t = Instantiate(backCard);
        t.SetActive(true);
        t.transform.SetParent(Setpostion, false);
        t.transform.Find("card/Image").GetComponent<Image>().sprite = GetPrefabSprite(thecard);
        SetNullCard(Setpostion);
        SetNullCard(Setpostion);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Setpostion"></param>
    /// <param name="thecard"></param>
    void SetGangCard(Transform Setpostion, string thecard)
    {
        var t = Instantiate(gangcard);
        t.SetActive(true);
        t.transform.SetParent(Setpostion, false);
        t.transform.Find("Image").GetComponent<Image>().sprite = GetPrefabSprite(thecard);
        t.transform.Find("Right/Image").GetComponent<Image>().sprite = GetPrefabSprite(thecard);
        t.transform.Find("card/Image").GetComponent<Image>().sprite = GetPrefabSprite(thecard);
        SetNullCard(Setpostion);
        SetNullCard(Setpostion);
        SetNullCard(Setpostion);
    }
    /// <summary>
    /// 设定当前的碰牌
    /// </summary>
    /// <param name="Setpostion"></param>
    /// <param name="thecard"></param>
    void SetPengCard(Transform Setpostion, string thecard)
    {
        for (int i = 0; i < 3; i++)
        {
            var t = Instantiate(card);
            t.SetActive(true);
            t.transform.SetParent(Setpostion, false);
            t.transform.GetChild(0).GetComponent<Image>().sprite = GetPrefabSprite(thecard);
        }
        SetNullCard(Setpostion);
    }
    /// <summary>
    /// 设定当前的牌
    /// </summary>
    /// <param name="Setpostion"></param>
    /// <param name="theCard"></param>
    void SetHandCard(Transform Setpostion, string theCard)
    {
        var t = Instantiate(card);
        t.SetActive(true);
        t.transform.SetParent(Setpostion, false);
        t.transform.GetChild(0).GetComponent<Image>().sprite = GetPrefabSprite(theCard);
    }
    Sprite tempSprite = null;
    Sprite GetPrefabSprite(string name)
    {
        tempSprite = Resources.Load<GameObject>("Prefabs/Cards/" + name).GetComponent<Image>().sprite;
        if (tempSprite != null)
        {
            return tempSprite;
        }
        else
        {
            Debug.Log("老哥，翻车啦！");
            tempSprite = Resources.Load<Image>("Prefabs/Cards/5h").sprite;
        }
        return tempSprite;

    }
    #endregion
    string GetHuType(int value)
    {
        switch (value)
        {
            case 1:
                return "清龙七对";
            case 2:
                return "清幺九";
            case 3:
                return "清七对";
            case 4:
                return "龙七对";
            case 5:
                return "清大对";
            case 6:
                return "将对";
            case 7:
                return "幺九";
            case 8:
                return "小七对";
            case 9:
                return "清一色";
            case 10:
                return "大对子";
            case 11:
                return "平胡";
            case 12:
                return "地胡";
            case 13:
                return "天胡";
            case 21:
                return "清金钩钓";
            case 22:
                return "金钩钓";
            default:
                break;
        }
        return "";
    }
    
}
