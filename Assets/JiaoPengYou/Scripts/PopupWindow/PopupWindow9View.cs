using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace Lang
{
	/// <summary>
	/// “温馨提示”弹窗，自动销毁
	/// 
	///     内容...
	/// </summary>
	public class PopupWindow9View : View
	{
	    public Text content;

        public override ViewLayer GetLayer()
        {
            return ViewLayer.Topmost;
        }

        public void Init(string content, float autoDestroySeconds)
	    {
	        this.content.text = content;
	        StartCoroutine(AutoDestroy(autoDestroySeconds));
	    }
	
	    IEnumerator AutoDestroy(float seconds)
	    {
	        yield return new WaitForSeconds(seconds);
	        ViewManager.Destroy<PopupWindow9View>();
	    }
	}
}