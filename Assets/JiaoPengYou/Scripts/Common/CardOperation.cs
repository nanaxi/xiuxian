using System.Collections.Generic;
using System.Linq;

namespace Lang
{
    public class CardOperation
    {
        public enum CardType
        {
            None,
            Single,
            Pair,
            Straight,
            BombThree,
            BombSameSuit,
            BombStraightPair,
            BombFour
        }

        public List<Card> ChePai(List<Card> handCards, Card target)
        {
            return handCards.FindAll(c => c.NormalizedValue == target.NormalizedValue);
        }

        public bool Che_CheckNeedSelectCard(List<Card> handCards, Card target)
        {
            List<Card> findCards = ChePai(handCards, target);
            if (findCards.Count == 2)
            {
                return false;
            }
            else if (findCards.Count >= 3)
            {
                return true;
            }
            else
            {
                throw new System.Exception(string.Format("牌数量错误：手牌中{0}的数量小于2，无法扯牌", target.ToString()));
            }
        }

        public bool IsPlay(List<Card> selectedCards, List<Card> targets)
        {
            if (selectedCards == null || selectedCards.Count == 0)
            {
                return false;
            }

            selectedCards.Sort(new CardAscSort());
            CardType selectedCardType = GetType(selectedCards);
            if (selectedCardType == CardType.None)
            {
                return false;
            }
            if (targets == null || targets.Count == 0)
            {
                return true;
            }

            targets.Sort(new CardAscSort());
            CardType targetCardType = GetType(targets);
            if (selectedCardType == CardType.BombSameSuit && targetCardType == CardType.BombStraightPair)
            {
                return selectedCards.Count > targets.Count / 2;
            }
            if (selectedCardType == CardType.BombStraightPair && targetCardType == CardType.BombSameSuit)
            {
                return selectedCards.Count / 2 >= targets.Count;
            }
            int result = CompareCardType(selectedCardType, targetCardType);
            if (result == 0)
            {
                return IsBiggerUnderSameCardType(selectedCards, targets);
            }
            return result == 1;
        }

        public bool IsBomb(List<Card> selectedCards)
        {
            if (selectedCards == null || selectedCards.Count == 0)
            {
                return false;
            }
            selectedCards.Sort(new CardAscSort());
            CardType selectedCardType = GetType(selectedCards);
            return selectedCardType >= CardType.BombThree;
        }

        public List<EachPrompt> Prompt(List<Card> handCards, List<Card> targets)
        {
            // 自己出牌，永远返回第一张
            if (targets == null || targets.Count == 0)
            {
                return new List<EachPrompt>() { new EachPrompt(new List<Card>() { handCards[0] }) };
            }

            handCards.Sort(new CardAscSort());
            targets.Sort(new CardAscSort());

            List<EachPrompt> pDatas = new List<EachPrompt>();
            var type = GetType(targets);
            switch (type)
            {
                case CardType.None:
                    return pDatas;
                case CardType.Single:
                    pDatas.AddRange(GetBiggerSingles(handCards, targets[0]));
                    break;
                case CardType.Pair:
                    pDatas.AddRange(GetBiggerPairs(handCards, targets));
                    break;
                case CardType.Straight:
                    pDatas.AddRange(GetBiggerStraights(handCards, targets));
                    break;
                case CardType.BombThree:
                    pDatas.AddRange(GetBiggerBombThree(handCards, targets));
                    {
                        var sorted = GetAllBombSameSuitAndStraightPairWithSorted(handCards);
                        pDatas.AddRange(sorted);
                    }
                    pDatas.AddRange(GetBiggerBombFour(handCards, null));
                    break;
                case CardType.BombSameSuit:
                    {
                        var bombSameSuits = GetBiggerBombSameSuit(handCards, targets);
                        var bombStraightPairs = GetBiggerBombStraightPair(handCards, null, targets.Count * 2);
                        var sorted = SameSuitAndStraightPairSort(bombSameSuits, bombStraightPairs);
                        pDatas.AddRange(sorted);
                    }
                    pDatas.AddRange(GetBiggerBombFour(handCards, null));
                    break;
                case CardType.BombStraightPair:
                    {
                        var bombStraightPairs = GetBiggerBombStraightPair(handCards, targets);
                        var bombSameSuits = GetBiggerBombSameSuit(handCards, null, targets.Count / 2 + 1);
                        var sorted = SameSuitAndStraightPairSort(bombSameSuits, bombStraightPairs);
                        pDatas.AddRange(sorted);
                    }
                    pDatas.AddRange(GetBiggerBombFour(handCards, null));
                    break;
                case CardType.BombFour:
                    pDatas.AddRange(GetBiggerBombFour(handCards, targets));
                    break;
            }

            // 所有炸弹
            if (CardType.Single <= type && type <= CardType.Straight)
            {
                pDatas.AddRange(GetBiggerBombThree(handCards, null));
                var sorted = GetAllBombSameSuitAndStraightPairWithSorted(handCards);
                pDatas.AddRange(sorted);
                pDatas.AddRange(GetBiggerBombFour(handCards, null));
            }

            return pDatas;
        }

