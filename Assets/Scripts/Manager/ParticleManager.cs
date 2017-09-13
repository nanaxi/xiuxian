using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager GetIns = null;
    public ParticleSystem red = null, bule = null;
    public GameObject MainBg = null;
    public AudioSource BGM;
    /// <summary>
    /// 过场动画
    /// </summary>
    public GameObject changec = null;
    void Awake()
    {
        if (GetIns == null)
            GetIns = this;
    }
    IEnumerator OpenVoice()
    {
        yield return new WaitForSeconds(5.0f);
        //this.gameObject.GetComponent<YunVaSDK_API>().enabled = true;
    }
    Vector3 Yun_DefaultPos = Vector3.one;
    public void Start()
    {
        Yun_DefaultPos = changec.GetComponent<RectTransform>().anchoredPosition3D;
    }
    public Tween t;
    /// <summary>
    /// 切换场景的时候顺带调用 0：切换到登陆界面   1：切换到主界面  2：过场切换  3：反向切换
    /// </summary>
    public void SwitchSence(int Num)
    {
        switch (Num)
        {
            case 0:
                t.Kill();
                BGM.clip = (AudioClip)Resources.Load("AudioClips/bgm/datingbgm", typeof(AudioClip));
                BGM.Play();
                changec.GetComponent<RectTransform>().anchoredPosition3D = Yun_DefaultPos;
                PublicEvent.GetINS.voteTime = 0;
                PublicEvent.GetINS.IsMyVote = false;
                BaseProto.playerInfo.m_atRoomId = 0;
                BaseProto.playerInfo.m_cdRoomId = 0;
                changec.SetActive(true);
                
                Invoke("ReLogin", 0.4f);
                changec.transform.SetAsLastSibling();
                t = changec.transform.DOLocalMoveX(-4700, 1.5f).From();
                t.SetAutoKill(false);
                break;
            case 1:
                //Debug.Log("asd");
                //战绩查询
                if(GameManager.GM.DS.Main!=null&& !GameManager.GM.DS.Main.activeSelf)
                GameManager.GM.DS.Main.SetActive(true);
                DataManage.Instance.RuleH3Z = false;
                t.Kill();
                BGM.clip = (AudioClip)Resources.Load("AudioClips/bgm/datingbgm", typeof(AudioClip));
                BGM.Play();
                changec.GetComponent<RectTransform>().anchoredPosition3D = Yun_DefaultPos;
                GameManager.GM.GameType = "none";
                PublicEvent.GetINS.ZhanjiHuiFangRequst();
                PublicEvent.GetINS.voteTime = 0;
                PublicEvent.GetINS.IsMyVote = false;
                BaseProto.playerInfo.m_atRoomId = 0;
                BaseProto.playerInfo.m_cdRoomId = 0;
                DataManage.Instance.ClearHistoryChat();
                if (GameManager.GM.DS.Main!=null)
                GameManager.GM.DS.Main.GetComponent<UI_Main>().SetInfo(GameManager.GM._AllPlayerData[0].Name, GameManager.GM._AllPlayerData[0].ID.ToString(), GameManager.GM._AllPlayerData[0].Diamond.ToString(),GlobalSettings.avatarUrl);
                changec.SetActive(true);
                changec.transform.SetAsLastSibling();
                Invoke("show",0.4f);
                t =changec.transform.DOLocalMoveX(-4000, 1).From();
                t.SetAutoKill(false);
                break;
            case 2:
                t.Kill();
                changec.GetComponent<RectTransform>().anchoredPosition3D = Yun_DefaultPos;
                Invoke("dis", 0.1f);
                t = changec.transform.DOLocalMoveX(-2000, 1.5f).From();
                t.SetAutoKill(false);
                BGM.clip = (AudioClip)Resources.Load("AudioClips/bgm/bgm", typeof(AudioClip));
                BGM.Play();
                if (GameManager.GM.DS.Login != null)
                {
                    Destroy(GameManager.GM.DS.Login);
                }
                break;
            case 3:
                t.Kill();
                changec.GetComponent<RectTransform>().anchoredPosition3D = Yun_DefaultPos;
                changec.SetActive(true);
                changec.transform.SetAsLastSibling();
                Invoke("dis", 0.6f);
                t.PlayBackwards();
                t = changec.transform.DOLocalMoveX(-4000, 1.5f).From();
                break;
            default:
                break;
        }
    }
    void ReLogin()
    {
        GameManager.GM.DS.Main.GetComponent<RectTransform>().anchoredPosition = new Vector2(-3544, 0);
        GameManager.GM.DS.Login =GameManager.GM.PopUI(ResPath.Login);   
    }
    void show()
    {
        GameManager.GM.DS.Main.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
    }
    void dis()
    {
        GameManager.GM.DS.Main.GetComponent<RectTransform>().anchoredPosition = new Vector2(-3544, 0);
        GameManager.GM.DS.Main.SetActive(false);
    }
    public void showRedYun()
    {
        red.Play();
    }
    public void showBuleYun()
    {
        bule.Play();
    }
}
