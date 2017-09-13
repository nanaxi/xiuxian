using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>麻将结束一局
/// </summary>
public class MjOver_1 : MonoBehaviour {
    public End_MaJiang_Player_Gm_UI[] end_Mj_P_UI;
    [SerializeField]private List<Transform> all_EndMj = new List<Transform>();
    [SerializeField]
    private List<Transform> all_EndMj_Tm = new List<Transform>();
    [SerializeField]private List<Transform> all_EndMj_AnPai = new List<Transform>();
    [SerializeField]
    private Transform t_Img_AnPaiLayout;
    [SerializeField]private Transform t_AllMjLayout;
    [SerializeField]
    private Transform t_TouMingLayout;
    private Vector2[] jl_Point;
    //private Button btn_QueDing;
    private Vector2 /*v2_btn_QueDing,*/ v2_btn_EndStart;
    public Button btn_EndStart, btn_EndQueDing , btn_Share_Data;
    public List<string> end_DataShowList = new List<string>();
    private Text t_Time;
    public void EndShow_Add(string str_)
    {
        end_DataShowList.Add(str_);
    }
    public void EndShow_ResAll()
    {
        end_DataShowList = null;
        end_DataShowList = new List<string>();
    }

    public void EndShow_Set()
    {
        for (int i = 0; i < end_DataShowList.Count; i++)
        {
            string[] str_S1 = end_DataShowList[i].Split('|');
            if (str_S1.Length>=2)
            {
                for (int i_1 = 0; i_1 < end_Mj_P_UI.Length; i_1++)
                {
                    if (end_Mj_P_UI[i_1].t_Name.text.IndexOf(str_S1[0]) >= 0)
                    {
                        end_Mj_P_UI[i_1].t_HuPai_Prompt.text += str_S1[1];
                        break;
                    }
                }
            }
        }
    }
    // Use this for initialization
    void Start () {
        if (end_Mj_P_UI.Length == 0)
        {
            Init();
        }
        Debug.Log("ENDMJ =="+end_Mj_P_UI.Length);
    }

    public void Init()
    {
        end_Mj_P_UI = new End_MaJiang_Player_Gm_UI[4];
        for (int i = 0; i < end_Mj_P_UI.Length; i++)
        {
            end_Mj_P_UI[i] = new End_MaJiang_Player_Gm_UI();
            end_Mj_P_UI[i].NewEnd_MaJiang_Player_Gm_UI(transform.Find("Img_Bg0/Img_Layout_Bg/Img_Player_Bg_" + i));
            end_Mj_P_UI[i].mjOverManage = this;
        }
        t_Time = transform.Find("Img_Bg0/T_Time").GetComponent<Text>();
        t_Time.text = "";
        t_AllMjLayout = transform.Find("Img_Bg0/Img_Layout");
        t_TouMingLayout = transform.Find("Img_Bg0/Img_TouMingLayout");
        t_Img_AnPaiLayout = transform.Find("Img_Bg0/Img_AnPaiLayout");

        for (int i = 0; i < t_AllMjLayout.childCount; i++)
        {
            all_EndMj.Add(t_AllMjLayout.GetChild(i));
        }
        for (int i = 0; i < t_TouMingLayout.childCount; i++)
        {
            all_EndMj_Tm.Add(t_TouMingLayout.GetChild(i));
        }
        for (int i = 0; i < t_Img_AnPaiLayout.childCount; i++)
        {
            all_EndMj_AnPai.Add(t_Img_AnPaiLayout.GetChild(i));
        }
        
        all_EndMj_Tm[0].SetParent(transform);
        jl_Point = new Vector2[2];
        jl_Point[0] = GetComponent<RectTransform>().anchoredPosition;
        jl_Point[1] = Vector2.one * 3000;


        //SetPx();
        Set_Time();
        btn_Share_Data = transform.Find("Img_Bg0/Btn_Share_Data").GetComponent<Button>();
        btn_EndStart = transform.Find("Img_Bg0/Img_BtnBg_0/Btn_MjEnd_Start_Game").GetComponent<Button>();
        btn_EndQueDing = transform.Find("Img_Bg0/Img_BtnBg_0/Btn_EndQueDing").GetComponent<Button>();
        //v2_btn_QueDing = btn_QueDing.GetComponent<RectTransform>().anchoredPosition;
        v2_btn_EndStart = btn_EndStart.GetComponent<RectTransform>().anchoredPosition;
    }

    public void Open_EndStart()
    {
        if (btn_EndStart!=null)
        {
            btn_EndStart.gameObject.SetActive(false);
        }

    }

    public void Set_Time()
    {
        
        System.DateTime moment = System.DateTime.Now;
        // Year gets 1999.
        int year = moment.Year;
        // Month gets 1 (January).
        int month = moment.Month;
        // Day gets 13.
        int day = moment.Day;

        if (DataManage.Instance.isGameEnter_1)
        {
            string strGuiZe = DataManage.Instance.RoomInfoNxStr.Length > 0 ? "\t\t规则：" : "\t\t";
            t_Time.text ="房间号："+ DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString() + "\t\t局数：" + DataManage.Instance.roomJuShu_Max + strGuiZe + DataManage.Instance.RoomInfoNxStr.Replace("\n","\t\t" );
        }
        t_Time.text += year + "年" + month + "月" + day + "日" + "\t" + moment.Hour + ":" + moment.Minute;
        Debug.Log(year + "年" + month + "月" + day + "日" + "\t" + moment.Hour+":"+moment.Minute);   
    }

