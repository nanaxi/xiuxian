using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class PopChatText : MonoBehaviour
    {
        public Text text;
        public ContentSizeFitter sizeFilter;

        public float showTime = 3;

        IEnumerator handle;

        public void Show(string str)
        {
            gameObject.SetActive(true);

            text.text = str;

            sizeFilter.enabled = true;
            var width = text.preferredWidth;
            sizeFilter.enabled = false;

            var textRt = text.transform as RectTransform;
            var textSize = textRt.sizeDelta;
            var textSizeX = Mathf.Clamp(textSize.x, 0, 390);
            textRt.sizeDelta = new Vector2(textSizeX, textSize.y);
            textRt.offsetMin = new Vector2(10, textRt.offsetMin.y);
            textRt.offsetMax = new Vector2(-15, textRt.offsetMax.y);

            width = Mathf.Clamp(width + 25, 221, 415);
            var rt = transform as RectTransform;
            var size = rt.sizeDelta;
            size = new Vector2(width, size.y);
            rt.sizeDelta = size;

            if (handle != null)
            {
                StopCoroutine(handle);
            }
            handle = WaitHide();
            StartCoroutine(handle);
        }

        IEnumerator WaitHide()
        {
            yield return new WaitForSeconds(showTime);
            gameObject.SetActive(false);
        }
    }
}