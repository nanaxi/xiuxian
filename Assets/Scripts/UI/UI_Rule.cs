using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Rule : MonoBehaviour
{

    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public GameObject jpyBtn = null;
    public GameObject xzBtn = null;
    public Image jpy = null;
    public Image xz = null;
    public GameObject jpyScroll = null;
    public GameObject xzScroll = null;
    public Sprite[] IMG = new Sprite[8];
    //Tween x;
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
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (jpyBtn != null)
        {
            jpyBtn.GetComponent<Button>().onClick.AddListener(SetJpyRule);
        }
        if (xzBtn != null)
        {
            xzBtn.GetComponent<Button>().onClick.AddListener(SetXzRule);
        }
    }

    void SetJpyRule()
    {
        if (jpyScroll != null)
        {
            jpyScroll.SetActive(true);
            xzScroll.SetActive(false);
            jpyBtn.GetComponent<Image>().sprite = IMG[2];
            jpy.GetComponent<Image>().sprite = IMG[3];
            xzBtn.GetComponent<Image>().sprite = IMG[6];
            xz.GetComponent<Image>().sprite = IMG[7];
            
        }
    }
    void SetXzRule()
    {
        if (jpyScroll != null)
        {
            jpyScroll.SetActive(false);
            xzScroll.SetActive(true);
            jpyBtn.GetComponent<Image>().sprite = IMG[0];
            jpy.GetComponent<Image>().sprite = IMG[1];
            xzBtn.GetComponent<Image>().sprite = IMG[4];
            xz.GetComponent<Image>().sprite = IMG[5];
        }
    }
    void Rest()
    {
        GameManager.GM.DS.Rule = null;
        //x = Content.DOLocalMoveX(-1800, 0.3f);
        //x.OnComplete(delegate { Destroy(this.gameObject); });
        Destroy(this.gameObject);
    }
}
