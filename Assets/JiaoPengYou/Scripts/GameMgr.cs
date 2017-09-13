using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;
using System;
using System.Linq;
using System.Collections;
using System.Text;

namespace Lang
{
    public class GameMgr : MonoBehaviour
    {
        public static GameMgr Inst;

        // true，正在游戏中(已经在打牌了)。false，未开始打牌(可能是未进入房间或者正在准备或者已准备好)
        public bool IsGaming { get; private set; }
        // 手牌(已排序)
        public readonly List<Card> handCards = new List<Card>();
        // 已经收的牌
        readonly List<Card> pickedCards = new List<Card>();
        public List<Card> SortedPickedCards
        {
            get
            {
                pickedCards.Sort(new CardAscSort());
                return pickedCards;
            }
        }
        // 当前房间信息
        [NonSerialized]
        public PokerRoomInfo room = null;
        // 玩家是否已经进入房间
        public bool IsInRoom
        {
            get
            {
                if (IsGaming)
                {
                    return true;
                }
                return room != null;
            }
        }
        // 最后一位出牌的玩家id
        // 注意：因这是后来需要增加的原因，重连的时候，非自己操作时，无法从服务器获取该id
        // 这个id目前只会在结束的时候用，因此前面无法获取也无影响
        public uint LastOutCardId { get; private set; }
        // 最后一位玩家出的牌
        public readonly List<Card> lastOutCards = new List<Card>();
        // 当前正在进行的局数(当下一局开始的时候才会计数)
        private uint currentRound = 1;
        public uint CurrentRound
        {
            get { return currentRound; }
            private set { currentRound = value; }
        }
        // 总局数
        public uint MaxRound { get; private set; }
        // 已经打完的局数(当一局游戏结束才会计数)
        public uint PlayedRound { get; private set; }
        // 房主id
        public uint OwnerId { get; set; }
        // 庄id
        public uint BankerId { get; set; }
        // 朋友id(出的牌显示庄或友)
        public uint FriendId { get; set; }
        // 游戏是否结束（牌局打完了）
        public bool IsGameOver { get; private set; }
        // 抢牌玩家id
        public uint QiangId { get; set; }
        // 最后一位玩家是否为扯牌操作
        public bool IsLastPlayerChe { get; set; }
        // 指示最后操作的玩家id(表明轮到ta操作了)，不包括扯牌玩家
        public uint LastOperatedId { get; set; }
        // 是否是玩过至少一局游戏时解散房间
        public bool IsMidwayDisbandRoom { get; private set; }
        // 是否是通过重连进入房间
        public bool IsReconnect { get; private set; }
        // 初始化进入房间是否完成
        public bool IsInitCompleted { get; set; }


        GameStartDelay gameStartDelay = new GameStartDelay();
        TurnOverDelay turnOverDelay = new TurnOverDelay();

        void Awake()
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            Inst = null;
        }

        void OnApplicationQuit()
        {
            LoginProcessor.Inst().ApplyLogout();
        }

        void LoadBasicView()
        {
            ViewManager.Open<GameBgView>();
            ViewManager.Open<TopStatusBarView>();
            ViewManager.Open<PlayerGamingView>();
            ViewManager.Open<ShortcutView>();
            ViewManager.Open<OutCardView>();
            ViewManager.Open<HandCardView>();
            ViewManager.Open<CardOperationView>();
            ViewManager.Open<ChatView>(true);
            ViewManager.Open<GamingTimerView>(true);
            ViewManager.Open<HuDongDaoJuView>(true);
        }

        // 当某个玩家收牌时，清理这一轮数据
        public void ClearRoundData()
        {
            LastOutCardId = 0;
            lastOutCards.Clear();
            IsLastPlayerChe = false;
        }

        // 当单局结束时，清理这局数据
        public void ClearTurnData()
        {
            ClearRoundData();
            IsGaming = false;
            handCards.Clear();
            pickedCards.Clear();
            FriendId = 0;
            BankerId = 0;
            QiangId = 0;
            LastOperatedId = 0;
            turnOverDelay.Clear();
        }

        // 当退出扑克房间或游戏结束时，清理所有数据
        public void ClearAllData()
        {
            ClearTurnData();
            room = null;
            CurrentRound = 1;
            MaxRound = 1;
            PlayedRound = 0;
            OwnerId = 0;
            IsGameOver = false;
            PlayerMgr.Inst.Clear();
            IsMidwayDisbandRoom = false;
            IsReconnect = false;
            IsInitCompleted = false;
        }

        #region 与服务器交互的公开方法

