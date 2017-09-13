using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public enum Face
{
    peng,
    gang,
    hu,
    zimo,

    /// <summary>
    /// 扩充包
    /// </summary>
    angang,
    guafeng,
    xiayu,
    diangang,
    ///


    dianpao,
    baochou,
    ciya,
    cry,
    fangle,
    han,
    huaixiao,
    jiayou,
    keai,
    leipi,
    meiqianle,
    shengqi,
    shuile,
    weiqu,
    yun,
    zhuangbi,

    /// <summary>
    /// 道具
    /// </summary>
    /// 
    hua,
    xie,
    pijiu,

    none,
}
/// <summary>
/// 将它挂在canvas下面就可以了
/// </summary>
public class ShowFace : MonoBehaviour
{
    struct face
    {
        /// <summary>
        /// 动画
        /// </summary>
        public Image Pic;
        /// <summary>
        /// 动画
        /// </summary>
        public Face TheAnimation;
        /// <summary>
        /// 速率
        /// </summary>
        public float rate;
        /// <summary>
        /// 当前帧
        /// </summary>
        public int click;
        /// <summary>
        /// 总帧数
        /// </summary>
        public int allclick;
        /// <summary>
        ///         
        /// 初始化
        /// </summary>
        /// <param name="rate">速率</param>
        /// <param name="click">从这个帧开始</param>
        public face(float rate, int click)
        {
            this.Pic = null;
            this.TheAnimation = Face.none;
            this.rate = rate;
            this.click = click;
            this.allclick = 0;
        }

    }
    /// <summary>
    /// 动画系统的实例
    /// </summary>
    public static ShowFace Ins = null;

    /// <summary>
    /// 存储所加载的所有动画资源
    /// </summary>
    Dictionary<Face, List<Sprite>> faceDic0 = new Dictionary<Face, List<Sprite>>();
    Dictionary<Face, List<Sprite>> faceDic1 = new Dictionary<Face, List<Sprite>>();
    Dictionary<Face, List<Sprite>> faceDic2 = new Dictionary<Face, List<Sprite>>();
    Dictionary<Face, List<Sprite>> faceDic3 = new Dictionary<Face, List<Sprite>>();

