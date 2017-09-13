using UnityEngine.UI;
using System;

namespace Lang
{
	/// <summary>
	/// 类似“取消一条龙”弹窗
	/// 
	/// 您确定要取消一条龙么？
	///     12
	///  确定    取消
	/// </summary>
	public class PopupWindow2View : View
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
	            ViewManager.Destroy<PopupWindow2View>();
	        });
	
	        no.onClick.AddListener(() =>
	        {
	            if (onClickNo != null)
	            {
	                onClickNo();
	            }
	            ViewManager.Destroy<PopupWindow2View>();
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