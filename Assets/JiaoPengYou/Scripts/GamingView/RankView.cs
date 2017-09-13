using UnityEngine;

namespace Lang
{
    public class RankView : View
    {
        public Transform[] poss;
        public GameObject[] icons;

        public void Show(Seat seat, RankType rankType)
        {
            GameObject obj = null;
            switch (rankType)
            {
                case RankType.Up:
                    obj = icons[0];
                    break;
                case RankType.Middle:
                    if (!icons[1].activeSelf)
                    {
                        obj = icons[1];
                    }
                    else
                    {
                        obj = icons[2];
                    }
                    break;
                case RankType.Down:
                    obj = icons[3];
                    break;
                default:
                    break;
            }
            if (obj != null)
            {
                Transform parent = poss[(int)seat];
                obj.transform.SetParent(parent, false);
                obj.SetActive(true);
            }
        }
    }
}