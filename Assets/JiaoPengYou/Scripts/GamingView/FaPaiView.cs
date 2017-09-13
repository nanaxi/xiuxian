using UnityEngine;
using DG.Tweening;
using System;

namespace Lang
{
    public class FaPaiView : View
    {
        public Transform cardHeaps;
        public Transform seats;

        public float moveSpeed = 10;
        public float interval = 1;

        public void Show(Action onComplete = null)
        {
            var view = ViewManager.Open<HandCardView>();
            int nextSeat = 0;
            int index = 0;
            int cardIndex = 0;
            var initCardCount = cardHeaps.childCount;
            for (int i = cardHeaps.childCount - 1; i >= 0; i--)
            {
                var cardBg = cardHeaps.GetChild(i);
                Transform destination = seats.GetChild(nextSeat);
                int tmpIndex = index;
                cardBg.DOMove(destination.position, moveSpeed).SetSpeedBased().
                    SetDelay(index * interval).
                    OnComplete(() =>
                    {
                        cardBg.gameObject.SetActive(false);
                        if (tmpIndex % 4 == 0)
                        {
                            view.ShowCard(cardIndex);
                            cardIndex++;
                        }
                        initCardCount--;
                        if (initCardCount <= 0 && onComplete != null)
                        {
                            onComplete();
                        }
                    });
                nextSeat = (nextSeat + 1) % 4;
                index++;
            }
        }

    }
}