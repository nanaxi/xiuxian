using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhizAnima : MonoBehaviour {
    public Image[] phizImgs;
    public TextParentSize_[] chat_Shows;
    public float[] closeWaitTime_S;
    // Use this for initialization
    void Start () {
        closeWaitTime_S = new float[4];
        phizImgs =transform.Find("Phiz_Bg").GetComponentsInChildren<Image>(true);
        chat_Shows = transform.Find("Chat_Bg").GetComponentsInChildren<TextParentSize_>(true);
        Init_CloseShow();
    }

    public void Init_CloseShow()
    {
        for (int i = 0; i < phizImgs.Length; i++)
        {
            phizImgs[i].gameObject.SetActive(false);
            chat_Shows[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="i_Index"> ==座位号。 0、1、2、3</param>
    public void Play_PhizAnima(int i_Index,string path)
    {
        if (i_Index<0 || i_Index>3)
        {
            Debug.LogError("确定座位号是正确的 ？ ");
        }
        if (phizImgs[i_Index].enabled)
        {//防止上一个动画未播放完

            GameObject gm_Img = phizImgs[i_Index].gameObject;
            GameObject gmIns_Img = Instantiate(gm_Img);
            gmIns_Img.transform.parent = gm_Img.transform.parent;
            gmIns_Img.GetComponent<RectTransform>().anchoredPosition3D = gm_Img.GetComponent<RectTransform>().anchoredPosition3D;
            gmIns_Img.GetComponent<RectTransform>().localScale = gm_Img.GetComponent<RectTransform>().localScale;
            gmIns_Img.name = gm_Img.name;
            phizImgs[i_Index] = gmIns_Img.GetComponent<Image>();
            Destroy(gm_Img);
        }
        phizImgs[i_Index].gameObject.SetActive(true);
        phizImgs[i_Index].enabled = true;
        Sprite[] sprS_ = Resources.LoadAll<Sprite>(path);
        StartCoroutine(Start_Anima_(sprS_,phizImgs[i_Index]));
    }

    float anim_Speed = 0.05f;//动画速度
    /// <summary>使图片播放序列帧动画
    /// </summary>
    /// <param name="phiz_Sp">给定一个图集</param>
    /// <param name="img">是给定Image播放该图集</param>
    /// <param name="i_CiShu">播放的次数</param>
    /// <returns></returns>
    IEnumerator Start_Anima_(Sprite[] phiz_Sp, Image img = null, int i_CiShu = 1, bool yn_enabled = false)
    {
        if (img == null)
        {
            Debug.Log("<color=yellow>想要播放的图片为空~ </color>");
            //All_Prefab.Recycle_Prefabs(img);
        }
        i_CiShu = phiz_Sp.Length < 17 ? 3 : 1;//表情帧数小于多少，就循环播放3遍
        print("开始了 播放一个表情动画,有多少张图片" + phiz_Sp.Length);
        if (img!=null && img.enabled)
        {
            for (int i = 0; i < i_CiShu && img != null; i++)
            {
                for (int i_Time = 0; i_Time < phiz_Sp.Length; i_Time++)
                {
                    if (img != null)
                    {
                        img.enabled = true;
                        img.sprite = phiz_Sp[i_Time];
                        if (i_Time == 0)
                        {
                            //img.SetNativeSize();
                            img.rectTransform.sizeDelta = Vector2.one * 300;
                        }
                    }
                    yield return new WaitForSeconds(anim_Speed);///图片播放的速度
                }
            }
        }
        
        if (img != null)
        {
            img.enabled = yn_enabled;
            //All_Prefab.Recycle_Prefabs(img);
        }
        print("停止了 播放一个表情动画");
        yield return null;
    }


    /**/
    /// <summary>打开玩家说话的窗口
    /// </summary>
    public void Open_ShowChat(int i_Index, string str_Chat)
    {
        if (i_Index < 0 || i_Index > 3)
        {
            Debug.LogError("确定座位号是正确的 ？ ");
        }

        if (chat_Shows[i_Index].gameObject.activeInHierarchy)
        {
            chat_Shows[i_Index].gameObject.SetActive(false);
            chat_Shows[i_Index].gameObject.SetActive(false);
        }
        chat_Shows[i_Index].gameObject.SetActive(true);
        //closeWaitTime_S[i_Index] = waitTime_CT;
        chat_Shows[i_Index].GetComponentInChildren<Text>().text = str_Chat;
        //chat_Shows[i_Index].OnDrag_S_Rect();
        if (closeWaitTime_S[i_Index] != 0)
        {//如果正在显示说话，初始化时间
            closeWaitTime_S[i_Index] = waitTime_CT;
        }
        else
        {//如果没有显示，则打开说话
            closeWaitTime_S[i_Index] = waitTime_CT;
            StartCoroutine(WaitTime_Close_ShowChat(chat_Shows[i_Index], i_Index));
        }
    }

    const int waitTime_CT = 5;
    IEnumerator WaitTime_Close_ShowChat(TextParentSize_ t_,int i_Index)
    {
        //float f_ = 0;
        yield return GameManager.waitForEndOfFrame;
        //t_.OnDrag_S_Rect();
        while (closeWaitTime_S[i_Index]> 0 && t_.gameObject.activeInHierarchy)
        {
            //t_.OnDrag_S_Rect();
            closeWaitTime_S[i_Index] -= 0.1f;
            yield return GameManager.wait01;
        }
        closeWaitTime_S[i_Index] = 0;
        t_.gameObject.SetActive(false);
        yield return null;
    }

}
