using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class UI_Share : MonoBehaviour
{

    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public Button ToFriend = null;
    public Button ToQuan = null;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        DeFault();
        //x = Content.DOLocalMoveX(-1285, 0.5f).From();
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
        if (ToFriend != null)
            ToFriend.onClick.AddListener(ShareToFriend);
        if (ToQuan != null)
            ToQuan.onClick.AddListener(ShareToQuan);
    }
    void Rest()
    {
        //x = Content.DOLocalMoveX(-1285, 0.5f);
        // x.OnComplete(delegate { Destroy(this.gameObject); });
        Destroy(this.gameObject);
        GameManager.GM.DS.Share = null;
    }
    /// <summary>
    /// 分享给朋友
    /// </summary>
    void ShareToFriend()
    {
        Onyaoqing(0);
    }

    /// <summary>
    /// 分享给朋友圈
    /// </summary>
    void ShareToQuan()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameManager.GM.IsShareToQuan = true;
            Onyaoqing(1);
        }
        else {
            GameManager.GM.IsShareToQuan = true;
            if (GameManager.GM.IsShareToQuan)
            {
                BaseProto.Inst().MissionComplete();
            }
            GameManager.GM.IsShareToQuan = false;
        }
    }
    void Onyaoqing(int SentTo)
    {
        Dictionary<string, string> info = new Dictionary<string, string>();
        info["mediaType"] = "2"; //分享类型： 0-文字 1-图片 2-网址  
        info["shareTo"] = SentTo.ToString(); //分享到：0-聊天 1-朋友圈 2-收藏  
        info["title"] = "休闲茶馆正式上线，渠县人自己的游戏“叫朋友”，等你来战!";
        info["imagePath"] = Application.persistentDataPath + "/icon.png";
        info["url"] = "xiuxianchaguan.com";
        info["text"] = "休闲茶馆正式上线，渠县人的本地棋牌游戏“叫朋友”“3D麻将”实时语音，等你来战!";
        if (Application.platform == RuntimePlatform.IPhonePlayer)//判断平台
        {
            info["thumbImage"] = Application.persistentDataPath + "/icon.png";
        }
        else
        {
            info["thumbSize"] = "64";
        }
        AnySDKManager.SendShare(info);
    }
}