        public List<EachPrompt> PromptBombs(List<Card> handCards)
        {
            handCards.Sort(new CardAscSort());

            List<EachPrompt> pDatas = new List<EachPrompt>();
            // 炸弹
            pDatas.AddRange(GetBiggerBombThree(handCards, null));
            var sorted = GetAllBombSameSuitAndStraightPairWithSorted(handCards);
            pDatas.AddRange(sorted);
            pDatas.AddRange(GetBiggerBombFour(handCards, null));

            return pDatas;
        }

        public CardType GetType(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return CardType.None;
            }
            if (IsSingle(cards))
            {
                return CardType.Single;
            }
            if (IsPair(cards))
            {
                return CardType.Pair;
            }
            if (IsSameSuit(cards))
            {
                return CardType.BombSameSuit;
            }
            if (IsStraight(cards))
            {
                return CardType.Straight;
            }
            if (IsThree(cards))
            {
                return CardType.BombThree;
            }
            if (IsStraightPair(cards))
            {
                return CardType.BombStraightPair;
            }
            if (IsFour(cards))
            {
                return CardType.BombFour;
            }
            return CardType.None;
        }

        public int CompareCardType(CardType source, CardType target)
        {
            if (source == target)
            {
                return 0;
            }
            if (source <= CardType.Straight && target <= CardType.Straight)
            {
                return -1;
            }
            if (source > target)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public bool IsBiggerUnderSameCardType(List<Card> sources, List<Card> targets)
        {
            CardType type = GetType(sources);
            switch (type)
            {
                case CardType.None:
                    return false;
                case CardType.Single:
                case CardType.Pair:
                case CardType.BombThree:
                case CardType.BombFour:
                    return sources[0].NormalizedValue > targets[0].NormalizedValue;
                case CardType.Straight:
                    if (sources.Count == targets.Count)
                    {
                        return sources[0].NormalizedValue > targets[0].NormalizedValue;
                    }
                    else
                    {
                        return false;
                    }
                case CardType.BombSameSuit:
                case CardType.BombStraightPair:
                    if (sources.Count == targets.Count)
                    {
                        return sources[0].NormalizedValue > targets[0].NormalizedValue;
                    }
                    return sources.Count > targets.Count;
                default:
                    return false;
            }
        }

        // 2不能组成连子，清一色，连对
        bool Contains2(List<Card> cards)
        {
            return cards[cards.Count - 1].NormalizedValue == 15;
        }

        List<EachPrompt> GetAllBombSameSuitAndStraightPairWithSorted(List<Card> handCards)
        {
            var bombSameSuits = GetBiggerBombSameSuit(handCards, null);
            var bombStraightPairs = GetBiggerBombStraightPair(handCards, null);
            var sorted = SameSuitAndStraightPairSort(bombSameSuits, bombStraightPairs);
            return sorted;
        }

        List<EachPrompt> SameSuitAndStraightPairSort(List<EachPrompt> bombSameSuits, List<EachPrompt> bombStraightPairs)
        {
            Dictionary<EachPrompt, int> sorts = new Dictionary<EachPrompt, int>();
            if (bombSameSuits != null)
            {
                foreach (var boomSameSuit in bombSameSuits)
                {
                    sorts.Add(boomSameSuit, boomSameSuit.cards.Count);
                }
            }
            if (bombStraightPairs != null)
            {
                foreach (var bombStraightPair in bombStraightPairs)
                {
                    sorts.Add(bombStraightPair, bombStraightPair.cards.Count / 2);
                }
            }

            if (sorts.Count == 0)
            {
                return new List<EachPrompt>();
            }
            else
            {
                var sorted = sorts.OrderBy(pair => pair.Value).ToDictionary(p => p.Key, q => q.Value);
                return sorted.Keys.ToList();
            }
        }

        #region GetType
        bool IsSingle(List<Card> cards)
        {
            return cards.Count == 1;
        }

        bool IsPair(List<Card> cards)
        {
            if (cards.Count != 2)
            {
                return false;
            }

            return cards[0].NormalizedValue == cards[1].NormalizedValue;
        }

        bool IsStraight(List<Card> cards)
        {
            if (cards.Count < 3)
            {
                return false;
            }
            if (Contains2(cards))
            {
                return false;
            }

            var value = cards[0].NormalizedValue;
            for (int i = 1; i < cards.Count; i++)
            {
                if (cards[i].NormalizedValue - i != value)
                {
                    return false;
                }
            }

            return true;
        }

        bool IsThree(List<Card> cards)
        {
            if (cards.Count != 3)
            {
                return false;
            }

            var minCard = cards[0];
            for (int i = 1; i < cards.Count; i++)
            {
                if (cards[i].NormalizedValue != minCard.NormalizedValue)
                {
                    return false;
                }
            }

            return true;
        }

        bool IsSameSuit(List<Card> cards)
        {
            if (cards.Count < 3)
            {
                return false;
            }
            if (Contains2(cards))
            {
                return false;
            }

            var minCard = cards[0];
            for (int i = 1; i < cards.Count; i++)
            {
                var current = cards[i];
                if (current.NormalizedValue - i != minCard.NormalizedValue || current.Suit != minCard.Suit)
                {
                    return false;
                }
            }

            return true;
        }

        bool IsStraightPair(List<Card> cards)
        {
            if (cards.Count < 6)
            {
                return false;
            }
            if (cards.Count % 2 != 0)
            {
                return false;
            }
            if (Contains2(cards))
            {
                return false;
            }

            uint value = cards[0].NormalizedValue;
            for (int i = 0; i < cards.Count; i += 2)
            {
                uint currentValue = cards[i].NormalizedValue;
                if (currentValue != cards[i + 1].NormalizedValue)
                {
                    return false;
                }
                if (i > 0)
                {
                    if (currentValue - 1 == value)
                    {
                        value = currentValue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool IsFour(List<Card> cards)
        {
            if (cards.Count != 4)
            {
                return false;
            }

            var minCard = cards[0];
            for (int i = 1; i < cards.Count; i++)
            {
                if (cards[i].NormalizedValue != minCard.NormalizedValue)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Prompt

        List<EachPrompt> GetBiggerSingles(List<Card> handCards, Card target)
        {
            SortedDictionary<uint, Card> singleCards = new SortedDictionary<uint, Card>();
            foreach (var card in handCards)
            {
                if (card.NormalizedValue > target.NormalizedValue)
                {
                    if (!singleCards.ContainsKey(card.NormalizedValue))
                    {
                        singleCards.Add(card.NormalizedValue, card);
                    }
                }
            }

            List<EachPrompt> pDatas = new List<EachPrompt>(singleCards.Count);
            foreach (var card in singleCards.Values)
            {
                EachPrompt p = new EachPrompt(new List<Card>() { card });
                pDatas.Add(p);
            }

            return pDatas;
        }

        List<EachPrompt> GetBiggerPairs(List<Card> handCards, List<Card> targets)
        {
            if (handCards.Count < targets.Count)
            {
                return new List<EachPrompt>();
            }

            List<EachPrompt> pDatas = new List<EachPrompt>();
            var targetValue = targets[0].NormalizedValue;
            for (int i = 0; i < handCards.Count - 1; i++)
            {
                Card c1 = handCards[i];
                if (c1.NormalizedValue > targetValue)
                {
                    Card c2 = handCards[i + 1];
                    if (c1.NormalizedValue == c2.NormalizedValue)
                    {
                        EachPrompt p = new EachPrompt(new List<Card>() { c1, c2 });
                        pDatas.Add(p);
                        i++;
                    }
                }
            }

            return pDatas;
        }

        List<EachPrompt> GetBiggerStraights(List<Card> handCards, List<Card> targets)
        {
            if (handCards.Count < targets.Count)
            {
                return new List<EachPrompt>();
            }

            var targetMinValue = targets[0].NormalizedValue;
            SortedDictionary<uint, Card> singleCards = new SortedDictionary<uint, Card>();
            foreach (var handCard in handCards)
            {
                if (handCard.NormalizedValue == 15)
                {
                    continue;
                }
                if (handCard.NormalizedValue <= targetMinValue)
                {
                    continue;
                }
                if (!singleCards.ContainsKey(handCard.NormalizedValue))
                {
                    singleCards.Add(handCard.NormalizedValue, handCard);
                }
            }

            if (singleCards.Count < targets.Count)
            {
                return new List<EachPrompt>();
            }

            List<EachPrompt> pDatas = new List<EachPrompt>(singleCards.Count);

            var cardDatas = singleCards.Values.ToList();
            for (int i = 0; i < cardDatas.Count - targets.Count + 1; i++)
            {
                bool findStraight = true;
                var cardValue = cardDatas[i].NormalizedValue;
                for (int j = 1; j < targets.Count; j++)
                {
                    if (cardDatas[i + j].NormalizedValue - j != cardValue)
                    {
                        findStraight = false;
                        break;
                    }
                }
                if (findStraight)
                {
                    EachPrompt p = new EachPrompt(new List<Card>());
                    for (int j = 0; j < targets.Count; j++)
                    {
                        var data = cardDatas[i + j];
                        p.cards.Add(data);
                    }
                    pDatas.Add(p);
                }
            }

            return pDatas;
        }

        // targets==null时，表示所有的三个，下同
        List<EachPrompt> GetBiggerBombThree(List<Card> handCards, List<Card> targets)
        {
            if (handCards.Count < 3)
            {
                return new List<EachPrompt>();
            }

            uint minCardValue = 0;
            int cardCount = 3;
            if (targets != null)
            {
                minCardValue = targets[0].NormalizedValue;
                cardCount = targets.Count;
            }

            List<EachPrompt> pDatas = new List<EachPrompt>();
            for (int i = 0; i < handCards.Count - 2; i++)
            {
                bool findThree = true;
                var cardValue = handCards[i].NormalizedValue;
                for (int j = 1; j < 3; j++)
                {
                    if (cardValue < minCardValue)
                    {
                        findThree = false;
                        break;
                    }
                    if (handCards[i + j].NormalizedValue != cardValue)
                    {
                        findThree = false;
                        break;
                    }
                }
                if (findThree)
                {
                    EachPrompt p = new EachPrompt(new List<Card>());
                    for (int j = 0; j < cardCount; j++)
                    {
                        var data = handCards[i + j];
                        p.cards.Add(data);
                    }
                    pDatas.Add(p);
                    i += 2;
                }
            }

            return pDatas;
        }

        List<EachPrompt> GetBiggerBombSameSuit(List<Card> handCards, List<Card> targets, int minCount = 0)
        {
            if (targets != null && handCards.Count < targets.Count)
            {
                return new List<EachPrompt>();
            }

            if (targets == null)
            {
                var findCount = System.Math.Max(minCount, 3);
                return FindSameSuit(handCards, findCount, 100, 0);
            }
            else
            {
                var findCount = System.Math.Max(minCount, targets.Count);
                var result1 = FindSameSuit(handCards, findCount, findCount, targets[0].NormalizedValue);
                // 大于目标的数量
                var result2 = FindSameSuit(handCards, findCount + 1, 100, 0);
                result1.AddRange(result2);

                return result1;
            }
        }

        List<EachPrompt> GetBiggerBombStraightPair(List<Card> handCards, List<Card> targets, int minCount = 0)
        {
            if (targets != null && handCards.Count < targets.Count)
            {
                return null;
            }

            if (targets == null)
            {
                var findCount = System.Math.Max(minCount, 6);
                return FindStraightPair(handCards, findCount, 100, 0);
            }
            else
            {
                var findCount = System.Math.Max(minCount, targets.Count);
                var result1 = FindStraightPair(handCards, findCount, findCount, targets[0].NormalizedValue);
                // 大于目标的数量
                var result2 = FindStraightPair(handCards, findCount + 2, 100, 0);
                result1.AddRange(result2);

                return result1;
            }
        }

        List<EachPrompt> GetBiggerBombFour(List<Card> handCards, List<Card> targets)
        {
            if (targets != null && handCards.Count < 4)
            {
                return new List<EachPrompt>();
            }

            List<EachPrompt> pDatas = new List<EachPrompt>();

            uint minCardValue = 0;
            int cardCount = 4;
            if (targets != null)
            {
                minCardValue = targets[0].NormalizedValue;
                cardCount = targets.Count;
            }

            for (int i = 0; i < handCards.Count - 3; i++)
            {
                bool findFour = true;
                var cardValue = handCards[i].NormalizedValue;
                for (int j = 1; j < 4; j++)
                {
                    if (cardValue < minCardValue)
                    {
                        findFour = false;
                        break;
                    }
                    if (handCards[i + j].NormalizedValue != cardValue)
                    {
                        findFour = false;
                        break;
                    }
                }
                if (findFour)
                {
                    EachPrompt p = new EachPrompt(new List<Card>());
                    for (int j = 0; j < cardCount; j++)
                    {
                        var data = handCards[i + j];
                        p.cards.Add(data);
                    }
                    pDatas.Add(p);
                    i += 3;
                }
            }

            return pDatas;
        }

        List<EachPrompt> FindSameSuit(List<Card> handCards, int minCount, int maxCount, uint minCardValue)
        {
            List<EachPrompt> sameSuits = new List<EachPrompt>();
            List<List<Card>> groupCards = GroupCards(handCards);
            foreach (var groupCard in groupCards)
            {
                int nextMaxCount = maxCount;
                int nextMinCount = minCount;
                uint nextMinCardValue = minCardValue;
                while (true)
                {
                    if (nextMinCount > nextMaxCount)
                    {
                        break;
                    }

                    var straights = SplitStraight(groupCard, nextMinCount, nextMinCardValue);
                    if (straights.Count > 0)
                    {
                        foreach (var straight in straights)
                        {
                            sameSuits.Add(new EachPrompt(straight));
                        }
                        nextMinCount++;
                        nextMinCardValue = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            sameSuits.Sort(new EachPromptSort());

            return sameSuits;
        }

        List<EachPrompt> FindStraightPair(List<Card> handCards, int minCount, int maxCount, uint minCardValue)
        {
            List<EachPrompt> straightPairs = new List<EachPrompt>();
            List<List<Card>> pairs = SplitPair(handCards);
            List<Card> singleCards = new List<Card>();
            for (int i = 0; i < pairs.Count; i++)
            {
                singleCards.Add(pairs[i][0]);
            }

            int nextMaxCount = maxCount / 2;
            int nextMinCount = minCount / 2;
            uint nextMinCardValue = minCardValue;
            while (true)
            {
                if (nextMinCount > nextMaxCount)
                {
                    break;
                }

                var straights = SplitStraight(singleCards, nextMinCount, nextMinCardValue);
                if (straights.Count > 0)
                {
                    foreach (var straight in straights)
                    {
                        EachPrompt p = new EachPrompt(new List<Card>());
                        foreach (var card in straight)
                        {
                            // 根据item查找另外一张牌，凑成一对
                            var singleIndex = singleCards.FindIndex(c => c.NormalizedValue == card.NormalizedValue);
                            p.cards.Add(card);
                            p.cards.Add(pairs[singleIndex][1]);
                        }
                        straightPairs.Add(p);
                    }
                    nextMinCount++;
                    nextMinCardValue = 0;
                }
                else
                {
                    break;
                }
            }

            return straightPairs;
        }

        List<List<Card>> GroupCards(List<Card> handCards)
        {
            List<List<Card>> groupCards = new List<List<Card>>(4) { new List<Card>(),
                new List<Card>(), new List<Card>(), new List<Card>() };
            foreach (var handCard in handCards)
            {
                int index = (int)handCard.Suit;
                groupCards[index].Add(handCard);
            }

            return groupCards;
        }

        List<List<Card>> SplitStraight(List<Card> cards, int count, uint minCardValue)
        {
            List<List<Card>> straights = new List<List<Card>>();
            if (cards.Count < count)
            {
                return straights;
            }
            for (int i = 0; i < cards.Count - count + 1; i++)
            {
                var current = cards[i];
                if (current.NormalizedValue <= minCardValue)
                {
                    continue;
                }
                List<Card> cardDatas = new List<Card>() { current };
                for (int j = 1; j < count; j++)
                {
                    var next = cards[i + j];
                    cardDatas.Add(next);
                }
                if (IsStraight(cardDatas))
                {
                    straights.Add(cardDatas);
                }
            }

            return straights;
        }

        List<List<Card>> SplitPair(List<Card> cards)
        {
            List<List<Card>> pairs = new List<List<Card>>();
            uint lastPairCardValue = 0;
            for (int i = 0; i < cards.Count - 1; i++)
            {
                List<Card> pair = new List<Card>() { cards[i], cards[i + 1] };
                if (IsPair(pair) && pair[0].NormalizedValue != lastPairCardValue)
                {
                    pairs.Add(pair);
                    lastPairCardValue = pair[0].NormalizedValue;
                    i++;
                }
            }

            return pairs;
        }

        #endregion
    }

    public class EachPrompt
    {
        public List<Card> cards;

        public EachPrompt()
        {
        }

        public EachPrompt(List<Card> cards)
        {
            this.cards = cards;
        }
    }

    public class EachPromptSort : IComparer<EachPrompt>
    {
        public int Compare(EachPrompt x, EachPrompt y)
        {
            if (x.cards.Count == y.cards.Count)
            {
                if (x.cards[0].NormalizedValue < y.cards[0].NormalizedValue)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            if (x.cards.Count < y.cards.Count)
            {
                return -1;
            }
            return 1;
        }
    }
}