        // 自己进入房间，断线重连
        public void EnterRoom(EnterRoomRsp rsp)
        {
            IsInitCompleted = false;

            ParticleManager.GetIns.MainBg.SetActive(false);
            PublicEvent.GetINS.Fun_joinRoomSuccess(rsp);
            ParticleManager.GetIns.SwitchSence(2);

            this.room = rsp.pokerRoom;
            LoadBasicView();

            CurrentRound = room.roundTh;
            MaxRound = room.roomRuleInfo.pokerRule.roundNum;
            PlayedRound = CurrentRound - 1;
            OwnerId = room.ownerId;
            BankerId = room.zjId;
            QiangId = room.qiangId;
            IsReconnect = IsReconnectRoom();

            RefreshTopInfo();
            foreach (var s in room.charStates)
            {
                PlayerJoin(s.charId);
            }
            // 显示玩家的总分数
            ShowTotalScore(room.charStates);

            // 正在游戏中(已经在打牌了)
            if (room.inGame != 1)
            {
                EnterRoomGaming();
            }
            else
            {
                EnterRoomInit();
            }

            IsInitCompleted = true;

            GpsManager.Inst.GetGps(g =>
            {
                Log.D("Gps", string.Format("获取自己的Gps : {0} ({1},{2})", PlayerMgr.Inst.Self.id, g.x, g.y));

                PlayerMgr.Inst.Self.latlng = g;
                ShowGps(room.charStates);

                GpsManager.Inst.GetCity(g, c =>
                {
                    PlayerMgr.Inst.Self.cityName = c;
                });

                NetSender.Inst.UploadGps(g);
            });

            RealTimeVoice.Inst.JoinRoom(room.roomId.ToString());
            RealTimeVoice.Inst.onJoinRoomSuccess = () =>
            {
                RealTimeVoice.Inst.OpenSpeaker(true);
            };
        }

        // 玩家已经开始游戏时初始化
        void EnterRoomGaming()
        {
            // 显示朋友icon
            var friendId = room.charStates.Find(x => x.charId == BankerId).friendId;
            ShowFriend(friendId);
            // 显示房主icon
            ShowOwnerIcon();
            // 显示庄家
            ShowBanker(BankerId);
            // 显示抢牌玩家
            ShowQiang(QiangId);
            // 各个玩家剩余的牌
            PokerCardsInfo myCardInfo = room.cardsInfos.Find(x => PlayerMgr.Inst.IsSelf(x.charId));
            Dictionary<uint, int> remainCardCount = new Dictionary<uint, int>();
            foreach (var info in room.cardsInfos)
            {
                remainCardCount.Add(info.charId, info.handCards.Count);
                // 显示其他玩家本轮已打出的所有牌
                ShowOutCards(info.charId, info.curChuCard.ToCards());
            }
            // 显示游戏开始后，各玩家信息
            InitGame(remainCardCount);
            // 显示己方手牌
            DrawCard(myCardInfo.handCards.ToCards());
            // 显示上中下游
            ShowRank();
            // 显示叫的牌
            ShowBidFriend(room.shoutCard.ToCard());
            // 已收的牌
            pickedCards.AddRange(myCardInfo.getCards.ToCards());
            // 自己待进行的操作
            ShowSelfOperation();
            // 显示当前正在出牌的玩家
            bool lastChe = false;
            var charList = room.roomCache.charList[0];
            foreach (var opInfo in charList.opInfo)
            {
                var op = (PokerGameOP)opInfo.op;
                if (op == PokerGameOP.Poker_OP_CHE)
                {
                    lastChe = true;
                    break;
                }
            }
            ShowPlayerOperating(lastChe ? room.nextCharId : charList.charId, new List<PokerGameOP>(0));
            // 投票解散房间
            ShowVoteDisbandRoom();
        }

        // 玩家进入房间，未准备，准备状态初始化
        void EnterRoomInit()
        {
            foreach (var charState in room.charStates)
            {
                // 玩家已准备
                if (charState.isZB > 0)
                {
                    PlayerReady(charState.charId);
                }
                else if (!IsReconnect && CurrentRound <= 1 && PlayerMgr.Inst.IsSelf(charState.charId))
                {
                    ViewManager.Open<PopupWindow7View>();
                }
            }
            ShowOwnerIcon();
        }

        // 在玩家头像下面，显示玩家总分数
        void ShowTotalScore(List<CharacterState> states)
        {
            if (PlayedRound == 0)
            {
                return;
            }
            var view = ViewManager.Open<PlayerGamingView>();
            foreach (var s in states)
            {
                Seat seat = PlayerMgr.Inst.GetSeat(s.charId);
                view.SetTotalScore(seat, s.restGold, true);
            }
        }

        // 是否为重连进入的房间
        bool IsReconnectRoom()
        {
            bool isReconnect = false;

            var roomKey = GetRoomKey();

            int lastRoomId = PlayerPrefs.GetInt(roomKey, 0);
            if (lastRoomId == 0)
            {
                isReconnect = false;
                PlayerPrefs.SetInt(roomKey, (int)room.roomId);
            }
            else
            {
                if ((uint)lastRoomId == room.roomId)
                {
                    isReconnect = true;
                }
                else
                {
                    isReconnect = false;
                    PlayerPrefs.SetInt(roomKey, (int)room.roomId);
                }
            }

            return isReconnect;
        }

