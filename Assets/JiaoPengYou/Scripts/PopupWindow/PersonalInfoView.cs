using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class PersonalInfoView : View
    {
        public Image avatar;
        public Button close;
        public Text userName;
        public GameObject sexman;
        public GameObject sexwoman;
        public Text id;
        public Text ip;
        public Text cityName;
        public SpriteNumber roomCardNum;

        public override ViewLayer GetLayer()
        {
            return ViewLayer.Topmost;
        }

        void Start()
        {
            Show();

            close.onClick.AddListener(() =>
            {
                ViewManager.Destroy<PersonalInfoView>();
            });
        }

        void Show()
        {
            var p = PlayerMgr.Inst.Self;
            p.SetAvatar(avatar);
            userName.text = p.name;
            sexman.SetActive(p.sex == 1);
            sexwoman.SetActive(p.sex != 1);
            id.text = p.id.ToString();
            ip.text = p.ip;
            roomCardNum.Number = p.diamond.ToString();
            cityName.text = p.GetNormalizedCityName();
        }
    }
}