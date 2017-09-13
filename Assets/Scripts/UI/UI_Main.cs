using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Lang;

public class UI_Main : MonoBehaviour
{
    Transform ThisTrans = null;

    public Button Head = null;

    public Button Setting = null;
    public Button Rule = null;
    public Button Message = null;
    public Button CheckPoint = null;
    public Button Share = null;
    public Button Store = null;
    public Button ComBat = null;
    public Button Call = null;
    public Button Roll = null;

    public Button JoinRoom = null;
    public Button CreatRoom = null;
    public Button Quit = null;

    public Transform HeadAround = null;

    public Text Name = null;
    public Text ID = null;
    public Text RoomCard = null;
    public Image HeadSprite = null;
    public Transform RollText = null;
    public GameObject Playerinfo_ip = null;
    public Text Playerinfo_ip_txt = null;
    public Transform meizi = null;
    string versionNumberUrl = "http://www.xiuxianchaguan.com/versionNumber.txt";
    void Awake()
    {
        Init();
        StartCoroutine(SetConfig());
    }
    Tween Ro;
    // Use this for initialization
    void Start()
    {
        Default();
        //SetInfo("李红衣","666","888","");
        //Invoke("ShowMeizi", 0.0f);
        //Ro = RollText.DOLocalMoveX(-1000, 20.0f);
       // Ro.SetLoops(-1);
       // Ro.SetAutoKill(false);
       // Ro.PlayForward();
        GameManager.GM.DS.Main = gameObject;
    }
    void Init()
    {

        ThisTrans = this.gameObject.transform;
        HeadAround = ThisTrans.Find("UP/PlayerInformation/round").GetComponent<Transform>();

        //Head = ThisTrans.Find("UP/PlayerInformation/Mask/Head").GetComponent<Button>();

        Setting = ThisTrans.Find("Down/Setting").GetComponent<Button>();
        Rule = ThisTrans.Find("Down/Rule").GetComponent<Button>();
        //Message = ThisTrans.Find("UP/Message").GetComponent<Button>();
        Quit = ThisTrans.Find("Down/Quit").GetComponent<Button>();
        //CheckPoint = ThisTrans.Find("Down/CheckPoint").GetComponent<Button>();
        Share = ThisTrans.Find("Down/Share").GetComponent<Button>();
        //Store = ThisTrans.Find("Down/Store").GetComponent<Button>();
        ComBat = ThisTrans.Find("Down/ComBat").GetComponent<Button>();
        //Call = ThisTrans.Find("Down/Call").GetComponent<Button>();
        //Roll = ThisTrans.Find("Down/Roll").GetComponent<Button>();

        CreatRoom = ThisTrans.Find("Middle/CreatRoom").GetComponent<Button>();
        JoinRoom = ThisTrans.Find("Middle/JoinRoom").GetComponent<Button>();

        //Name = ThisTrans.Find("UP/PlayerInformation/ID").GetComponent<Text>();
        //Gold = ThisTrans.Find("UP/PlayerInformation/Gold").GetComponent<Text>();
        //RoomCard = ThisTrans.Find("UP/PlayerInformation/RoomCard").GetComponent<Text>();
        //HeadSprite = ThisTrans.Find("UP/PlayerInformation/Mask/Head").GetComponent<Image>();
    }

    
    IEnumerator SetConfig()
    {
        WWW www = new WWW(versionNumberUrl);
        yield return www;
        string[] sArray = www.text.Split('|');

        for (int i = 0; i < sArray.Length; i++)
        {
            Debug.Log(sArray[i]);
        }


        Debug.Log("我获取了提示信息+main");
        LoadConfig(sArray[6]);
    }
    void LoadConfig(string context)
    {
        Debug.Log(context);
        if (context != null)
        {
            SetNotice(context);
        }

    }

