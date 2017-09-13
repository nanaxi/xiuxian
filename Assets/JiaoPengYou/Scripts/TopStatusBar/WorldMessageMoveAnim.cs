using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Lang
{
	public class WorldMessageMoveAnim : MonoBehaviour
	{
	    public float from;
	    public float to;
	    public float duration;
	
	    public Text notice;
	
	    void Start()
	    {
	        Play();
	    }
	
	    public void Play()
	    {
	        transform.DOKill();
	
	        var position = transform.localPosition;
	        position.x = from;
	        transform.localPosition = position;
	        float endValue = to - notice.preferredWidth;
	        transform.DOLocalMoveX(endValue, duration).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
	    }
	}
}