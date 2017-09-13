using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class OtherPlayerGaming : PlayerGaming
    {
        public Text distance;

        uint id;

        public override void OnInit(PlayerInfo info)
        {
            this.id = info.id;
            info.SetAvatar(this.avatar);
            this.userId.text = info.name;
            this.distance.text = "定位中...";
        }

        public override void OnPlayerLeave()
        {
            this.distance.text = "";
            id = 0;
        }

        void Start()
        {
            avatar.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (id > 0)
                {
                    var go = GameManager.GM.PopUI(ResPath.PlayerInfo);
                    go.transform.SetParent(ViewManager.GetLayer(ViewLayer.Topmost), false);
                    go.transform.Find("Window").localScale = Vector3.one;

                    var p = PlayerMgr.Inst.GetPlayer(id);
                    var uiPlayer = go.GetComponent<UI_PlayerInfo>();
                    uiPlayer.SetInfo(p.name, p.id.ToString(), p.ip, "", null, p.GetNormalizedCityName(), (int)p.sex);
                    p.SetAvatar(uiPlayer.HeadSprite);

                    ViewManager.AddOther(go);
                }
            });
        }

        public void UpdateDistance(UnityEngine.Vector2 latlng)
        {
            var selfLatlng = PlayerMgr.Inst.Self.latlng;
            var distance = Gps.CalcDistance(selfLatlng, latlng);
            if (selfLatlng == UnityEngine.Vector2.zero || latlng == UnityEngine.Vector2.zero)
            {
                this.distance.text = string.Format("未开启定位");
            }
            else
            {
                this.distance.text = string.Format("距您{0}米", distance);
            }
        }
    }
}