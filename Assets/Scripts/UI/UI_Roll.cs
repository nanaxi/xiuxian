using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Roll : MonoBehaviour {
    Transform ThisTrans = null;
    Button Close = null;
    public Transform Roll;
    public Button StartRoll;
    public ParticleSystem RollAnim;
    public ParticleSystem jieshu;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        DeFault();
    }
    void Init()
    {
        ThisTrans = gameObject.transform;
        Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
        if (StartRoll != null)
            StartRoll.onClick.AddListener(ClickRoll);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Rest()
    {
        GameManager.GM.DS.Roll = null;
        Destroy(this.gameObject);
    }
    void ClickRoll()
    {
        RollAnim.Play();
        x = Roll.DORotate(new Vector3(0, 0, 18000 + Random.Range(0, 8))*45,3,RotateMode.LocalAxisAdd);
        x.SetEase(Ease.InOutQuart);
        //x.SetLoops(-1);
        x.SetAutoKill(false);
        x.PlayForward();
        Invoke("t", 2.5f);
        
        x.OnComplete(delegate { RollAnim.Stop();});
    }
    void GetValue(int item)
    {
        RollAnim.Play();
        x = Roll.DORotate(new Vector3(0, 0, 360/ item), 10, RotateMode.LocalAxisAdd);
        x.SetEase(Ease.OutSine);
        //x.SetLoops(-1);
        x.SetAutoKill(false);
        x.PlayForward();
        x.OnComplete(delegate { RollAnim.Stop(); });
    }
    void t()
    {
        jieshu.Play();
    }

}
