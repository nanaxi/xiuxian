using UnityEngine.UI;
using System;

namespace Lang
{
    /// <summary>
    /// 类似“同一IP”弹窗
    /// 
    /// 系统检测到...
    ///     好的
    /// </summary>
    public class PopupWindow8View : View
    {
        public Text content;
        public Button yes;

        Action onClickYes;

        public override ViewLayer GetLayer()
        {
            return ViewLayer.Topmost;
        }

        void Start()
        {
            yes.onClick.AddListener(() =>
            {
                if (onClickYes != null)
                {
                    onClickYes();
                }
                ViewManager.Destroy<PopupWindow8View>();
            });
        }

        public void Init(string content, Action clickYes)
        {
            this.content.text = content;
            onClickYes = clickYes;
        }
    }
}