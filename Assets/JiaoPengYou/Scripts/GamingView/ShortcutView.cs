using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class ShortcutView : View
    {
        public Button speaker;
        public GameObject speakerOff;
        public Button mic;
        public GameObject micOff;
        public Button chat;
        public Button setting;
        public Button pickCardDetail;
        public Button exit;

        void Start()
        {
            speaker.onClick.AddListener(ClickSpeaker);
            mic.onClick.AddListener(ClickMic);
            chat.onClick.AddListener(ClickChat);
            setting.onClick.AddListener(ClickSetting);
            pickCardDetail.onClick.AddListener(ClickCardDetail);
            exit.onClick.AddListener(ClickExit);
        }

        void ClickSpeaker()
        {
            bool b = !speakerOff.activeSelf;
            speakerOff.SetActive(b);

            RealTimeVoice.Inst.OpenSpeaker(!b);
        }

        void ClickMic()
        {
            bool b = !micOff.activeSelf;
            micOff.SetActive(b);

            RealTimeVoice.Inst.OpenMic(!b);
        }

        void ClickChat()
        {
            GameManager.GM.DS.UsedChat = GameManager.GM.PopUI(ResPath.UsedChat);

            GameObject go = GameManager.GM.DS.UsedChat;
            go.transform.SetParent(ViewManager.GetLayer(ViewLayer.Topmost), false);
            go.transform.localScale = Vector3.one * 0.69479F;
            var rt = go.transform as RectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(-293, -164.89F);
            rt.offsetMax = new Vector2(293, 164.89F);

            ViewManager.AddOther(go);

            string[] voiceTexts = { "大哥这盘就巴到你超了哦", "紧道摸紧到摸，快点出噻", "看到这牌都够了，硬是西狗屎撇" , "肯定是那个兄弟信号不好，断网了",
            "哪个发的牌额，恩是全是药，囊开打嘛，简直莫法打","你个演员，你继续装嘛，装像些","你还敢抢，皮连口水都给你打出来","你甩起顶嘛，一看就晓得你不是一头的",
            "朋友你这牌打的相当经典","他的牌太耿了，兄弟~确实莫得法","遭了，遭了，坨都坨了"};

            var usedChat = go.GetComponent<UI_UsedChat>();
            for (int i = 0; i < usedChat.Chats.Count; i++)
            {
                var button = usedChat.Chats[i];
                button.GetComponentInChildren<Text>().text = voiceTexts[i];
            }
        }

        void ClickSetting()
        {
            ViewManager.Open<SettingView>();
        }

        void ClickCardDetail()
        {
            var view = ViewManager.Open<PickCardDetailView>(true);
            view.Init();
            view.Show(true);
        }

        void ClickExit()
        {
            // 已经开始游戏或已经打完至少一局时中途解散房间
            //   发起投票确认
            // 未开始游戏
            //   房主退出
            //     房间解散弹窗确认，其余玩家弹窗房间解散
            //   其他玩家退出
            //     直接退出
            if (GameMgr.Inst.IsGaming || GameMgr.Inst.CurrentRound > 1)
            {
                ViewManager.Open<PopupWindow4View>().Init(() =>
                {
                    NetSender.Inst.VoteDisbandRoom(true);
                }, null);
            }
            else
            {
                if (PlayerMgr.Inst.IsSelf(GameMgr.Inst.OwnerId))
                {
                    ViewManager.Open<PopupWindow6View>().Init("房主在游戏开始前退出游戏，房间即刻解散。", "是否现在退出？", () =>
                    {
                        if (GameMgr.Inst.IsGaming)
                        {
                            ViewManager.Open<PopupWindow9View>().Init("游戏已开始，退出失败！", 1F);
                        }
                        else
                        {
                            NetSender.Inst.VoteDisbandRoom(true);
                        }
                    }, null);
                }
                else
                {
                    NetSender.Inst.ExitRoom();
                }
            }
        }
    }
}