using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
public class SVMJDrag : MonoBehaviour ,IBeginDragHandler,IEndDragHandler , IPointerDownHandler , IDragHandler{

    public UnityEngine.Events.UnityAction onDragStart;
    public UnityEngine.Events.UnityAction onDragEnd;
    public ScrollRect sVRect;
    public Vector2 v2DragStart,v2DragEnd;
    public Image img_MJ;
    public Image image_My;
    private const bool isSetPosition = true;
    private int f_ChuPaiHight;
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        //throw new NotImplementedException();
        if (onDragStart!=null)
        {
            onDragStart.Invoke();
        }
        //Debug.Log(""+Input.mousePosition);
        if (sVRect!=null)
        {
            v2DragStart = sVRect.content.anchoredPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (sVRect != null)
        {
            v2DragEnd = sVRect.content.anchoredPosition;
        }

        //throw new NotImplementedException();
        if (onDragStart != null)
        {
            onDragEnd.Invoke();
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSetPosition && sVRect != null)
        {
            Vector2 v2Position = Input.mousePosition;// new Vector2(Screen.width / Input.mousePosition.x, Screen.width / Input.mousePosition.x);

            Vector3 v3 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            v2Position = new Vector2(v3.x * 1920 /*- bedPos.anchoredPosition.x*/, v3.y * 1080);
            sVRect.content.anchoredPosition = v2Position;// Input.mousePosition;
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
        if (Input.mousePosition.y > (v2DragStart.y + f_ChuPaiHight))
        {
            sVRect.content.localScale = Vector3.one * 1.5f;
        }
        else
        {
            sVRect.content.localScale = Vector3.one;
        }
    }

    //void OnGUI()
    //{
    //    GUI.Box(new Rect(128,128,256,128), sVRect.content.anchoredPosition+"鼠标位置：" +Input.mousePosition);
    //}
    // Use this for initialization
    void Start()
    {
        f_ChuPaiHight = (int)(Screen.height * 0.25f);
    }

}
