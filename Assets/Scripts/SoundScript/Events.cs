using UnityEngine;
using System.Collections;
using ProtoBuf;
using System;

public class Events : ISingleton<Events>
{
    public delegate void OnClick(bool isSend = true);
    public OnClick StartMic;
    public delegate void playSound(string url);
    public event playSound PlaySound;
    public void F_PlaySound(string url)
    {
        Debug.Log("播放！");
        PlaySound(url);
    }


}
