using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_UsedDaoJU : MonoBehaviour
{



    // Use this for initialization
    //道具 图片
    public Sprite[] DaojuImage = new Sprite[3];
    Transform ThisTrans = null;
    GameObject Sender0;
    GameObject Sender1;
    GameObject Sender2;
    GameObject Sender3;
    public GameObject[] playerpostion = new GameObject[4];
    float zSpeed = -10f;
    float speed =3f;
    bool playend = false;


    private void Awake()
    {
        ThisTrans = this.gameObject.transform;
        PublicEvent.GetINS.Event_reciveHuDongDaoJu += ShowPopHuDongDaoJu;

    }
    private void OnDestroy()
    {
        PublicEvent.GetINS.Event_reciveHuDongDaoJu -= ShowPopHuDongDaoJu;
    }

    public void ShowPopHuDongDaoJu(uint Sender, uint Reciver, uint Value)
    {
        int T, j = 0;
        if (Lang.GameMgr.Inst.IsInRoom)
        {
            T = (int)Lang.PlayerMgr.Inst.GetSeat(Sender);
            j = (int)Lang.PlayerMgr.Inst.GetSeat(Reciver);
            Debug.Log("在扑克里发送互动道具");
        }
        else
        {
            T = DataManage.Instance.PData_GetIndex(Sender);
            j = DataManage.Instance.PData_GetIndex(Reciver);
            Debug.Log("在麻将里发送互动道具");
        }

        if (T != j)
        {
            switch (T)
            {
                case 0:

                    StartCoroutine(MoveToPositionPlayer0(T, j, int.Parse(Value.ToString())));

                    break;
                case 1:

                    StartCoroutine(MoveToPositionPlayer1(T, j, int.Parse(Value.ToString())));


                    break;
                case 2:

                    StartCoroutine(MoveToPositionPlayer2(T, j, int.Parse(Value.ToString())));

                    break;
                case 3:

                    StartCoroutine(MoveToPositionPlayer3(T, j, int.Parse(Value.ToString())));

                    break;
                default:
                    break;
            }
        }


    }


    IEnumerator MoveToPositionPlayer0(int i, int j, int value)
    {
        int vlauenum;
        GameObject Reciver;

        if (Sender0 == null)
        {
            vlauenum = value;
            Sender0 = new GameObject();
            Sender0.transform.SetParent(this.gameObject.transform, false);

            Debug.Log("几号" + j);
            Reciver = playerpostion[j];


            Sender0.AddComponent<RectTransform>().transform.position = playerpostion[i].GetComponent<RectTransform>().transform.position;

            Sender0.AddComponent<Image>().sprite = DaojuImage[value];
            Sender0.transform.SetParent(Reciver.transform);
            Debug.Log("几号" + Vector3.Distance(Sender0.transform.localPosition, Reciver.transform.localPosition));
            yield return GameManager.waitForEndOfFrame;
            while (Vector3.Distance(Sender0.transform.localPosition, Reciver.transform.localPosition) > 4)
            {

                Sender0.transform.Rotate(0, 0, zSpeed);

                Sender0.transform.localPosition = Vector3.MoveTowards(Sender0.transform.localPosition, Reciver.transform.localPosition, 500 * Time.deltaTime * speed);

                yield return 0;
            }

            if (Vector3.Distance(Sender0.transform.localPosition, Reciver.transform.localPosition) < 5)
            {
                Rest(j, 0, Reciver, Sender0, vlauenum);
            }
        }

    }
    IEnumerator MoveToPositionPlayer1(int i, int j, int value)
    {
        int vlauenum;
        GameObject Reciver;
        if (Sender1 == null)
        {
            vlauenum = value;
            Sender1 = new GameObject();
            Sender1.transform.SetParent(this.gameObject.transform, false);

            Debug.Log("几号" + j);
            Reciver = playerpostion[j];

            //if (me)
            //{

            //    Sender1.transform.localPosition = Vector3.zero;
            //}
            //else
            //{
            Sender1.AddComponent<RectTransform>().transform.position = playerpostion[i].GetComponent<RectTransform>().transform.position;
            //}
            Sender1.AddComponent<Image>().sprite = DaojuImage[value];
            Sender1.transform.SetParent(Reciver.transform);
            Debug.Log("几号" + Vector3.Distance(Sender1.transform.localPosition, Reciver.transform.localPosition));
            yield return GameManager.waitForEndOfFrame;
            while (Vector3.Distance(Sender1.transform.localPosition, Reciver.transform.localPosition) > 4)
            {

                Sender1.transform.Rotate(0, 0, zSpeed);

                Sender1.transform.localPosition = Vector3.MoveTowards(Sender1.transform.localPosition, Reciver.transform.localPosition, 500 * Time.deltaTime * speed);

                yield return 0;
            }

            if (Vector3.Distance(Sender1.transform.localPosition, Reciver.transform.localPosition) < 5)
            {
                Rest(j, 0, Reciver, Sender1, vlauenum);
            }
        }

    }
    IEnumerator MoveToPositionPlayer2(int i, int j, int value)
    {
        int vlauenum;
        GameObject Reciver;
        if (Sender2 == null)
        {
            vlauenum = value;
            Sender2 = new GameObject();
            Sender2.transform.SetParent(this.gameObject.transform, false);

            Debug.Log("几号" + j);
            Reciver = playerpostion[j];

            //if (me)
            //{
            //    Sender2.transform.localPosition = Vector3.zero;
            //}
            //else
            //{
            Sender2.AddComponent<RectTransform>().transform.position = playerpostion[i].GetComponent<RectTransform>().transform.position;
            //}
            Sender2.AddComponent<Image>().sprite = DaojuImage[value];
            Sender2.transform.SetParent(Reciver.transform);
            Debug.Log("几号" + Vector3.Distance(Sender2.transform.localPosition, Reciver.transform.localPosition));
            yield return GameManager.waitForEndOfFrame;
            while (Vector3.Distance(Sender2.transform.localPosition, Reciver.transform.localPosition) > 4)
            {

                Sender2.transform.Rotate(0, 0, zSpeed);

                Sender2.transform.localPosition = Vector3.MoveTowards(Sender2.transform.localPosition, Reciver.transform.localPosition, 500 * Time.deltaTime * speed);

                yield return 0;
            }

            if (Vector3.Distance(Sender2.transform.localPosition, Reciver.transform.localPosition) < 5)
            {
                Rest(j, 0, Reciver, Sender2, vlauenum);
            }
        }


    }
    IEnumerator MoveToPositionPlayer3(int i, int j, int value)
    {
        int vlauenum;
        GameObject Reciver;
        if (Sender3 == null)
        {
            vlauenum = value;
            Sender3 = new GameObject();
            Sender3.transform.SetParent(this.gameObject.transform, false);

            Debug.Log("几号" + j);
            Reciver = playerpostion[j];

            //if (me)
            //{

            //    //Sender3.AddComponent<RectTransform>().transform.position = new Vector3(0, 0, 0);
            //    Sender3.transform.localPosition = Vector3.zero;
            //}
            //else
            //{
            Sender3.AddComponent<RectTransform>().transform.position = playerpostion[i].GetComponent<RectTransform>().transform.position;
            //}
            Sender3.AddComponent<Image>().sprite = DaojuImage[value];
            Sender3.transform.SetParent(Reciver.transform);
            Debug.Log("几号" + Vector3.Distance(Sender3.transform.localPosition, Reciver.transform.localPosition));
            yield return GameManager.waitForEndOfFrame;
            while (Vector3.Distance(Sender3.transform.localPosition, Reciver.transform.localPosition) > 4)
            {

                Sender3.transform.Rotate(0, 0, zSpeed);

                Sender3.transform.localPosition = Vector3.MoveTowards(Sender3.transform.localPosition, Reciver.transform.localPosition, 500 * Time.deltaTime * speed);

                yield return 0;
            }

            if (Vector3.Distance(Sender3.transform.localPosition, Reciver.transform.localPosition) < 5)
            {
                Rest(j, 0, Reciver, Sender3, vlauenum);
            }
        }

    }
    //0是本地玩家，1右边，2上边，3左边</param>
    void Rest(int j, int i, GameObject Reciver, GameObject Sender, int value)
    {
        int vlauenum = value;
        switch (vlauenum)
        {
            case 0:
                ShowFace.Ins.PlayAnimDaoju(Face.hua, j, playerpostion[j]);
                SoundMag.GetINS.PlayGiftAudioClips("hua");
                break;
            case 1:
                ShowFace.Ins.PlayAnimDaoju(Face.xie, j, playerpostion[j]);
                SoundMag.GetINS.PlayGiftAudioClips("xie");
                break;
            case 2:
                ShowFace.Ins.PlayAnimDaoju(Face.pijiu, j, playerpostion[j]);
                SoundMag.GetINS.PlayGiftAudioClips("pijiu");
                break;
            default:
                break;
        }
        Destroy(Sender);
    }

}
