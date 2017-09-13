using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_IsQuit : MonoBehaviour
{
    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public Button conform = null;
    public Button refuse = null;
    void Awake()
    {
        Init();
    }
    Tween x;
    // Use this for initialization
    void Start()
    {
        DeFault();
        x = Content.DOLocalMoveX(-1380, 0.5f).From();
        x.SetEase(Ease.OutSine);
        x.SetAutoKill(false);
        x.PlayForward();
        //x.OnComplete(delegate{});
    }
    void Init()
    {
        ThisTrans = this.gameObject.transform;
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (conform != null)
            conform.onClick.AddListener(Conform);
        if (refuse != null)
            refuse.onClick.AddListener(Refuse);

        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
    }
    void Conform()
    {
        PublicEvent.GetINS.VoteRequest(true);
        PublicEvent.GetINS.IsMyVote = true;
        Rest();
    }
    void Refuse()
    {
        Rest();
        //PublicEvent.GetINS.VoteRequest(false);
    }
    // Update is called once per frame
    void Update()
    {

    }
    void Rest()
    {
        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
        GameManager.GM.DS.IsVote = null;
        x = Content.DOLocalMoveX(-1380, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
    }
}
