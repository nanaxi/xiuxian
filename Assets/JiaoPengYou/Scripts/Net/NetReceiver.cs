using ProtoBuf;
using System.Text;

namespace Lang
{
    public class NetReceiver : Singleton<NetReceiver>
    {
        // 断开网络
        public void OnNetClosed()
        {
            Log.D("网络", "网络断开了");
            if (GameMgr.Inst.IsInRoom)
            {
                GameMgr.Inst.WaitReconnect();
            }
        }

        // 帐号登录成功返回用户信息
        public void OnAccountLoginSuccess(AccountLoginRsp rsp)
        {
            PlayerInfo p = new PlayerInfo();
            p.id = rsp.charId;
            p.inGame = GameType.GT_Poker;
            p.crRoomId = rsp.crRoomId;
            p.atRoomId = rsp.atRoomId;
            p.ip = rsp.ip;
            p.diamond = rsp.diamond;
            p.name = Util.ConvertUserName(rsp.userName);
            PlayerMgr.Inst.Add(p, Seat.Down);
        }

        public void OnNotifyMessage(NotifyServerMessage rsp)
        {
        }

        // 创建房间结果
        public void OnCreateRoom(CreateRoomRsp rsp)
        {
            switch (rsp.result)
            {
                case CreateRoomRsp.Result.SUCC:
                    PlayerMgr.Inst.Self.id = rsp.charId;
                    PlayerMgr.Inst.Self.crRoomId = rsp.roomId;
                    PlayerMgr.Inst.Self.inGame = rsp.gameType;

                    NetSender.Inst.EnterRoom();
                    break;
                case CreateRoomRsp.Result.FAIL:
                    ViewManager.Open<PopupWindow9View>().Init("创建房间失败！", 3);
                    break;
                case CreateRoomRsp.Result.HAS:
                    ViewManager.Open<PopupWindow9View>().Init("该账号已经存在在该房间内，创建房间失败！", 3);
                    break;
                case CreateRoomRsp.Result.NOT_ENOUGH_MONEY:
                    ViewManager.Open<PopupWindow9View>().Init("房卡不足,创建房间失败！", 3);
                    break;
                default:
                    break;
            }
        }

        // 当自己进入房间
        public void OnEnterRoom(EnterRoomRsp rsp)
        {
            switch (rsp.result)
            {
                case EnterRoomRsp.Result.SUCC:
                    PlayerMgr.Inst.Self.inGame = rsp.gameType;
                    PlayerMgr.Inst.Self.atRoomId = rsp.roomId;
                    InitRoomPlayerInfo(rsp.pokerRoom);
                    GameMgr.Inst.EnterRoom(rsp);
                    break;
                case EnterRoomRsp.Result.FAIL:
                    ViewManager.Open<PopupWindow9View>().Init("房间不存在,进入房间失败！", 3);
                    break;
                case EnterRoomRsp.Result.FULL:
                    if (GameManager.GM.DS.Notic == null)
                    {
                       
                        GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("房间人数已满!");
                    }
                    break;
                case EnterRoomRsp.Result.HASIN:
                    ViewManager.Open<PopupWindow9View>().Init("该账号已在房间中,进入房间失败！", 3);
                    break;
            }
        }

        // 当自己退出房间
        public void OnExitRoom(ExitRoomRsp rsp)
        {
            if (rsp.result == ExitRoomRsp.Result.SUCC)
            {
                GameMgr.Inst.ExitRoom();
            }
        }

        // 当其他玩家进入或者退出房间时，刷新房间里面的消息
        public void OnRoomInfo(SyncRoomInfo rsp)
        {
            CharacterInfo current = rsp.charInfos.Find((c) => c.charId == rsp.triggerCharId);
            if (current == null)
            {
                Log.D("玩家退出房间", string.Format("{0},{1}", rsp.triggerCharId, rsp.roomId));
                // 玩家退出，异常掉线时，删掉玩家信息
                Seat seat = PlayerMgr.Inst.GetSeat(rsp.triggerCharId);
                GameMgr.Inst.LeaveRoom(seat);
                PlayerMgr.Inst.Remove(rsp.triggerCharId);
            }
            else
            {
                // 如果玩家已经进入过了，则不分配位置
                if (!PlayerMgr.Inst.Contains(current.charId))
                {
                    Log.D("玩家加入房间", string.Format("{0},{1},{2}", PlayerMgr.Inst.Self.id, current.charId, rsp.roomId));
                    // 新玩家分配到空位置
                    PlayerInfo p = Util.ConvertToPlayer(current);
                    PlayerMgr.Inst.AddToEmptySeat(p);
                    GameMgr.Inst.PlayerJoin(current.charId);
                }
            }
        }

        // 发放手牌
        public void OnHandCards(PokerHandCardInfo rsp)
        {
            Log.D("收到手牌", "游戏开始了");
            // 服务器发牌下来了，表明游戏已经开始了
            GameMgr.Inst.GameStarted(rsp.cards.ToCards());
        }

        // 通知当前玩家待进行的操作
        public void OnAskGameOP(AskPokerGameOP rsp)
        {
#if UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            foreach (var canOp in rsp.canOps)
            {
                sb.Append(canOp + ",");
            }
            Log.D("通知玩家操作", string.Format("{0}，待进行的操作：{1}", rsp.doer, sb.ToString()));
#endif

            GameMgr.Inst.ShowPlayerOperating(rsp.doer, rsp.canOps);
            GameMgr.Inst.IsLastPlayerChe = rsp.lastOp == PokerGameOP.Poker_OP_CHE;

            // 属于自己的操作，否则为其他位置的玩家操作
            if (rsp.doer == PlayerMgr.Inst.Self.id)
            {
                GameMgr.Inst.ShowCardOperation(rsp.canOps);
                GameMgr.Inst.AddLastPlayerOutCards(rsp.doer, rsp.cards.ToCards());
            }
        }

