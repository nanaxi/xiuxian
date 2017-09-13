using ProtoBuf;
using System.Collections.Generic;

namespace Lang
{
    public class NetSender : Singleton<NetSender>
    {
        /// <summary>
        /// 选择某个游戏，比如选择扑克
        /// </summary>
        public void EnterGame(GameType gameType)
        {
            EnterGameReq pack = new EnterGameReq();
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.enterGame = gameType;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_ENTER_GAME;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 创建某个游戏的房间
        /// </summary>
        public void CreateRoom(PokerRule rule)
        {
            PokerRoomRuleInfo room = new PokerRoomRuleInfo();
            room.gameRule = PokerRoomRule.POKER_ROOM_RULE;
            room.pokerRule = rule;

            CreateRoomReq pack = new CreateRoomReq();
            pack.gameType = GameType.GT_Poker;
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.account = "";
            pack.pokerRoom = room;
            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_CREATE_ROOM;

            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 进入某个游戏的房间
        /// </summary>
        public void EnterRoom()
        {
            EnterRoomReq pack = new EnterRoomReq();
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.gameType = PlayerMgr.Inst.Self.inGame;
            pack.roomId = PlayerMgr.Inst.Self.crRoomId;
            pack.account = "";
            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_ENTER_ROOM;

            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 申请加入房间
        /// </summary>
        public void AppJoin(uint roomId)
        {
            PlayerMgr.Inst.Self.crRoomId = roomId;
            EnterRoom();
        }

        /// <summary>
        /// 客户端准备
        /// </summary>
        public void Ready()
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_PREP;
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 退出游戏的房间
        /// </summary>
        public void ExitRoom()
        {
            ExitRoomReq pack = new ExitRoomReq();
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.roomId = PlayerMgr.Inst.Self.atRoomId;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_EXIT_ROOM;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 一条龙
        /// </summary>
        public void YiTiaoLong()
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_YTL;
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 抢牌
        /// </summary>
        public void QiangPai()
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_QP;
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 不抢牌
        /// </summary>
        public void BuQiangPai()
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_GUO;
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 叫朋友(叫牌)
        /// </summary>
        public void CallFriend(uint card)
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_SHOUT;
            pack.cardIds.Add(card);
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 出牌
        /// </summary>
        public void PlayCard(List<uint> cards)
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_CHU;
            pack.cardIds.AddRange(cards);
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 扯牌
        /// </summary>
        public void ChePai(List<uint> cards)
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_CHE;
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.cardIds.AddRange(cards);

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// “过”
        /// </summary>
        public void Pass()
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_GUO;
            pack.charId = PlayerMgr.Inst.Self.id;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 发起解散房间的投票
        /// </summary>
        public void VoteDisbandRoom(bool agree)
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_VOTE_JSROOM;
            pack.charId = PlayerMgr.Inst.Self.id;
            if (agree)
                pack.param = 2;
            else
                pack.param = 1;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }

        /// <summary>
        /// 上传定位信息
        /// </summary>
        public void UploadGps(UnityEngine.Vector2 data)
        {
            PokerGameOpReq pack = new PokerGameOpReq();
            pack.op = PokerGameOP.Poker_OP_COORD;
            pack.charId = PlayerMgr.Inst.Self.id;
            pack.latitude = data.x;
            pack.longitude = data.y;

            ushort command = (ushort)CLIToLGIProtocol.CLI_TO_LGI_POKER_GAME_OP;
            GameNetWork.Inst().SendDataToLoginServer(command, pack);
        }
    }
}