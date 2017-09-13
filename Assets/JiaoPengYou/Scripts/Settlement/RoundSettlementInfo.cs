using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lang
{
    public class RoundSettlementInfo : MonoBehaviour
    {
        public Transform rank;

        public Image avatar;
        public GameObject ownerIcon;
        public GameObject bankerIcon;
        public GameObject friendIcon;
        public GameObject qiangIcon;
        public Text userName;
        public SpriteNumber totalScore;

        public Text fan;
        public Button fanDetail;
        public SpriteNumber roomCardNumPositive;
        public SpriteNumber roomCardNumNagative;

        List<Card> fanCards;

        int fanRate = 10;
        int upRank = 10;
        int downRank = -10;

        void Start()
        {
            fanDetail.onClick.AddListener(() =>
            {
                ViewManager.Open<FanDetailView>().Init(fanCards);
            });
        }

        public void SetSettlementInfo(PlayerSettlement s)
        {
            if (s.isYiTiaoLong)
            {
                SetYiTiaoLong();
            }
            else if (s.isWinDouble)
            {
                SetWinDouble();
            }
            else
            {
                SetRank(s.rank);
            }
            SetUserInfo(s);
            SetCardResult(s);
            SetScore(s.score);
            fanCards = s.fanCards;
        }

        void SetRank(RankType type)
        {
            if (type == RankType.None)
            {
                return;
            }
            rank.gameObject.SetActive(true);
            rank.GetChild((int)type).gameObject.SetActive(true);
        }

        void SetYiTiaoLong()
        {
            rank.gameObject.SetActive(true);
            rank.GetChild(3).gameObject.SetActive(true);
        }

        void SetWinDouble()
        {
            rank.gameObject.SetActive(true);
            rank.GetChild(4).gameObject.SetActive(true);
        }

        void SetUserInfo(PlayerSettlement player)
        {
            PlayerMgr.Inst.GetPlayer(player.id).SetAvatar(this.avatar);
            ownerIcon.SetActive(player.isOwner);
            bankerIcon.SetActive(player.isBanker);
            friendIcon.SetActive(player.isFriend);
            qiangIcon.SetActive(player.isQiang);
            this.userName.text = player.userName;
            if (player.totalScore >= 0)
            {
                this.totalScore.Number = "+" + player.totalScore.ToString();
            }
            else
            {
                this.totalScore.Number = "-" + Mathf.Abs(player.totalScore).ToString();
            }
        }

        void SetCardResult(PlayerSettlement s)
        {
            if (s.isWinDouble || s.isQiang || s.isYiTiaoLong)
            {
                fan.text = string.Format("{0}张+{1}张（+{2}番）", s.pickedCardCount, s.fan * fanRate, s.fan);
            }
            else
            {
                if (s.rank == RankType.Up)
                {
                    fan.text = string.Format("{0}张+{1}张（上游、+{2}番）", s.pickedCardCount, s.fan * fanRate + upRank, s.fan);
                }
                else if (s.rank == RankType.Down)
                {
                    int count = (int)s.fan * fanRate + downRank;
                    string str = count >= 0 ? "+" + count : count.ToString();
                    fan.text = string.Format("{0}张{1}张（下游、+{2}番）", s.pickedCardCount, str, s.fan);
                }
                else
                {
                    fan.text = string.Format("{0}张+{1}张（+{2}番）", s.pickedCardCount, s.fan * fanRate, s.fan);
                }
            }
        }

        void SetScore(int score)
        {
            if (score >= 0)
            {
                roomCardNumPositive.Number = "+" + score;
            }
            else
            {
                roomCardNumNagative.Number = "-" + Mathf.Abs(score);
            }
        }
    }
}