        // 服务器广播给所有玩家的操作(玩家已经操作过的操作)
        public void OnNotifyGameOP(NotifyPokerGameOP rsp)
        {
#if UNITY_EDITOR
            Log.D("服务器广播", string.Format("{0}，进行{1}操作", rsp.doer, rsp.op));
#endif

            switch (rsp.op)
            {
                case PokerGameOP.Poker_OP_QP:// 抢牌
                    GameMgr.Inst.ShowQiang(rsp.doer);
                    break;
                case PokerGameOP.Poker_OP_CHE:// 扯牌
                case PokerGameOP.Poker_OP_CHU:// 出牌
                    // 朋友牌出现了
                    if (rsp.friends != null && rsp.friends.Count > 0)
                    {
                        // 朋友一定是跟庄家一头的
                        var friend = rsp.friends.Find(f => f.FriendId == GameMgr.Inst.BankerId);
                        GameMgr.Inst.ShowFriend(friend.charId);
                    }
                    var outCards = rsp.cards.ToCards();
                    // 如果自己为扯牌，那么不播放具体牌的语音，而是通过PlayCardOpVoice来播放扯的语音
                    bool isChe = rsp.op == PokerGameOP.Poker_OP_CHE;
                    GameMgr.Inst.PlayCard(rsp.doer, outCards, !isChe);
                    GameMgr.Inst.AddLastPlayerOutCards(rsp.doer, outCards);

                    if (rsp.op == PokerGameOP.Poker_OP_CHE)
                    {
                        GameMgr.Inst.Che(rsp.doer);
                    }
                    break;
                case PokerGameOP.Poker_OP_YTL:
                    GameMgr.Inst.YiTiaoLong(rsp.doer);
                    break;
                case PokerGameOP.Poker_OP_GUO:// 过
                    if (PlayerMgr.Inst.IsSelf(rsp.doer))
                    {
                        GameMgr.Inst.HideCardOperation();
                    }
                    break;
                case PokerGameOP.Poker_OP_PREP:// 准备
                    GameMgr.Inst.PlayerReady(rsp.doer);
                    break;
                case PokerGameOP.Poker_OP_EXITROOM:
                    break;
                case PokerGameOP.Poker_OP_ROUND_OVER:// 总游戏结束，进入总结算
                    GameMgr.Inst.GameOver();
                    break;
                case PokerGameOP.Poker_OP_VOTE_JSROOM:// 发起/接收投票解散房间，接收到的第一个玩家即为投票发起人
                    GameMgr.Inst.VoteDisbandRoom(rsp.doer, rsp.param == 2);
                    break;
                case PokerGameOP.Poker_OP_VOTE_RESULT:
                    GameMgr.Inst.VoteDisbandRoomResult(rsp.param == 2);
                    break;
                case PokerGameOP.Poker_OP_ON_LINE:
                    GameMgr.Inst.ShowPlayerOffline(rsp.doer, rsp.param == 0);
                    break;
                case PokerGameOP.Poker_OP_SHOUT:// 叫朋友
                    GameMgr.Inst.ShowBidFriend(rsp.cards[0].ToCard());
                    break;
                case PokerGameOP.Poker_OP_PICK:// 收牌
                    GameMgr.Inst.PickCard(rsp.doer, rsp.cards.ToCards());
                    break;
                case PokerGameOP.Poker_OP_CHU_OVER:// 出牌完成
                    GameMgr.Inst.ShowRank(rsp.doer, rsp.param);
                    break;
                case PokerGameOP.Poker_OP_ZJ:// 庄家
                    GameMgr.Inst.ShowBanker(rsp.param);
                    break;
                case PokerGameOP.Poker_OP_SHOW:// 最后一个玩家的剩余手牌
                    GameMgr.Inst.FourthPlayerShowCards(rsp.doer, rsp.cards.ToCards());
                    break;
                case PokerGameOP.Poker_OP_COORD:// 定位
                    var player = PlayerMgr.Inst.GetPlayer(rsp.doer);
                    if (player != null)
                    {
                        player.latlng = new UnityEngine.Vector2(rsp.latitude, rsp.longitude);
                        GameMgr.Inst.UpdateDistance(player.id, player.latlng);
                    }
                    break;
                case PokerGameOP.Poker_OP_RESULT:
                    break;
                default:
                    break;
            }

            GameMgr.Inst.PlayCardOpVoice(rsp.doer, rsp.op);
        }

        // 单局游戏结束，进入单局结算
        public void OnGameOver(PokerGameOver rsp)
        {
            GameMgr.Inst.TurnOver(rsp);
        }

        // 初始化已经加入到房间内的玩家信息
        void InitRoomPlayerInfo(PokerRoomInfo room)
        {
            var selfId = PlayerMgr.Inst.Self.id;
            var selfServerPos = room.charStates.Find(c => c.charId == selfId).position;
            for (int i = 0; i < room.charStates.Count; i++)
            {
                var cs = room.charStates[i];
                // 登录的时候(OnAccountLoginSuccess)，已经设置了部分信息。这里只设置未设置过的信息
                if (PlayerMgr.Inst.IsSelf(cs.charId))
                {
                    PlayerMgr.Inst.Self.avatarUrl = room.charInfos[i].portrait;
                    PlayerMgr.Inst.Self.sex = room.charInfos[i].sex;
                    continue;
                }

                var ci = room.charInfos[i];
                PlayerInfo p = Util.ConvertToPlayer(ci);
                p.latlng = new UnityEngine.Vector2(cs.latitude, cs.longitude);
                PlayerMgr.Inst.Add(p, (int)cs.position, (int)selfServerPos);
            }
        }
    }
}