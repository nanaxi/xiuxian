using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Vote : MonoBehaviour {
    Transform ThisTrans = null;
    public Button Jujue = null;
    public Button TongYi = null;
    public Button Close = null;
    public Text PlayerReq = null;
    public Sprite agree=null, disAgree = null;
    /// <summary>
    /// 玩家同意或者拒绝的显示
    /// </summary>
    public List<Image> playersVote = new List<Image>();   
	// Use this for initialization
	void Start () {
        Init();
        //Default();
    }
    private void Awake()
    {

        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
        PublicEvent.GetINS.voteQuit += Rest;
    }
    void Init()
    {
        GameManager.GM.DS.Voting = gameObject;
        ThisTrans = this.gameObject.transform;

        //Jujue = ThisTrans.Find("BG/JuJue").GetComponent<Button>();
        //TongYi = ThisTrans.Find("BG/TongYi").GetComponent<Button>();
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
        //PlayerReq = ThisTrans.Find("BG/Req").GetComponent<Text>();   
    }
    public void Default(string Name="李红衣")
    {
        PlayerReq.text = "是否同意 "+Name.ToString()+" 退出游戏？";
        if(Jujue!=null)
        Jujue.onClick.AddListener(ReJect);
        if(TongYi!=null)
        TongYi.onClick.AddListener(Agree);
        if(Close!=null)
        Close.onClick.AddListener(ReJect);
        
    }
    public void SetVote(uint charid,bool value)
    {
        for (int i = 0; i < playersVote.Count; i++)
        {
            if (playersVote[i].sprite == null)
            {
                playersVote[i].gameObject.SetActive(true);
                playersVote[i].transform.GetChild(0).GetComponent<Text>().text = GameManager.GM.GetPlayerName(charid);
                playersVote[i].sprite = value ? agree : disAgree;
                break;
            }
        }
        
    }
    void ReJect()
    {
        Debug.Log("拒绝！");
        PublicEvent.GetINS.VoteRequest(false);
        DisableAllBtn();
        //Rest();
    }
    void Agree()
    {
        Debug.Log("同意！");
        PublicEvent.GetINS.VoteRequest(true);
        DisableAllBtn();
        //Rest();
    }
    void Rest()
    {
        Destroy(this.gameObject);
    }
    public void DisableAllBtn()
    {
        Jujue.gameObject.SetActive(false);
        TongYi.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        PublicEvent.GetINS.voteQuit -= Rest;
        GameManager.GM.DS.Voting = null;
        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
    }
}