    void SetPx()
    {
        //RectT_S.Set_Sc_Px(transform, Resources.Load<Transform>(ResPath.uiWin_MjEnd_1));
    }

    public void OpenOrClose_MJEnd_1(bool b_)
    {
        t_AllMjLayout.gameObject.SetActive(true);
        t_TouMingLayout.gameObject.SetActive(true);
        t_Img_AnPaiLayout.gameObject.SetActive(true);
        if (b_)
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(b_);
            
        }
        if (!b_)
        {
            transform.SetAsFirstSibling() ;
            if (t_Img_AnPaiLayout!=null)
            {
                
                HuiShou_MJ();
                if (gameObject.activeInHierarchy)
                {//等待麻将回收完成
                    StartCoroutine(WaitTime_Invoke(
                        delegate () {
                            t_AllMjLayout.gameObject.SetActive(false);
                            t_TouMingLayout.gameObject.SetActive(false);
                            t_Img_AnPaiLayout.gameObject.SetActive(false);
                            gameObject.SetActive(b_);
                        }, Time.deltaTime));
                }
                
            }
        }
        
    }
    void OnDisable()
    {
        HuiShou_MJ();
        print("script was removed____________");
    }

    IEnumerator WaitTime_Invoke(UnityEngine.Events.UnityAction evenS_, float waitTime_ = 0.5f)
    {
        yield return new WaitForSeconds(waitTime_);
        evenS_.Invoke();
        yield return null;
    }

    /// <summary> 1 = 正常麻将。 2 = 麻将背面。 3、透明麻将
    /// </summary>
    public Transform GetMj_(int i_)
    {
        List<Transform> t_List = new List<Transform>();
        Transform t_Win = t_AllMjLayout;
        GameObject mj_Res = null; //Resources.Load<GameObject>(ResPath.end_TouMing_ShouPai);
        switch (i_)
        {
            case 1:
                t_List = all_EndMj;
                t_Win = t_AllMjLayout;
                //mj_Res = Resources.Load<GameObject>(ResPath.end_Z_ShouPai);
                break;
            case 2:
                t_List = all_EndMj_AnPai;
                t_Win = t_Img_AnPaiLayout;
                //mj_Res = Resources.Load<GameObject>(ResPath.end_F_ShouPai);
                break;
            case 3:
                t_List = all_EndMj_Tm;
                t_Win = t_TouMingLayout;
                //mj_Res = Resources.Load<GameObject>(ResPath.end_TouMing_ShouPai);
                break;
            default:
                break;
        }

        for (int i = 0; i < t_List.Count; i++)
        {
            if (t_List[i].parent == t_Win)
            {
                return t_List[i].transform;
            }
        }
        Debug.Log("INveke___________11231");
        GameObject mj_Ins = null;// AllPrefab.Instantiate_Gm(mj_Res, transform, true);
        mj_Ins.transform.SetParent(t_Win);
        //RectT_S.Set_Sc_Px(mj_Ins.transform, mj_Res.transform);
        
        switch (i_)
        {
            case 1:
                all_EndMj.Add(mj_Ins.transform);
                break;
            case 2:
                all_EndMj_AnPai.Add(mj_Ins.transform);
                break;
            case 3:
                all_EndMj_Tm.Add(mj_Ins.transform);
                break;
            default:
                break;
        }
        
        return mj_Ins.transform;
    }

    public void HuiShou_MJ()
    {
        

        for (int i = 0; i < all_EndMj.Count; i++)
        {
            if (all_EndMj[i]!=null)
            {
                Transform t_ = all_EndMj[i];
                t_.gameObject.SetActive(true);
                t_.SetParent(this.t_AllMjLayout);
            }

        }
        for (int i = 0; i < all_EndMj_Tm.Count; i++)
        {
            if (all_EndMj_Tm[i] != null && gameObject.activeInHierarchy)
            {
                Transform t_ = all_EndMj_Tm[i];
                t_.gameObject.SetActive(true);
                t_.SetParent(this.t_TouMingLayout);
            }
        }

        for (int i = 0; i < all_EndMj_AnPai.Count; i++)
        {
            if (all_EndMj_AnPai[i] != null)
            {
                Transform t_ = all_EndMj_AnPai[i];
                t_.gameObject.SetActive(true);
                t_.SetParent(this.t_Img_AnPaiLayout);
            }
        }
        
    }

    //public void Open_QueDing()
    //{
    //    //btn_EndQueDing.GetComponent<RectTransform>().anchoredPosition = mj_EndWindow_1.FindChild("Btn_MjEnd_Start_Game").GetComponent<RectTransform>().anchoredPosition;
    //    RectTransform btn_MjEnd_Start_GameRectT = mj_EndWindow_1.FindChild("Btn_MjEnd_Start_Game").GetComponent<RectTransform>();

    //    Vector2 v2_ = btn_EndQueDing.GetComponent<RectTransform>().anchoredPosition;
    //    btn_EndQueDing.GetComponent<RectTransform>().anchoredPosition
    //        = btn_MjEnd_Start_GameRectT.anchoredPosition;
    //    btn_MjEnd_Start_GameRectT.anchoredPosition = v2_;
    //}
}
