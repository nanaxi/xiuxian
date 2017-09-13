using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Lang
{
    public class RoundSettlementView : View
    {
        public Text time;
        public GameObject titleVictory;
        public GameObject titleFail;
        public GameObject titleDogfall;
        public Transform settlementInfos;

        public SpriteNumber roundNum;
        public Text roomId;

        public Button share;
        public Button bContinue;

        Settlement currentSettlement;

        void Start()
        {
            share.onClick.AddListener(() =>
            {
                GameManager.GM.Share(0);
            });

            bContinue.onClick.AddListener(() =>
            {
                ViewManager.Destroy<RoundSettlementView>();
                GameMgr.Inst.NextTurn(currentSettlement);
            });

            StartCoroutine(UpdateTime());
        }

        public void Init(Settlement data)
        {
            currentSettlement = data;
            SetTitle(data.resultType);
            roundNum.Number = data.currentRound.ToString();
            roomId.text = string.Format("房间号：{0}", data.roomId);
            for (int i = 0; i < settlementInfos.childCount; i++)
            {
                var settlement = settlementInfos.GetChild(i).GetComponent<RoundSettlementInfo>();
                var player = data.players[i];
                settlement.SetSettlementInfo(player);
            }
        }

        void SetTitle(ResultType type)
        {
            // 界面默认为显示
            titleVictory.SetActive(false);

            switch (type)
            {
                case ResultType.Victory:
                    titleVictory.SetActive(true);
                    break;
                case ResultType.Fail:
                    titleFail.SetActive(true);
                    break;
                case ResultType.Dogfall:
                    titleDogfall.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        IEnumerator UpdateTime()
        {
            while (true)
            {
                time.text = Util.GetSettlementTime();
                yield return GameManager.wait1;
            }
        }
    }
}