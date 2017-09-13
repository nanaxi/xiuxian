using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SoundRecording : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    float bgvalue = 0f;
    public void OnPointerDown(PointerEventData eventData)
    {
        Events.Inst().StartMic(true);
        bgvalue = SoundMag.BgValue;
        SoundMag.GetINS.ChangeBgValue(0);
        //Debug.Log("按下");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        MicphoneTest.IsCancelMic = false;
        Events.Inst().StartMic(false);    
        SoundMag.GetINS.ChangeBgValue(bgvalue);
        //Debug.Log("弹起");
    }
}
