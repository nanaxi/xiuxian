using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lang
{
    public class FanDetailView : View
    {
        public Button close;
        public Transform cards;

        void Start()
        {
            close.onClick.AddListener(() =>
            {
                ViewManager.Destroy<FanDetailView>();
            });
        }

        public void Init(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return;
            }

            var res = Resources.Load<GameObject>("Numbers/FanNumber");

            cards.Sort(new CardAscSort());
            foreach (var card in cards)
            {
                var o = Util.LoadCard(card, this.cards);
                GameObject fan = Instantiate(res);
                fan.transform.SetParent(o.transform, false);
                fan.GetComponent<RectTransform>().anchoredPosition = new Vector2(90, 0);
            }
        }
    }
}