    public void SetNotice(string msg)
    {
        RollText.transform.GetComponent<Text>().text = msg;
        RollText.transform.GetComponentInParent<WorldMessageMoveAnim>().Play();
    }
    void Default()
    {
        if (Quit != null)
            Quit.onClick.AddListener(delegate
            {
                Debug.Log("打开Quit");
                GameManager.GM.DS.Quit = GameManager.GM.PopUI(ResPath.Quit);
            });
        if (Setting != null)
            Setting.onClick.AddListener(delegate
            {
                Debug.Log("打开Setting");
                GameManager.GM.DS.Setting = GameManager.GM.PopUI(ResPath.Setting);
            });
        if (Rule != null)
            Rule.onClick.AddListener(delegate
            {
                Debug.Log("打开Rule");
                GameManager.GM.DS.Rule = GameManager.GM.PopUI(ResPath.Rule);
                //GameManager.GM.Share(0);
            });
        if (Message != null)
            Message.onClick.AddListener(delegate
            {
                //Debug.Log("打开Message");
                //GameManager.GM.DS.Message = GameManager.GM.PopUI(ResPath.Message);
                PublicEvent.GetINS.DiamondRequst();
            });


        if (CheckPoint != null)
            CheckPoint.onClick.AddListener(delegate
            {
                Debug.Log("打开CheckPoint");
                GameManager.GM.DS.CheckPoint = GameManager.GM.PopUI(ResPath.CheckPoint);
            });
        if (Share != null)
            Share.onClick.AddListener(delegate
            {
                Debug.Log("打开Share");
                GameManager.GM.DS.Share = GameManager.GM.PopUI(ResPath.Share);
                //ShowFace.Ins.PlayAnim(Face.guafeng,1, 5);
                //ShowFace.Ins.PlayAnim(Face.xiayu, 3, 3);
            });
        if (Store != null)
            Store.onClick.AddListener(delegate
            {
                //ShowFace.Ins.PlayAnim(Face.gang, 0, 1);
                Debug.Log("打开Store");
                GameManager.GM.DS.Store = GameManager.GM.PopUI(ResPath.Store);
            });
        if (ComBat != null)
            ComBat.onClick.AddListener(delegate
            {
                Debug.Log("打开ComBat");
                if (GameManager.GM.combatGainRsp == null && GameManager.GM.qpcombatGainRsp == null)
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("没有查询到相关战绩记录！");
                else
                {
                    GameManager.GM.DS.Combat = GameManager.GM.PopUI(ResPath.Combat);
                    GameManager.GM.DS.Combat.GetComponent<UI_ComBat>().SetComBat(GameManager.GM.combatGainRsp, GameManager.GM.qpcombatGainRsp);
                }
            });
        if (Call != null)
            Call.onClick.AddListener(delegate
            {
                Debug.Log("打开Call");
                GameManager.GM.DS.Call = GameManager.GM.PopUI(ResPath.Call);
            });
        if (Roll != null)
            Roll.onClick.AddListener(delegate
            {
                Debug.Log("打开Roll");
                GameManager.GM.DS.Roll = GameManager.GM.PopUI(ResPath.Roll);
            });
        //if (Head != null)
        //    Head.onClick.AddListener(delegate
        //    {
        //        StartCoroutine("PlayAround");
        //        Debug.Log("打开PlayerInformation");
        //        if (Playerinfo_ip != null)
        //        {
        //            Playerinfo_ip.SetActive(!Playerinfo_ip.activeSelf);
        //            if (Playerinfo_ip.activeSelf)
        //            {
        //                Playerinfo_ip_txt.text = ("IP:" + GameManager.GM._AllPlayerData[0].IP);
        //            }

        //        }
        //        //if (GameManager.GM.DS.PlayerInfo != null)
        //        //{
        //        //    Destroy(GameManager.GM.DS.PlayerInfo);
        //        //    GameManager.GM.DS.PlayerInfo = null;
        //        //}
        //        //if (GameManager.GM.DS.PlayerInfo == null)
        //        //{
        //        //    GameManager.GM.DS.PlayerInfo = GameManager.GM.PopUI(ResPath.PlayerInfo);
        //        //    GameManager.GM.DS.PlayerInfo.GetComponent<UI_PlayerInfo>().SetInfo(GameManager.GM._AllPlayerData[0].Name, GameManager.GM._AllPlayerData[0].ID.ToString(), GameManager.GM._AllPlayerData[0].IP.ToString(), GameManager.GM._AllPlayerData[0].Diamond.ToString(), GlobalSettings.avatarUrl);
        //        //}
        //    });
        if (CreatRoom != null)
            CreatRoom.onClick.AddListener(delegate
            {
                Debug.Log("打开创建房间");
                GameManager.GM.DS.CreateRoom_Hoom = GameManager.GM.PopUI(ResPath.CreateRoom_Hoom);
            });
        if (JoinRoom != null)
        {
            JoinRoom.onClick.AddListener(delegate
            {
                Debug.Log("打开加入房间");
                GameManager.GM.PopUI(ResPath.JoinRoom);
            });
        }
    }

