using UnityEngine.UI;
using System;

namespace Lang
{
	/// <summary>
	/// 类似“房主退出”弹窗，标题大文字为“温馨提示”
	/// 
	/// 游戏已开局，若退出游戏
	///     是否现在退出？
	///  是    否
	/// </summary>
	public class PopupWindow6View : View
	{
	    public Text title;
	    public Text content;
	
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
	            ViewManager.Destroy<PopupWindow6View>();
	        });
	
	        no.onClick.AddListener(() =>
	        {
	            if (onClickNo != null)
	            {
	                onClickNo();
	            }
	            ViewManager.Destroy<PopupWindow6View>();
	        });
	    }
	
	    public void Init(string title, string content, Action clickYes, Action clickNo)
	    {
	        this.title.text = title;
	        this.content.text = content;
	        onClickYes = clickYes;
	        onClickNo = clickNo;
	    }
	}
}