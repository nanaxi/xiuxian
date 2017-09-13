using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ProtoBuf;

namespace Lang
{
    public class TopStatusBarView : View
    {
        public Button rule;
        public Text roomId;
        public Text roundCount;
        public Text notice;
        public Text date;
        public Text time;
        public Text batteryText;
        string versionNumberUrl = "http://www.xiuxianchaguan.com/versionNumber.txt";

        PokerRoomRuleInfo ruleInfo;

        void Start()
        {
            rule.onClick.AddListener(() =>
            {
                ShowRuleButton(false);
                ViewManager.Open<RoomRuleView>().ShowRuleInfo(ruleInfo);
            });
            StartCoroutine(SetConfig());
            StartCoroutine(UpdateTime());
            StartCoroutine(UpdataBattery());
        }
        IEnumerator SetConfig()
        {
            WWW www = new WWW(versionNumberUrl);
            yield return www;
            string[] sArray = www.text.Split('|');

            for (int i = 0; i < sArray.Length; i++)
            {
                Debug.Log(sArray[i]);
            }


            Debug.Log("我获取了提示信息+main");
            LoadConfig(sArray[7]);
        }
        void LoadConfig(string context)
        {
            Debug.Log(context);
            if (context != null)
            {
                SetNotice(context);
            }

        }
        IEnumerator UpdateTime()
        {
            while (true)
            {
                date.text = DateTime.Now.ToString("yyyy年M月dd日");
                time.text = DateTime.Now.ToString("HH:mm");
                yield return GameManager.wait1;
            }
        }

        public void SetBasicInfo(uint roomId, uint currentRound, uint maxRound)
        {
            this.roomId.text = string.Format("房间号：{0}", roomId);
            roundCount.text = string.Format("当前局数：{0} / {1}", currentRound, maxRound);
        }

        public void SetRuleInfo(PokerRoomRuleInfo ruleInfo)
        {
            this.ruleInfo = ruleInfo;
        }

        public void ShowRuleButton(bool show)
        {
            rule.gameObject.SetActive(show);
        }

        public void SetNotice(string msg)
        {
            notice.text = msg;
            notice.GetComponentInParent<WorldMessageMoveAnim>().Play();
        }

        IEnumerator UpdataBattery()
        {
            while (true)
            {
                //此处的battery是一个百分比数字，比如电量是93%，则这个数字是93
                batteryText.text = GetBatteryLevel().ToString();

                yield return new WaitForSeconds(60);
            }
        }

        int GetBatteryLevel()
        {
            try
            {
                string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                return int.Parse(CapacityString);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to read battery power; " + e.Message);
            }
            return 100;
        }
    }
}