    IEnumerator PlayAround()
    {
        Tween temp = HeadAround.DORotate(new Vector3(0, 0, -360), 0.8f, RotateMode.FastBeyond360);
        yield return null;
    }
    public SkeletonDataAsset Man, Woman;
    public Material MAN, WOMAN;
    /// <summary>
    /// 设定角色的 各部分属性
    /// </summary>
    /// <param name="Pic"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="ip"></param>
    /// <param name="homecard"></param>
    public void SetInfo(string name = "缺省", string id = "缺省", string roomCard = "缺省", string Pic = null)
    {
        Name.text = "昵称：" + name;
        ID.text = "ID：" + id;
        RoomCard.text =roomCard;
        //HeadSprite.sprite = Pic;
        GameManager.GM.GetHead(Pic, SetHead);
        GameManager.GM.GmType = GameSceneType.gm_Home;
        if (GameManager.GM._AllPlayerData[0].sex == 0)
        {
            anima_Stand = "xxcg_nv_001";
            anima_OnClick = "xxcg_nv_002";
            meizi.GetComponent<SkeletonAnimation>()._animationName = "xxcg_nv_001";
            meizi.GetComponent<SkeletonAnimation>().skeletonDataAsset = Woman;
            meizi.GetComponent<SkeletonAnimation>().Reset();
            //meizi.GetComponent<MeshRenderer>().materials[0] = WOMAN;
            waitTime_Mz = 1.5F;

        }
        else {

            anima_Stand = "xxcg_nan_01";
            anima_OnClick = "xxcg_nan_02";
            meizi.GetComponent<SkeletonAnimation>()._animationName = "xxcg_nan_01";
            meizi.GetComponent<SkeletonAnimation>().skeletonDataAsset = Man;
            meizi.GetComponent<SkeletonAnimation>().Reset();
            //meizi.GetComponent<MeshRenderer>().materials[0] = MAN;
            waitTime_Mz = 2F;

        }
    }

    public void SetRoomCard(string roomCard = "缺省") {
        RoomCard.text = roomCard;
    }
    bool isMzPlay = false;//是否正在播放Spine点击动画
    public float waitTime_Mz = 2f;//等待
    public string anima_Stand = "stand", anima_OnClick = "dianji";//请在Inspector根据实际情况赋值

    public string clickAudioPath;//点击 的时候播放的声音

    public void OnClick_DiamondRequst() {

        PublicEvent.GetINS.DiamondRequst();
    }
    public void OnClick_SpineObj()
    {
        if (isMzPlay == false)
        {
            isMzPlay = true;

            //Audio_Manage.Instance.Play_Audio(clickAudioPath);

            StartCoroutine("SpinePlay");
        }
    }

    IEnumerator SpinePlay()
    {
        meizi.GetComponent<SkeletonAnimation>().AnimationName = anima_OnClick;
        float f_WaitTime = waitTime_Mz;
        while (f_WaitTime > 0)
        {//等待动画播放完毕才能够再次触发点击动画
            f_WaitTime -= 0.1f;
            yield return GameManager.wait01;
        }
        isMzPlay = false;
        meizi.GetComponent<SkeletonAnimation>().AnimationName = anima_Stand;
        yield return null;
    }

    void OnDisable()
    {
        StopCoroutine("SpinePlay");
        isMzPlay = false;
        meizi.GetComponent<SkeletonAnimation>().AnimationName = anima_Stand;
    }

    void SetHead(Sprite sprite, int num = 0)
    {
        HeadSprite.sprite = sprite;
        HeadSprite.color = new Color(255, 255, 255, 255);
    }

    Tween t;
    SkeletonAnimation SwitchAnim;
    bool AnimFinshed = true;
    //public void ShowMeizi()
    //{
    //    if (AnimFinshed)
    //    {
    //        AnimFinshed = false;
    //        SwitchAnim = meizi.GetComponent<SkeletonAnimation>();
    //        SwitchAnim.AnimationName = "TQT_02";
    //        Invoke("ReMeizi", 6.0f);
    //    }

    //}/// <summary>
    // /// 妹子会到之前的状态
    // /// </summary>
    //void ReMeizi()
    //{
    //    AnimFinshed = true;
    //    SwitchAnim.AnimationName = "TQT_01";
    //}
}
