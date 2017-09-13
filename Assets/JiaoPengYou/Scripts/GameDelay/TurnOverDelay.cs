using ProtoBuf;

namespace Lang
{
    public class TurnOverDelay
    {
        // 一局结束后的数据
        public PokerGameOver rsp;

        public bool isTurnOverDelay = true;

        public float delayTime = 3;

        public bool isFourthPlayerShowCard;

        public void Clear()
        {
            rsp = null;
            isTurnOverDelay = true;
            delayTime = 3;
            isFourthPlayerShowCard = false;
        }
    }
}