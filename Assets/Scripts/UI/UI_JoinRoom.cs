using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
public class UI_JoinRoom : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Button[] ClickNum = new Button[10];
    public Text[] ShowNum = new Text[6];
    Button backInput = null;
    Button reInput = null;
    public Transform Content = null;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        DeFault();
        //x = Content.DOLocalMoveX(-1800, 0.5f).From();
        //x.SetEase(Ease.OutSine);
        //x.SetAutoKill(false);
        //x.PlayForward();
    }
    void Init()
    {
        ThisTrans = gameObject.transform;
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
        for (int i = 0; i < 10; i++)
        {
            ClickNum[i] = ThisTrans.Find("maskImage/Mask/Content/BG/NumberButton/Btn_Number_" + i).GetComponent<Button>();
        }
        for (int i = 0; i < 6; i++)
        {
            ShowNum[i] = ThisTrans.Find("maskImage/Mask/Content/BG/Input_Num/" + i).GetComponentInChildren<Text>();
        }
        backInput = ThisTrans.Find("maskImage/Mask/Content/BG/NumberButton/Btn_Number_12").GetComponent<Button>();
        reInput = ThisTrans.Find("maskImage/Mask/Content/BG/NumberButton/Btn_Number_11").GetComponent<Button>();
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (ClickNum.Length > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                int t = i;
                ClickNum[i].onClick.AddListener(delegate
                {
                    //Debug.Log("Pressed " + t);
                    InputAndShow(t.ToString());
                });
            }
        }
        if (backInput != null)
            backInput.onClick.AddListener(BackInput);
        if (reInput != null)
            reInput.onClick.AddListener(ReInput);

        PublicEvent.GetINS.Event_joinRoomSuccess += enter;
    }
    List<string> RoomNum = new List<string>();
    void InputAndShow(string value)
    {
        if (RoomNum.Count < 6)
            RoomNum.Add(value);
        for (int i = 0; i < RoomNum.Count; i++)
        {
            int temp = i + 1;
            ShowNum[i].text = "";
            if (RoomNum[i] != "")
            {
                ShowNum[i].text = RoomNum[i];
            }
        }
        if (RoomNum.Count >= 6)
        {
            Enter();
        }
    }

    void Enter()
    {
        int temp = 0;
        if (RoomNum.Count >= 6)
        {
            Debug.Log("进入的部分已经写了");
            //Debug.Log(RoomNum[0]+ RoomNum[1]+ RoomNum[2]+ RoomNum[3]+ RoomNum[4]+ RoomNum[5]);

            //list<string>转码为int，输出为Temp,再强转换为uint
            for (int i = 0; i < 6; i++)
            {
                int Beishu = 1;
                for (int j = 5 - i; j > 0; j--)
                {
                    Beishu *= 10;
                }
                temp += int.Parse(RoomNum[i]) * Beishu;
            }
            //相当于  temp = int.Parse(RoomNum[0]) * 100000+ int.Parse(RoomNum[1]) * 10000+int.Parse(RoomNum[2]) * 1000+int.Parse(RoomNum[3]) * 100+int.Parse(RoomNum[4]) * 10+ int.Parse(RoomNum[5]) * 1;
            PublicEvent.GetINS.AppJoin((uint)temp);
            Debug.Log("进入的房间号：" + temp);
        }
        else
        {
            Debug.Log("进入房间失败");
            //var window = GameManager.GM.SearchEmpty().AddComponent<Notic>();
            //window.Ins("房间号不为6位，发送错误！");
            //window.transform.SetAsLastSibling();
        }
    }

    void ReInput()
    {
        //SoundMag.GetINS.PlaySound(1);
        Debug.Log("重输");
        RoomNum.Clear();
        for (int i = 0; i < 6; i++)
        {
            ShowNum[i].text = null;
        }
    }
    void BackInput()
    {
        //SoundMag.GetINS.PlaySound(1);
        Debug.Log("退格");
        if (RoomNum.Count == 1)
            ReInput();
        if (RoomNum.Count > 0)
        {
            RoomNum.RemoveAt(RoomNum.Count - 1);
            for (int i = 1; i < 6; i++)
            {
                ShowNum[i].text = null;
            }
            for (int i = 0; i < RoomNum.Count; i++)
            {
                int temp = i + 1;
                ShowNum[i].text = null;
                if (RoomNum[i] != null)
                {
                    //Debug.Log("当前的下标是" + i + "当前的值是" + RoomNum[i]);
                    ShowNum[i].text = RoomNum[i];
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    void Rest()
    {
        GameManager.GM.DS.JoinRoom = null;
        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        //x = Content.DOLocalMoveX(-1800, 0.3f);
        //x.OnComplete(
        //delegate
        //{
        //    Destroy(this.gameObject);
        //    Destroy(this);
        //}
        //);
        Destroy(this.gameObject);
        Destroy(this);
    }
    void enter(ProtoBuf.EnterRoomRsp rsp)
    {
        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        GameManager.GM.DS.JoinRoom = null;

        {
            if (rsp.gameType == ProtoBuf.GameType.GT_MJ)
            {
                DataManage.Instance._roomEnterRsp = rsp;
                //PublicEvent.GetINS.ReciveData(rsp);
                Debug.Log("注入");
                ///向房间注入当前服务器返回的房间信息    
            }
            ParticleManager.GetIns.SwitchSence(2);
        }

        x = Content.DOLocalMoveX(-1800, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
    }
}
