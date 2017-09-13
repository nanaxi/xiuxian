using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class UI_CreatRoomXz : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Button Enter = null;
    //public Transform Content = null;
    public Slider jushu = null;
    public Text jushutext = null;
   // public Toggle Ju4, Ju8;
    public Toggle[] fengding=new Toggle[5];
    public Toggle hsz, tiandihu, /*dingque*/ mengqingzhongzhnag, /*yaojiujiangdui,*/ zimojiafan, zimojiadi, dianganghua, diangangpao,hujiaozhuangyi,daiyaojiujiangdui;
    //GameObject r;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        if (jushu != null)
        {
            jushu.onValueChanged.AddListener((float value) => OnSliderClick(jushu, value));
        }
        DeFault();
    }
    void Init()
    {
        ThisTrans = gameObject.transform;
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (Enter != null)
            Enter.onClick.AddListener(EnterRoom);
        PublicEvent.GetINS.Event_joinRoomSuccess += enter;
        {
            fengding[0].onValueChanged.AddListener(delegate {
                    ReturnValue[1] = 2;
                    Debug.Log("fengding开启2");
            });
            fengding[1].onValueChanged.AddListener(delegate {
                    ReturnValue[1] = 3;
                    Debug.Log("sanfang开启3");
            });
            fengding[2].onValueChanged.AddListener(delegate {
                    ReturnValue[1] = 4;
                    Debug.Log("sifang开启4");
            });
            fengding[3].onValueChanged.AddListener(delegate {
                    ReturnValue[1] = 5;
                    Debug.Log("wufang开启5");
            });
            fengding[4].onValueChanged.AddListener(delegate {
                    ReturnValue[1] = 0;
                    Debug.Log("buxianfang开启无限");               
            });
        }

        hsz.onValueChanged.AddListener(delegate {
            if (ReturnValue[2] == 1)
            {
                ReturnValue[2] = 0;
            }
            else
            {
                ReturnValue[2] = 1;
                Debug.Log("hsz开启");
            }
        });
        tiandihu.onValueChanged.AddListener(delegate {
            if (ReturnValue[3] == 1)
            {
                ReturnValue[3] = 0;
            }
            else
            {
                ReturnValue[3] = 1;
                Debug.Log("tiandihu开启");
            }
        });

        hujiaozhuangyi.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[4] == 1)
            {
                ReturnValue[4] = 0;
            }
            else
            {
                ReturnValue[4] = 1;
                Debug.Log("hujiaozhuangyi开启");
            }
        });
        mengqingzhongzhnag.onValueChanged.AddListener(delegate {
            if (ReturnValue[5] == 1)
            {
                ReturnValue[5] = 0;
            }
            else
            {
                ReturnValue[5] = 1;
                Debug.Log("mengqingzhongzhnag开启");
            }
        });
        daiyaojiujiangdui.onValueChanged.AddListener(delegate
        {
            if (ReturnValue[6] == 1)
            {
                ReturnValue[6] = 0;
            }
            else
            {
                ReturnValue[6] = 1;
                Debug.Log("daiyaojiujiangdui开启");
            }
        });
        zimojiafan.onValueChanged.AddListener(delegate {
            if (ReturnValue[7] == 1)
            {
                ReturnValue[7] = 0;
            }
            else
            {
                ReturnValue[7] = 1;
                Debug.Log("zimojiafan开启");
            }
        });
        zimojiadi.onValueChanged.AddListener(delegate {
            if (ReturnValue[8] == 1)
            {
                ReturnValue[8] = 0;
            }
            else
            {
                ReturnValue[8] = 1;
                Debug.Log("zimojiadi开启");
            }
        });
        dianganghua.onValueChanged.AddListener(delegate {
            if (ReturnValue[9] == 1)
            {
                ReturnValue[9] = 0;
            }
            else
            {
                ReturnValue[9] = 1;
                Debug.Log("dianganghua开启");
            }
        });
        diangangpao.onValueChanged.AddListener(delegate {
            if (ReturnValue[10] == 1)
            {
                ReturnValue[10] = 0;
            }
            else
            {
                ReturnValue[10] = 1;
                Debug.Log("diangangpao开启");
            }
        });
        if (PlayerPrefs.HasKey("xz0"))
            SetDelaultDate();
    }
    void SetAllFengDingFalse()
    {
        fengding[0].isOn = false;
        fengding[1].isOn = false;
        fengding[2].isOn = false;
        fengding[3].isOn = false;
        fengding[4].isOn = false;
    }
    void SetDelaultDate()
    {
        switch (PlayerPrefs.GetInt("xz1"))
        {
            case 2:
                SetAllFengDingFalse();
                fengding[0].isOn = true;
                break;
            case 3:
                SetAllFengDingFalse();
                fengding[1].isOn = true;
                break;
            case 4:
                SetAllFengDingFalse();
                fengding[2].isOn = true;
                break;
            case 5:
                SetAllFengDingFalse();
                fengding[3].isOn = true;
                break;
            case 0:
                SetAllFengDingFalse();
                fengding[4].isOn = true;
                break;
            default:
                break;
        }

        for (int i = 2; i < ReturnValue.Length; i++)
        {
            if (PlayerPrefs.GetInt("xz" + i.ToString())==1)
            {
                switch (i)
                {
                    case 2:
                        hsz.isOn = true;
                        break;
                    case 3:
                        tiandihu.isOn = true;
                        break;
                    case 4:
                        hujiaozhuangyi.isOn = true;
                        break;
                    case 5:
                        mengqingzhongzhnag.isOn = true;
                        break;
                    case 6:
                        daiyaojiujiangdui.isOn = true;
                        break;
                    case 7:
                        zimojiafan.isOn = true;
                        zimojiadi.isOn = false;
                        break;
                    case 8:
                        zimojiadi.isOn = true;
                        zimojiafan.isOn = false;
                        break;
                    case 9:
                        dianganghua.isOn = true;
                        diangangpao.isOn = false;
                        break;
                    case 10:
                        diangangpao.isOn = true;
                        dianganghua.isOn = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void Rest()
    {
       
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        GameManager.GM.DS.CreateRoom_Hoom = null;
    }
    int[] ReturnValue = { 4, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1 };
    void EnterRoom()
    {
        for (int i = 0; i < ReturnValue.Length; i++)
        {
            PlayerPrefs.SetInt(("xz" + i.ToString()), ReturnValue[i]);
            ////Debug.Log("保存 "+ ReturnValue[i]);
        }
        PlayerPrefs.Save();

       
        if (Enter.IsInteractable())
        Enter.interactable = false;
        Invoke("reOpen", 2.0f);
        Debug.Log("进入房间");
        PublicEvent.GetINS.NewRoom(ReturnValue,"xz");
    }
    void reOpen()
    {
        Enter.interactable = true;
    }
    void OnSliderClick(Slider toggle, float value)
    {
        if (toggle.value == 0 || toggle.value < 1)
        {
            jushutext.text = null;
            jushutext.text = "四局（12张房卡）";
            ReturnValue[0] = 4;
        }
        if (toggle.value > 0 && toggle.value < 2)
        {
            jushutext.text = null;
            jushutext.text = "八局（24张房卡）";
            ReturnValue[0] = 8;
        }
        if (toggle.value > 2 && toggle.value < 3)
        {
            jushutext.text = null;
            jushutext.text = "十二局（36张房卡）";
            ReturnValue[0] = 12;
        }
        if (toggle.value > 3 && toggle.value < 4)
        {
            jushutext.text = null;
            jushutext.text = "十六局（48张房卡）";
            ReturnValue[0] = 16;
        }
        if (toggle.value > 4 && toggle.value < 5)
        {
            jushutext.text = null;
            jushutext.text = "二十局（60张房卡）";
            ReturnValue[0] = 20;
        }
        if (toggle.value > 5 && toggle.value < 6)
        {
            jushutext.text = null;
            jushutext.text = "二十四局（72张房卡）";
            ReturnValue[0] = 24;
        }
        if (toggle.value > 6 && toggle.value < 7)
        {
            jushutext.text = null;
            jushutext.text = "二十八局（84张房卡）";
            ReturnValue[0] = 28;
        }
        if (toggle.value > 7 && toggle.value < 8)
        {
            jushutext.text = null;
            jushutext.text = "三十二局（96张房卡）";
            ReturnValue[0] = 32;

        }
        if (toggle.value > 8 && toggle.value < 9)
        {
            jushutext.text = null;
            jushutext.text = "三十六局（108张房卡）";
            ReturnValue[0] = 36;
        }
        if (toggle.value == 9)
        {
            jushutext.text = null;
            jushutext.text = "四十局（120张房卡）";
            ReturnValue[0] = 40;
        }
        //Debug.Log("局数"+ ReturnValue[0]);
    }
    void enter(ProtoBuf.EnterRoomRsp rsp)
    {
        PublicEvent.GetINS.Event_joinRoomSuccess -= enter;
        GameManager.GM.DS.CreateRoom_Hoom = null;
        {
            //创建playroom
            DataManage.Instance._roomEnterRsp = rsp;

            //对当前的玩家进行排序
            PublicEvent.GetINS.ReciveData(rsp);

            ParticleManager.GetIns.SwitchSence(2);
        }
            Destroy(this.gameObject);
    }
}
