using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Lang
{
    public class OutCardView : View
    {
        public Transform[] outCards;
        public GameObject bankerCardIcon;
        public GameObject friendCardIcon;
        public Transform[] pickedPositions;
        public float moveTime = 0.5F;
        public float scaleTime = 0.25F;

        GameObject[] icons = new GameObject[4];

        List<GameObject> cardObjs = new List<GameObject>();
        List<GameObject> cardIntervalObjs = new List<GameObject>();

        public void Show(Seat seat, List<Card> cards)
        {
            Transform cardParent = outCards[(int)seat];
            if (cardParent.childCount > 0)
            {
                var o = Util.Load("Cards/OutCardInterval", cardParent);
                cardIntervalObjs.Add(o);
            }
            foreach (var card in cards)
            {
                var o = Util.LoadCard(card, cardParent);
                var le = o.AddComponent<LayoutElement>();
                o.GetComponent<Image>().raycastTarget = false;
                le.preferredWidth = 61.875F;
                cardObjs.Add(o);
            }
        }

        /// <summary>
        /// type = 0无，1庄，2朋
        /// </summary>
        public void SetIcon(Seat seat, int type)
        {
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    SetIcon(seat, bankerCardIcon);
                    break;
                case 2:
                    SetIcon(seat, friendCardIcon);
                    break;
                default:
                    break;
            }
        }

        public void Clear()
        {
            foreach (var cardObj in cardObjs)
            {
                Destroy(cardObj);
            }
            cardObjs.Clear();
            foreach (var obj in cardIntervalObjs)
            {
                Destroy(obj);
            }
            cardIntervalObjs.Clear();
        }

        public void PickCard(Seat seat)
        {
            var dest = pickedPositions[(int)seat];
            float scaleDelayTime = moveTime - scaleTime;
            foreach (var cardObj in cardObjs)
            {
                cardObj.transform.SetParent(transform, true);
                var tmpObj = cardObj;
                cardObj.transform.DOMove(dest.position, moveTime);
                cardObj.transform.DOScale(0, scaleTime).SetDelay(scaleDelayTime).OnComplete(() => Destroy(tmpObj));
            }
            cardObjs.Clear();
            foreach (var obj in cardIntervalObjs)
            {
                Destroy(obj);
            }
            cardIntervalObjs.Clear();
        }

        void SetIcon(Seat seat, GameObject res)
        {
            int index = (int)seat;
            var outCardParent = outCards[index];
            if (outCardParent.childCount == 0)
            {
                return;
            }

            Transform card = outCardParent.GetChild(outCardParent.childCount - 1);
            GameObject icon = icons[index];
            if (icon == null)
            {
                icon = Instantiate(res);
                icon.name = res.name;
                icons[index] = icon;
            }
            icon.transform.SetParent(card, false);
            icon.transform.localPosition = new Vector3(13, 26, 0);
        }
    }
}