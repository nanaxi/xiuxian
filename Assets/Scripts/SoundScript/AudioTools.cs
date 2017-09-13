using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AudioTools : MonoBehaviour
{

    Dictionary<string, AudioClip> SoundWarehouse = new Dictionary<string, AudioClip>();
    List<AudioSource> SoundPlayer = new List<AudioSource>();
    public AudioSource MusicPlayer;
    float SoundPlayerVolume;

    public AudioClip 背景音乐;

    void Start()
    {

      //  loadingStr();
       // StartCoroutine(LoadingAudioClip());

        ////Tools.Tool.AT = this;
        //SoundPlayerVolume = 1;
        //MusicPlayer = gameObject.AddComponent<AudioSource>();
        //MusicPlayer.loop = true;
        //MusicPlayer.clip = 背景音乐;
        //MusicPlayer.Play();
        //MusicPlayer.volume = 0.45f;
        //MusicPlayer.playOnAwake = false;

    }


    void loadingStr()
    {
        for (int i = 1; i < 11; i++)
        {
            if (i < 10)
            {
                Crad.Add(i + "w");
                Crad.Add(i + "b");
                Crad.Add(i + "t");
            }
            Msg.Add("msg_" + i);
        }
        Oper.Add("peng");
        Oper.Add("gang");
        Oper.Add("hu");
        Oper.Add("zimo");

        Teshu.Add("Button_Click");
        Teshu.Add("Card_Click");
        Teshu.Add("Card_Out");
        Teshu.Add("Deal_Card");//摸牌
        Teshu.Add("FlowBureau");//流局
        Teshu.Add("GuaFeng");
        Teshu.Add("JoinRoom");
        Teshu.Add("left");
        Teshu.Add("Lose");//失败
        Teshu.Add("Ready");//准备
        Teshu.Add("Timeup_Alarm");//倒计时
        Teshu.Add("Win");//胜利
        Teshu.Add("XiaYu");
    }
    public List<string> Crad = new List<string>();
    List<string> Msg = new List<string>();
    List<string> Oper = new List<string>();
    List<string> Teshu = new List<string>();
    IEnumerator LoadingAudioClip()
    {

        for (int i = 0; i < Crad.Count; i++)
        {
            SoundWarehouse.Add("Man" + Crad[i], Resources.Load<AudioClip>("Sound/Man/Card/" + Crad[i]));
            SoundWarehouse.Add("WoMan" + Crad[i], Resources.Load<AudioClip>("Sound/WoMan/Card/" + Crad[i]));
            yield return null;
        }
        for (int i = 0; i < Msg.Count; i++)
        {
            SoundWarehouse.Add("Man" + Msg[i], Resources.Load<AudioClip>("Sound/Man/Msg/" + Msg[i]));
            SoundWarehouse.Add("WoMan" + Msg[i], Resources.Load<AudioClip>("Sound/WoMan/Msg/" + Msg[i]));
            yield return null;
        }
        for (int i = 0; i < Oper.Count; i++)
        {
            SoundWarehouse.Add("Man" + Oper[i], Resources.Load<AudioClip>("Sound/Man/Oper/" + Oper[i]));
            SoundWarehouse.Add("WoMan" + Oper[i], Resources.Load<AudioClip>("Sound/WoMan/Oper/" + Oper[i]));
            yield return null;
        }
        for (int i = 0; i < Teshu.Count; i++)
        {
            SoundWarehouse.Add(Teshu[i], Resources.Load<AudioClip>("Sound/common/" + Teshu[i]));
        }
        Debug.Log("音频加载完成");
    }
    public void PlaySound(string SoundName)
    {
        if (SoundWarehouse.ContainsKey(SoundName))
        {
            if (SoundName != "Timeup_Alarm")
            {
                AudioClip temp = SoundWarehouse[SoundName];
                if (temp != null)
                {
                    AudioSource tempAS = GetSoundPlayer();
                    tempAS.clip = temp;
                    tempAS.Play();
                }
            }
            else
            {
                if (Timeup_Alarm == null)
                {
                    Timeup_Alarm = gameObject.AddComponent<AudioSource>();
                    Timeup_Alarm.loop = false;
                    Timeup_Alarm.playOnAwake = false;
                    Timeup_Alarm.volume = SoundPlayerVolume;
                    Timeup_Alarm.clip = SoundWarehouse[SoundName];
                }
                Timeup_Alarm.Play();
            }
        }

    }
    AudioSource Timeup_Alarm = null;
    public void StopDownTime()
    {
        if (Timeup_Alarm != null)
        {
            if (Timeup_Alarm.isPlaying)
            {
                Timeup_Alarm.Stop();
            }
        }
    }


    public void PlaySound(AudioClip Sound)
    {
        AudioClip temp = Sound;
        if (temp != null)
        {
            AudioSource tempAS = GetSoundPlayer();
            tempAS.clip = temp;
            tempAS.Play();
        }
    }
    /// <summary>
    /// 设置背景音乐音频播放器的音量
    /// </summary>
    /// <param name="num"></param>
    public void SetMusicVolume(float num)
    {
        MusicPlayer.volume = num;
    }
    /// <summary>
    /// 获取背景音乐播放器的当前音量；
    /// </summary>
    /// <returns></returns>
    public float GetMusicVolume()
    {
        return MusicPlayer.volume;
    }
    /// <summary>
    /// 设置音效播放器的音量
    /// </summary>
    /// <param name="num"></param>
    public void SetSoundVolume(float num)
    {
        SoundPlayerVolume = num;
        if (SoundPlayer.Count > 0)
        {
            for (int i = 0; i < SoundPlayer.Count; i++)
            {
                SoundPlayer[i].volume = SoundPlayerVolume;
            }
        }
        if (Timeup_Alarm != null)
        {
            Timeup_Alarm.volume = num;
        }
    }
    /// <summary>
    /// 获取音效播放器的音量。
    /// </summary>
    /// <returns></returns>
    public float GetSoundVolume()
    {
        return SoundPlayerVolume;
    }

    AudioSource GetSoundPlayer()
    {
        AudioSource AS = null;
        for (int i = 0; i < SoundPlayer.Count; i++)
        {
            if (!SoundPlayer[i].isPlaying)
            {
                AS = SoundPlayer[i];
                break;
            }
        }
        if (AS == null)
        {
            AS = gameObject.AddComponent<AudioSource>();
            SoundPlayer.Add(AS);
            AS.loop = false;
            AS.playOnAwake = false;
            AS.volume = SoundPlayerVolume;
        }
        return AS;
    }
}
