using System.Collections.Generic;

namespace Lang
{
    public class CardAscSort : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (x.NormalizedValue < y.NormalizedValue)
            {
                return -1;
            }
            if (x.NormalizedValue == y.NormalizedValue)
            {
                return 0;
            }
            return 1;
        }
    }
}