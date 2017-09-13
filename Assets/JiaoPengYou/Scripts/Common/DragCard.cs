using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lang
{
    /// <summary>
    /// 可以提牌组合牌
    /// </summary>
    public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        RectTransform draggingCard;

        Canvas canvas;
        Transform handView;
        Vector3 originalPosition;
        int originalSibling;

        Transform upLeft;
        Transform bottomLeft;

        float backTime = 0.1F;
        float moveDuration = 0.1F;

        bool dragging;

        void OnApplicationFocus(bool focus)
        {
            if (!focus && dragging)
            {
                PointerEventData ped = new PointerEventData(EventSystem.current);
                OnEndDrag(ped);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!enabled)
            {
                return;
            }

            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            handView = transform.parent;
            draggingCard = transform as RectTransform;

            var halfWidth = draggingCard.rect.width * 0.5F;
            var halfHeight = draggingCard.rect.height * 0.5F;

            upLeft = CreateCorner("UpLeft", new Vector2(-halfWidth, halfHeight));
            bottomLeft = CreateCorner("BottomLeft", new Vector2(-halfWidth, -halfHeight));

            originalPosition = draggingCard.anchoredPosition;
            originalSibling = draggingCard.GetSiblingIndex();

            transform.SetAsLastSibling();

            SetDraggedPosition(eventData);
            SetSelfCardOperation(false);
            dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enabled)
            {
                return;
            }
            if (!dragging)
            {
                return;
            }

            SetDraggedPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!enabled)
            {
                return;
            }

            dragging = false;

            var upLeftScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, upLeft.position);
            var bottomLeftScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, bottomLeft.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(handView as RectTransform, upLeftScreenPos, eventData.pressEventCamera) ||
                RectTransformUtility.RectangleContainsScreenPoint(handView as RectTransform, bottomLeftScreenPos, eventData.pressEventCamera))
            {
                InRangeOfDrag(upLeftScreenPos, bottomLeftScreenPos);
            }
            else
            {
                OutsideRangeOfDrag();
            }

            Destroy(upLeft.gameObject);
            Destroy(bottomLeft.gameObject);
        }

        void InRangeOfDrag(Vector2 upLeftScreenPos, Vector2 bottomLeftScreenPos)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = upLeftScreenPos;
            var currentSibling = -1;
            List<RaycastResult> results = new List<RaycastResult>();
            canvas.GetComponent<GraphicRaycaster>().Raycast(ped, results);
            results = RaycastFilter(results);
            if (results.Count == 0)
            {
                ped.position = bottomLeftScreenPos;
                canvas.GetComponent<GraphicRaycaster>().Raycast(ped, results);
                results = RaycastFilter(results);
            }
            if (results.Count > 0)
            {
                currentSibling = results[0].gameObject.transform.GetSiblingIndex() + 1;
            }
            else
            {
                currentSibling = GetSiblingFromTwoSide();
            }
            MoveCard(currentSibling);
        }

        void OutsideRangeOfDrag()
        {
            SetAllCardOperation(false);
            transform.SetSiblingIndex(originalSibling);
            draggingCard.DOAnchorPos(originalPosition, backTime).OnComplete(() => SetAllCardOperation(true));
        }

        void MoveCard(int currentSibling)
        {
            if (currentSibling < 0)
                return;

            List<Vector3> movePositions = new List<Vector3>();
            List<int> siblingIndexs = new List<int>();

            // 所有牌左移动
            if (currentSibling >= originalSibling)
            {
                for (int i = originalSibling; i < currentSibling; i++)
                    siblingIndexs.Add(i);
            }
            else
            {
                for (int i = originalSibling - 1; i >= currentSibling; i--)
                    siblingIndexs.Add(i);
            }

            SetAllCardOperation(false);

            movePositions.Add(originalPosition);
            foreach (var i in siblingIndexs)
            {
                Vector3 pos = (handView.GetChild(i).transform as RectTransform).anchoredPosition;
                movePositions.Add(pos);
            }
            int posIndex = 0;
            foreach (var i in siblingIndexs)
            {
                var toPos = movePositions[posIndex++];
                (handView.GetChild(i).transform as RectTransform).DOAnchorPos(toPos, moveDuration);
            }
            draggingCard.DOAnchorPos(movePositions[movePositions.Count - 1], moveDuration).OnComplete(() => SetAllCardOperation(true));

            transform.SetSiblingIndex(currentSibling);
        }

        void SetDraggedPosition(PointerEventData eventData)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingCard, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                draggingCard.position = globalMousePos;
            }
        }

        int GetSiblingFromTwoSide()
        {
            int currentSibling;
            // childCount == 1时，为自己本身
            if (handView.childCount > 1)
            {
                // 在两边
                if (transform.position.x <= handView.GetChild(0).position.x)
                {
                    currentSibling = 0;
                }
                else
                {
                    // 有可能是在中间的牌的中间，并不是在最右边(这里因为牌布局的原因不需要判断是否在右边的中间)
                    if (transform.position.x > handView.GetChild(handView.childCount - 2).position.x)
                    {
                        currentSibling = handView.childCount - 1;
                    }
                    else
                    {
                        currentSibling = originalSibling;
                    }
                }
            }
            else
            {
                currentSibling = 0;
            }

            return currentSibling;
        }

        Transform CreateCorner(string name, Vector2 position)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(transform, false);
            (go.transform as RectTransform).anchoredPosition = position;
            return go.transform;
        }

        // 过滤掉其他UI的raycast
        List<RaycastResult> RaycastFilter(List<RaycastResult> results)
        {
            if (results.Count == 0)
            {
                return results;
            }
            var list = results.FindAll(r => r.gameObject.transform.IsChildOf(handView));
            return list;
        }

        // 防止快速操作导致最终显示位置不一致
        void SetAllCardOperation(bool op)
        {
            var imgs = handView.GetComponentsInChildren<Image>();
            foreach (var img in imgs)
            {
                img.raycastTarget = op;
            }
        }

        void SetSelfCardOperation(bool op)
        {
            GetComponent<Image>().raycastTarget = op;
        }
    }
}