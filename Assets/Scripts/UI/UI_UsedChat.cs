using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
public class UI_UsedChat : MonoBehaviour
{
    Transform ThisTrans = null;
    List<Button> Faces = new List<Button>();
    public List<Button> Chats = new List<Button>();

    public Transform FaceContent = null;
    public Transform ChatContent = null;
    public Transform HistoryContent = null;
    public Text InputText = null;
    public Button Sent = null;
    public Button Close = null;

    public Image btn_QuickVoice = null;
    public Image btn_Face = null;
    public Image btn_HistoryText = null;
    public Transform Content = null;
    public Sprite[] img = new Sprite[6];
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        Default();
        GameManager.GM.DS.UI_UsedChat = gameObject;
        //x = Content.DOLocalMoveX(-1800, 0.8f).From();
        //x.SetEase(Ease.OutSine);
        //x.SetAutoKill(false);
        //x.PlayForward();
    }
    void Init()
    {
        ThisTrans = this.gameObject.transform;
        int temp = FaceContent.childCount;
        for (int i = 0; i < temp; i++)
        {
            int tempNum = 0;
            tempNum = i;
            Faces.Add(FaceContent.GetChild(i).GetComponent<Button>());
            Faces[i].onClick.AddListener(delegate
            {
                SentFace(tempNum);
                Rest();
            });
        }
        temp = ChatContent.childCount;

        for (int i = 0; i < temp; i++)
        {
            Chats.Add(ChatContent.GetChild(i).GetComponent<Button>());
            Button tempButton = null;
            tempButton = Chats[i];
            Chats[i].onClick.AddListener(
                delegate
                {
                    SendMessagePre(tempButton.transform.GetComponentInChildren<Text>().text);
                }
                );
        }
    }
    void Default()
    {
        Sent.onClick.AddListener(SentMessage);
        Close.onClick.AddListener(Rest);
        PublicEvent.GetINS.Event_ExitRoomSucc += Rest;
    }
    /// <summary>
    /// 切换到表情
    /// </summary>
    public void SwitchToFace()
    {
        btn_Face.sprite = img[2];
        btn_HistoryText.sprite = img[5];
        btn_QuickVoice.sprite = img[1];

        FaceContent.parent.gameObject.SetActive(true);
        ChatContent.parent.gameObject.SetActive(false);
        HistoryContent.gameObject.SetActive(false);
    }
    /// <summary>
    /// 切换到快捷语音文本
    /// </summary>
    public void SwitchToQuickVoice()
    {
        btn_Face.sprite = img[3];
        btn_HistoryText.sprite = img[5];
        btn_QuickVoice.sprite = img[0];

        FaceContent.parent.gameObject.SetActive(false);
        ChatContent.parent.gameObject.SetActive(true);
        HistoryContent.gameObject.SetActive(false);
    }
    /// <summary>
    /// 切换到历史记录
    /// </summary>
    public void SwitchToHistoryText()
    {
        btn_Face.sprite = img[3];
        btn_HistoryText.sprite = img[4];
        btn_QuickVoice.sprite = img[1];

        FaceContent.parent.gameObject.SetActive(false);
        ChatContent.parent.gameObject.SetActive(false);
        HistoryContent.gameObject.SetActive(true);
        HistoryContent.GetChild(0).GetComponent<Text>().text = DataManage.Instance.ChatRecord_Get();
    }
    public void SetHistoryContent(string value)
    {
        HistoryContent.GetChild(0).GetComponent<Text>().text = DataManage.Instance.ChatRecord_Get();
    }
    /// <summary>
    /// 发送表情
    /// </summary>
    /// <param name="face"></param>
    void SentFace(int face)
    {
        Debug.Log("第" + face + "个face！");
        PublicEvent.GetINS.SentMegssageImage("x0xxd" + face);
    }
    void SendMessagePre(string Value)
    {
        Debug.Log(Value);
        PublicEvent.GetINS.SentMegssageText(Value);
        Rest();
    }
    void SentMessage()
    {
        if (InputText.text != "")
        {
            Debug.Log(InputText.text);
            PublicEvent.GetINS.SentMegssageText(InputText.text);
            InputText.text = "aaaa";
        }
        SwitchToHistoryText();
        Rest();
    }
    void Rest()
    {

        //x = Content.DOLocalMoveX(-1600, 0.4f);
        //x.OnComplete(delegate 
        //{
        Destroy(this.gameObject);
        Destroy(this);
        //});
    }
    private void OnDestroy()
    {
        PublicEvent.GetINS.Event_ExitRoomSucc -= Rest;
        GameManager.GM.DS.UI_UsedChat = null;
    }
}
