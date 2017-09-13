using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_PlayerInfo : MonoBehaviour
{
    public Text Name = null;
    public GameObject sexman;
    public GameObject sexwoman;
    public Text ID = null;
    public Text IP = null;
    public Text HomeCards = null;
    public Text City = null;
    public Image HeadSprite = null;
    public Button Close = null;
    public string IDdaoju = "";
    public Transform HuoDongDaoJu = null;

    Tween x;

    void Start()
    {
        Default();
    }

    void Default()
    {
        Close.onClick.AddListener(Rest);
    }
    /// <summary>
    /// 设定角色的 各部分属性
    /// </summary>
    /// <param name="Pic"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="ip"></param>
    /// <param name="homecard"></param>
    public void SetInfo(string name = "缺省", string id = "缺省", string ip = "缺省", string homecard = "缺省", Sprite Pic = null, string city = "未开启定位", int sex = 1)
    {
        if (Name != null)
            Name.text = "昵称：" + name;
        if (ID != null)
            ID.text = "ID：" + id;
        IDdaoju = id;
        if (IP != null)
            IP.text = "IP：" + ip;
        if (HomeCards != null)
            HomeCards.text = homecard;
        if (City != null)
            if (city != "")
            {
                City.text = city;
            }
            else
            {
                City.text = "未开启定位";
            }
        SetHead(Pic);
        if (id == BaseProto.playerInfo.m_id.ToString())
        {
            HuoDongDaoJu.gameObject.SetActive(false);
            HomeCards.transform.parent.gameObject.SetActive(true);
        }
        sexman.SetActive(sex == 1);
        sexwoman.SetActive(sex != 1);
    }
    /// <summary>
    /// 如果上面的不行就用这个
    /// </summary>
    public void SetAdress(string city)
    {
        City.text = city;
    }

    void SetHead(Sprite sprite, int num = 0)
    {
        if (HeadSprite != null)
        {
            HeadSprite.sprite = sprite;
            HeadSprite.color = new Color(255, 255, 255, 255);
        }
    }
    void Rest()
    {
        Destroy(this.gameObject);
    }
    void OnDestory()
    {
        GameManager.GM.DS.PlayerInfo = null;
    }

}
