using UnityEngine;
using System.Collections;
using System;

namespace GPS
{
    /// <summary>根据纬度、经度、获取地址（中文）
    /// </summary>
    public class NeGetSite : MonoBehaviour
    {

        #region Instance Define
        static NeGetSite inst;

        static public NeGetSite Instance
        {
            get
            {
                if (inst == null)
                {
                    inst = new GameObject("__NeGetSite", typeof(NeGetSite)).GetComponent<NeGetSite>();
                }
                return inst;
            }
        }
        #endregion

        private Vector2 v2NE_1 = new Vector2(29.5687f, 106.5459f);//测距测试经纬度
        private Vector2 v2NE_2 = new Vector2(29.56811f, 106.5456f);//测距测试经纬度

        /// <summary>当接收到了地址执行的委托
        /// </summary>
        public Action<string> onRcvSite;

        public Vector2 v2CSite;//当前经纬度
                               /// <summary>地址
                               /// </summary>
        public string strDZ="定位中...";
        private string[] strAry;

        void Awake()
        {
            if (NeGetSite.inst != null && NeGetSite.inst != this)
            {
                Debug.Log("<color=red>TestNE.inst!=this</color>");
                Destroy(this);
                return;
            }
            inst = this;
        }

        // Use this for initialization
        void Start()
        {
            //string.

        }

        void OnDestroy()
        {
            inst = null;
        }
        

        public void Test_GetDis()
        {
            double dValue = GetGPS.GetDistance_A1(v2NE_1.x, v2NE_2.y, v2NE_2.x, v2NE_2.y);
            Debug.Log("距离：" + dValue);

        }

        /// <summary>获取地址，请设置委托onRcvSite，将返回中文地址
        /// </summary>
        /// <param name="v2NE"></param>
        public void GetSite(Vector2 v2NE, System.Action<string> gpsSite_ = null)
        {
            v2CSite = v2NE;
            eventGpsSite = gpsSite_;
            StartCoroutine(W3Rqs_DZ());
        }

        private System.Action<string> eventGpsSite;
        IEnumerator W3Rqs_DZ()
        {
            //根据纬度、经度获取地址
            WWW w3 = new WWW("http://maps.google.cn/maps/api/geocode/json?latlng=" + v2CSite.x + "," + v2CSite.y + "&language=CN");
            ////*山西*/"http://maps.google.cn/maps/api/geocode/json?latlng=36.0860410097,112.3409636363&language=CN");
            // /*重庆*/"http://maps.google.cn/maps/api/geocode/json?latlng=29.5687,106.5459&language=CN");
            yield return w3;
            if (w3.isDone && w3.error == null)
            {
                Debug.Log("Down Over :" + w3.text);
                strAry = w3.text.Split(new string[] { "formatted_address" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (strAry.Length > 2)
                {
                    strAry = strAry[1].Split('\n');
                    //" : "中国山西省临汾市安泽县009乡道",
                    strDZ = strAry[0];
                    strDZ = strDZ.Substring(strDZ.LastIndexOf("中国"), strDZ.Length - 7);//去掉符号
                }
                else
                {
                    strDZ = "定位失败！";
                }
                if (eventGpsSite != null)
                {
                    eventGpsSite(strDZ);
                    
                }

                if (onRcvSite != null && eventGpsSite == null)
                {//此委托仅本地玩家使用
                    onRcvSite(strDZ);
                }
                
            }
        }
    }
}

