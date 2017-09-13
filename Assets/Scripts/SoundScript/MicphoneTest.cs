using UnityEngine;
using UnityEngine.UI;
public class MicphoneTest : MonoBehaviour
{

    static MicphoneTest _INS;
    //public GameObject MicAnim;
    public static MicphoneTest GetINS
    {
        get
        {
            if (_INS == null)
            {
                _INS = new MicphoneTest();
            }
            return _INS;
        }
    }

    public MicphoneTest()
    {
        _INS = this;
    }
    
    

    void Awake()
    {
        Events.Inst().StartMic += OnClick;
        //Event.Inst().PlaySound += PlaySound;
    }
    //int deviceCount;
    //int sFrequency = 10000;

    /// <summary>
    /// 开始录音
    /// </summary>
    float bgvalue = 0;
    float effectvalue = 0;
    void StartRecord()
    {
        bgvalue = SoundMag.BgValue;
        SoundMag.GetINS.ChangeBgValue(0);
        effectvalue = SoundMag.EffectValue;
        SoundMag.GetINS.ChangeEffectValue(0);
        Debug.Log("开始");
        //MicAnim.transform.SetAsLastSibling();
        //MicAnim.GetComponent<Image>().fillAmount = 0;
        //MicAnim.GetComponentInChildren<Text>().text = "";
        //ReSetCenterClock();       
    }
    //void ReSetCenterClock()
    //{
    //    MicAnim.SetActive(true);
    //    MicAnim.GetComponent<Image>().fillAmount = 0;
    //    MicAnim.GetComponentInChildren<Text>().text = "";
    //    InvokeRepeating("RefreshCenterClock", 0.0f, 0.1f);
    //}
    //float CenterClock = 0;
    //void RefreshCenterClock()
    //{
    //    if (CenterClock <60)
    //    {
    //        CenterClock+=0.1f;
    //        MicAnim.SetActive(true);

    //        MicAnim.GetComponent<Image>().fillAmount = (float)CenterClock/60f;
    //       // MicAnim.GetComponentInChildren<Text>().text = CenterClock.ToString("0#.0");
    //    }
    //}
    /// <summary>
    /// 结束录音
    /// </summary>
    public void StopRecord()
    {
        SoundMag.GetINS.ChangeBgValue(bgvalue);      
        SoundMag.GetINS.ChangeEffectValue(effectvalue);
        Debug.Log("停止录音");
        //CancelInvoke();
       // MicAnim.SetActive(false);
       // CenterClock = 0;
        //MicAnim.GetComponent<Image>().fillAmount = 0;
        //MicAnim.GetComponentInChildren<Text>().text = "";
    }
    /// <summary>
    /// 是否取消发送。==true时不发送
    /// </summary>
    public static bool IsCancelMic;

    bool IsStart = true;
    public void OnClick(bool isSend)
    {
        //if (IsStart)
        //{
        //    StartRecord();
        //}
        //else
        //{
        //    StopRecord();
        //}
        //IsStart = !IsStart;
    }
}
