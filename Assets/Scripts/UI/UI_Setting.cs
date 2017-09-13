using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 刚开始会初始化组件,需要对音乐音效数值进行赋值，否则默认为1进行赋值
/// </summary>
public class UI_Setting : MonoBehaviour
{

    float MusicValue = 1;
    float FxValue = 1;
    bool CanRead = true;
    bool CanShake = true;

    public Transform Content = null;
    public Slider MusicSlider = null;
    public Slider FxSlider = null;
    public Toggle ReadCard = null;
    public Toggle Shake = null;
    public Button Close = null;
    public Button OK = null;
    public Button ChangeAccount = null;
    Transform ThisTrans = null;

    Tween x;
    #region 获取当前数值
    /// <summary>
    ///获取音乐滑动条的值
    /// </summary>
    public float MusicValue1
    {
        get
        {
            return MusicValue;
        }

        set
        {
            MusicValue = value;
        }
    }
    /// <summary>
    /// 获取音效滑动条的值
    /// </summary>
    public float FxValue1
    {
        get
        {
            return FxValue;
        }

        set
        {
            FxValue = value;
        }
    }
    /// <summary>
    /// 获取是否读牌
    /// </summary>
    public bool CanRead1
    {
        get
        {
            return CanRead;
        }

        set
        {
            CanRead = value;
        }
    }
    /// <summary>
    /// 获取是否震动
    /// </summary>
    public bool CanShake1
    {
        get
        {
            return CanShake;
        }

        set
        {
            CanShake = value;
        }
    }
    #endregion
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        GameManager.GM.DS.Setting = gameObject;
        Default();
        //x = Content.DOLocalMoveX(-1380, 0.5f).From();
        //x.SetEase(Ease.OutSine);
        //x.SetAutoKill(false);
        //x.PlayForward();
    }
    /// <summary>
    /// 初始化组件,并且读取当前的数值进行对组件的属性进行复制
    /// </summary>
	void Init()
    {
        ThisTrans = this.gameObject.transform;
        //Debug.Log(ThisTrans);
        //if (MusicSlider == null)
        //{
        //    MusicSlider = ThisTrans.GetChild(0).Find("Music").GetComponent<Slider>();
        //}

        //if (FxSlider == null)
        //{
        //    FxSlider = ThisTrans.GetChild(0).Find("Fx").GetComponent<Slider>();
        //}

        //if (ReadCard == null)
        //{
        //    ReadCard = ThisTrans.GetChild(0).Find("ReadCard").GetComponent<Toggle>();
        //}

        //if (Shake == null)
        //{
        //    Shake = ThisTrans.GetChild(0).Find("Shake").GetComponent<Toggle>();
        //}
        //if (Close == null)
        //{
        //    Close = ThisTrans.GetChild(1).Find("Close").GetComponent<Button>();
        //}
        //if (OK == null)
        //{
        //    OK = ThisTrans.GetChild(0).Find("Conform").GetComponent<Button>();
        //}
    }
    /// <summary>
    /// 对数值进行默认绑定
    /// </summary>
	void Default()
    {
        if (PlayerPrefs.HasKey("MusicValue"))
        {
            MusicValue = PlayerPrefs.GetFloat("MusicValue", 1);
            
        }
        SoundMag.GetINS.ChangeBgValue(MusicValue);
        if (PlayerPrefs.HasKey("FxValue"))
        {
            FxValue = PlayerPrefs.GetFloat("FxValue", 1);
            
        }
        SoundMag.GetINS.ChangeEffectValue(FxValue);
        if (PlayerPrefs.HasKey("CanRead"))
        {
            if (PlayerPrefs.GetInt("CanRead", 1) == 1)
            {
                CanRead = true;
            }
            else
            {
                CanRead = false;
            }
        }
        if (PlayerPrefs.HasKey("CanShake"))
        {
            if (PlayerPrefs.GetInt("CanShake", 1) == 1)
            {
                CanShake = true;
            }
            else
            {
                CanShake = false;
            }
        }

        MusicSlider.onValueChanged.AddListener(MValue);
        FxSlider.onValueChanged.AddListener(FValue);
        if(ReadCard!=null)
        ReadCard.onValueChanged.AddListener(iSRead);
        if(Shake!=null)
        Shake.onValueChanged.AddListener(IsShake);
        if(ChangeAccount!=null&& ChangeAccount.IsActive())
        ChangeAccount.onClick.AddListener(ChangeTheAccount);
        //在实例化之后把当前的组件的值改为之前操作的数值
        MusicSlider.value = MusicValue;
        FxSlider.value = FxValue;
        ReadCard.isOn = CanRead;
        Shake.isOn = CanShake;


        
        if(Close!=null)
        Close.onClick.AddListener(Rest);
        OK.onClick.AddListener(Rest);

        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
        if (GameManager.GM.ingame)
            ChangeAccount.gameObject.SetActive(false);
    }
    void MValue(float value)
    {
        MusicValue = value;
        SoundMag.GetINS.ChangeBgValue(MusicValue);
        //Debug.Log("MusicValue:" + MusicValue);
    }
    void FValue(float value)
    {
        FxValue = value;
        SoundMag.GetINS.ChangeEffectValue(FxValue);
        //Debug.Log("FxValue:" + FxValue);
    }
    void iSRead(bool value)
    {
        CanRead = value;
        Debug.Log("CanRead" + CanRead);
    }
    void IsShake(bool value)
    {
        CanShake = value;
        Debug.Log("CanShake" + CanShake);
    }
    void ChangeTheAccount()
    {
        ParticleManager.GetIns.SwitchSence(0);
        PublicEvent.GetINS.AppLoginOut();
        Rest();
        //ParticleManager.
    }
    void Rest()
    {
        PlayerPrefs.SetFloat("MusicValue", MusicValue);
        PlayerPrefs.SetFloat("FxValue", FxValue);
        SoundMag.GetINS.ChangeBgValue(MusicValue);
        SoundMag.GetINS.ChangeEffectValue(FxValue);
        if (CanRead)
            PlayerPrefs.SetInt("CanRead", 1);
        else
            PlayerPrefs.SetInt("CanRead", 0);
        if (CanShake)
            PlayerPrefs.SetInt("CanShake", 1);
        else
            PlayerPrefs.SetInt("CanShake", 0);
        PlayerPrefs.Save();

        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
        GameManager.GM.DS.Setting = null;
        //x = Content.DOLocalMoveX(-1380, 0.3f);
        //x.OnComplete(delegate { Destroy(this.gameObject); });
         Destroy(this.gameObject);
        //Destroy(this);
    }
    void Conform()
    {
        PlayerPrefs.SetFloat("MusicValue", MusicValue);
        PlayerPrefs.SetFloat("FxValue", FxValue);
        SoundMag.GetINS.ChangeBgValue(MusicValue);
        SoundMag.GetINS.ChangeEffectValue(FxValue);
        if (CanRead)
            PlayerPrefs.SetInt("CanRead", 1);
        else
            PlayerPrefs.SetInt("CanRead", 0);
        if (CanShake)
            PlayerPrefs.SetInt("CanShake", 1);
        else
            PlayerPrefs.SetInt("CanShake", 0);
        PlayerPrefs.Save();
        Rest();
    }
    void OnDestroy()
    {
        GameManager.GM.DS.Setting = null;
    }
}
