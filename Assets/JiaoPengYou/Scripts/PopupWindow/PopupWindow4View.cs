using UnityEngine.UI;
using System;

namespace Lang
{
	/// <summary>
	/// “申请退出”弹窗
	/// 
	/// 游戏已开局，若现在退出游戏...
	///     是否发起退出申请？
	///  是    否
	/// </summary>
	public class PopupWindow4View : View
	{
	    public Button yes;
	    public Button no;
	
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
	            ViewManager.Destroy<PopupWindow4View>();
	        });
	
	        no.onClick.AddListener(() =>
	        {
	            if (onClickNo != null)
	            {
	                onClickNo();
	            }
	            ViewManager.Destroy<PopupWindow4View>();
	        });
	    }
	
	    public void Init(Action clickYes, Action clickNo)
	    {
	        onClickYes = clickYes;
	        onClickNo = clickNo;
	    }
	}
}