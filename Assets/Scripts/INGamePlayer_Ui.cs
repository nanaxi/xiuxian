using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class INGamePlayer_Ui : UiWin_Parent
{
    public Button btnHead;
    public Text[] textAry;
    [SerializeField]
    private Player_Data p_TheData;
    public uint playerCharID;
    public string GetNameID
    {
        get
        {
            string result = "";
            if (textAry != null && textAry.Length >= 2)
            {
                result = textAry[1].text;
            }
            return result;
        }
    }
    void Awake()
    {
        if (!isInitVar)
        {
            Init_Var();
        }
        //if (btnHead!=null)
        //{
        //    btnHead.onClick.AddListener(delegate() { OnClick_Head(); });
        //}
    }

    public void Init_()
    {
        btnHead.image.sprite = null;
        playerCharID = 0;
        textAry[1].text = "";
        textAry[2].text = "";

        p_TheData = new Player_Data();
        gameObject.SetActive(false);

    }

    public void Init_Var()
    {
        if (isInitVar)
        {
            return;
        }
        textAry = transform.GetComponentsInChildren<Text>();
        btnHead = transform.Find("Head").GetComponentInChildren<Button>();
        btnHead.onClick.AddListener(delegate () {
            OnClick_Head();
        });
        //gameObject.SetActive(false);
        isInitVar = true;
    }

    public void Set_PlayerInfoUI(Player_Data pData)
    {
        if (!isInitVar)
        {
            Init_Var();
        }
        if (pData == null)
        {
            Debug.Log("<color=red>玩家Data == Null 没有刷新UI</color>");
            return;
        }
        p_TheData = pData;
        textAry[1].text = pData.P_Name + "\nID:" + pData.p_ID + "";
        //textAry[1].text = "底分：" + pData.p_ID.ToString();
        playerCharID = pData.p_ID;
        btnHead.image.sprite = DataManage.Instance.Head_GetSprite(pData.p_ID);
        //Debug.Log("_______"+gameObject.activeInHierarchy);
        if (btnHead.image.sprite == null && gameObject.activeInHierarchy)
        {
            StartCoroutine(DataManage.Instance.W3_Request_Tx(pData.p_ID, pData.p_TxPath, SetHead, 1));
        }
    }

    public void Set_DiFen(int i_Gold)
    {
        textAry[2].text = "" + i_Gold.ToString();
    }

    public Text GetText_DiFen()
    {
        if (textAry[2] != null)
        {
            return textAry[2];
        }
        Debug.Log(" Text == NULL");
        return null;
    }

    public void Set_Que(string str)
    {
        textAry[0].text = str;
        textAry[0].gameObject.SetActive(false);
    }

    public void BaoTing_OpenOrClose(bool isOpen)
    {
        btnHead.transform.GetChild(1).gameObject.SetActive(isOpen);
    }

    void SetHead(Sprite sprite_, int i_Index)
    {
        btnHead.image.sprite = sprite_;
    }

    public void OnClick_Head()
    {

        GameManager.GM.DS.PlayerInfo = GameManager.GM.PopUI(ResPath.PlayerInfo);
        string adress = "";
       
        GameManager.GM.DS.PlayerInfo.GetComponent<UI_PlayerInfo>().SetInfo(
            DataManage.Instance.PData_GetData(p_TheData.p_ID).P_Name,
            p_TheData.p_ID.ToString(),
            DataManage.Instance.PData_GetData(p_TheData.p_ID).p_Ip,
            DataManage.Instance.PData_GetData(p_TheData.p_ID).p_Diamond.ToString(),
             DataManage.Instance.Head_GetSprite(p_TheData.p_ID), adress);

        Lang.GpsManager.Inst.GetCity(DataManage.Instance.PData_GetData(p_TheData.p_ID).adress, delegate (string t) {
            adress = t;
            GameManager.GM.DS.PlayerInfo.GetComponent<UI_PlayerInfo>().SetAdress(t);
        });

        Debug.Log("P_Name" + DataManage.Instance.PData_GetData(p_TheData.p_ID).P_Name);
        Debug.Log("ClickHead");
    }

    public void SetIsOnLine(bool isOnLine)
    {
        btnHead.image.color = isOnLine ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);
    }
}

