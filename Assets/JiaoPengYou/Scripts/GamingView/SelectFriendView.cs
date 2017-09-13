using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lang
{
    public class SelectFriendView : View
    {
        public GameObject selectCardSuit;
        public GameObject selectBidWindow;
        public Transform content;
        public Button back;
        public Button ok;
        public Text time;
        public Transform selectedCardRect;

        Card selectedCard;

        void Start()
        {
            ok.onClick.AddListener(() =>
            {
                NetSender.Inst.CallFriend(selectedCard.Value);
                ViewManager.Destroy<SelectFriendView>();
            });

            back.onClick.AddListener(() =>
            {
                selectCardSuit.SetActive(true);
                selectBidWindow.SetActive(false);
                ClearCards();
                SetOkInteractable(false);
                SetSelectedCardRect(false, Vector3.zero);
            });

            Util.UpdateTime(this, 12, time);
        }

        // 显示4种花色按钮
        public void ShowSuitButtons(List<Card> handCards)
        {
            selectCardSuit.SetActive(true);

            foreach (Transform t in selectCardSuit.transform)
            {
                var suitName = t.name;
                t.GetComponent<Button>().onClick.AddListener(() => ClickCardSuit(suitName, handCards));
            }
        }

        // 显示一种花色中所有的牌，灰掉已有的手牌
        void ClickCardSuit(string suitName, List<Card> handCards)
        {
            selectCardSuit.SetActive(false);
            var op = (CardSuit)System.Enum.Parse(typeof(CardSuit), suitName);
            switch (op)
            {
                case CardSuit.Heart:
                    LoadCards(Card.Hearts, handCards);
                    break;
                case CardSuit.Diamond:
                    LoadCards(Card.Diamonds, handCards);
                    break;
                case CardSuit.Spade:
                    LoadCards(Card.Spades, handCards);
                    break;
                case CardSuit.Club:
                    LoadCards(Card.Clubs, handCards);
                    break;
                default:
                    break;
            }

            selectBidWindow.SetActive(true);
        }

        void LoadCards(Card[] cards, List<Card> handCards)
        {
            foreach (var card in cards)
            {
                GameObject o = Util.LoadCard(card, content);
                if (handCards.Contains(card))
                {
                    Image img = o.GetComponent<Image>();
                    img.color = new Color(0.51F, 0.51F, 0.51F);
                    img.raycastTarget = false;
                }
                else
                {
                    Card c = card;
                    o.AddComponent<Button>().onClick.AddListener(() => ClickCard(c, o.transform));
                }
            }
        }

        void ClearCards()
        {
            foreach (Transform t in content)
            {
                Destroy(t.gameObject);
            }
        }

        void ClickCard(Card card, Transform t)
        {
            selectedCard = card;
            SetOkInteractable(true);
            SetSelectedCardRect(true, t.position);
        }

        void SetOkInteractable(bool on)
        {
            var cg = ok.GetComponent<CanvasGroup>();
            cg.interactable = on;
            cg.alpha = on ? 1 : 0.25F;
        }

        void SetSelectedCardRect(bool show, Vector3 position)
        {
            selectedCardRect.gameObject.SetActive(show);
            if (show)
            {
                selectedCardRect.position = position;
            }
        }
    }
}