        string GetRoomKey()
        {
            var id = PlayerMgr.Inst.Self.id;
            var roomKey = id + "LastPokerRoomId";
            return roomKey;
        }

        // 显示同意投票解散房间的玩家
        void ShowVoteDisbandRoom()
        {
            foreach (var agreeId in room.agreedCharId)
            {
                VoteDisbandRoom(agreeId, true);
            }
        }

        // 等待重连
        public void WaitReconnect()
        {
            ParticleManager.GetIns.MainBg.SetActive(true);

            ClearAllData();
            ViewManager.DestroyAll();
        }

        // 退出房间，返回到大厅
        public void ExitRoom()
        {
            ParticleManager.GetIns.MainBg.SetActive(true);
            ParticleManager.GetIns.SwitchSence(1);

            if (room != null)
            {
                RealTimeVoice.Inst.QuitRoom(room.roomId.ToString());
            }

            ClearAllData();
            ViewManager.DestroyAll();

            var roomKey = GetRoomKey();
            if (PlayerPrefs.HasKey(roomKey))
            {
                PlayerPrefs.DeleteKey(roomKey);
            }
        }

        // 玩家准备
        public void PlayerReady(uint id)
        {
            // quick
            if (IsGaming)
            {
                return;
            }

            if (PlayerMgr.Inst.IsSelf(id))
            {
                ViewManager.Open<PlayerGamingView>().SelfReady();
            }
            else
            {
                Seat seat = PlayerMgr.Inst.GetSeat(id);
                ViewManager.Open<PlayerGamingView>().Ready(seat);
            }
        }

        // 其他玩家进入房间
        public void PlayerJoin(uint id)
        {
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            PlayerInfo info = PlayerMgr.Inst.GetPlayer(seat);
            ViewManager.Open<PlayerGamingView>().Join(seat, info);
            CheckIpEnvironment();
        }

        // 其他玩家离开房间
        public void LeaveRoom(Seat seat)
        {
            ViewManager.Open<PlayerGamingView>().Leave(seat);
        }

        // 收到手牌，游戏开始了
        public void GameStarted(List<Card> cards)
        {
            Dictionary<uint, int> remainCardCount = new Dictionary<uint, int>();
            var players = PlayerMgr.Inst.allPlayers;
            foreach (var p in players)
            {
                remainCardCount.Add(p.id, 13);
            }
            InitGame(remainCardCount);
            DrawCard(cards, true);
            StartCoroutine(ShowGameStartAnim());
        }

        IEnumerator ShowGameStartAnim()
        {
            gameStartDelay.isTurnFirstPlay = true;

            // 播放开始游戏动画
            var go = Util.Load("Effects/GameStart", ViewManager.Root);
            float animTime = Util.GetAnimTime(go);
            yield return new WaitForSeconds(animTime);
            Destroy(go);

            bool animFinish = false;
            // 完成后进行发牌动画
            ViewManager.Open<FaPaiView, HandCardView>(false).Show(() =>
            {
                animFinish = true;
                gameStartDelay.isTurnFirstPlay = false;
            });
            SoundMag.GetINS.PlayGiftAudioClips("fapai");
            while (!animFinish)
            {
                yield return null;
            }
            // 如果因为网络等原因，还未后面的消息数据，那么后面就不能执行，一直等待
            while (BankerId == 0 || gameStartDelay.playingId == 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5F);

            ShowBanker(BankerId);
            ShowPlayerOperating(gameStartDelay.playingId, gameStartDelay.selfCardOps);
            ShowCardOperation(gameStartDelay.selfCardOps);

            gameStartDelay.Clear();
            ViewManager.Destroy<FaPaiView>();
        }

        // 摸牌
        public void DrawCard(List<Card> cards, bool hide = false)
        {
            cards.Sort(new CardAscSort());
            handCards.AddRange(cards);
            ViewManager.Open<HandCardView>().AddCards(cards, hide);
            IsGaming = true;
        }

