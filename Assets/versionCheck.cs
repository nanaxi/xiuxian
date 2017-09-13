using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class versionCheck : MonoBehaviour
{

    static versionCheck _INS;
    public static versionCheck GetINSv
    {
        get
        {
            if (_INS == null)
            {
                _INS = new versionCheck();
            }
            return _INS;
        }
    }

    const string versionNumberUrl = "http://www.xiuxianchaguan.com/versionNumber.txt";

    public GameObject Mask;
    //public Text RollText;
    void Awake()
    {
#if UNITY_ANDROID
        StartCoroutine(SetConfig());
#endif

#if UNITY_IPHONE
        StartCoroutine(SetConfig());   
#endif

    }

    public bool isNull = false;

    int waitFor = 0;
    int MaxWaitFor = 3;
    int configReturnValue;


    bool IsNewest(string strValue)
    {
        if ("Bate1.2" == strValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LoadConfig(string versionNum, string url, string tishi, string waits ,string AppleAdress)
    {
        if (IsNewest(versionNum))
        {
            Mask.gameObject.SetActive(false);
        }
        else
        {
            //版本号非最新。打开最新版本的下载地址 。启动一个禁止触发的页面
            Mask.gameObject.SetActive(true);
            Debug.Log(versionNum);
            Debug.Log(tishi);
            Debug.Log(waits);
            Mask.transform.SetAsLastSibling();
            var tem = GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>();
            tem.Close.gameObject.SetActive(false);
            tem.SetMessage("\n\n当前不是最新版,即将跳转至新的下载链接！");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                StartCoroutine(wait(int.Parse(waits), AppleAdress));
            }
            else
            {
                StartCoroutine(wait(int.Parse(waits), url));
            }

        }
    }
    IEnumerator wait(int te, string url)
    {
        yield return new WaitForSeconds(te);
        Application.OpenURL(url);
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    /// <returns></returns>
    IEnumerator SetConfig()
    {
        WWW www = new WWW(versionNumberUrl);
        yield return www;
        string[] sArray = www.text.Split('|');
        //GameManager.GM.PubMes = sArray[5];

        //LoadConfig(sArray[1], sArray[2], sArray[3], sArray[4]);
        for (int i = 0; i < sArray.Length; i++)
        {
            Debug.Log(sArray[i]);
        }


        Debug.Log("我检查了 版本号");
        LoadConfig(sArray[1], sArray[2], sArray[3], sArray[4],sArray[5]);


       
        //RollText.GetComponent<Text>().text = sArray[5];
    }

}
