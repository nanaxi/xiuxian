using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_End1_MJ : MonoBehaviour
{

    [SerializeField]
    private GameObject gm_End_Result;
    [SerializeField]
    public Button btn_CloseWin;
    [SerializeField]
    private Button btn_OpenEndResult;

    public Button btn_End1Setout;
    public Text t_EndResult;

    // Use this for initialization
    void Start()
    {
        btn_CloseWin.onClick.AddListener(delegate () { OnClick_Btn_CloseWin(); });
        btn_OpenEndResult.onClick.AddListener(delegate () { OnClick_Btn_OpenEndResult(); });
    }

    void OnClick_Btn_OpenEndResult()
    {
        gm_End_Result.SetActive(true);
    }
    void OnClick_Btn_CloseWin()
    {
        gm_End_Result.SetActive(false);
    }

    /// <summary>打开关闭结果界面
    /// </summary>
    /// <param name="isActive"></param>
    public void OpenOrClose_Result(bool isActive)
    {
        if (isActive)
        {
            //gm_End_Result.GetComponentInChildren<TextParentSize_>().OnDrag_S_Rect();
        }
        gm_End_Result.SetActive(isActive);
    }

    public string GetHuType(uint huType)
    {
        string sResult = "";
        switch (huType)
        {
            case 1:
                sResult = "清一色龙七对";
                break;
            case 2:
                sResult = "清一色小七对";
                break;
            case 3:
                sResult = "龙七对";
                break;
            case 4:
                sResult = "七对";
                break;
            case 5:
                sResult = "清对";
                break;
            case 6:
                sResult = "金钩钓";
                break;
            case 7:
                sResult = "对对胡";
                break;
            case 8:
                sResult = "清一色平胡";
                break;
            case 9:
                sResult = "平胡";
                break;
            default:
                break;
        }
        return sResult;
    }
}