    /// <summary>
    /// 初始化加载资源
    /// </summary>
    void Awake()
    {
        if (Ins == null)
            Ins = this;
        Debug.Log("动画加载开启！");
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }
    /// <summary>
    /// 只在加载的时候调用一次
    /// </summary>
	public void Init()
    {
        p0 = new GameObject();
        p0.AddComponent<Image>();
        p0.SetActive(false);

        p1 = new GameObject();
        p1.AddComponent<Image>();
        p1.SetActive(false);

        p2 = new GameObject();
        p2.AddComponent<Image>();
        p2.SetActive(false);

        p3 = new GameObject();
        p3.AddComponent<Image>();
        p3.SetActive(false);
    }
    void load0(Face item)
    {

        List<Sprite> tempList = new List<Sprite>();
        GameObject[] temp = null;
        string adress = "";
        switch (item)
        {
            case Face.none:
                adress = "";
                break;
            case Face.baochou:
                adress = "baochou";
                break;
            case Face.ciya:
                adress = "ciya";
                break;
            case Face.cry:
                adress = "cry";
                break;
            case Face.fangle:
                adress = "fangle";
                break;
            case Face.han:
                adress = "han";
                break;
            case Face.huaixiao:
                adress = "huaixiao";
                break;
            case Face.jiayou:
                adress = "jiayou";
                break;
            case Face.keai:
                adress = "keai";
                break;
            case Face.leipi:
                adress = "leipi";
                break;
            case Face.meiqianle:
                adress = "meiqianle";
                break;
            case Face.shengqi:
                adress = "shengqi";
                break;
            case Face.shuile:
                adress = "shuile";
                break;
            case Face.weiqu:
                adress = "weiqu";
                break;
            case Face.yun:
                adress = "yun";
                break;
            case Face.zhuangbi:
                adress = "zhuangbi";
                break;
            case Face.hua:
                adress = "hua";
                break;
            case Face.pijiu:
                adress = "pijiu";
                break;
            case Face.xie:
                adress = "xie";
                break;
            default:
                adress = "none";
                break;
        }
        temp = null;
        temp = Resources.LoadAll<GameObject>("Anim/" + adress);
        int tempNum = temp.Length;
        tempList.Clear();
        for (int i = 0; i < tempNum; i++)
        {
            tempList.Add(temp[i].GetComponent<Image>().sprite);
        }
        if (faceDic0.ContainsKey(item))
        {
            //faceDic.Add(item, tempList);
        }
        else
        {
            faceDic0.Add(item, tempList);
        }
    }
    void load1(Face item)
    {
        List<Sprite> tempList = new List<Sprite>();
        GameObject[] temp = null;
        string adress = "";
        switch (item)
        {
            case Face.none:
                adress = "";
                break;
            case Face.baochou:
                adress = "baochou";
                break;
            case Face.ciya:
                adress = "ciya";
                break;
            case Face.cry:
                adress = "cry";
                break;
            case Face.fangle:
                adress = "fangle";
                break;
            case Face.han:
                adress = "han";
                break;
            case Face.huaixiao:
                adress = "huaixiao";
                break;
            case Face.jiayou:
                adress = "jiayou";
                break;
            case Face.keai:
                adress = "keai";
                break;
            case Face.leipi:
                adress = "leipi";
                break;
            case Face.meiqianle:
                adress = "meiqianle";
                break;
            case Face.shengqi:
                adress = "shengqi";
                break;
            case Face.shuile:
                adress = "shuile";
                break;
            case Face.weiqu:
                adress = "weiqu";
                break;
            case Face.yun:
                adress = "yun";
                break;
            case Face.zhuangbi:
                adress = "zhuangbi";
                break;
            case Face.hua:
                adress = "hua";
                break;
            case Face.pijiu:
                adress = "pijiu";
                break;
            case Face.xie:
                adress = "xie";
                break;
            default:
                adress = "none";
                break;
        }
        temp = null;
        temp = Resources.LoadAll<GameObject>("Anim/" + adress);
        int tempNum = temp.Length;
        tempList.Clear();
        for (int i = 0; i < tempNum; i++)
        {
            tempList.Add(temp[i].GetComponent<Image>().sprite);
        }
        if (faceDic1.ContainsKey(item))
        {
            //faceDic.Add(item, tempList);
        }
        else
        {
            faceDic1.Add(item, tempList);
        }
    }
    void load2(Face item)
    {
        List<Sprite> tempList = new List<Sprite>();
        GameObject[] temp = null;
        string adress = "";
        switch (item)
        {
            case Face.none:
                adress = "";
                break;
            case Face.baochou:
                adress = "baochou";
                break;
            case Face.ciya:
                adress = "ciya";
                break;
            case Face.cry:
                adress = "cry";
                break;
            case Face.fangle:
                adress = "fangle";
                break;
            case Face.han:
                adress = "han";
                break;
            case Face.huaixiao:
                adress = "huaixiao";
                break;
            case Face.jiayou:
                adress = "jiayou";
                break;
            case Face.keai:
                adress = "keai";
                break;
            case Face.leipi:
                adress = "leipi";
                break;
            case Face.meiqianle:
                adress = "meiqianle";
                break;
            case Face.shengqi:
                adress = "shengqi";
                break;
            case Face.shuile:
                adress = "shuile";
                break;
            case Face.weiqu:
                adress = "weiqu";
                break;
            case Face.yun:
                adress = "yun";
                break;
            case Face.zhuangbi:
                adress = "zhuangbi";
                break;
            case Face.hua:
                adress = "hua";
                break;
            case Face.pijiu:
                adress = "pijiu";
                break;
            case Face.xie:
                adress = "xie";
                break;
            default:
                adress = "none";
                break;
        }
        temp = null;
        temp = Resources.LoadAll<GameObject>("Anim/" + adress);
        int tempNum = temp.Length;
        tempList.Clear();
        for (int i = 0; i < tempNum; i++)
        {
            tempList.Add(temp[i].GetComponent<Image>().sprite);
        }
        if (faceDic2.ContainsKey(item))
        {
            //faceDic.Add(item, tempList);
        }
        else
        {
            faceDic2.Add(item, tempList);
        }
    }
    void load3(Face item)
    {
        List<Sprite> tempList = new List<Sprite>();
        GameObject[] temp = null;
        string adress = "";
        switch (item)
        {
            case Face.none:
                adress = "";
                break;
            case Face.baochou:
                adress = "baochou";
                break;
            case Face.ciya:
                adress = "ciya";
                break;
            case Face.cry:
                adress = "cry";
                break;
            case Face.fangle:
                adress = "fangle";
                break;
            case Face.han:
                adress = "han";
                break;
            case Face.huaixiao:
                adress = "huaixiao";
                break;
            case Face.jiayou:
                adress = "jiayou";
                break;
            case Face.keai:
                adress = "keai";
                break;
            case Face.leipi:
                adress = "leipi";
                break;
            case Face.meiqianle:
                adress = "meiqianle";
                break;
            case Face.shengqi:
                adress = "shengqi";
                break;
            case Face.shuile:
                adress = "shuile";
                break;
            case Face.weiqu:
                adress = "weiqu";
                break;
            case Face.yun:
                adress = "yun";
                break;
            case Face.zhuangbi:
                adress = "zhuangbi";
                break;
            case Face.hua:
                adress = "hua";
                break;
            case Face.pijiu:
                adress = "pijiu";
                break;
            case Face.xie:
                adress = "xie";
                break;
            default:
                adress = "none";
                break;
        }
        temp = null;
        temp = Resources.LoadAll<GameObject>("Anim/" + adress);
        int tempNum = temp.Length;
        tempList.Clear();
        for (int i = 0; i < tempNum; i++)
        {
            tempList.Add(temp[i].GetComponent<Image>().sprite);
        }
        if (faceDic3.ContainsKey(item))
        {
            //faceDic.Add(item, tempList);
        }
        else
        {
            faceDic3.Add(item, tempList);
        }
    }
    int t0Times = 0, t1Times = 0, t2Times = 0, t3Times = 0;
    face t0 = new face(0.3f, 0);
    face t1 = new face(0.3f, 0);
    face t2 = new face(0.3f, 0);
    face t3 = new face(0.3f, 0);
    GameObject p0 = null, p1 = null, p2 = null, p3 = null;
    float Time0 = 0.08f, Time1 = 0.08f,Time2=0.08f,Time3=0.08f;
    public Sprite hu, zimo;
    public List<GameObject> TempAnim = new List<GameObject>();
    Vector2 leftVect, RightVect, UpVect;
    /// <summary>
    /// 调用动画
    /// </summary>
    /// <param name="item">需要调用的表情（拼音）</param>
    /// <param name="Player">0是本地玩家，1右边，2上边，3左边</param>
    public void PlayAnim(Face item, int Player, int Times = 1)
    {
        leftVect = new Vector2(-GameManager.GM.SWide * 0.328f, 0);
        RightVect = new Vector2(GameManager.GM.SWide * 0.328f, 0);
        UpVect = new Vector2(0, GameManager.GM.SHeight * 0.324f);
        switch (Player)
        {
            case 0:
                load0(item);
                StopCoroutine(Play0());
                if (p0 != null)
                    p0.SetActive(true);
                else
                {
                    p0 = new GameObject();
                    p0.SetActive(true);
                }
                if (p0.GetComponent<Image>() != null)
                    t0.Pic = p0.GetComponent<Image>();
                else
                {        
                    t0.Pic = p0.AddComponent<Image>();
                }
                if (item==Face.hu|| item == Face.zimo|| item == Face.gang|| item == Face.peng|| item == Face.angang|| item == Face.guafeng|| item == Face.xiayu|| item == Face.dianpao|| item == Face.diangang)//-200
                {
                    t0.Pic.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
                }
                else {
                    t0.Pic.GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, -170);
                }              
                t0.Pic.transform.SetParent(transform, false);
                t0.Pic.transform.SetAsLastSibling();
                t0.allclick = faceDic0[item].Count;
                t0.TheAnimation = item;
                t0.click = 0;
                t0Times = Times;
                StartCoroutine(Play0());
                break;
            case 3:
                load1(item);
                StopCoroutine(Play1());
                if (p1 != null)
                    p1.SetActive(true);
                else
                {
                    p1 = new GameObject();
                    p1.SetActive(true);
                }
                if (p1.GetComponent<Image>() != null)
                    t1.Pic = p1.GetComponent<Image>();
                else
                {
                    t1.Pic = p1.AddComponent<Image>();
                }
                t1.Pic.GetComponent<RectTransform>().anchoredPosition = leftVect;
                t1.Pic.transform.SetParent(transform, false);
                t1.Pic.transform.SetAsLastSibling();
                t1.allclick = faceDic1[item].Count;
                t1.TheAnimation = item;
                t1.click = 0;
                t1Times = Times;
                StartCoroutine(Play1());
                break;
            case 2:
                load2(item);
                StopCoroutine(Play2());
                if (p2 != null)
                    p2.SetActive(true);
                else
                {
                    p2 = new GameObject();
                    p2.SetActive(true);
                }
                if (p2.GetComponent<Image>() != null)
                    t2.Pic = p2.GetComponent<Image>();
                else
                {
                    t2.Pic = p2.AddComponent<Image>();
                }
                t2.Pic.GetComponent<RectTransform>().anchoredPosition = UpVect;
                t2.Pic.transform.SetParent(transform, false);
                t2.Pic.transform.SetAsLastSibling();
                t2.allclick = faceDic2[item].Count;
                t2.TheAnimation = item;
                t2.click = 0;
                t2Times = Times;
                StartCoroutine(Play2());
                break;
            case 1:
                load3(item);
                StopCoroutine(Play3());
                if (p3 != null)
                    p3.SetActive(true);
                else
                {
                    p3 = new GameObject();
                    p3.SetActive(true);
                }
                if (p3.GetComponent<Image>() != null)
                    t3.Pic = p3.GetComponent<Image>();
                else
                {
                    t3.Pic = p3.AddComponent<Image>();
                }
                t3.Pic.GetComponent<RectTransform>().anchoredPosition = RightVect;
                t3.Pic.transform.SetParent(transform, false);
                t3.Pic.transform.SetAsLastSibling();
                t3.allclick = faceDic3[item].Count;
                t3.TheAnimation = item;
                t3.click = 0;
                t3Times = Times;
                StartCoroutine(Play3());
                break;
            default:
                break;
        }

    }
    IEnumerator Play0()
    {
        while (t0.TheAnimation != Face.none)
        {
            t0.Pic.sprite = faceDic0[t0.TheAnimation][t0.click];
            t0.Pic.SetNativeSize();
            t0.click++;
            if (t0.TheAnimation == Face.gang)
            {
                Time0 = 0.09f;
            }
            else
            {
                Time0 = 0.08f;
            }
            yield return new WaitForSeconds(Time0);
            if (t0.click >= t0.allclick)
            {
                t0Times--;
                if (t0Times > 0)
                {
                    t0.click = 0;
                    Debug.Log("播放重置");
                }
                else
                {
                    if (t0.TheAnimation == Face.hu || t0.TheAnimation == Face.zimo&& !GameManager.GM.GameEnd)
                    {
                        GameObject P = Instantiate(new GameObject());
                        P.SetActive(false);
                        var t = P.AddComponent<Image>();
                        if (t0.TheAnimation == Face.hu)
                        {
                            t.sprite = hu;
                        }
                        else {
                            t.sprite = zimo;
                        }
                        t.SetNativeSize();
                        t.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
                        t.transform.SetParent(transform, false);
                        t.transform.SetAsLastSibling();
                        TempAnim.Add(t.gameObject);
                        t.gameObject.SetActive(true);
                    }
                    t0.TheAnimation = Face.none;
                    p0.SetActive(false);
                }
            }
        }

    }
    IEnumerator Play1()
    {

        while (t1.TheAnimation != Face.none)
        {
            t1.Pic.sprite = faceDic1[t1.TheAnimation][t1.click];
            t1.Pic.SetNativeSize();
            t1.click++;
            if (t1.TheAnimation == Face.gang)
            {
                Time1 = 0.09f;
            }
            else
            {
                Time1 = 0.08f;
            }
            yield return new WaitForSeconds(Time1);
            if (t1.click >= t1.allclick)
            {
                t1Times--;//循环次数减少
                if (t1Times > 0)
                {
                    t1.click = 0;
                    Debug.Log("播放结束");
                }
                else
                {
                    if (t1.TheAnimation == Face.hu || t1.TheAnimation == Face.zimo && !GameManager.GM.GameEnd)
                    {
                        GameObject P = Instantiate(new GameObject());
                        P.SetActive(false);
                        var t = P.AddComponent<Image>();
                        if (t1.TheAnimation == Face.hu)
                        {
                            t.sprite = hu;
                        }
                        else
                        {
                            t.sprite = zimo;
                        }
                        t.SetNativeSize();
                        t.GetComponent<RectTransform>().anchoredPosition =leftVect;
                        t.transform.SetParent(transform, false);
                        t.transform.SetAsLastSibling();
                        TempAnim.Add(t.gameObject);
                        t.gameObject.SetActive(true);
                    }
                    t1.TheAnimation = Face.none;
                    p1.SetActive(false);
                }
            }
        }

    }
    IEnumerator Play2()
    {
        while (t2.TheAnimation != Face.none)
        {
            t2.Pic.sprite = faceDic2[t2.TheAnimation][t2.click];
            t2.Pic.SetNativeSize();
            t2.click++;
            if (t2.TheAnimation == Face.gang)
            {
                Time2 = 0.09f;
            }
            else
            {
                Time2 = 0.08f;
            }
            yield return new WaitForSeconds(Time2);
            if (t2.click >= t2.allclick)
            {
                t2Times--;
                if (t2Times > 0)
                {
                    t2.click = 0;
                    Debug.Log("播放结束");
                }
                else
                {
                    if (t2.TheAnimation == Face.hu || t2.TheAnimation == Face.zimo && !GameManager.GM.GameEnd)
                    {
                        GameObject P = Instantiate(new GameObject());
                        P.SetActive(false);
                        var t = P.AddComponent<Image>();
                        if (t2.TheAnimation == Face.hu)
                        {
                            t.sprite = hu;
                        }
                        else
                        {
                            t.sprite = zimo;
                        }
                        t.SetNativeSize();
                        t.GetComponent<RectTransform>().anchoredPosition = UpVect;
                        t.transform.SetParent(transform, false);
                        t.transform.SetAsLastSibling();
                        TempAnim.Add(t.gameObject);
                        t.gameObject.SetActive(true);
                    }
                    t2.TheAnimation = Face.none;
                    p2.SetActive(false);
                }
            }
        }

    }
    IEnumerator Play3()
    {
        while (t3.TheAnimation != Face.none)
        {
            t3.Pic.sprite = faceDic3[t3.TheAnimation][t3.click];
            t3.Pic.SetNativeSize();
            t3.click++;
            if (t3.TheAnimation == Face.gang)
            {
                Time3 = 0.09f;
            }
            else
            {
                Time3 = 0.08f;
            }
            yield return new WaitForSeconds(Time3);
            if (t3.click >= t3.allclick)
            {
                t3Times--;
                if (t3Times > 0)
                {
                    t3.click = 0;
                    Debug.Log("播放结束");
                }
                else
                {
                    if (t3.TheAnimation == Face.hu || t3.TheAnimation == Face.zimo && !GameManager.GM.GameEnd)
                    {
                        GameObject P = Instantiate(new GameObject());
                        P.SetActive(false);
                        var t = P.AddComponent<Image>();
                        if (t3.TheAnimation == Face.hu)
                        {
                            t.sprite = hu;
                        }
                        else
                        {
                            t.sprite = zimo;
                        }
                        t.SetNativeSize();
                        t.GetComponent<RectTransform>().anchoredPosition = RightVect;
                        t.transform.SetParent(transform, false);
                        t.transform.SetAsLastSibling();
                        TempAnim.Add(t.gameObject);
                        t.gameObject.SetActive(true);
                    }
                    t3.TheAnimation = Face.none;
                    p3.SetActive(false);
                }
            }
        }

    }
    //0是本地玩家，1右边，2上边，3左边</param>
    public void PlayAnimDaoju(Face item, int Player, GameObject reciver, int Times = 1)
    {
        Debug.Log("PlayAnimDaoju发给几号玩家" + Player);
        switch (Player)
        {
            case 0:
                load0(item);
                StopCoroutine(Play0());
                if (p0 != null)
                    p0.SetActive(true);
                else
                {
                    p0 = new GameObject();
                    p0.SetActive(true);
                }
                if (p0.GetComponent<Image>() != null)
                    t0.Pic = p0.GetComponent<Image>();
                else
                {
                    t0.Pic = p0.AddComponent<Image>();
                }
                t0.Pic.transform.SetParent(reciver.transform, false);
                t0.Pic.GetComponent<RectTransform>().anchoredPosition = reciver.transform.localPosition;
                t0.Pic.transform.SetAsLastSibling();
                t0.allclick = faceDic0[item].Count;
                t0.TheAnimation = item;
                t0.click = 0;
                t0Times = Times;
                StartCoroutine(Play0());
                break;
            case 1:
                leftVect = reciver.transform.localPosition;
                load1(item);
                StopCoroutine(Play1());
                if (p1 != null)
                    p1.SetActive(true);
                else
                {
                    p1 = new GameObject();
                    p1.SetActive(true);
                }
                if (p1.GetComponent<Image>() != null)
                    t1.Pic = p1.GetComponent<Image>();
                else
                {
                    t1.Pic = p1.AddComponent<Image>();
                }
                t1.Pic.transform.SetParent(reciver.transform, false);
                t1.Pic.GetComponent<RectTransform>().anchoredPosition = leftVect;
                
                t1.Pic.transform.SetAsLastSibling();
                t1.allclick = faceDic1[item].Count;
                t1.TheAnimation = item;
                t1.click = 0;
                t1Times = Times;
                StartCoroutine(Play1());
                break;
            case 2:
                UpVect = reciver.transform.localPosition;
                load2(item);
                StopCoroutine(Play2());
                if (p2 != null)
                    p2.SetActive(true);
                else
                {
                    p2 = new GameObject();
                    p2.SetActive(true);
                }
                if (p2.GetComponent<Image>() != null)
                    t2.Pic = p2.GetComponent<Image>();
                else
                {
                    t2.Pic = p2.AddComponent<Image>();
                }
                t2.Pic.transform.SetParent(reciver.transform, false);
                t2.Pic.GetComponent<RectTransform>().anchoredPosition = UpVect;
               
                t2.Pic.transform.SetAsLastSibling();
                t2.allclick = faceDic2[item].Count;
                t2.TheAnimation = item;
                t2.click = 0;
                t2Times = Times;
                StartCoroutine(Play2());
                break;
            case 3:
                RightVect = reciver.transform.localPosition;
                load3(item);
                StopCoroutine(Play3());
                if (p3 != null)
                    p3.SetActive(true);
                else
                {
                    p3 = new GameObject();
                    p3.SetActive(true);
                }
                if (p3.GetComponent<Image>() != null)
                    t3.Pic = p3.GetComponent<Image>();
                else
                {
                    t3.Pic = p3.AddComponent<Image>();
                }
                t3.Pic.transform.SetParent(reciver.transform, false);
                t3.Pic.GetComponent<RectTransform>().anchoredPosition = RightVect;
                
                t3.Pic.transform.SetAsLastSibling();
                t3.allclick = faceDic3[item].Count;
                t3.TheAnimation = item;
                t3.click = 0;
                t3Times = Times;
                StartCoroutine(Play3());
                break;
            default:
                break;
        }
    }
    public void DisAllAnim()
    {
        if (p0 != null)
            p0.SetActive(false);
        if (p1 != null)
            p1.SetActive(false);
        if (p2 != null)
            p2.SetActive(false);
        if (p3 != null)
            p3.SetActive(false);
    }
    public void DistoryDelayAnim()
    {
        for (int i = 0; i < TempAnim.Count; i++)
        {
            Destroy(TempAnim[i].gameObject);
        }
        TempAnim.Clear();
    }
}
