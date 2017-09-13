using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_Login : MonoBehaviour
{
    Button login_btn = null;
    public Text InputName = null;
    //void Awake()
    //{
    //    //PublicEvent.GetINS.LoginRest += turnToMain;
    //}
    // Use this for initialization
    void Start()
    {
        GameManager.GM.DS.Login=gameObject;
        Init();
        Default();
    }
    void Init()
    {
        login_btn = transform.Find("BG/Login").GetComponent<Button>();
    }
    void Default()
    {
#if UNITY_ANDROID
        InputName.transform.parent.gameObject.SetActive(false);
        //InputName.transform.parent.gameObject.SetActive(true);
#endif

#if UNITY_IPHONE
        InputName.transform.parent.gameObject.SetActive(false);
#endif

#if UNITY_EDITOR
        InputName.transform.parent.gameObject.SetActive(true);
#endif

#if UNITY_STANDALONE_WIN
        InputName.transform.parent.gameObject.SetActive(true);
#endif

        if (login_btn != null)
            login_btn.onClick.AddListener(login);  
    }
    void login()
    {
        login_btn.interactable = false;



#if UNITY_ANDROID
        AnySDKManager.SendLogin();
        //LoginTest();
#endif

#if UNITY_IPHONE
        AnySDKManager.SendLogin();
#endif

#if UNITY_EDITOR
        LoginTest();
#endif

#if UNITY_STANDALONE_WIN
        LoginTest();
#endif  
        
        Debug.Log("登陆！");
        Invoke("Show", 5.0f);
    }
    void Show()
    {
        login_btn.interactable = true;
    }

    public void turnToMain()
    {
        ParticleManager.GetIns.SwitchSence(1);
        //GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("切换到主界面");
        Invoke("Rest", 0.2f);
    }
    void Rest()
    {
        //PublicEvent.GetINS.LoginRest -= turnToMain;
        //ParticleManager.GetIns.LoginBg.SetActive(false);
        //ParticleManager.GetIns.MainBg.SetActive(true);
        GameManager.GM.DS.Login = null;
        Destroy(this.gameObject);
        //gameObject.SetActive(false);
    }
    void LoginTest()
    {
        byte[] byteArray;
        if (InputName.text != "")
        {
            byteArray = System.Text.Encoding.UTF8.GetBytes(InputName.text); 
        }
        else {
            byteArray = System.Text.Encoding.UTF8.GetBytes("zxc");//System.DateTime.Now.Day + "" + System.DateTime.Now.Hour + "" + System.DateTime.Now.Minute + "" + System.DateTime.Now.Second
        }
        
        string base64Str = Convert.ToBase64String(byteArray);
        GlobalSettings.LoginUserName = base64Str;
        //GlobalSettings.LoginUserName = System.DateTime.Now.Day + "" + System.DateTime.Now.Hour + "" + System.DateTime.Now.Minute + "" + System.DateTime.Now.Second;//Name.textComponent.text;//"荣昌店，糖糖"; // //"1113535";"1113535"; "1113535";// "6145131";9151653 ; "13165316"; "21213945";//"22171936";荣昌店，糖糖
        GlobalSettings.LoginUserId = GlobalSettings.LoginUserName;//ID.textComponent.text;//"Y-ai188";//GlobalSettings.LoginUserName;Y-ai188
        GlobalSettings.sex = 0;
        LoginProcessor.Inst().Login();
    }
    void OnDestory()
    {
        //PublicEvent.GetINS.LoginRest -= turnToMain;
        GameManager.GM.DS.Login = null;
    }
}
