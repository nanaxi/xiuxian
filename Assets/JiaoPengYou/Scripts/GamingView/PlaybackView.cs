using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    // 回放界面，从GameMgr和NetReceiver中抽取的部分
    public class PlaybackView : View
    {
        public GameObject UiWin_ZJHFPrefab;

        UiWin_ZJHF zjhf;

        StringBuilder zjhfLog = new StringBuilder();

        QPRoundRecord record;

        IEnumerator playbackCoroutine;
        bool isPaused;

        // 所有操作列表
        List<Action> gameOps = new List<Action>();

        // 所有玩家当前手牌
        Dictionary<uint, List<Card>> playerHandCards = new Dictionary<uint, List<Card>>();

        // 叫的朋友那张牌
        Card calledFriend = new Card(0);

        // 红桃10
        Card red10 = new Card(0x0A);

        // 排名，本地记录
        List<uint> playerRanks = new List<uint>(4);

        // 记录当前有多少玩家手里的牌打完牌了
        int playedCardOverCount;

        // 是否为抢牌
        bool isQiang;

        // 当前播放的操作下标，用于暂停时记录
        int playbackIndex;

        public void Init(QPRoundRecord record, int maxCount)
        {
            this.record = record;

            LoadBasicView();
            InitRoom(record, maxCount);
            InitHandCards(record.playBack.players);
            GameMgr.Inst.ShowBanker(GameMgr.Inst.BankerId);

            gameOps = InitGameOp(record.playBack);

            InitZJHF_Window();

            Play();
            StartPlay();
        }

        // 拖动小窗口
        void InitZJHF_Window()
        {
            GameObject go = Instantiate(UiWin_ZJHFPrefab);
            go.transform.SetParent(ViewManager.GetLayer(ViewLayer.Topmost), false);
            go.transform.localScale = Vector3.one * 0.6948F;
            zjhf = go.GetComponent<UiWin_ZJHF>();
            ViewManager.AddOther(go);
            Button[] btnAry = zjhf.tBtnParent.GetComponentsInChildren<Button>();
            foreach (Button btnNew in btnAry)
            {
                string btnName = btnNew.name;
                switch (btnName)
                {
                    case "Btn_Play":
                        btnNew.onClick.AddListener(Play);
                        break;
                    case "Btn_Stop":
                        btnNew.onClick.AddListener(Pause);
                        break;
                    case "Btn_AnewPlay":
                        btnNew.onClick.AddListener(Replay);
                        break;
                }
            }
        }

        // 播放
        public void Play()
        {
            isPaused = false;
        }

        void StartPlay()
        {
            if (playbackCoroutine != null)
            {
                StopCoroutine(playbackCoroutine);
            }

            playbackIndex = 0;
            playbackCoroutine = DelayPlay();
            StartCoroutine(playbackCoroutine);
        }

        // 回放具体操作，固定时间间隔
        IEnumerator DelayPlay()
        {
            yield return new WaitForSeconds(2);

            float time = 2;
            while (playbackIndex < gameOps.Count)
            {
                if (isPaused)
                {
                    yield return null;
                }
                else
                {
                    var ops = gameOps[playbackIndex];
                    ops.Invoke();
                    playbackIndex++;

                    yield return new WaitForSeconds(time);
                }
            }

            RestPlayerShowCards();

            yield return new WaitForSeconds(2);

            ViewManager.Open<PopupWindow9View>().Init("回放结束！", 3);
        }

        // 暂停
        public void Pause()
        {
            isPaused = true;
        }

        // 重播
        public void Replay()
        {
            ViewManager.Destroy<HandCardPlaybackView>();
            ViewManager.Destroy<RankView>();
            ViewManager.Open<BidFriendView>().Clear();
            ViewManager.Open<OutCardView, HandCardPlaybackView>(false);
            ViewManager.Open<OutCardView>().Clear();
            ViewManager.Open<PlayerGamingView>().ClearIcons();

            playerHandCards.Clear();
            playerRanks.Clear();
            playedCardOverCount = 0;
            isQiang = false;
            zjhfLog.Length = 0;
            zjhf.t_CLog.text = "";
            zjhf.t_Log.text = "";

            InitHandCards(record.playBack.players);

            Play();
            StartPlay();
        }

        void LoadBasicView()
        {
            ViewManager.Open<GameBgView>();
            ViewManager.Open<TopStatusBarView>();
            ViewManager.Open<PlayerGamingView>();
            ViewManager.Open<ShortcutPlaybackView>();
            ViewManager.Open<HandCardPlaybackView>();
            ViewManager.Open<OutCardView>();
        }

        void InitRoom(QPRoundRecord record, int maxCount)
        {
            InitPlayerInfo(record.players);

            foreach (var p in record.players)
            {
                GameMgr.Inst.PlayerJoin(p.charId);
            }

            ViewManager.Open<TopStatusBarView>().SetBasicInfo(record.playBack.roomId, record.roundNum + 1, (uint)maxCount);

            var gamingView = ViewManager.Open<PlayerGamingView>();
            var playerView = ViewManager.Open<PlayerGamingView>().playerGaming[0] as OwnPlayerGaming;
            playerView.ready.gameObject.SetActive(false);
            playerView.inviteFriend.gameObject.SetActive(false);
            for (int i = 1; i < gamingView.playerGaming.Length; i++)
            {
                var other = gamingView.playerGaming[i] as OtherPlayerGaming;
                other.distance.gameObject.SetActive(false);
            }

            ViewManager.Open<TopStatusBarView>().rule.gameObject.SetActive(false);

            GameMgr.Inst.IsInitCompleted = true;
            GameMgr.Inst.OwnerId = record.players[0].charId;
            GameMgr.Inst.ShowOwnerIcon();
        }

        // 初始化已经加入到房间内的玩家信息
        void InitPlayerInfo(List<EachRoundPlayer> players)
        {
            var selfId = PlayerMgr.Inst.Self.id;
            var selfServerPos = players.Find(c => c.charId == selfId).position;
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                // 登录的时候，自己已经设置过信息了，而且比较全，这里信息不全不再进行设置。只设置其他玩家的信息
                if (PlayerMgr.Inst.IsSelf(player.charId))
                {
                    PlayerMgr.Inst.Self.avatarUrl = player.portrait;
                    PlayerMgr.Inst.Self.sex = player.sex;
                    continue;
                }

                PlayerInfo p = Util.ConvertToPlayer(player);
                PlayerMgr.Inst.Add(p, (int)player.position, (int)selfServerPos);
            }
        }

        // 初始化手牌
        void InitHandCards(List<QPEachPBPlayer> players)
        {
            var playbackView = ViewManager.Open<HandCardPlaybackView>();
            var mgr = PlayerMgr.Inst;
            foreach (var p in players)
            {
                int seat = (int)mgr.GetSeat(p.charId);
                var handCards = ConvertCards(p.handCards);
                handCards.Sort(new CardAscSort());
                playbackView.handCardViews[seat].AddCards(handCards);

                if (GameMgr.Inst.BankerId == 0 && handCards.Contains(red10))
                {
                    GameMgr.Inst.BankerId = p.charId;
                }

                playerHandCards.Add(p.charId, handCards);
            }
        }

        // 服务器发的牌对应到本地
        List<Card> ConvertCards(List<uint> serverCards)
        {
            var cards = new List<uint>();
            foreach (var c in serverCards)
            {
                if (c != 0)
                {
                    cards.Add(c);
                }
            }
            return cards.ToCards();
        }

        // 初始化操作，加入到列表中
        List<Action> InitGameOp(QPPlayBack playBack)
        {
            List<Action> actions = new List<Action>();

            foreach (var op in playBack.opInfo)
            {
                Action tmp = null;
                switch (op.op)
                {
                    case PokerGameOP.Poker_OP_QP:// 抢牌
                        tmp = QiangAction(op);
                        break;
                    case PokerGameOP.Poker_OP_CHE:// 扯
                    case PokerGameOP.Poker_OP_CHU:// 出牌
                        tmp = PlayCardAction(op);
                        break;
                    case PokerGameOP.Poker_OP_YTL:// 一条龙
                        tmp = YTLAction(playBack, op);
                        break;
                    case PokerGameOP.Poker_OP_GUO:// 过
                        tmp = PassAction(playBack, op);
                        break;
                    case PokerGameOP.Poker_OP_SHOUT:// 叫朋友
                        tmp = CallFriendAction(op);
                        break;
                    case PokerGameOP.Poker_OP_PICK:// 拾牌
                        tmp = PickCardAction(op);
                        break;
                    default:
                        break;
                }

                if (tmp != null)
                {
                    switch (op.op)
                    {
                        case PokerGameOP.Poker_OP_QP:
                        case PokerGameOP.Poker_OP_CHE:
                        case PokerGameOP.Poker_OP_YTL:
                        case PokerGameOP.Poker_OP_GUO:
                            uint id = op.charId;
                            var oop = op.op;
                            tmp += () => GameMgr.Inst.PlayCardOpVoice(id, oop);
                            break;
                        default:
                            break;
                    }

                    actions.Add(tmp);
                }
            }

            return actions;
        }

        // 抢牌命令
        Action QiangAction(QPEachPBPlayerOP op)
        {
            isQiang = true;

            var id = op.charId;

            return () =>
            {
                GameMgr.Inst.ShowQiang(id);

                string log = "<color=#00ffffff>【玩家抢牌】</color>" + "名字：" + GetPlayerName(id) + "\n";
                AddLog(log);
            };
        }

        // 出牌命令
        Action PlayCardAction(QPEachPBPlayerOP op)
        {
            return () =>
            {
                var outCards = ConvertCards(op.cards);
                if (outCards.Contains(calledFriend))
                {
                    // 朋友牌出现了
                    GameMgr.Inst.ShowFriend(op.charId);
                }

                // 如果自己为扯牌，那么不播放具体牌的语音，而是通过PlayCardOpVoice来播放扯的语音
                bool isChe = op.op == PokerGameOP.Poker_OP_CHE;

                PlayCard(op.charId, outCards, !isChe);
                GameMgr.Inst.AddLastPlayerOutCards(op.charId, outCards);

                if (op.op == PokerGameOP.Poker_OP_CHE)
                {
                    GameMgr.Inst.Che(op.charId);

                    string log = "<color=yellow>【玩家扯牌】</color>" + "名字：" + GetPlayerName(op.charId) + "—扯牌：" + GetCardString(outCards) + "\n";
                    AddLog(log);
                }
                else
                {
                    string log = "<color=green>【玩家出牌】</color>" + "名字：" + GetPlayerName(op.charId) + "—出牌：" + GetCardString(outCards) + "\n";
                    AddLog(log);
                }

                // 该玩家牌打完了
                var handCards = playerHandCards[op.charId];
                if (handCards.Count == 0)
                {
                    if (!isQiang)
                    {
                        ShowRank(op.charId);
                    }
                    playedCardOverCount++;
                }

                // 如果为叫朋友情况下，剩下第3家和第4家的时候，第3家最后一首牌出完后，需要和第4的一个玩家比牌
                if (playedCardOverCount == 3)
                {
                    StartCoroutine(ShowFourthPlayer());
                }
            };
        }

        // 一条龙命令
        Action YTLAction(QPPlayBack playBack, QPEachPBPlayerOP op)
        {
            return () =>
            {
                Seat seat = PlayerMgr.Inst.GetSeat(op.charId);
                GameMgr.Inst.ShowEffect(OperationEffectView.EffectType.YiTiaoLong, seat);

                var doing = playBack.players.Find(p => p.charId == op.charId);
                var handCards = ConvertCards(doing.handCards);
                PlayCard(doing.charId, handCards, doing.charId == op.charId);

                foreach (var p in playBack.players)
                {
                    if (p.charId != op.charId)
                    {
                        var otherCards = ConvertCards(p.handCards);
                        PlayCard(p.charId, otherCards, p.charId == op.charId);
                    }
                }

                string log = "<color=red>【玩家一条龙】</color>" + "名字：" + GetPlayerName(op.charId) + "\n";
                AddLog(log);
            };
        }

        // 过命令
        Action PassAction(QPPlayBack playBack, QPEachPBPlayerOP op)
        {
            return () =>
            {
                string log = "<color=red>【玩家过】</color>" + "名字：" + GetPlayerName(op.charId) + "\n";
                AddLog(log);
            };
        }

        // 叫的朋友命令
        Action CallFriendAction(QPEachPBPlayerOP op)
        {
            var card = ConvertCards(op.cards)[0];
            calledFriend = card;

            return () =>
            {
                GameMgr.Inst.ShowBidFriend(card);

                string log = "<color=blue>【玩家叫朋友】</color>" + "名字：" + GetPlayerName(op.charId) + "—朋友牌：" + card.ToString() + "\n";
                AddLog(log);
            };
        }

        // 拾取牌命令
        Action PickCardAction(QPEachPBPlayerOP op)
        {
            return () =>
            {
                Seat seat = PlayerMgr.Inst.GetSeat(op.charId);
                if (isQiang)
                {
                    ViewManager.Open<OutCardView>().Clear();
                }
                else
                {
                    ViewManager.Open<OutCardView>().PickCard(seat);
                }
                GameMgr.Inst.ClearRoundData();

                string log = "<color=blue>【玩家收牌】</color>" + "名字：" + GetPlayerName(op.charId) + "\n";
                AddLog(log);
            };
        }

        // 显示第3家和第4家玩家的牌，并比对大小，播放收牌动画
        IEnumerator ShowFourthPlayer()
        {
            // 找到第4家玩家(手里面还有牌的)
            uint charId = 0;
            foreach (var id in playerHandCards.Keys)
            {
                var handCards = playerHandCards[id];
                if (handCards.Count > 0)
                {
                    charId = id;
                    break;
                }
            }

            var cards = new List<Card>(playerHandCards[charId]);
            PlayCard(charId, cards, false);
            ShowRank(charId);

            yield return new WaitForSeconds(2);

            // 比较两玩家牌大小，进行收牌
            var lastOutCards = GameMgr.Inst.lastOutCards;
            var datas = new List<Card>(cards.Count + lastOutCards.Count);
            datas.AddRange(cards);
            datas.AddRange(lastOutCards);

            CardOperation co = new CardOperation();

            // 如果第4玩家可以扯牌，那么第4玩家牌大
            if (lastOutCards.Count == 1)
            {
                var cheCards = co.ChePai(cards, lastOutCards[0]);
                if (cheCards.Count >= 2)
                {
                    GameMgr.Inst.PickCard(charId, datas);
                    yield break;
                }
            }

            var prompts = co.Prompt(cards, lastOutCards);
            // 第3玩家牌大
            if (prompts.Count == 0)
            {
                GameMgr.Inst.PickCard(GameMgr.Inst.LastOutCardId, datas);
            }
            else
            {
                GameMgr.Inst.PickCard(charId, datas);
            }
        }


        // 手上的牌打出去
        void PlayCard(uint id, List<Card> outCards, bool playWithEffects = true, bool playCardVoice = true)
        {
            GameMgr.Inst.ShowOutCards(id, outCards);

            var playbackView = ViewManager.Open<HandCardPlaybackView>();
            int seat = (int)PlayerMgr.Inst.GetSeat(id);
            var handCardView = playbackView.handCardViews[seat];
            handCardView.RemoveSelectedCards(outCards);
            handCardView.Sort();

            RemoveHandCards(id, outCards);

            if (playWithEffects)
            {
                GameMgr.Inst.PlayCardEffect(id, outCards);
                if (playCardVoice)
                {
                    GameMgr.Inst.PlayCardVoice(id, outCards);
                }
            }
        }

        // 玩家出完牌，显示排名
        void ShowRank(uint charId)
        {
            playerRanks.Add(charId);

            var endSort = (uint)playerRanks.Count;
            GameMgr.Inst.ShowRank(charId, endSort);
        }

        // 当所有牌命令都执行完成后，如果还有剩余的玩家手里有牌，那么都亮出来
        void RestPlayerShowCards()
        {
            foreach (var id in playerHandCards.Keys)
            {
                var handCards = playerHandCards[id];
                if (handCards.Count > 0)
                {
                    PlayCard(id, new List<Card>(handCards), false);
                }
            }
        }

        // 移除玩家打出的手牌
        void RemoveHandCards(uint charId, List<Card> outCards)
        {
            var handCards = playerHandCards[charId];
            foreach (var card in outCards)
            {
                handCards.Remove(card);
            }
        }

        string GetCardString(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var card in cards)
            {
                sb.Append(card.ToString() + ",");
            }

            sb = sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        string GetPlayerName(uint id)
        {
            return PlayerMgr.Inst.GetPlayer(id).name;
        }

        // 添加日志到小窗口中
        void AddLog(string log)
        {
            zjhfLog.Append(log);

            zjhf.t_CLog.text = log;
            zjhf.t_Log.text = zjhfLog.ToString();
        }
    }
}