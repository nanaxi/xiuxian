using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class PlayerGaming : MonoBehaviour
    {
        public Image avatar;
        public GameObject[] icons;
        public Text userId;
        public GameObject fen;
        public SpriteNumber fenNum;
        public GameObject remainCard;
        public Text remainCardCount;
        public GameObject readyIcon;

        [HideInInspector]
        public int cardCount;

        Sprite originalAvatar;
        GameObject fireAnim;

        public virtual void Awake()
        {
            originalAvatar = avatar.sprite;
        }

        public virtual void OnInit(PlayerInfo info)
        {
        }

        public void SetFen(bool show, string fen)
        {
            this.fen.SetActive(show);
            if (show)
            {
                fenNum.Number = fen;
            }
        }

        public void SetRemainCard(bool show, int cardCount)
        {
            if (cardCount < 0)
            {
                cardCount = 0;
            }
            this.cardCount = cardCount;
            remainCard.SetActive(show);
            if (show)
            {
                remainCardCount.text = ":" + cardCount;
            }
        }

        public void SetOwnerIcon(bool show)
        {
            if (icons[0] != null)
            {
                icons[0].SetActive(show);
            }
        }

        public void SetBankerIcon(bool show)
        {
            if (icons[1] != null)
            {
                icons[1].SetActive(show);
            }
        }

        public void SetFriendIcon(bool show)
        {
            if (icons[2])
            {
                icons[2].SetActive(show);
            }
        }

        public void SetQiangIcon(bool show)
        {
            if (icons[3] != null)
            {
                icons[3].SetActive(show);
            }
            if (show)
            {
                fireAnim = Util.Load("Effects/Fire", transform);
                fireAnim.transform.SetAsFirstSibling();
                fireAnim.transform.position = avatar.transform.position;
                var rt = fireAnim.transform as RectTransform;
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 60);
            }
            else
            {
                if (fireAnim != null)
                {
                    Destroy(fireAnim);
                }
            }
        }

        public void Ready()
        {
            SetReadyIcon(true);
        }

        public void ResetReady()
        {
            SetReadyIcon(false);
        }

        protected void SetReadyIcon(bool show)
        {
            readyIcon.SetActive(show);
        }

        public void Leave()
        {
            SetFen(false, "");
            ResetReady();
            SetOwnerIcon(false);
            this.avatar.sprite = originalAvatar;
            this.avatar.SetNativeSize();
            this.userId.text = "";

            OnPlayerLeave();
        }

        public virtual void OnPlayerLeave() { }

        public void GameStarted()
        {
            SetReadyIcon(false);
            OnGameStarted();
        }

        public virtual void OnGameStarted() { }
    }
}