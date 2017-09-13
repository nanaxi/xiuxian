using ProtoBuf;
using System.Text;
using UnityEngine.UI;

namespace Lang
{
    public class RoomRuleView : View
    {
        public Text cost;
        public Text rule;
        public Text point;
        public Button close;

        void Start()
        {
            close.onClick.AddListener(() =>
            {
                ViewManager.Open<TopStatusBarView>().ShowRuleButton(true);
                ViewManager.Destroy<RoomRuleView>();
            });
        }

        public void ShowRuleInfo(PokerRoomRuleInfo ruleInfo)
        {
            var strs = Util.GetRuleString(ruleInfo);
            var cost = strs[0];
            var rule = strs[1];
            var point = strs[2];

            SetInfo(cost, rule, point);
        }

        void SetInfo(string cost, string rule, string point)
        {
            this.cost.text = cost;
            this.rule.text = rule;
            this.point.text = point;
        }
    }
}