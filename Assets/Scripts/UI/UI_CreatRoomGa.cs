using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_CreatRoomGa : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Button Enter = null;
    public Transform Content = null;
    public Toggle Ju4;
    public Toggle Ju8;
    public Toggle dasanyuan, xiaosanyuan, shibaluohan, banzi, feiji, dingque, zipaisuanfeiji, bifan, hsz, zimojiafan, kaxinwu, pihubunenghu, damengqing, yitiaolong;
    GameObject r;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        DeFault();
        //x = Content.DOLocalMoveX(-1800, 0.8f).From();
        //x.SetEase(Ease.OutSine);
        x = Content.DOScale(new Vector3(0.5f,0.5f,0.5f), 0.2f).From();
        x.SetEase(Ease.InOutFlash);
        x.SetAutoKill(false);
        x.PlayForward();
    }
    void Init()
    {
        ThisTrans = gameObject.transform;
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (Enter != null)
            Enter.onClick.AddListener(EnterRoom);

        r = ThisTrans.Find("R/r").gameObject;

        PublicEvent.GetINS.Event_joinRoomSuccess += enter;
        Ju4.onValueChanged.AddListener(delegate { ReturnValue[0] = 4; });
        Ju8.onValueChanged.AddListener(delegate { ReturnValue[0] = 8; });
        dasanyuan.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[1] == 1)
            {
                ReturnValue[1] = 0;
            }
            else
            {
                ReturnValue[1] = 1;
                Debug.Log("dasanyuan开启");
            }
        });

        xiaosanyuan.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[2] == 1)
            {
                ReturnValue[2] = 0;
            }
            else
            {
                ReturnValue[2] = 1;
                Debug.Log("xiaosanyuan开启");
            }
        });
        shibaluohan.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[3] == 1)
            {
                ReturnValue[3] = 0;
            }
            else
            {
                ReturnValue[3] = 1;
                Debug.Log("shibaluohan开启");
            }
        });
        banzi.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[4] == 1)
            {
                ReturnValue[4] = 0;
            }
            else
            {
                ReturnValue[4] = 1;
                Debug.Log("banzi开启");
            }
        });
        feiji.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[5] == 1)
            {
                ReturnValue[5] = 0;
            }
            else
            {
                ReturnValue[5] = 1;
                Debug.Log("feiji开启");
            }
        });
        if (dingque != null)
            dingque.onValueChanged.AddListener(delegate
            {
                if (ReturnValue[6] == 1)
                {
                    ReturnValue[6] = 0;
                }
                else
                {
                    ReturnValue[6] = 1;
                    Debug.Log("dingque开启");
                }
            });
        zipaisuanfeiji.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[7] == 1)
            {
                ReturnValue[7] = 0;
            }
            else
            {
                ReturnValue[7] = 1;
                Debug.Log("zipaisuanfeiji开启");
            }
        });
        bifan.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[8] == 1)
            {
                ReturnValue[8] = 0;
            }
            else
            {
                ReturnValue[8] = 1;
                Debug.Log("bifan开启");
            }
        });
        hsz.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[9] == 1)
            {
                ReturnValue[9] = 0;
            }
            else
            {
                ReturnValue[9] = 1;
                Debug.Log("换三张开启");
            }
        });
        zimojiafan.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[10] == 1)
            {
                ReturnValue[10] = 0;
            }
            else
            {
                ReturnValue[10] = 1;
                Debug.Log("zimojiafan开启");
            }
        });
        kaxinwu.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[11] == 1)
            {
                ReturnValue[11] = 0;
            }
            else
            {
                ReturnValue[11] = 1;
                Debug.Log("kaxinwu开启");
            }
        });
        pihubunenghu.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[12] == 1)
            {
                ReturnValue[12] = 0;
            }
            else
            {
                ReturnValue[12] = 1;
                Debug.Log("pihubunenghu开启");
            }
        });
        damengqing.onValueChanged.AddListener(
            delegate
            {
                if (ReturnValue[13] == 1)
                {
                    ReturnValue[13] = 0;
                }
                else
                {
                    ReturnValue[13] = 1;
                    Debug.Log("damengqing开启");
                }
            }
            );
        yitiaolong.onValueChanged.AddListener(
            delegate
            {
                if (ReturnValue[14] == 1)
                {
                    ReturnValue[14] = 0;
                }
                else
                {
                    ReturnValue[14] = 1;
                    Debug.Log("yitiaolong开启");
                }
            });

    }
    void Rest()
    {
        if (r != null)
            Destroy(r);
        GameManager.GM.DS.CreateRoom_Hoom = GameManager.GM.PopUI(ResPath.CreateRoom_Hoom);
        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        GameManager.GM.DS.CreateRoom_Ga = null;
        //x = Content.DOLocalMoveX(-1600, 0.4f);
        x = Content.DOScale(new Vector3(0.5f,0.5f,0.5f), 0.2f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        //Destroy(this.gameObject);
    }
    int[] ReturnValue = { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    void EnterRoom()
    {
        if (Enter.IsInteractable())
            Enter.interactable = false;
        //if (r != null)
        //    Destroy(r);
        Invoke("reOpen", 2.0f);
        Debug.Log("进入房间");
        PublicEvent.GetINS.NewRoom(ReturnValue, "poker");
    }
    void reOpen()
    {
        Enter.interactable = true;
    }
    void enter(ProtoBuf.EnterRoomRsp rsp)
    {

        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        GameManager.GM.DS.CreateRoom_Ga = null;
        {
            //创建playroom
            //GameManager.GM.DS.MJGameController = GameManager.GM.PopUI(ResPath.MJGameController,false);
            //wait();
            ///向房间注入当前服务器返回的房间信息
            //GameManager.GM.DS.uiWin_InGame_MJ.GetComponent<uiWin_InGame_MJ>().ReciveRoomData(rsp);

            //对当前的玩家进行排序
            //PublicEvent.GetINS.ReciveData(rsp);

            ParticleManager.GetIns.SwitchSence(2);
        }
        //x = Content.DOLocalMoveX(-1600, 0.3f);
        x = Content.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f);
        x.OnComplete(delegate
        {

            Destroy(this.gameObject);
            //if (r != null)
            //    Destroy(r);
        });
    }
}
