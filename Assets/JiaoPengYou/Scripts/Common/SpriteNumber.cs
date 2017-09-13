using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class SpriteNumber : MonoBehaviour
	{
	    // Resources Path
	    public string path;
	
	    [SerializeField, SetProperty("Number")]
	    private string number;
	    public string Number
	    {
	        get
	        {
	            return number;
	        }
	        set
	        {
	            number = value;
	
	            Clear();
	            SetNumber();
	        }
	    }
	
	    void SetNumber()
	    {
	        foreach (var sptiteName in Number.ToCharArray())
	        {
	            Sprite sprite = LoadSprite(sptiteName.ToString());
	            if (sprite == null)
	            {
	                continue;
	            }
	            GameObject go = new GameObject(sprite.name);
	            go.transform.SetParent(transform, false);
	            Image img = go.AddComponent<Image>();
	            img.raycastTarget = false;
	            img.sprite = sprite;
	            img.SetNativeSize();
	        }
	    }
	
	    Sprite LoadSprite(string spriteName)
	    {
	        return Resources.Load<Sprite>(path + "/" + spriteName);
	    }
	
	    public void Clear()
	    {
	        if (Application.isPlaying)
	        {
	            foreach (Transform child in transform)
	            {
	                Destroy(child.gameObject);
	            }
	        }
	        else
	        {
	            while (transform.childCount > 0)
	            {
	                var child = transform.GetChild(0).gameObject;
	                DestroyImmediate(child);
	            }
	        }
	    }
	}
}
