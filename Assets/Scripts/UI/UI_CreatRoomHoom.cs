using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_CreatRoomHoom : MonoBehaviour
{

    Transform ThisTrans = null;
   // public Button Close = null;
    public Button jpy = null;
    public Button xzdd = null;
    public Image jpyimg = null;
    public Image xzddimg = null;
    /// <summary>
    /// 龙
    /// </summary>
    public GameObject JPYType = null;
    public GameObject XZDDType = null;

    public Transform Content = null;
   
    //GameObject r;
    Tween x;
    Tween GL;
    Tween XL;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    //void Start()
    //{
    //    //lambda表达式转换为委托类型  
    //    toggle.onValueChanged.AddListener((bool value) => OnToggleClick(toggle, value));

    //    //匿名委托调用  
    //    button.onClick.AddListener(delegate ()
    //    {
    //        Debug.Log("toggle is " + (toggle.isOn ? "On" : "Off"));
    //    });
    //}

    //public void OnToggleClick(Toggle toggle, bool value)
    //{
    //    Debug.Log("toggle change " + (value ? "On" : "Off"));
    //}
    void Start()
    {

        GameManager.GM.DS.CreateRoom_Hoom = gameObject;
        DeFault();
        //x = Content.DOLocalMoveX(-1600, 0.4f).From();
        //x.SetEase(Ease.OutSine);
        //x.SetAutoKill(false);
        //x.PlayForward();

        //GL = GaLong.DOLocalMoveY(47, 2f).From();
        //GL.SetEase(Ease.InOutSine);
        //GL.SetLoops(-1, LoopType.Yoyo);
        //GL.SetAutoKill(false);

        //XL = XZDDLong.DOLocalMoveY(47, 2f).From();
        //XL.SetEase(Ease.InOutSine);
        //XL.SetLoops(-1, LoopType.Yoyo);
        //XL.SetAutoKill(false);

    }
    void Init()
    {
        ThisTrans = gameObject.transform;
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
        //ga = ThisTrans.Find("BG/GA").GetComponent<Button>();
        //xzdd = ThisTrans.Find("BG/XZDD").GetComponent<Button>();
        //r = ThisTrans.Find("R/r").gameObject;
    }
    void DeFault()
    {
        //if (Close != null)
        //    Close.onClick.AddListener(Rest);
        if (jpy != null)
            jpy.onClick.AddListener(SetJpy);
        if (xzdd != null)
            xzdd.onClick.AddListener(SetXz);
       
    }

    // Update is called once per frame

    //void Rest()
    //{
    //    // Destroy(r);
    //    Destroy(this.gameObject);
    //    GameManager.GM.DS.CreateRoom_Hoom = null;
    //    //x = Content.DOLocalMoveX(-1600, 0.3f);
    //    //x.OnComplete(delegate { Destroy(this.gameObject); });

    //}
    public Sprite[] IMG = new Sprite[8];
    void SetJpy()
    {

        JPYType.SetActive(true);
        XZDDType.SetActive(false);
        jpy.GetComponent<Image>().sprite = IMG[2];//Resources.Load("Texture/anniu_huang", typeof(Sprite)) as Sprite;
        jpyimg.GetComponent<Image>().sprite = IMG[3];//Resources.Load("Texture/chuangjianfangjan_huan_jiaopengyou", typeof(Sprite)) as Sprite;
        xzdd.GetComponent<Image>().sprite = IMG[6];//Resources.Load("Texture/anniu_lv", typeof(Sprite)) as Sprite;
        xzddimg.GetComponent<Image>().sprite = IMG[7];//Resources.Load("Texture/chuangjianfangjian_lv_xuezhandaodi", typeof(Sprite)) as Sprite;


    }
    void SetXz()
    {

        JPYType.SetActive(false);
        XZDDType.SetActive(true);
        jpy.GetComponent<Image>().sprite = IMG[0];//Resources.Load("Texture/anniu_lv", typeof(Sprite)) as Sprite;
        jpyimg.GetComponent<Image>().sprite = IMG[1];//Resources.Load("Texture/chuangjianfangjan_lv_jiaopengyou", typeof(Sprite)) as Sprite;
        xzdd.GetComponent<Image>().sprite = IMG[4];// Resources.Load("Texture/anniu_huang", typeof(Sprite)) as Sprite;
        xzddimg.GetComponent<Image>().sprite = IMG[5];//Resources.Load("Texture/chuangjianfangjian_huang_xuezhandaodi", typeof(Sprite)) as Sprite;
      
    }
    void OpenGA()
    {
        //Destroy(r);
        ParticleManager.GetIns.showBuleYun();
        Invoke("openga", 0);
    }
    
    void openga()
    {
        x = Content.DOLocalMoveX(-1600, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        GameManager.GM.DS.CreateRoom_Ga = GameManager.GM.PopUI(ResPath.CreateRoom_Ga);

    }
    void OpenXZDD()
    {
        ParticleManager.GetIns.showRedYun();
        Invoke("openxzdd", 0);
    }
    void openxzdd()
    {
       // Destroy(r);
        x = Content.DOLocalMoveX(-1600, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        GameManager.GM.DS.CreateRoom_Xz = GameManager.GM.PopUI(ResPath.CreateRoom_Xz);
    }
   
}
