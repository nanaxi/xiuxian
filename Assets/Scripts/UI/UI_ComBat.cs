using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ComBat : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public Transform ContentList = null;
    public Transform jpyContentList = null;
    public GameObject jpyBtn = null;
    public GameObject xzBtn = null;
    public Image jpy = null;
    public Image xz = null;
    public GameObject jpyComBat = null;
    public GameObject xzComBat = null;
    GameObject DarkItem, LightItem;
    void Awake()
    {
        Init();
    }
    Tween x;

    void Init()
    {
        ThisTrans = gameObject.transform;
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
        DeFault();
        PublicEvent.GetINS.ReturnToMain += Rest;
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (jpyBtn != null)
        {
            jpyBtn.GetComponent<Button>().onClick.AddListener(SetJpyComBat);
        }
        if (xzBtn != null)
        {
            xzBtn.GetComponent<Button>().onClick.AddListener(SetXzComBat);
        }
        DarkItem = Resources.Load<GameObject>("Prefabs/Btn_Zj_Select_Bg_dark");
        LightItem = Resources.Load<GameObject>("Prefabs/Btn_Zj_Select_Bg_light");
    }

    public ProtoBuf.GameType gameType = ProtoBuf.GameType.GT_Poker;
    ProtoBuf.QueryInfoRsp CombatGainsRsp = null;
    public void SetComBat(ProtoBuf.QueryInfoRsp combatGainRsp, ProtoBuf.QueryInfoRsp qpcombatGainRsp)
    {
        if (qpcombatGainRsp != null)
        {
            for (int i = 0; i < qpcombatGainRsp.qpRecords.Count; i++)
            {
                Transform tempItem = null;
                if (i % 2 == 0)
                {
                    tempItem = Instantiate(DarkItem).transform;
                    tempItem.SetParent(jpyContentList, false);
                    tempItem.GetComponent<Combat>().SetCombatInformation(qpcombatGainRsp.qpRecords[i]);
                }
                else
                {
                    tempItem = Instantiate(LightItem).transform;
                    tempItem.SetParent(jpyContentList, false);
                    tempItem.GetComponent<Combat>().SetCombatInformation(qpcombatGainRsp.qpRecords[i]);
                }
            }
        }
        if (combatGainRsp != null)
        {
            CombatGainsRsp = combatGainRsp;
            Debug.Log("CombatGainsRsp.mjRecords.Count:" + CombatGainsRsp.mjRecords.Count);
            for (int i = 0; i < CombatGainsRsp.mjRecords.Count; i++)
            {
                Transform tempItem = null;
                if (i % 2 == 0)
                {
                    tempItem = Instantiate(DarkItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetCombatInformation(CombatGainsRsp.mjRecords[i]);
                }
                else
                {
                    tempItem = Instantiate(LightItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetCombatInformation(CombatGainsRsp.mjRecords[i]);
                }
            }
        }

      
    }
    public Sprite[] IMG = new Sprite[8];
    void SetJpyComBat()
    {
     
        jpyComBat.SetActive(true);
        xzComBat.SetActive(false);
        jpyBtn.GetComponent<Image>().sprite = IMG[2]; //Resources.Load("Texture/anniu_huang", typeof(Sprite)) as Sprite;
        jpy.GetComponent<Image>().sprite = IMG[3];//Resources.Load("Texture/chuangjianfangjan_huan_jiaopengyou", typeof(Sprite)) as Sprite;
        xzBtn.GetComponent<Image>().sprite = IMG[6];//Resources.Load("Texture/anniu_lv", typeof(Sprite)) as Sprite;
        xz.GetComponent<Image>().sprite = IMG[7];//Resources.Load("Texture/chuangjianfangjian_lv_xuezhandaodi", typeof(Sprite)) as Sprite;


        gameType = ProtoBuf.GameType.GT_Poker;
    }
    void SetXzComBat()
    {
       
        jpyComBat.SetActive(false);
        xzComBat.SetActive(true);
        jpyBtn.GetComponent<Image>().sprite = IMG[0];// Resources.Load("Texture/anniu_lv", typeof(Sprite)) as Sprite;
        jpy.GetComponent<Image>().sprite = IMG[1];//Resources.Load("Texture/chuangjianfangjan_lv_jiaopengyou", typeof(Sprite)) as Sprite;
        xzBtn.GetComponent<Image>().sprite = IMG[4];//Resources.Load("Texture/anniu_huang", typeof(Sprite)) as Sprite;
        xz.GetComponent<Image>().sprite = IMG[5];//Resources.Load("Texture/chuangjianfangjian_huang_xuezhandaodi", typeof(Sprite)) as Sprite;


        gameType = ProtoBuf.GameType.GT_MJ;
    }
    void Rest()
    {
        PublicEvent.GetINS.ReturnToMain -= Rest;
        //x = Content.DOLocalMoveX(-1800, 0.3f);
        //x.OnComplete(delegate { Destroy(this.gameObject); });
        GameManager.GM.DS.Combat = null;
        Destroy(this.gameObject);
    }
}
