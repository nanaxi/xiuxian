using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Quit : MonoBehaviour {


    Transform ThisTrans = null;
    public Button Close = null;
    public Transform Content = null;
    public Button conform = null;
    public Button refuse = null;
    void Awake()
    {
        Init();
    }
  
    // Use this for initialization
    void Start()
    {
        DeFault();
       
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
        ParticleManager.GetIns.SwitchSence(0);
        PublicEvent.GetINS.AppLoginOut();
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
        GameManager.GM.DS.Quit = null;
        Destroy(this.gameObject);
    }
}
