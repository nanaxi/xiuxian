using ProtoBuf;
using System.Collections.Generic;

namespace Lang
{
    public class GameStartDelay
    {
        // 正在出牌的玩家id
        public uint playingId;

        // 当第一次接到手牌的时候为true，当播放完游戏开始动画之后为false
        public bool isTurnFirstPlay;

        // 当前玩家进行的待进行的操作
        public readonly List<PokerGameOP> selfCardOps = new List<PokerGameOP>();

        public void Clear()
        {
            playingId = 0;
            isTurnFirstPlay = false;
            selfCardOps.Clear();
        }
    }
}
