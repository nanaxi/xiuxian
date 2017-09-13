using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CombatItems : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public Transform ContentList = null;
    GameObject DarkItem, LightItem;

    void Awake()
    {
        Init();
    }
    Tween x;
    // Use this for initialization
    void Start()
    {
        //DeFault();
        x = Content.DOLocalMoveX(-1800, 0.5f).From();
        x.SetEase(Ease.OutSine);
        x.SetAutoKill(false);
        x.PlayForward();
    }
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
        DarkItem = Resources.Load<GameObject>("Prefabs/Btn_Zj_Select_Bg_dark");
        LightItem = Resources.Load<GameObject>("Prefabs/Btn_Zj_Select_Bg_light");
    }
    ProtoBuf.MJRoomRecord CombatGainsRsp = null;
    public void SetComBat(ProtoBuf.MJRoomRecord MjRoomRec)//CombatGainsRsp.mjRecords[j].rounds[t]
    {
        if (MjRoomRec != null)
        {
            CombatGainsRsp = MjRoomRec;
            for (int i = 0; i < CombatGainsRsp.rounds.Count; i++)
            {
                Transform tempItem = null;
                if (i % 2 == 0)
                {
                    tempItem = Instantiate(DarkItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetLocalCombatInformation(MjRoomRec.rounds[i]);
                }
                else
                {
                    tempItem = Instantiate(LightItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetLocalCombatInformation(MjRoomRec.rounds[i]);
                }
            }
        }
    }

    public void SetComBat(ProtoBuf.QPRoomRecord qpRoomRec)//CombatGainsRsp.mjRecords[j].rounds[t]
    {
        if (qpRoomRec != null)
        {
            for (int i = 0; i < qpRoomRec.rounds.Count; i++)
            {
                Transform tempItem = null;
                if (i % 2 == 0)
                {
                    tempItem = Instantiate(DarkItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetLocalCombatInformation(qpRoomRec.rounds[i]);
                }
                else
                {
                    tempItem = Instantiate(LightItem).transform;
                    tempItem.SetParent(ContentList, false);
                    tempItem.GetComponent<Combat>().SetLocalCombatInformation(qpRoomRec.rounds[i]);
                }
            }
        }
    }

    void Rest()
    {
        PublicEvent.GetINS.ReturnToMain -= Rest;
        x = Content.DOLocalMoveX(-1800, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        //Destroy(this.gameObject);
    }
}
