using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

namespace Lang
{
    public class CardOperationView : View
    {
        public Transform buttons;
        public Text promptText;

        public enum Operation
        {
            /// <summary>
            /// 一条龙
            /// </summary>
            YiTiaoLong,

            /// <summary>
            /// 叫牌
            /// </summary>
            JiaoPai,

            /// <summary>
            /// 抢牌
            /// </summary>
            Qiang,

            /// <summary>
            /// 不抢
            /// </summary>
            BuQiang,

            /// <summary>
            /// 提示
            /// </summary>
            TiShi,

            /// <summary>
            /// 扯
            /// </summary>
            Che,

            /// <summary>
            /// 出牌
            /// </summary>
            ChuPai,

            /// <summary>
            /// 过
            /// </summary>
            Guo
        }

        CardOperation cardOp = new CardOperation();
        List<EachPrompt> eachPrompts = new List<EachPrompt>();
        int promptIndex = 0;
        bool showPromptText = true;
        IEnumerator hidePromptText;
        // 用于取消“一条龙”时，点击“过”弹窗再次确认
        bool haveYiTiaoLong = false;

        void Start()
        {
            foreach (Transform t in buttons)
            {
                var op = (Operation)System.Enum.Parse(typeof(Operation), t.name);
                t.GetComponent<Button>().onClick.AddListener(() => ClickButton(op));
            }
        }

        public void ShowButton(Operation op)
        {
            if (op == Operation.YiTiaoLong)
            {
                haveYiTiaoLong = true;
            }
            var o = buttons.Find(op.ToString()).gameObject;
            o.SetActive(true);
        }

        public void HideButtons()
        {
            foreach (Transform btn in buttons)
            {
                btn.gameObject.SetActive(false);
            }
        }

        void ClickButton(Operation op)
        {
            bool turnOver = true;
            switch (op)
            {
                case Operation.YiTiaoLong:
                    NetSender.Inst.YiTiaoLong();
                    break;
                case Operation.JiaoPai:
                    ViewManager.Open<SelectFriendView>().ShowSuitButtons(GameMgr.Inst.handCards);
                    break;
                case Operation.Qiang:
                    turnOver = DealQiang();
                    break;
                case Operation.BuQiang:
                    NetSender.Inst.BuQiangPai();
                    break;
                case Operation.TiShi:
                    DealTiShi();
                    turnOver = false;
                    break;
                case Operation.Che:
                    turnOver = DealChe();
                    break;
                case Operation.ChuPai:
                    turnOver = DealChuPai();
                    break;
                case Operation.Guo:
                    if (haveYiTiaoLong)
                    {
                        turnOver = false;
                        ViewManager.Open<PopupWindow2View>().Init("您确定取消一条龙么？", 12, () =>
                        {
                            haveYiTiaoLong = false;
                            ClickButton(Operation.Guo);
                        }, null);
                    }
                    else
                    {
                        NetSender.Inst.Pass();
                    }
                    break;
                default:
                    break;
            }

            if (turnOver)
            {
                HideButtons();
                ClearPrompts();
            }
        }

        bool DealQiang()
        {
            ViewManager.Open<PopupWindow1View>().Init("您确定要抢牌么？", 12, () =>
           {
               NetSender.Inst.QiangPai();
           }, () =>
           {
               NetSender.Inst.Pass();
           });

            return true;
        }

