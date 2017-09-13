using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Store : MonoBehaviour {
    Transform ThisTrans = null;
    public Button Close = null;
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
        x = Content.DOLocalMoveX(-1600, 0.5f).From();
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
        x = Content.DOLocalMoveX(-1600, 0.3f);
        x.OnComplete(delegate { Destroy(this.gameObject); });
        GameManager.GM.DS.Store = null;
        //Destroy(this.gameObject);
    }
}
