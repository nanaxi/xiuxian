using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Notic : MonoBehaviour
{
    Transform ThisTrans = null;
    public Text TheText = null;
    public Button Close = null;
    public Transform Content = null;

    public Button goon_btn = null;
    public Button quit_btn = null;
    public Text DelyTime = null;
    void Awake()
    {
        Init();
    }
    Tween x;
    // Use this for initialization
    void Start()
    {
        DeFault();
        //GameManager.GM.DS.Notic = gameObject;
        //x = Content.DOLocalMoveX(-1380, 0.5f).From();
        //x.SetEase(Ease.OutSine);
        //x.SetAutoKill(false);
        //x.PlayForward();
        //x.OnComplete(delegate{});
    }
    void Init()
    {
        ThisTrans = this.gameObject.transform;
    }
    void DeFault()
    {
        if (Close != null)
        {
            Close.onClick.AddListener(Rest);
        }
        if (goon_btn != null)
        {
            goon_btn.onClick.AddListener(Rest);
        }
        //if (quit_btn != null)
        //{
        //    quit_btn.onClick.AddListener(Quit);
        //}
        //if (goon_btn != null)
        //{
        //    goon_btn.onClick.AddListener(Rest);
        //}
    }
    public void SetMessage(string value)
    {
        if(TheText!=null)
        TheText.text = value;
    }
    int defaulttime = 3;

    public void Dely3ToClose()
    {
        Close.gameObject.SetActive(false);
        DelyTime.gameObject.SetActive(true);
        InvokeRepeating("Dely", 0, 1);
    }
    void Dely()
    {
        DelyTime.text = defaulttime.ToString();
        if (defaulttime == 0)
        {
            Close.gameObject.SetActive(true);
            goon_btn.gameObject.SetActive(true);
            DelyTime.gameObject.SetActive(false);
        }
        else {
            defaulttime--;
        }
        
    }
    public void QuitApp()
    {
        Close.gameObject.SetActive(false);
        StartCoroutine(DelyQuitApp());
    }
    IEnumerator DelyQuitApp()
    {
        yield return new WaitForSeconds(6.0f);
        Application.Quit();
    }

    void Quit()
    {

        PublicEvent.GetINS.OnExitRoom();
        //if (goon_btn.IsActive() == true)
        //{
        //    goon_btn.gameObject.SetActive(false);
        //}
        //if (quit_btn.IsActive() == true)
        //{
        //    quit_btn.gameObject.SetActive(false);
        //}
    }
    void Rest()
    {
        //GameManager.GM.DS.Notic = null;
        //x = Content.DOLocalMoveX(-1380, 0.3f);
        //x.OnComplete(delegate {  });
        Destroy(this.gameObject);
    }
    //void OnDestroy()
    //{
    //    GameManager.GM.DS.Notic = null;
    //}
}
