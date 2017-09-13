using UnityEngine.UI;
using System;
using UnityEngine;

namespace Lang
{
	/// <summary>
	/// “进场提醒”弹窗
	/// </summary>
	public class PopupWindow7View : View
	{
	    public Text time;
	    public Button yes;
	
	    Action onClickYes;
	
	    int totalSeconds = 3;

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
	            ViewManager.Destroy<PopupWindow7View>();
	        });
	
	        time.text = totalSeconds.ToString();
	        StartCoroutine(Util.UpdateTime(totalSeconds, c =>
	       {
	           time.text = c.ToString();
	       }, () =>
	       {
	           time.gameObject.SetActive(false);
	           var cg = yes.GetComponent<CanvasGroup>();
	           cg.alpha = 1;
	           cg.interactable = true;
	       }));
	    }
	
	    public void Init(Action clickYes)
	    {
	        onClickYes = clickYes;
	    }
	}
}