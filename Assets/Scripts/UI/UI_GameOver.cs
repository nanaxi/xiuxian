using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoBuf;

public class UI_GameOver : MonoBehaviour
{
    Transform ThisTrans = null;
    Button Share = null;
    Button EndToHall = null;
    public Text JushuMax = null;
    public Text RoomText = null;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        //Init();
        //Default();
    }
    /// <summary>
    /// 初始化并且获取组件
    /// </summary>
    void Init()
    {
        ThisTrans = transform;
        Share = ThisTrans.Find("BG/Share").GetComponent<Button>();
        EndToHall = ThisTrans.Find("BG/EndToHall").GetComponent<Button>();
        Share.onClick.AddListener(ShareToWX);
        EndToHall.onClick.AddListener(End);
    }
    /// <summary>
    /// 初始化数据,绑定函数
    /// </summary>
    public void Default(MJGameOver Thisrsp = null)
    {
        MJProto.Inst().GameOver = false;
        PublicEvent.GetINS.GameOverRsp = null;
        List<int> YingJia = new List<int>();
        if (Thisrsp != null)
            for (int i = 0; i < Thisrsp.players.Count; i++)
            {
                int[] tag = new int[] { 0, 0, 0, 0, 0, 0 };

                //int pos = GameManager.GM.GetPlayerNum(Thisrsp.players[i].charId);
                //GameManager.GM.GetHead(GameManager.GM._AllPlayerData[pos].Head, SetHead, i);

                ThisTrans.Find("BG/Player" + i.ToString() + "/Head/Mask/HeadSprite").GetComponent<Image>().sprite = DataManage.Instance.Head_GetSprite(Thisrsp.players[i].charId);

                string temp = "BG/Player" + i.ToString() + "/NickName";
                Debug.Log(temp);
                ThisTrans.Find(temp).GetComponent<Text>().text = "昵称：" + DataManage.Instance.PData_GetData(Thisrsp.players[i].charId).P_Name;

                temp = "BG/Player" + i.ToString() + "/FinalGold/Text";
                Debug.Log(temp);
                ThisTrans.Find(temp).GetComponent<Text>().text = Thisrsp.players[i].restGold.ToString();

                temp = "BG/Player" + i.ToString() + "/ChangeRoomCard/Text";
                Debug.Log(temp);
                ThisTrans.Find(temp).GetComponent<Text>().text = Thisrsp.players[i].hasdiamond.ToString();//ChangeRoomCard

                YingJia.Add(Thisrsp.players[i].restGold);
                
            }
        int[] Value = new int[4];
        Value = GetTop(YingJia);

        for (int i = 0; i < YingJia.Count; i++)
        {
            string temp = "BG/Player" + i.ToString() + "/jiangpai";
            if (Value[i] != -999)
            {
                ThisTrans.Find(temp).gameObject.SetActive(true);
                ThisTrans.Find("BG/Player" + i.ToString() + "/jiangbei/guanjun").gameObject.SetActive(true);
                YingJia[i]= -999;//删除金牌
            }
            else
            {
                ThisTrans.Find(temp).gameObject.SetActive(false);
            }
        }
        Value = GetTop(YingJia);
        for (int i = 0; i < 4; i++)
        {
            if (Value[i] != -999)
            {
                ThisTrans.Find("BG/Player" + i.ToString() + "/jiangbei/yajun").gameObject.SetActive(true);
                YingJia[i] = -999;//删除银牌
            }
        }
        Value = GetTop(YingJia);
        for (int i = 0; i < 4; i++)
        {
            if (Value[i] != -999)
            {
                ThisTrans.Find("BG/Player" + i.ToString() + "/jiangbei/jijun").gameObject.SetActive(true);
                YingJia[i] = -999;//删除铜牌
            }
        }
        JushuMax.text = DataManage.Instance.roomJuShu_Max.ToString();
        RoomText.text = "房间号：" + Thisrsp.roomId.ToString();
        GameManager.GM.UpDateDiamond(Thisrsp.diamond);
    }
    /// <summary>
    /// 得到的数字如果是大于-1的，则是最大的一组
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int[] GetTop(List<int> v)
    {
        int[] value = { 0, 0, 0, 0 };
        v.CopyTo(value);
        for (int i = 0; i < 4; i++)
        {
            for (int p = 0; p < 4; p++)
            {
                if (value[i] < value[p])
                {
                    value[i] = -999;
                }
            }
        }
        return value;
    }
    void SetHead(Sprite sprite, int num = 0)
    {
        
    }
    void ShareToWX()
    {
        Debug.Log("分享");
        GameManager.GM.Share(0);
    }
    void End()
    {
        PublicEvent.GetINS.GameOverRsp = null;
        MJProto.Inst().GameOver = false;
        DataManage.Instance.RuleH3Z = false;
        DataManage.Instance.PData_RemoveOtherPlayerData();
        GameManager.GM.DS.GameOver = null;
        PublicEvent.GetINS.Fun_ExitRoomSucc();
        Debug.Log("返回大厅");
        Invoke("Rest", 0.2f);
    }
    void Rest()
    {

        Destroy(this.gameObject);
        Destroy(this);
    }
    void OnDestroy()
    {
        if(GameManager.GM.DS.Main!=null)
        GameManager.GM.DS.Main.GetComponent<UI_Main>().SetInfo(
            GameManager.GM._AllPlayerData[0].Name,
            GameManager.GM._AllPlayerData[0].ID.ToString(),
            GameManager.GM._AllPlayerData[0].Diamond.ToString(),
            GlobalSettings.avatarUrl);
    }
}
