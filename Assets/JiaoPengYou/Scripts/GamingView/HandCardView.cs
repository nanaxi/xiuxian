using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Lang
{
    public class HandCardView : View
    {
        public readonly List<Card> selectedCards = new List<Card>();
        public readonly Dictionary<Card, GameObject> cardObjs = new Dictionary<Card, GameObject>();

        Dictionary<uint, DragCard> dragCards = new Dictionary<uint, DragCard>();

        public void ShowCard(int cardIndex)
        {
            transform.GetChild(cardIndex).gameObject.SetActive(true);
            Sort();
        }

        public void AddCards(List<Card> cards, bool hide = false)
        {
            foreach (var card in cards)
            {
                GameObject o = Util.LoadCard(card, transform);
                o.SetActive(!hide);
                var tmpCard = card;
                var btn = o.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                var nav = new Navigation();
                nav.mode = Navigation.Mode.None;
                btn.navigation = nav;
                btn.onClick.AddListener(() => ClickCard(o, tmpCard));
                cardObjs.Add(card, o);

                var dc = o.AddComponent<DragCard>();
                dragCards.Add(card.Value, dc);
            }
            Sort();
        }

        public void RemoveSelectedCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                GameObject o = cardObjs[card];
                Destroy(o);
                cardObjs.Remove(card);
                dragCards.Remove(card.Value);
            }

            selectedCards.Clear();
            SetDragCard(true);
        }

        public void Sort()
        {
            StartCoroutine(DelaySort());
        }

        public Card GetLeftFirstCard()
        {
            if (transform.childCount > 0)
            {
                var card = uint.Parse(transform.GetChild(0).name).ToCard();
                return card;
            }

            return default(Card);
        }

        public void SelectCards(List<Card> cards)
        {
            // 上一次选择的牌如果没在当前选择的牌中，那么则取消选择
            foreach (var lastSelectedCard in selectedCards.ToArray())
            {
                if (!cards.Contains(lastSelectedCard))
                {
                    cardObjs[lastSelectedCard].GetComponent<Button>().onClick.Invoke();
                }
            }

            // 选中当前选择的牌
            foreach (var card in cards)
            {
                if (!selectedCards.Contains(card))
                {
                    cardObjs[card].GetComponent<Button>().onClick.Invoke();
                }
            }
        }

        IEnumerator DelaySort()
        {
            HorizontalLayoutGroup h = GetComponent<HorizontalLayoutGroup>();
            h.enabled = true;
            yield return GameManager.waitForEndOfFrame;
            h.enabled = false;
        }

        void ClickCard(GameObject o, Card card)
        {
            RectTransform rt = o.GetComponent<RectTransform>();
            if (selectedCards.Contains(card))
            {
                rt.DOAnchorPosY(-72, 0.1F);
                selectedCards.Remove(card);
            }
            else
            {
                rt.DOAnchorPosY(-35, 0.1F);
                selectedCards.Add(card);
            }
            SetDragCard(selectedCards.Count == 0);
        }

        void SetDragCard(bool drag)
        {
            foreach (var c in dragCards.Values)
            {
                c.enabled = drag;
            }
        }
    }
}