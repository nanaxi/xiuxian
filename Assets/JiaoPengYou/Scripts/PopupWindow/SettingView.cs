using UnityEngine;
using UnityEngine.UI;
using Lang;

public class SettingView : View
{
    public Button close;
    public Slider volumeSlider;
    public Slider soundSlider;

    public override ViewLayer GetLayer()
    {
        return ViewLayer.Topmost;
    }

    public float Volume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicValue", 1);
        }
        set
        {
            volumeSlider.value = value;
            SoundMag.GetINS.ChangeBgValue(value);
            PlayerPrefs.SetFloat("MusicValue", value);
        }
    }

    public float Sound
    {
        get
        {
            return PlayerPrefs.GetFloat("FxValue", 1);
        }
        set
        {
            soundSlider.value = value;
            SoundMag.GetINS.ChangeEffectValue(value);
            PlayerPrefs.SetFloat("FxValue", value);
        }
    }

    void Start()
    {
        volumeSlider.value = Volume;
        soundSlider.value = Sound;

        close.onClick.AddListener(() =>
        {
            PlayerPrefs.Save();
            ViewManager.Destroy<SettingView>();
        });

        volumeSlider.onValueChanged.AddListener(value =>
        {
            Volume = value;
        });

        soundSlider.onValueChanged.AddListener(value =>
        {
            Sound = value;
        });
    }
}