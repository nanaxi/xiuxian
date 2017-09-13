using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class TotalSettlementView : View
    {
        public Text time;
        public Transform totalSettlementInfos;
        public SpriteNumber roundNum;
        public Text roomId;
        public Button share;
        public Button back;

        void Start()
        {
            share.onClick.AddListener(() =>
            {
                GameManager.GM.Share(0);
            });

            back.onClick.AddListener(() =>
            {
                GameMgr.Inst.ExitRoom();
            });

            StartCoroutine(UpdateTime());
        }

        public void Init(Settlement data)
        {
            roundNum.Number = data.currentRound.ToString();
            roomId.text = string.Format("房间号：{0}", data.roomId);
            for (int i = 0; i < totalSettlementInfos.childCount; i++)
            {
                var settlement = totalSettlementInfos.GetChild(i).GetComponent<TotalSettlementInfo>();
                var player = data.players[i];
                settlement.SetSettlementInfo(player);
            }
        }

        IEnumerator UpdateTime()
        {
            while (true)
            {
                time.text = Util.GetSettlementTime();
                yield return new WaitForSeconds(1);
            }
        }
    }
}