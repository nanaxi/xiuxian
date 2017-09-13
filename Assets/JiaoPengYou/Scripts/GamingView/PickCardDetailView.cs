using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class PickCardDetailView : View
    {
        public Button close;
        public GameObject window;
        public ScrollRect scrollRect;
        public Transform content;
        public Text cardCount;

        List<uint> cards = new List<uint>();
        Dictionary<Card, GameObject> cardObjs = new Dictionary<Card, GameObject>();

        void Start()
        {
            close.onClick.AddListener(() =>
            {
                scrollRect.verticalNormalizedPosition = 1;
                Show(false);
            });
        }

        public void Init()
        {
            var sortedCards = GameMgr.Inst.SortedPickedCards;

            cardCount.text = string.Format("牌量：{0}", sortedCards.Count);

            foreach (var card in sortedCards)
            {
                if (!cards.Contains(card.Value))
                {
                    var o = Util.LoadCard(card, content);
                    cards.Add(card.Value);
                    cardObjs.Add(card, o);
                }
            }

            if (sortedCards.Count > 0)
            {
                var sortedCardObjs = (from cardObj in cardObjs
                                      orderby cardObj.Key.NormalizedValue
                                      select cardObj).ToDictionary(item => item.Key, item => item.Value);
                int index = 0;
                foreach (var cardObj in sortedCardObjs.Values)
                {
                    cardObj.transform.SetSiblingIndex(index++);
                }
            }
        }

        public void Show(bool show)
        {
            window.SetActive(show);
        }
    }
}