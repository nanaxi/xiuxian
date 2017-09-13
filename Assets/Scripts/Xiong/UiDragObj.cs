using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UiDragObj : MonoBehaviour ,IBeginDragHandler ,IDragHandler{
    public RectTransform m_dragRect;
    public RectTransform m_dragObj;
    public RectTransform m_Canvas;

    public bool isClampDragRect = true;
    public bool isBeginDragSetAsLastSibling = true;
    [SerializeField]
    private Vector2 v2_BeginDragPosition;
    [SerializeField]
    private Vector2 v2_BeginDragObjPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_dragObj != null && m_dragRect != null)
        {
            v2_BeginDragPosition = m_dragRect.anchoredPosition;
            v2_BeginDragObjPosition = m_dragObj.anchoredPosition;
        }
        if (m_Canvas==null)
        {
            m_Canvas = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_dragObj!=null && m_dragRect != null)
        {
            if (isClampDragRect && m_Canvas!=null)
            {//是否限制在画布以内
                m_dragRect.anchoredPosition = new Vector2(Mathf.Clamp(m_dragRect.anchoredPosition.x, 0, m_Canvas.sizeDelta.x - m_dragRect.sizeDelta.x),
                    Mathf.Clamp(m_dragRect.anchoredPosition.y, 0, m_Canvas.sizeDelta.y- m_dragRect.sizeDelta.y));
            }

            m_dragObj.anchoredPosition = v2_BeginDragObjPosition+(m_dragRect.anchoredPosition - v2_BeginDragPosition);
            if (isBeginDragSetAsLastSibling)
            {//是否设置UI层级为显示在最前方
                m_dragRect.GetComponentInParent<ScrollRect>().transform.SetAsLastSibling();
                m_dragObj.SetAsLastSibling();
            }
            
        }
    }

}