        void DealTiShi()
        {
            // 为自己首先出牌时，永远选择第一张，但当用户调整了界面中牌的位置之后，数据并未调整位置，因此无法直接使用数据的第一项，只能够根据界面中的对象来获取
            if (GameMgr.Inst.lastOutCards.Count == 0)
            {
                var card = ViewManager.Open<HandCardView>().GetLeftFirstCard();
                eachPrompts = new List<EachPrompt>() { new EachPrompt(new List<Card>() { card }) };
            }
            if (eachPrompts.Count == 0)
            {
                if (GameMgr.Inst.IsLastPlayerChe)
                {
                    eachPrompts = cardOp.PromptBombs(GameMgr.Inst.handCards);
                }
                else
                {
                    eachPrompts = cardOp.Prompt(GameMgr.Inst.handCards, GameMgr.Inst.lastOutCards);
                }
            }
            if (eachPrompts.Count == 0)
            {
                ClickButton(Operation.Guo);
                ShowPromptText("您没有牌能大过上家！");
            }
            else
            {
                var prompt = eachPrompts[promptIndex];
                promptIndex = (promptIndex + 1) % eachPrompts.Count;
                // 在手牌中显示可以出的牌
                var view = ViewManager.Open<HandCardView>();
                view.SelectCards(prompt.cards);
            }
        }

        bool DealChe()
        {
            bool needSelectChe = cardOp.Che_CheckNeedSelectCard(GameMgr.Inst.handCards, GameMgr.Inst.lastOutCards[0]);
            if (needSelectChe)
            {
                var view = ViewManager.Open<HandCardView>();
                if (view.selectedCards.Count == 0)
                {
                    ShowPromptText("选择要扯的牌！");
                    return false;
                }
                else
                {
                    // 判断是否选择正确的扯牌
                    var cards = view.selectedCards;
                    if (cards.Count == 2 && cards[0].NormalizedValue == cards[1].NormalizedValue && cards[0].NormalizedValue == GameMgr.Inst.lastOutCards[0].NormalizedValue)
                    {
                        NetSender.Inst.ChePai(view.selectedCards.ToCards());
                        return true;
                    }
                    else
                    {
                        ShowPromptText("选择要扯的牌！");
                        return false;
                    }
                }
            }
            else
            {
                var cheCards = cardOp.ChePai(GameMgr.Inst.handCards, GameMgr.Inst.lastOutCards[0]);
                NetSender.Inst.ChePai(cheCards.ToCards());
                return true;
            }
        }

        bool DealChuPai()
        {
            var handCardView = ViewManager.Open<HandCardView>();
            if (handCardView.selectedCards.Count == 0)
            {
                ShowPromptText("选择要出的牌！");
                return false;
            }
            bool play = false;
            if (GameMgr.Inst.IsLastPlayerChe)
            {
                play = cardOp.IsBomb(handCardView.selectedCards);
            }
            else
            {
                play = cardOp.IsPlay(handCardView.selectedCards, GameMgr.Inst.lastOutCards);
            }
            if (!play)
            {
                if (GameMgr.Inst.lastOutCards.Count == 0)
                {
                    ShowPromptText("您出的牌不合法！");
                }
                else
                {
                    ShowPromptText("您出的牌无法大过上家！");
                }
                return false;
            }

            NetSender.Inst.PlayCard(handCardView.selectedCards.ToCards());
            return true;
        }

        // 本轮操作已结束
        void ClearPrompts()
        {
            eachPrompts.Clear();
            promptIndex = 0;
        }

        public void ShowPromptText(string text)
        {
            if (!showPromptText)
            {
                return;
            }
            if (hidePromptText != null)
            {
                StopCoroutine(hidePromptText);
                hidePromptText = null;
            }

            ClearPromptText();
            promptText.text = text;

            promptText.DOFade(1, 0.25F);
            promptText.transform.DOPunchScale(new Vector3(0.25F, 0.25F, 0.25F), 0.25F);

            hidePromptText = HidePromptText(2);
            StartCoroutine(hidePromptText);

            StartCoroutine(SetShowPromptText());
        }

        IEnumerator HidePromptText(float seconds)
        {
            if (seconds > 0)
            {
                yield return new WaitForSeconds(seconds);
            }
            ClearPromptText();
        }

        IEnumerator SetShowPromptText()
        {
            showPromptText = false;
            yield return new WaitForSeconds(0.25F);
            showPromptText = true;
        }

        void ClearPromptText()
        {
            promptText.text = "";
            Color color = promptText.color;
            color.a = 0;
            promptText.color = color;
            promptText.transform.localScale = Vector3.one;
        }
    }
}