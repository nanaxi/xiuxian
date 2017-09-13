using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HuDongDaoJu : MonoBehaviour {


    Transform ThisTrans = null;
    List<Button> DaoJU = new List<Button>();
    public Transform DaoJUContent = null;
    public Sprite[] IMG = new Sprite[3];
    string Reciver;
    int daojunum;
    void Awake()
    {
        
    }

    void Init()
    {
        ThisTrans = this.gameObject.transform;
        if (DaoJUContent.gameObject.activeSelf)
        {
            int temp = DaoJUContent.childCount;
            for (int i = 0; i < temp; i++)
            {
                int tempNum = 0;
                tempNum = i;
                DaoJU.Add(DaoJUContent.GetChild(i).GetComponent<Button>());
                DaoJU[i].transform.GetChild(0).GetComponent<Image>().sprite = IMG[i];
                DaoJU[i].onClick.AddListener(delegate
                {
                    SentDaoJU(tempNum);
                    Debug.Log("第" + tempNum + "个道具！");
                    Rest();
                });
            }
        }
      
    }
    void SentDaoJU(int i)
    {
        Reciver = this.gameObject.GetComponent<UI_PlayerInfo>().IDdaoju;
        Debug.Log("第" + i + "个道具！");
        Debug.Log("接收者"+Reciver);
        PublicEvent.GetINS.SentMegssageDaoju(uint.Parse(Reciver), (uint)i);
    }
    
    // Use this for initialization
    void Start () {
        Init();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Rest()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
}
