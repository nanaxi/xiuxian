using System.Linq;

namespace Lang
{
    public class PlayerMgr : Singleton<PlayerMgr>
    {
        public readonly PlayerInfo[] allPlayers = new PlayerInfo[4];

        public PlayerInfo Self { get { return allPlayers[0]; } }

        public PlayerInfo GetPlayer(uint id)
        {
            return allPlayers.FirstOrDefault(p =>
            {
                return p != null && p.id == id;
            });
        }

        public PlayerInfo GetPlayer(Seat seat)
        {
            return allPlayers[(int)seat];
        }

        public void Add(PlayerInfo p, Seat seat)
        {
            allPlayers[(int)seat] = p;
        }

        public void Remove(uint id)
        {
            for (int i = 0; i < allPlayers.Length; i++)
            {
                var p = allPlayers[i];
                if (p != null && p.id == id)
                {
                    allPlayers[i] = null;
                }
            }
        }

        public void Clear()
        {
            // 不包括自己的信息
            for (int i = 1; i < allPlayers.Length; i++)
            {
                allPlayers[i] = null;
            }
        }

        public bool Contains(uint id)
        {
            return allPlayers.Any(c =>
            {
                if (c == null)
                {
                    return false;
                }
                return c.id == id;
            });
        }

        public void AddToEmptySeat(PlayerInfo p)
        {
            for (int i = 0; i < allPlayers.Length; i++)
            {
                if (allPlayers[i] == null)
                {
                    allPlayers[i] = p;
                    break;
                }
            }
        }

        public void Add(PlayerInfo p, int serverPos, int selfServerPos)
        {
            var index = 0;
            var pos = serverPos - selfServerPos;
            if (pos >= 0)
            {
                index = pos;
            }
            else
            {
                index = allPlayers.Length - UnityEngine.Mathf.Abs(pos);
            }
            allPlayers[index] = p;
        }

        public Seat GetSeat(uint id)
        {
            for (int i = 0; i < allPlayers.Length; i++)
            {
                var p = allPlayers[i];
                if (p != null && p.id == id)
                {
                    return (Seat)i;
                }
            }

            UnityEngine.Debug.LogErrorFormat("无座位信息，{0}", id);
            return Seat.Down;
        }

        public bool IsSelf(uint id)
        {
            return Self.id == id;
        }

        public PlayerInfo GetNextPlayer(uint id)
        {
            var playerIndex = GetPlayerIndex(id);
            if (playerIndex >= 0)
            {
                var index = (playerIndex + 1) % 4;
                return allPlayers[index];
            }
            else
            {
                return new PlayerInfo();
            }
        }

        public PlayerInfo GetPreviousPlayer(uint id)
        {
            var playerIndex = GetPlayerIndex(id);
            if (playerIndex >= 0)
            {
                var index = playerIndex - 1;
                index = index < 0 ? 3 : index;
                return allPlayers[index];
            }
            else
            {
                return new PlayerInfo();
            }
        }

        int GetPlayerIndex(uint id)
        {
            for (int i = 0; i < allPlayers.Length; i++)
            {
                var p = allPlayers[i];
                if (p != null && p.id == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}