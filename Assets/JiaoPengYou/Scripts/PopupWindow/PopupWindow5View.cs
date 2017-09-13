using UnityEngine.UI;
using UnityEngine;

namespace Lang
{
    /// <summary>
    /// “投票某玩家退出游戏”弹窗
    /// 
    /// 是否同意xxx退出游戏
    ///     x xxx
    ///     √ yyy
    ///       zzz
    ///    游戏将在xxx分钟后自动解散
    ///  时    否
    /// </summary>
    public class PopupWindow5View : View
    {
        public Text title;
        public Transform[] playerAgrees;
        public Text time;
        public Button yes;
        public Button no;

        int totalSeconds = 120;

        // 要退出游戏的用户id
        uint disbandId;

        public override ViewLayer GetLayer()
        {
            return ViewLayer.Topmost;
        }

        void Start()
        {
            yes.onClick.AddListener(() =>
            {
                HideButton();

                NetSender.Inst.VoteDisbandRoom(true);
            });

            no.onClick.AddListener(() =>
            {
                HideButton();

                NetSender.Inst.VoteDisbandRoom(false);
            });

            UpdateTime();
        }

        void HideButton()
        {
            yes.gameObject.SetActive(false);
            no.gameObject.SetActive(false);
        }

        void UpdateTime()
        {
            StartCoroutine(Util.UpdateTime(totalSeconds, c =>
            {
                int minute = c / 60;
                int seconds = c % 60;
                time.text = string.Format("游戏将在{0}:{1}分钟后自动解散", minute, seconds);
            }, null));
        }

        public void UpdatePlayer(uint id, bool agree)
        {
            if (PlayerMgr.Inst.IsSelf(id))
            {
                HideButton();
            }
            // 为投票发起人
            if (disbandId == 0)
            {
                disbandId = id;
                var allPlayers = PlayerMgr.Inst.allPlayers;
                int pIndex = 0;
                for (int i = 0; i < allPlayers.Length; i++)
                {
                    var player = allPlayers[i];
                    if (player == null)
                    {
                        continue;
                    }
                    if (player.id == disbandId)
                    {
                        title.text = string.Format("是否同意<color=\"#ff4632\">{0}</color>退出游戏？", player.name);
                    }
                    else
                    {
                        Transform obj = playerAgrees[pIndex++];
                        obj.Find("UserName").GetComponent<Text>().text = player.name;
                        obj.name = player.id.ToString();
                    }
                }
            }
            else
            {
                foreach (Transform t in playerAgrees)
                {
                    if (t.name.Equals(id.ToString()))
                    {
                        if (agree)
                        {
                            t.Find("Agree").gameObject.SetActive(true);
                        }
                        else
                        {
                            t.Find("Disagree").gameObject.SetActive(true);
                        }
                        break;
                    }
                }
            }
        }
    }
}