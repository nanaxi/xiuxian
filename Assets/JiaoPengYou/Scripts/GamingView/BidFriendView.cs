using UnityEngine;

namespace Lang
{
    // 显示叫的朋友牌
    public class BidFriendView : View
    {
        public GameObject bg;
        public Transform cardParent;

        GameObject card;

        public void Show(Card card)
        {
            bg.SetActive(true);
            GameObject o = Util.LoadCard(card, cardParent);
            this.card = o;

            var rt = o.GetComponent<RectTransform>();
            rt.sizeDelta = (cardParent as RectTransform).sizeDelta;
        }

        public void Clear()
        {
            if (card)
            {
                Destroy(card);
            }
            bg.SetActive(false);
        }
    }
}