using System.Collections.Generic;

namespace Lang
{
    public struct Card
    {
        public CardSuit Suit { get; private set; }

        public uint NormalizedValue { get; private set; }

        public uint Value { get; private set; }

        public readonly static Card[] Hearts =
        {   new Card(0x03), new Card(0x04), new Card(0x05), new Card(0x06), new Card(0x07),
            new Card(0x08), new Card(0x09), new Card(0x0A), new Card(0x0B),
            new Card(0x0C), new Card(0x0D), new Card(0x0E), new Card(0x0F)
        };
        public readonly static Card[] Diamonds =
        {   new Card(0x13), new Card(0x14), new Card(0x15), new Card(0x16), new Card(0x17),
            new Card(0x18), new Card(0x19), new Card(0x1A), new Card(0x1B),
            new Card(0x1C), new Card(0x1D), new Card(0x1E), new Card(0x1F)
        };
        public readonly static Card[] Spades =
        {   new Card(0x23), new Card(0x24), new Card(0x25), new Card(0x26), new Card(0x27),
            new Card(0x28), new Card(0x29), new Card(0x2A), new Card(0x2B),
            new Card(0x2C), new Card(0x2D), new Card(0x2E), new Card(0x2F)
        };
        public readonly static Card[] Clubs =
        {   new Card(0x33), new Card(0x34), new Card(0x35), new Card(0x36), new Card(0x37),
            new Card(0x38), new Card(0x39), new Card(0x3A), new Card(0x3B),
            new Card(0x3C), new Card(0x3D), new Card(0x3E), new Card(0x3F)
        };

        public Card(uint value)
        {
            if (value <= 0x0F)
            {
                Suit = CardSuit.Heart;
            }
            else if (value <= 0x1F)
            {
                Suit = CardSuit.Diamond;
            }
            else if (value <= 0x2F)
            {
                Suit = CardSuit.Spade;
            }
            else if (value <= 0x3F)
            {
                Suit = CardSuit.Club;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("超出范围的卡牌值：" + value);
            }

            this.Value = value;
            NormalizedValue = value % 16;
        }

        public override string ToString()
        {
            string ch = null;
            switch (Suit)
            {
                case CardSuit.Heart:
                    ch = "红桃";
                    break;
                case CardSuit.Diamond:
                    ch = "方块";
                    break;
                case CardSuit.Spade:
                    ch = "黑桃";
                    break;
                case CardSuit.Club:
                    ch = "梅花";
                    break;
            }
            string str = "";
            if (NormalizedValue == 11)
            {
                str = "J";
            }
            else if (NormalizedValue == 12)
            {
                str = "Q";
            }
            else if (NormalizedValue == 13)
            {
                str = "K";
            }
            else if (NormalizedValue == 14)
            {
                str = "A";
            }
            else if (NormalizedValue == 15)
            {
                str = "2";
            }
            else
            {
                str = NormalizedValue.ToString();
            }
            return ch + str;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj.GetType().Equals(this.GetType())))
            {
                return false;
            }
            Card c = (Card)obj;
            return Value == c.Value;
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }

    public static class CardExtensions
    {
        public static List<uint> ToCards(this List<Card> cardDatas)
        {
            List<uint> cards = new List<uint>(cardDatas.Count);
            foreach (var cardData in cardDatas)
            {
                cards.Add(cardData.Value);
            }
            return cards;
        }

        public static List<Card> ToCards(this List<uint> cards)
        {
            List<Card> cardDatas = new List<Card>(cards.Count);
            foreach (var card in cards)
            {
                cardDatas.Add(new Card(card));
            }
            return cardDatas;
        }

        public static Card ToCard(this uint card)
        {
            return new Card(card);
        }
    }

    public enum CardSuit
    {
        Heart,
        Diamond,
        Spade,
        Club
    }
}