        // 显示扑克牌操作
        public void ShowCardOperation(List<PokerGameOP> ops)
        {
            if (ops == null || ops.Count == 0)
            {
                return;
            }

            if (gameStartDelay.isTurnFirstPlay)
            {
                gameStartDelay.selfCardOps.AddRange(ops);
                return;
            }

            HideCardOperation();

            // 因后端“不抢”也是对应的是“过”。所以这里区分抢牌时的“抢牌、不抢(过)”状态，对应不同的按钮
            if (ops.Count == 2 && ops[0] == PokerGameOP.Poker_OP_QP && ops[1] == PokerGameOP.Poker_OP_GUO)
            {
                ShowCardOperation(CardOperationView.Operation.Qiang);
                ShowCardOperation(CardOperationView.Operation.BuQiang);
            }
            else
            {
                // 出牌阶段的操作
                foreach (var op in ops)
                {
                    switch (op)
                    {
                        case PokerGameOP.Poker_OP_CHE:
                            ShowCardOperation(CardOperationView.Operation.Che);
                            break;
                        case PokerGameOP.Poker_OP_CHU:
                            ShowCardOperation(CardOperationView.Operation.TiShi);
                            ShowCardOperation(CardOperationView.Operation.ChuPai);
                            break;
                        case PokerGameOP.Poker_OP_YTL:
                            ShowCardOperation(CardOperationView.Operation.YiTiaoLong);
                            break;
                        case PokerGameOP.Poker_OP_GUO:
                            ShowCardOperation(CardOperationView.Operation.Guo);
                            break;
                        case PokerGameOP.Poker_OP_SHOUT:// 叫朋友的意思
                            ShowCardOperation(CardOperationView.Operation.JiaoPai);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // 隐藏扑克牌操作
        public void HideCardOperation()
        {
            ViewManager.Open<CardOperationView>().HideButtons();
        }

        // 玩家掉线，重连回来提示
        public void ShowPlayerOffline(uint id, bool offline)
        {
            var player = PlayerMgr.Inst.GetPlayer(id);
            if (player != null)
            {
                string content = "";
                if (offline)
                {
                    content = string.Format("玩家{0}掉线了", player.name);
                }
                else
                {
                    content = string.Format("玩家{0}回来了", player.name);
                }
                ViewManager.Open<PopupWindow8View>().Init(content, null);
                ViewManager.Open<PlayerGamingView>().playerGaming[(int)PlayerMgr.Inst.GetSeat(id)].avatar.color = offline ? Color.gray : Color.white;
            }

        }

        // 显示庄叫的朋友那张牌
        public void ShowBidFriend(Card card)
        {
            // 有一方玩家抢牌
            if (card.Value == 0)
            {
                return;
            }

            ViewManager.Open<BidFriendView>().Show(card);
        }

        // 扯牌
        public void Che(uint id)
        {
            if (id <= 0)
            {
                return;
            }

            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ShowEffect(OperationEffectView.EffectType.Che, seat);
        }

        // 出牌
        public void PlayCard(uint id, List<Card> outCards, bool playWithEffects = true, bool playCardVoice = true)
        {
            UpdateRemainCardCount(id, outCards.Count);
            ShowOutCards(id, outCards);

            // 自己打出的牌已经得到服务器确认
            if (PlayerMgr.Inst.IsSelf(id))
            {
                var handCardView = ViewManager.Open<HandCardView>();
                handCardView.RemoveSelectedCards(outCards);
                handCardView.Sort();
            }

            // 移除本地记录的手牌
            foreach (var outCard in outCards)
            {
                handCards.Remove(outCard);
            }

            if (playWithEffects)
            {
                PlayCardEffect(id, outCards);
                if (playCardVoice)
                {
                    PlayCardVoice(id, outCards);
                }
            }
        }

        // 显示打出的牌类型特效文字(如连队，炸弹，一条龙，抢)
        public void PlayCardEffect(uint id, List<Card> outCards)
        {
            CardOperation co = new CardOperation();
            var type = co.GetType(outCards);
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            switch (type)
            {
                case CardOperation.CardType.None:
                    break;
                case CardOperation.CardType.Single:
                    break;
                case CardOperation.CardType.Pair:
                    break;
                case CardOperation.CardType.Straight:
                    ShowEffect(OperationEffectView.EffectType.LianZi, seat);
                    break;
                case CardOperation.CardType.BombThree:
                case CardOperation.CardType.BombSameSuit:
                case CardOperation.CardType.BombStraightPair:
                    ShowEffect(OperationEffectView.EffectType.ZhaDan, seat);
                    break;
                case CardOperation.CardType.BombFour:
                    ShowEffect(OperationEffectView.EffectType.ZhaDanHuo, seat);
                    break;
                default:
                    break;
            }
        }

        // 播放出牌语音，如3点，456，三个2
        public void PlayCardVoice(uint id, List<Card> outCards)
        {
            CardOperation op = new CardOperation();
            var cardType = op.GetType(outCards);
            if (cardType == CardOperation.CardType.None)
            {
                Log.D("出牌语音", "未知牌类型，无法播放语音");
                return;
            }

            var sex = PlayerMgr.Inst.GetPlayer(id).sex;
            var seat = PlayerMgr.Inst.GetSeat(id);
            string sexStr = sex == 1 ? "man" : "woman";
            string path = "";
            switch (cardType)
            {
                case CardOperation.CardType.Single:
                case CardOperation.CardType.Pair:
                case CardOperation.CardType.BombThree:
                case CardOperation.CardType.BombFour:
                    path = cardType.ToString() + "/" + outCards[0].NormalizedValue;
                    break;
                case CardOperation.CardType.Straight:
                case CardOperation.CardType.BombSameSuit:
                    var count = outCards.Count;
                    path = "Straight/" + count + "/" + outCards[0].NormalizedValue;
                    break;
                case CardOperation.CardType.BombStraightPair:
                    path = cardType.ToString() + "/" + UnityEngine.Random.Range(1, 3);
                    break;
            }

            string voicePath = string.Format("PlayCardVoice/{0}/{1}", sexStr, path);
            SoundMag.GetINS.PlayCardPoker((int)seat, voicePath);
        }

        // 播放牌操作语音，如抢，扯，过，一条龙
        public void PlayCardOpVoice(uint id, PokerGameOP op)
        {
            string path = "";
            bool playVoice = true;

            switch (op)
            {
                case PokerGameOP.Poker_OP_QP:
                case PokerGameOP.Poker_OP_CHE:
                case PokerGameOP.Poker_OP_YTL:
                case PokerGameOP.Poker_OP_GUO:
                    path = 1 + "/" + op.ToString();
                    break;
                default:
                    playVoice = false;
                    break;
            }

            if (playVoice)
            {
                var sex = PlayerMgr.Inst.GetPlayer(id).sex;
                var seat = PlayerMgr.Inst.GetSeat(id);
                string sexStr = sex == 1 ? "man" : "woman";
                string voicePath = string.Format("PlayCardVoice/{0}/Op/{1}", sexStr, path);
                SoundMag.GetINS.PlayCardPoker((int)seat, voicePath);
            }
        }

        // 一条龙操作
        public void YiTiaoLong(uint id)
        {
            turnOverDelay.delayTime += 2;

            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ShowEffect(OperationEffectView.EffectType.YiTiaoLong, seat);
        }

        // 收牌
        public void PickCard(uint id, List<Card> cards)
        {
            // 表明为抢牌
            if (cards.Count == 0)
            {
                ViewManager.Open<OutCardView>().Clear();
            }
            else
            {
                Seat seat = PlayerMgr.Inst.GetSeat(id);
                ViewManager.Open<OutCardView>().PickCard(seat);
            }
            if (PlayerMgr.Inst.IsSelf(id))
            {
                pickedCards.AddRange(cards);
            }

            ClearRoundData();
        }

        // 记录最后一个玩家及其出的牌
        public void AddLastPlayerOutCards(uint id, List<Card> cards)
        {
            LastOutCardId = id;
            lastOutCards.Clear();
            lastOutCards.AddRange(cards);
        }

        // 显示某个玩家上中下游
        public void ShowRank(uint id, uint endSort)
        {
            RankType rankType = Util.ConvertToRankType(endSort);
            if (rankType == RankType.None)
            {
                return;
            }
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<RankView>().Show(seat, rankType);
        }

        // 显示朋友
        public void ShowFriend(uint id)
        {
            if (id <= 0)
            {
                return;
            }

            FriendId = id;
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<PlayerGamingView>().SetFriendIcon(seat, true);

            ShowEffect(OperationEffectView.EffectType.PengYouChuXian, seat);
        }

        // 显示哪个玩家抢牌
        public void ShowQiang(uint id)
        {
            if (id <= 0)
            {
                return;
            }

            this.QiangId = id;
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<PlayerGamingView>().SetQiangIcon(seat, true);

            ShowEffect(OperationEffectView.EffectType.Qiang, seat);
        }

        // 显示庄家
        public void ShowBanker(uint id)
        {
            this.BankerId = id;
            if (gameStartDelay.isTurnFirstPlay)
            {
                return;
            }
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<PlayerGamingView>().SetBankerIcon(seat, true);
        }

        // 单局游戏结束，进入单局结算
        public void TurnOver(PokerGameOver rsp)
        {
            Log.D("游戏结束", "单局游戏结束");

            if (turnOverDelay.isTurnOverDelay)
            {
                // 第4家玩家显示牌走特别的逻辑
                if (!turnOverDelay.isFourthPlayerShowCard)
                {
                    RestPlayerShowCards(rsp.players);
                }

                turnOverDelay.rsp = rsp;
                turnOverDelay.isTurnOverDelay = false;
                StartCoroutine(TurnOverAllDelay());
                return;
            }

            PlayedRound++;

            ViewManager.Destroy<HandCardView>();
            ViewManager.Destroy<PickCardDetailView>();
            ViewManager.Destroy<OutCardView>();
            ViewManager.Destroy<CardOperationView>();
            ViewManager.Destroy<RankView>();
            ViewManager.Destroy<BidFriendView>();

            ViewManager.Open<PlayerGamingView>().Clear();
            Settlement data = GetSettlementData(rsp);
            ViewManager.Open<RoundSettlementView>().Init(data);
            ShowTotalScore(data.players);

            ClearTurnData();
        }

        // 显示总分数
        void ShowTotalScore(List<PlayerSettlement> settlements)
        {
            var view = ViewManager.Open<PlayerGamingView>();
            foreach (var s in settlements)
            {
                Seat seat = PlayerMgr.Inst.GetSeat(s.id);
                view.SetTotalScore(seat, s.totalScore, true);
            }
        }

        // 单局游戏结束延时操作
        IEnumerator TurnOverAllDelay()
        {
            yield return new WaitForSeconds(turnOverDelay.delayTime);
            // 如果因为网络等原因，还未收到本轮结束的消息数据，那么后面就不能执行，一直等待
            while (turnOverDelay.rsp == null)
            {
                yield return null;
            }
            TurnOver(turnOverDelay.rsp);
        }

        // 剩余的所有玩家亮牌(如果模式为“叫朋友”，收到的第4家命令为Poker_OP_SHOW，不走这里的亮牌)
        void RestPlayerShowCards(List<EachPokerPlayer> players)
        {
            Log.D("游戏结束", "摊开剩余玩家手牌");

            foreach (var player in players)
            {
                PlayCard(player.charId, player.restCards.ToCards(), false);
            }
        }

        // 进行下一局游戏
        public void NextTurn(Settlement settlement)
        {
            if (IsGameOver)
            {
                ViewManager.Open<TotalSettlementView>().Init(settlement);
            }
            else if (IsMidwayDisbandRoom)
            {
                ViewManager.Open<TotalSettlementView>().Init(settlement);
            }
            else
            {
                CurrentRound++;
                LoadBasicView();
                RefreshTopInfo();
                ViewManager.Open<PlayerGamingView>().AutoReady();
            }
        }

        // 整局游戏结束，先显示单局结算，然后再进入总结算
        public void GameOver()
        {
            Log.D("游戏结束", "整局游戏结束");
            IsGameOver = true;
        }

        // 发起/接收投票解散房间
        public void VoteDisbandRoom(uint id, bool agree)
        {
            ViewManager.Open<PopupWindow5View>().UpdatePlayer(id, agree);
        }

        // 投票解散房间结果
        public void VoteDisbandRoomResult(bool disband)
        {
            Log.D("解散房间结果 ", disband.ToString());
            if (disband)
            {
                // 如果为未开始游戏，房主退出房间时，那么这个也会显示，需要销毁，不销毁可能会导致ui的重合(现在刚好覆盖看不出来而已)
                ViewManager.Destroy<PopupWindow5View>();
                // 玩过至少一局时，中途解散房间，需要先显示结算，当点击“继续”时，退出房间
                if (PlayedRound > 0)
                {
                    IsMidwayDisbandRoom = true;
                    ViewManager.Open<PopupWindow3View>().Init("您所在的房间已被解散！", 3, null);
                }
                else
                {
                    ViewManager.Open<PopupWindow3View>().Init("您所在的房间已被解散！", 3, () =>
                    {
                        ExitRoom();
                    });
                }
            }
            else
            {
                ViewManager.Destroy<PopupWindow5View>();
                ViewManager.Open<PopupWindow9View>().Init("不同意解散房间，请继续游戏！", 2);
            }
        }

        // 显示当前正在操作的玩家
        public void ShowPlayerOperating(uint id, List<PokerGameOP> ops)
        {
            if (gameStartDelay.isTurnFirstPlay)
            {
                gameStartDelay.playingId = id;
                return;
            }

            uint playingId = 0;
            // 当某个玩家扯牌时，不显示ta正在操作，而显示上一出牌玩家的下一家，但下一家并不能真正操作。真正操作的还是扯牌玩家
            if (ops.Contains(PokerGameOP.Poker_OP_CHE))
            {
                playingId = PlayerMgr.Inst.GetNextPlayer(LastOperatedId).id;
            }
            else
            {
                LastOperatedId = id;
                playingId = id;
            }
            Seat seat = PlayerMgr.Inst.GetSeat(playingId);
            ViewManager.Open<PlayerGamingView>().SetPlayerPlaying(seat);
        }

        // 显示第4的一个玩家的剩余手牌
        public void FourthPlayerShowCards(uint id, List<Card> cards)
        {
            Log.D("游戏结束", "显示第4的一个玩家的剩余手牌");

            StartCoroutine(DelayFourthPlayerShowCards(id, cards));
        }

        IEnumerator DelayFourthPlayerShowCards(uint id, List<Card> cards)
        {
            float pickCardPauseTime = 2;
            var moveTime = ViewManager.Get<OutCardView>().moveTime;
            turnOverDelay.delayTime += moveTime + pickCardPauseTime;
            turnOverDelay.isFourthPlayerShowCard = true;

            PlayCard(id, cards, false);

            yield return new WaitForSeconds(pickCardPauseTime);

            // 比较两玩家牌大小，进行收牌
            var datas = new List<Card>(cards.Count + lastOutCards.Count);
            datas.AddRange(cards);
            datas.AddRange(lastOutCards);

            CardOperation co = new CardOperation();

            // 如果第4玩家可以扯牌，那么第4玩家牌大
            if (lastOutCards.Count == 1)
            {
                var cheCards = co.ChePai(cards,lastOutCards[0]);
                if (cheCards.Count >= 2)
                {
                    PickCard(id,datas);
                    yield break;
                }
            }

            var prompts = co.Prompt(cards, lastOutCards);
            // 第3玩家牌大
            if (prompts.Count == 0)
            {
                PickCard(LastOutCardId, datas);
            }
            else
            {
                PickCard(id, datas);
            }
        }

        // 战绩回放
        public void Playback(QPRoundRecord record, int maxCount)
        {
            ViewManager.Open<PlaybackView>().Init(record, maxCount);
        }

        // 更新距离
        public void UpdateDistance(uint id, Vector2 latlng)
        {
            if (PlayerMgr.Inst.IsSelf(id))
            {
                return;
            }

            var selfLatlng = PlayerMgr.Inst.Self.latlng;

            // 自己未开启定位，那么不能够定位别人
            if (selfLatlng == Vector2.zero)
            {
                Log.D("Gps", "自己未开启定位，不能够定位别人");
                latlng = Vector2.zero;
            }

            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<PlayerGamingView>().UpdateDistance(seat, latlng);

            GpsManager.Inst.GetCity(latlng, c =>
            {
                var player = PlayerMgr.Inst.GetPlayer(id);
                if (player != null)
                {
                    player.cityName = c;
                }
            });
        }

        #endregion

        #region 内部方法

        void InitGame(Dictionary<uint, int> remainCardCount)
        {
            ShowOwnerIcon();

            var view = ViewManager.Open<PlayerGamingView>();
            view.GameStarted();
            foreach (var player in PlayerMgr.Inst.allPlayers)
            {
                Seat seat = PlayerMgr.Inst.GetSeat(player.id);
                view.SetRemainCard(seat, remainCardCount[player.id]);
            }
            ViewManager.Open<PickCardDetailView>();
        }

        // 刷新顶部房间信息
        void RefreshTopInfo()
        {
            if (room == null)
            {
                return;
            }

            var topView = ViewManager.Open<TopStatusBarView>();
            uint roomId = room.roomId;
            topView.SetBasicInfo(roomId, CurrentRound, MaxRound);
            topView.SetRuleInfo(room.roomRuleInfo);
        }

        // 设置房主icon
        public void ShowOwnerIcon()
        {
            Seat seat = PlayerMgr.Inst.GetSeat(OwnerId);
            ViewManager.Open<PlayerGamingView>().SetOwnerIcon(seat, true);
        }

        // 显示扑克牌操作
        void ShowCardOperation(CardOperationView.Operation op)
        {
            ViewManager.Open<CardOperationView>().ShowButton(op);
        }

        // 更新玩家剩余手牌的数量
        public void UpdateRemainCardCount(uint id, int delta)
        {
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            ViewManager.Open<PlayerGamingView>().SetDeltaRemainCard(seat, delta);
        }

        // 显示打出去的牌
        public void ShowOutCards(uint id, List<Card> outCards)
        {
            var view = ViewManager.Open<OutCardView>();
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            outCards.Sort(new CardAscSort());
            view.Show(seat, outCards);

            if (FriendId == id)
            {
                view.SetIcon(seat, 2);
            }
            if (BankerId > 0)
            {
                Seat bankerSeat = PlayerMgr.Inst.GetSeat(BankerId);
                view.SetIcon(bankerSeat, 1);
            }
        }

        // 显示所有玩家的上中下游
        void ShowRank()
        {
            foreach (var charState in room.charStates)
            {
                ShowRank(charState.charId, charState.endSort);
            }
        }

        // 自己待进行的操作，记录最后一位玩家出的牌，记录最后一位玩家是否为扯牌操作
        void ShowSelfOperation()
        {
            var roomCache = room.roomCache.charList.Find(x => PlayerMgr.Inst.IsSelf(x.charId));
            if (roomCache != null && roomCache.opInfo != null && roomCache.opInfo.Count > 0)
            {
                List<PokerGameOP> ops = new List<PokerGameOP>();
                List<Card> outCards = new List<Card>();
                foreach (var opInfo in roomCache.opInfo)
                {
                    var op = (PokerGameOP)opInfo.op;
                    ops.Add(op);
                    // 出和扯的操作时，才会有最后一位玩家出的牌数据、扯操作
                    if (op == PokerGameOP.Poker_OP_CHU || op == PokerGameOP.Poker_OP_CHE)
                    {
                        outCards = opInfo.cards.ToCards();
                        IsLastPlayerChe = opInfo.lastOp == PokerGameOP.Poker_OP_CHE;
                    }
                }
                ShowCardOperation(ops);
                AddLastPlayerOutCards(0, outCards);
            }
        }

        // 显示所有玩家的定位信息
        void ShowGps(List<CharacterState> states)
        {
            foreach (var charState in states)
            {
                UpdateDistance(charState.charId, new Vector2(charState.latitude, charState.longitude));
            }
        }

        // 获取单局结算，总结算数据
        Settlement GetSettlementData(PokerGameOver rsp)
        {
            Settlement data = new Settlement();
            if (rsp.isPingJu == 1)
            {
                data.resultType = ResultType.Dogfall;
            }
            data.currentRound = PlayedRound;
            data.roomId = room.roomId;
            List<PlayerSettlement> settlements = new List<PlayerSettlement>();
            Dictionary<uint, RankType> totalRank = new Dictionary<uint, RankType>();
            totalRank = CalcTotalRank(rsp);
            var players = rsp.players.Count == 0 ? rsp.befores[rsp.befores.Count - 1].players : rsp.players;
            players.ForEach(p =>
            {
                if (PlayerMgr.Inst.IsSelf(p.charId))
                {
                    if (p.changeGold > 0)
                    {
                        data.resultType = ResultType.Victory;
                    }
                    else if (p.changeGold < 0)
                    {
                        data.resultType = ResultType.Fail;
                    }
                }
                var playerInfo = PlayerMgr.Inst.GetPlayer(p.charId);
                PlayerSettlement ps = new PlayerSettlement
                {
                    id = p.charId,
                    rank = Util.ConvertToRankType(p.endSord),
                    userName = playerInfo.name,
                    isOwner = p.charId == OwnerId,
                    isBanker = p.charId == BankerId,
                    isFriend = rsp.friendId == p.charId,
                    isQiang = p.isQiang == 1,
                    isYiTiaoLong = p.isYTL == 1,
                    isWinDouble = p.isDouble == 1,
                    totalScore = p.restGold,
                    roomCost = (int)rsp.costDiamond,
                    pickedCardCount = p.gotCardNum,
                    fan = p.multiply,
                    score = p.changeGold,
                    fanCards = new List<Card>(p.mulCards.ToCards()),
                    totalRank = totalRank.ContainsKey(p.charId) ? totalRank[p.charId] : RankType.None
                };
                settlements.Add(ps);
            });
            data.players = settlements;

            return data;
        }

        // 计算总的排名
        Dictionary<uint, RankType> CalcTotalRank(PokerGameOver rsp)
        {
            List<EachPokerPlayer> players = null;
            // 中途解散房间
            if (rsp.players == null || rsp.players.Count == 0)
            {
                players = rsp.befores[rsp.befores.Count - 1].players;
            }
            else
            {
                players = rsp.players;// 当前局玩家结算分数
            }

            Dictionary<uint, int> history = new Dictionary<uint, int>(4);
            foreach (var p in players)
            {
                history[p.charId] = p.restGold;
            }

            history = (from item in history
                       orderby item.Value descending
                       select item).ToDictionary(item => item.Key, item => item.Value);

            Dictionary<uint, RankType> ranks = new Dictionary<uint, RankType>(4);
            int rankIndex = 0;
            foreach (var id in history.Keys)
            {
                if (rankIndex >= 3)
                {
                    ranks[id] = RankType.None;
                }
                else
                {
                    var rank = (RankType)rankIndex;
                    ranks[id] = rank;
                    rankIndex++;
                }
            }

            return ranks;
        }

        // 检查相同ip环境
        void CheckIpEnvironment()
        {
            if (room == null)
            {
                return;
            }

            // 只有在第一次进入一个房间时才会显示ip检测。如果已经玩过一局后重连也不会显示
            if (room.inGame != 1 || CurrentRound > 1)
            {
                return;
            }

            List<string> names = new List<string>();
            uint selfId = PlayerMgr.Inst.Self.id;
            var players = PlayerMgr.Inst.allPlayers;

            var queryPlayers =
                 from player in players
                 where player != null && player.id != selfId
                 group player by player.ip into newGroup
                 orderby newGroup.Key
                 select newGroup;
            foreach (var nameGroup in queryPlayers)
            {
                if (nameGroup.Count() >= 2)
                {
                    foreach (var player in nameGroup)
                    {
                        names.Add(player.name);
                    }
                }
            }

            if (names.Count >= 2)
            {
                string sameIpNameStr = "";
                if (names.Count == 2)
                {
                    sameIpNameStr = names[0] + "和" + names[1];
                }
                else
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        if (i == names.Count - 1)
                        {
                            sameIpNameStr = sameIpNameStr.TrimEnd('、');
                            sameIpNameStr += "和" + names[i];
                        }
                        else
                        {
                            sameIpNameStr += names[i] + "、";
                        }
                    }
                }
                string content = string.Format("\u3000\u3000系统检测到{0}在同一IP环境中进行游戏！继续游戏将会有{1}打一的风险哦！", sameIpNameStr, names.Count);
                ViewManager.Open<PopupWindow8View>().Init(content, null);
            }
        }

        // 显示操作的特效
        public void ShowEffect(OperationEffectView.EffectType type, Seat seat)
        {
            if (!IsInitCompleted)
            {
                return;
            }

            ViewManager.Open<OperationEffectView>().Show(type, seat);
        }

        #endregion
    }
}