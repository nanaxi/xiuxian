namespace Lang
{
    public class PlayerGamingView : View
    {
        public PlayerGaming[] playerGaming;

        int showReminCardCount = 3;

        public void Join(Seat seat, PlayerInfo info)
        {
            playerGaming[(int)seat].OnInit(info);
        }

        public void SetRemainCard(Seat seat, int count)
        {
            bool show = count <= showReminCardCount;
            playerGaming[(int)seat].SetRemainCard(show, count);
        }

        public void SetDeltaRemainCard(Seat seat, int substract)
        {
            var pp = playerGaming[(int)seat];
            bool show = pp.cardCount - substract <= showReminCardCount;
            pp.SetRemainCard(show, pp.cardCount - substract);
        }

        public void SetTotalScore(Seat seat, int score, bool show)
        {
            playerGaming[(int)seat].SetFen(show, score >= 0 ? "+" + score : score.ToString());
        }

        public void SetOwnerIcon(Seat seat, bool show)
        {
            playerGaming[(int)seat].SetOwnerIcon(show);
        }

        public void SetBankerIcon(Seat seat, bool show)
        {
            playerGaming[(int)seat].SetBankerIcon(show);
        }

        public void SetFriendIcon(Seat seat, bool show)
        {
            playerGaming[(int)seat].SetFriendIcon(show);
        }

        public void SetQiangIcon(Seat seat, bool show)
        {
            playerGaming[(int)seat].SetQiangIcon(show);
        }

        public void ClearIcons()
        {
            foreach (var p in playerGaming)
            {
                p.SetBankerIcon(false);
                p.SetFriendIcon(false);
                p.SetQiangIcon(false);
            }
        }

        public void SetPlayerPlaying(Seat seat)
        {
            ViewManager.Open<GamingTimerView>().Show(seat);
        }

        public void Clear()
        {
            ClearIcons();
            ViewManager.Open<GamingTimerView>().Clear();
            SetRemainCard(Seat.Down, 13);
            for (int i = 1; i < playerGaming.Length; i++)
            {
                playerGaming[i].ResetReady();
            }
        }

        public void Ready(Seat seat)
        {
            playerGaming[(int)seat].Ready();
        }

        public void Leave(Seat seat)
        {
            playerGaming[(int)seat].Leave();
        }

        public void AutoReady()
        {
            var g = playerGaming[0] as OwnPlayerGaming;
            g.AutoReady();
        }

        public void SelfReady()
        {
            var g = playerGaming[0] as OwnPlayerGaming;
            g.SelfReady();
        }

        public void GameStarted()
        {
            foreach (var p in playerGaming)
            {
                p.GameStarted();
            }
        }

        public void UpdateDistance(Seat seat, UnityEngine.Vector2 latlng)
        {
            var p = playerGaming[(int)seat] as OtherPlayerGaming;
            p.UpdateDistance(latlng);
        }
    }
}