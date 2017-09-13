using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Lang
{
    public class GamingTimerView : View
    {
        IEnumerator updateTime;

        public void Show(Seat seat)
        {
            Clear();
            Transform t = transform.GetChild((int)seat);
            t.gameObject.SetActive(true);

            StartTimer(t);
        }

        public void Clear()
        {
            StopTimer();
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void StopTimer()
        {
            if (updateTime != null)
            {
                StopCoroutine(updateTime);
            }
        }

        void StartTimer(Transform t)
        {
            StopTimer();

            var txtTime = t.GetComponentInChildren<Text>();
            txtTime.text = Config.GamingTimeTip.ToString();
            updateTime = Util.UpdateTime(Config.GamingTimeTip, c =>
            {
                txtTime.text = c.ToString();
                if (c == 5)
                {
                    Lang.Log.D("倒计时", "TODO 倒数5秒了，手机震动");
                }
            }, null);
            StartCoroutine(updateTime);
        }
    }
}