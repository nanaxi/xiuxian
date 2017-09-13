using UnityEngine;

public class SoundMag : MonoBehaviour
{
    //public AudioSource 
    // Use this for initialization
    public static float BgValue = 1;
    public static float EffectValue = 1;

    public AudioSource Player0;
    public AudioSource Player1;
    public AudioSource Player2;
    public AudioSource Player3;
    public AudioSource BGM;
    public AudioSource EffectSound;

    static SoundMag _INS;
    public static SoundMag GetINS
    {
        get
        {
            if (_INS == null)
            {
                _INS = new SoundMag();
            }
            return _INS;
        }
    }

    public SoundMag()
    {
        _INS = this;
    }
    float MusicValue = 1, FxValue = 1;


    void Start()
    {
        PublicEvent.GetINS.EventChangeBgValue = ChangeBgValue;
        PublicEvent.GetINS.EventChangeEffectValue = ChangeEffectValue;


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
        //if (PlayerPrefs.HasKey("CanRead"))
        //{
        //    if (PlayerPrefs.GetInt("CanRead", 1) == 1)
        //    {
        //        CanRead = true;
        //    }
        //    else
        //    {
        //        CanRead = false;
        //    }
        //}
        //if (PlayerPrefs.HasKey("CanShake"))
        //{
        //    if (PlayerPrefs.GetInt("CanShake", 1) == 1)
        //    {
        //        CanShake = true;
        //    }
        //    else
        //    {
        //        CanShake = false;
        //    }
        //}
    }

    /// <summary>
    /// 1代表按钮按下去 ，2代表碰，3代表杠，4代表胡，5代表自摸,6代表准备，7代表加入房间，8代表牌面选中了，9代表牌打出，0代表牌放回
    /// </summary>
    /// <param name="num">1代表按钮按下去 ，2代表碰，3代表杠，4代表胡，5代表自摸,6代表准备，7代表加入房间，8代表牌面选中了，9代表牌打出</param>
    //public void PlaySound(int num, int sex = 1)
    //{
    //    switch (num)
    //    {

    //    }
    //}
    /// <summary>
    /// 打出牌的音效
    /// </summary>
    /// <param name="num">1w 1t 1b</param>
    /// <param name="sex">1为男，0为女</param>
    public void PlayPopCard(string num, int sex = 1, int pos = 0)
    {
        if (sex == 1)
        {
            #region 男
            var t = Resources.Load<AudioClip>("AudioClips/mjvoice/man/" + num);
            if (t == null)
            {
                Debug.Log("空值");
            }
            else
            {
                switch (pos)
                {
                    case 0:
                        Player0.clip = t;
                        Player0.Play();
                        break;
                    case 1:
                        Player1.clip = t;
                        Player1.Play();
                        break;
                    case 2:
                        Player2.clip = t;
                        Player2.Play();
                        break;
                    case 3:
                        Player3.clip = t;
                        Player3.Play();
                        break;
                    default:
                        break;
                }

            }

            #endregion
        }

        else
        {
            #region 女
            var t = Resources.Load<AudioClip>("AudioClips/mjvoice/woman/" + num);
            if (t == null)
            {
                Debug.Log("空值");
            }
            else
            {
                switch (pos)
                {
                    case 0:
                        Player0.clip = t;
                        Player0.Play();
                        break;
                    case 1:
                        Player1.clip = t;
                        Player1.Play();
                        break;
                    case 2:
                        Player2.clip = t;
                        Player2.Play();
                        break;
                    case 3:
                        Player3.clip = t;
                        Player3.Play();
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }


    }

    /// <summary>
    /// 扑克打出牌的音效
    /// </summary>
    public void PlayCardPoker(int pos, string voicePath)
    {
        var clip = Resources.Load<AudioClip>(voicePath);
        if (clip == null)
        {
            Debug.LogError("缺少扑克出牌音频 ： " + voicePath);
            return;
        }
        switch (pos)
        {
            case 0:
                Player0.clip = clip;
                Player0.Play();
                break;
            case 1:
                Player1.clip = clip;
                Player1.Play();
                break;
            case 2:
                Player2.clip = clip;
                Player2.Play();
                break;
            case 3:
                Player3.clip = clip;
                Player3.Play();
                break;
            default:
                break;
        }
    }
    public void PlayGiftAudioClips(string voice)
    {
        var clip = Resources.Load<AudioClip>("AudioClips/GifAudioClips/" + voice);
        if (clip == null)
        {
            Debug.Log("空值");
            return;
        }
        if (!Player0.isPlaying)
        {
            Player0.clip = clip;
            Player0.Play();
        }
        else {
            if (!Player1.isPlaying)
            {
                Player1.clip = clip;
                Player1.Play();
            }
            else {
                if (!Player2.isPlaying)
                {
                    Player2.clip = clip;
                    Player2.Play();
                }
                else {
                    if (!Player3.isPlaying)
                    {
                        Player3.clip = clip;
                        Player3.Play();
                    }
                }
            }
        }
        
    }
    /// <summary>
    /// 预制说的话
    /// </summary>
    /// <param name="value">说话的内容</param>
    /// <param name="sex">性别</param>
    /// <param name="pos">位置</param>
    public void ChatSound(string value, int sex = 1, int pos = 0)
    {
        if (sex == 1)
        {
            #region 男
            var t = Resources.Load<AudioClip>("AudioClips/quickvoice/man/" + value);
            if (t == null)
            {
                Debug.Log("空值");
            }
            else
            {
                switch (pos)
                {
                    case 0:
                        Player0.clip = t;
                        Player0.Play();
                        break;
                    case 1:
                        Player1.clip = t;
                        Player1.Play();
                        break;
                    case 2:
                        Player2.clip = t;
                        Player2.Play();
                        break;
                    case 3:
                        Player3.clip = t;
                        Player3.Play();
                        break;
                    default:
                        break;
                }

            }

            #endregion
        }

        else
        {
            #region 女
            var t = Resources.Load<AudioClip>("AudioClips/quickvoice/woman/" + value);
            if (t == null)
            {
                Debug.Log("空值");
            }
            else
            {
                switch (pos)
                {
                    case 0:
                        Player0.clip = t;
                        Player0.Play();
                        break;
                    case 1:
                        Player1.clip = t;
                        Player1.Play();
                        break;
                    case 2:
                        Player2.clip = t;
                        Player2.Play();
                        break;
                    case 3:
                        Player3.clip = t;
                        Player3.Play();
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }

    }
    public void ChangeBgValue(float value)
    {
        BgValue = value;
        BGM.volume = BgValue;
    }
    public void ChangeEffectValue(float value)
    {
        EffectValue = value;
        EffectSound.volume = EffectValue;
        Player0.volume = EffectValue;
        Player1.volume = EffectValue;
        Player2.volume = EffectValue;
        Player3.volume = EffectValue;
    }

}
