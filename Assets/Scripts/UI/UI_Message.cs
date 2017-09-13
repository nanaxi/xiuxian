using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Message : MonoBehaviour {
    Transform ThisTrans = null;
    Button Close = null;
    public Transform Content = null;
    Tween x;
    void Awake()
    {
        Init();
    }
    // Use this for initialization
    void Start()
    {
        DeFault();
        x = Content.DOLocalMoveX(-1800, 0.5f).From();
        x.SetEase(Ease.OutSine);
        x.SetAutoKill(false);
        x.PlayForward();
    }
    void Init()
    {
        ThisTrans = gameObject.transform;
        //Close = ThisTrans.Find("BG2/Close").GetComponent<Button>();
    }
    void DeFault()
    {
        if (Close != null)
            Close.onClick.AddListener(Rest);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Rest()
    {
        GameManager.GM.DS.Message = null;
        x = Content.DOLocalMoveX(-1800, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        //Destroy(this.gameObject);
    }
}
