using System;
using UnityEngine.UI;

namespace Lang
{
    /// <summary>
    /// 类似“房间已解散”弹窗
    /// 
    /// 您所在的房间已被解散!
    ///     倒计时
    ///     
    /// 窗口在一定时间后自动消失
    /// </summary>
    public class PopupWindow3View : View
    {
        public Text content;
        public Text time;

        int totalSeconds = 3;
        Action onFinish;

        public override ViewLayer GetLayer()
        {
            return ViewLayer.Topmost;
        }

        void Start()
        {
            time.text = totalSeconds.ToString();
            StartCoroutine(Util.UpdateTime(totalSeconds, c =>
            {
                time.text = c.ToString();
            }, () =>
            {
                if (onFinish != null)
                {
                    onFinish();
                }
                ViewManager.Destroy<PopupWindow3View>();
            }));
        }

        public void Init(string content, int time, Action onFinish)
        {
            this.content.text = content;
            this.time.text = time.ToString();
            totalSeconds = time;
            this.onFinish = onFinish;
        }
    }
}