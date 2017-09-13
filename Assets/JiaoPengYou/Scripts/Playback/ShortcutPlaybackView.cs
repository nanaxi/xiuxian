using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class ShortcutPlaybackView : View
    {
        public Button setting;
        public Button exit;

        void Start()
        {
            setting.onClick.AddListener(ClickSetting);
            exit.onClick.AddListener(ClickExit);
        }

        void ClickSetting()
        {
            ViewManager.Open<SettingView>();
        }

        void ClickExit()
        {
            GameMgr.Inst.ExitRoom();
        }
    }
}