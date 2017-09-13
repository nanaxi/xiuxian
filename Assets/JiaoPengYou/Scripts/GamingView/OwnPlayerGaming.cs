using System.Text;
using UnityEngine.UI;

namespace Lang
{
    public class OwnPlayerGaming : PlayerGaming
    {
        public Button ready;
        public Button inviteFriend;

        public override void OnInit(PlayerInfo info)
        {
            info.SetAvatar(this.avatar);
            this.userId.text = info.name;
        }

        public void ResetButton()
        {
            ready.gameObject.SetActive(true);
            inviteFriend.gameObject.SetActive(true);
            SetReadyIcon(false);
        }

        public override void OnGameStarted()
        {
            ready.gameObject.SetActive(false);
            SetReadyIcon(false);
            inviteFriend.gameObject.SetActive(false);
        }

        void Start()
        {
            avatar.GetComponent<Button>().onClick.AddListener(() =>
            {
                ViewManager.Open<PersonalInfoView>();
            });

            ready.onClick.AddListener(() =>
            {
                NetSender.Inst.Ready();
            });

            inviteFriend.onClick.AddListener(() =>
            {
                var strs = Util.GetRuleString(GameMgr.Inst.room.roomRuleInfo);
                if (strs == null)
                {
                    return;
                }

                var cost = strs[0];
                var rule = strs[1].Replace(Util.NewLine, ",");
                var point = strs[2];

                StringBuilder sb = new StringBuilder();
                sb.Append(cost + "。");
                sb.Append(rule + "。");
                point = point.Replace(Util.NewLine, ",");
                sb.Append(point);
                sb.Replace("\n", "");
                sb.Replace(Util.NewLine, "");
                sb.Replace(Util.NewLine, "");
                var ruleStr = sb.ToString();
                ruleStr.Replace("\n\u3000\u3000\u3000", "");
                GameManager.GM.ShareLink("叫朋友", GameMgr.Inst.MaxRound.ToString(), ruleStr, GameMgr.Inst.room.roomId.ToString());
            });
        }

        public void AutoReady()
        {
            ready.onClick.Invoke();
        }

        public void SelfReady()
        {
            ready.gameObject.SetActive(false);
            SetReadyIcon(true);
        }
    }
}