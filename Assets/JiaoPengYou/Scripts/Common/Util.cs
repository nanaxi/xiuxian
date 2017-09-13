using ProtoBuf;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lang
{
    public class Util
    {
        public const string NewLine = "\n\u3000\u3000\u3000";

        public static GameObject LoadCard(Card card, Transform parent)
        {
            return Load("Cards/" + card.Value, parent);
        }

        public static PlayerInfo ConvertToPlayer(ProtoBuf.CharacterInfo from)
        {
            return new PlayerInfo()
            {
                id = from.charId,
                name = ConvertUserName(from.userName),
                account = from.channelId,
                diamond = from.diamond,
                avatarUrl = from.portrait,
                ip = from.ip,
                sex = from.sex
            };
        }

        public static PlayerInfo ConvertToPlayer(ProtoBuf.EachRoundPlayer from)
        {
            return new PlayerInfo()
            {
                id = from.charId,
                name = ConvertUserName(from.name),
                diamond = (uint)from.restDiamond,
                avatarUrl = from.portrait,
                ip = from.ip,
                sex = from.sex
            };
        }

        public static RankType ConvertToRankType(uint endSort)
        {
            if (endSort == 1)
            {
                return RankType.Up;
            }
            else if (endSort == 2 || endSort == 3)
            {
                return RankType.Middle;
            }
            else if (endSort == 4)
            {
                return RankType.Down;
            }
            else
            {
                return RankType.None;
            }
        }

        public static string GetSettlementTime()
        {
            return string.Format("时间：{0}\u3000{1}", DateTime.Now.ToString("yyyy.M.d"), DateTime.Now.ToString("HH:mm"));
        }

        public static GameObject Load(string path, Transform parent)
        {
            var res = Resources.Load<GameObject>(path);
            var o = GameObject.Instantiate(res);
            o.name = res.name;
            o.transform.SetParent(parent, false);
            return o;
        }

        public static float GetAnimTime(GameObject go)
        {
            Animator anim = go.GetComponent<Animator>();
            if (anim == null)
            {
                return 0;
            }
            var stateInfo = anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer"));
            return stateInfo.length;
        }

        public static void UpdateTime(MonoBehaviour mb, int totalSeconds, Text time, bool finishHideTime = false, Action onFinish = null)
        {
            time.text = totalSeconds.ToString();
            mb.StartCoroutine(UpdateTime(totalSeconds, c =>
            {
                time.text = c.ToString();
            }, () =>
            {
                if (finishHideTime)
                {
                    time.gameObject.SetActive(false);
                }
                if (onFinish != null)
                {
                    onFinish();
                }
            }));
        }

        public static IEnumerator UpdateTime(int totalSeconds, Action<int> onSeconds, Action onFinish)
        {
            for (int current = totalSeconds - 1; current >= 0; current--)
            {
                yield return GameManager.wait1;
                if (onSeconds != null)
                {
                    onSeconds(current);
                }
            }
            if (onFinish != null)
            {
                onFinish();
            }
        }

        public static string ConvertUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(userName));
        }

        public static void AddTriggerListener(GameObject go, EventTriggerType eventID, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = go.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventID;
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        public static string[] GetRuleString(PokerRoomRuleInfo ruleInfo)
        {
            if (ruleInfo == null)
            {
                return null;
            }

            bool isChampionPay = false;
            bool isQiangFirst = false;
            bool isYiTiaoLong = false;
            bool isPingJuFanBei = false;
            bool isBaseScore2 = true;

            var info = ruleInfo.pokerRule;
            foreach (var pokerRule in info.pokerrules)
            {
                switch (pokerRule)
                {
                    case PokerRule.Poker.PingJuDouble:
                        isPingJuFanBei = true;
                        break;
                    case PokerRule.Poker.QiangFirst:
                        isQiangFirst = true;
                        break;
                    case PokerRule.Poker.YiTiaoLong:
                        isYiTiaoLong = true;
                        break;
                    case PokerRule.Poker.ChampionPay:
                        isChampionPay = true;
                        break;
                    default:
                        break;
                }
            }
            isBaseScore2 = info.baseScore == 2;

            string cost = "房费：" + (isChampionPay ? "冠军房费" : "均摊房费");
            string rule = "";
            string point = "";

            rule = "玩法：" + (isQiangFirst ? "抢牌出头" : "抢牌不出头");
            rule += isYiTiaoLong ? NewLine + "一条龙" : "";//NewLine "\n"
            rule += isPingJuFanBei ? NewLine + "平局翻倍" : "";//NewLine

            if (isQiangFirst)
            {
                if (isBaseScore2)
                {
                    point = "底分：2分（赢单）" + NewLine + "4分（赢双）" + NewLine + "6分（抢牌出头）" + NewLine + "8分（一条龙）";
                }
                else
                {
                    point = "底分：3分（赢单）" + NewLine + "6分（赢双）" + NewLine + "10分（抢牌出头）" + NewLine + "15分（一条龙）";
                }
            }
            else
            {
                if (isBaseScore2)
                {
                    point = "底分：2分（赢单）" + NewLine + "4分（赢双）" + NewLine + "6分（抢牌出头）" + NewLine + "8分（抢牌不出头）" + NewLine + "10分（一条龙）";
                }
                else
                {
                    point = "底分：3分（赢单）" + NewLine + "6分（赢双）" + NewLine + "10分（抢牌出头）" + NewLine + "15分（抢牌不出头）" + NewLine + "20分（一条龙）";
                }
            }

            return new string[3] { cost, rule, point };
        }
    }
}