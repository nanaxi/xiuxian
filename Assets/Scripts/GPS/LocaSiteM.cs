using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GPS
{

    /// <summary>一个显示地理位置的Text
    /// </summary>
    [RequireComponent(typeof(Text), typeof(ContentSizeFitter))]
    public class LocaSiteM : MonoBehaviour
    {

        /// <summary>显示本地地址
        /// </summary>
        public Text tLocaSite;
        private Vector2 v2NE;
        // Use this for initialization
        void Start()
        {
            tLocaSite = GetComponent<Text>();
            //tLocaSite.text = "";
            tLocaSite.text = NeGetSite.Instance.strDZ;//当根据经纬度获取地址完成，执行委托显示地址。
            //UpdateSite();
            NeGetSite.Instance.onRcvSite = delegate (string strSite) {
                Debug.Log("执行了吗？");
                tLocaSite.text = strSite;
            };
        }


        public void UpdateSite()
        {
            tLocaSite.text = "定位中...";
            GetGPS.Instance.onGPSSucc_ = delegate (Vector2 v2) {
                //定位完成获取到经纬度。
                v2NE = v2;
                //DataManage.Instance.v2GPS_NE = v2NE;
                NeGetSite.Instance.onRcvSite = delegate (string strSite) {
                    
                    //Destroy(GetGPS.Instance.gameObject);//销毁定位
                    //Destroy(NeGetSite.Instance.gameObject);//销毁获取地址
                };
                NeGetSite.Instance.GetSite(v2NE);//根据经纬度获取地址
            };

            GetGPS.Instance.StartGPS_IE();//获取经纬度
        }
    }
}


