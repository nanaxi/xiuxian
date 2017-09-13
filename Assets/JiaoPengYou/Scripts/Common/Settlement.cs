using System.Collections.Generic;

namespace Lang
{
    public class Settlement
    {
        public ResultType resultType;
        public uint currentRound;
        public uint roomId;
        public List<PlayerSettlement> players;
    }

    public class PlayerSettlement
    {
        public uint id;
        public RankType rank;
        public string userName;
        public bool isOwner;
        public bool isBanker;
        public bool isFriend;
        public bool isQiang;
        public bool isYiTiaoLong;
        public bool isWinDouble;
        public int totalScore;
        public int roomCost;
        public uint pickedCardCount;
        // 番数
        public uint fan;
        // 单局结算中，当前局得分情况
        public int score;
        // 加番详情
        public List<Card> fanCards;
        // 总结算中的总排名，冠军，季军，亚军三种
        public RankType totalRank;
    }
}