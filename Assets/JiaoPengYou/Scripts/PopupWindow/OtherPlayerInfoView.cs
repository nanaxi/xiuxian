using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class OtherPlayerInfoView : View
    {
        public GameObject window;
        public Image avatar;
        public Button close;
        public Text userName;
        public GameObject sexman;
        public GameObject sexwoman;
        public Text id;
        public Text ip;
        public Text cityName;
        public Button flower;
        public Button slipper;
        public Button beer;
        public Vector3[] positions;

        void Start()
        {
            close.onClick.AddListener(() =>
            {
                ViewManager.Destroy<OtherPlayerInfoView>();
            });

            flower.onClick.AddListener(() =>
            {

            });

            slipper.onClick.AddListener(() =>
            {

            });

            beer.onClick.AddListener(() =>
            {

            });
        }

        public void Init(uint id)
        {
            Seat seat = PlayerMgr.Inst.GetSeat(id);
            if (seat == Seat.Down)
            {
                return;
            }

            RectTransform rt = window.transform as RectTransform;
            rt.anchoredPosition = positions[(int)seat - 1];

            var p = PlayerMgr.Inst.GetPlayer(id);
            p.SetAvatar(avatar);
            userName.text = p.name;
            sexman.SetActive(p.sex == 1);
            sexwoman.SetActive(p.sex != 1);
            this.id.text = p.id.ToString();
            this.ip.text = p.ip;
            cityName.text = p.GetNormalizedCityName();
        }
    }
}