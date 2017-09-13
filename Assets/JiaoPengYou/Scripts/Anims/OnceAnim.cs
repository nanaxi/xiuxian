using System;
using System.Collections;
using UnityEngine;

namespace Lang
{
	public class OnceAnim : MonoBehaviour
	{
	    public float duration;
	    public Action onFinish;
	
	    IEnumerator Start()
	    {
	        yield return new WaitForSeconds(duration);
	        if (onFinish != null)
	        {
	            onFinish();
	        }
	        Destroy();
	    }
	
	    void OnDestroy()
	    {
	        Destroy();
	    }
	
	    void OnDisable()
	    {
	        Destroy();
	    }
	
	    void Destroy()
	    {
	        Destroy(gameObject);
	    }
	}
}