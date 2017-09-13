using UnityEngine.UI;
using System;

namespace Lang
{
	/// <summary>
	/// 类似“抢牌”弹窗
	/// 
	/// 您确定要抢牌么？
	///     倒计时
	///  确定    过
	/// </summary>
	public class PopupWindow1View : View
	{
	    public Text content;
	    public Text time;
	    public Button yes;
	    public Button no;
	
	    int totalSeconds = 3;
	
	    Action onClickYes;
	    Action onClickNo;

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
	            ViewManager.Destroy<PopupWindow1View>();
	        });
	
	        no.onClick.AddListener(() =>
	        {
	            if (onClickNo != null)
	            {
	                onClickNo();
	            }
	            ViewManager.Destroy<PopupWindow1View>();
	        });
	
	        Util.UpdateTime(this, totalSeconds, time);
	    }
	
	    public void Init(string content, int time, Action clickYes, Action clickNo)
	    {
	        this.content.text = content;
	        this.time.text = time.ToString();
	        totalSeconds = time;
	        onClickYes = clickYes;
	        onClickNo = clickNo;
	    }
	}
}