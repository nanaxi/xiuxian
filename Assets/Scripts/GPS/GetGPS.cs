using UnityEngine;
using System.Collections;
using System;

namespace GPS
{
    /// <summary>获取经纬度
    /// </summary>
    public class GetGPS : MonoBehaviour
    {
        #region Instance Define
        static GetGPS inst;

        static public GetGPS Instance
        {
            get
            {
                if (inst == null)
                {
                    inst = new GameObject("__GetGPS", typeof(GetGPS)).GetComponent<GetGPS>();
                }
                return inst;
            }
        }
        #endregion

        public string gps_info = "暂无！";
        public int flash_num = 1;

        /// <summary>x=latitude, y=longitude
        /// </summary>
        public Vector2 v2NE;
        public System.Action<Vector2> onGPSSucc_;

        void Awake()
        {

            if (GetGPS.inst != null && GetGPS.inst != this)
            {
                Debug.Log("<color=red>GetGPS.inst!=this</color>");
                Destroy(this);
                return;
            }
            inst = this;
        }

        // Use this for initialization  
        void Start()
        {

        }

        void OnDestroy()
        {
            inst = null;
        }

        //void OnGUI()
        //{
        //    GUI.skin.label.fontSize = 28;
        //    GUILayout.Label(gps_info);
        //    GUILayout.Label(flash_num.ToString());
        //    GUILayout.Label("地址：" + NeGetSite.Instance.strDZ);//测试显示根据经纬度获取地址

        //    GUI.skin.button.fontSize = 20;
        //    if (GUI.Button(new Rect(Screen.width / 2 - 110, 0, 220, 85), "GPS定位"))
        //    {
        //        onGPSSucc_ = delegate (Vector2 v2NE_X)
        //        {
        //            NeGetSite.Instance.GetSite(v2NE_X);//测试显示根据经纬度获取地址
        //        };
        //        // 这里需要启动一个协同程序  
        //        StartCoroutine("StartGPS");

        //    }
        //    if (GUI.Button(new Rect(Screen.width / 2 - 110, 100, 220, 85), "刷新GPS"))
        //    {
        //        this.gps_info = "维度N:" + Input.location.lastData.latitude + "经度E:" + Input.location.lastData.longitude;
        //        this.gps_info = this.gps_info + "时间戳Time:" + Input.location.lastData.timestamp;
        //        this.flash_num += 1;
        //    }

        //    if (GUI.Button(new Rect(Screen.width / 2 - 110, 200, 220, 85), "GPS是否开启" + Input.location.isEnabledByUser))
        //    {

        //    }

        //    if (GUI.Button(new Rect(Screen.width / 2 - 110, 300, 220, 85), "GPS Stop" + Input.location.isEnabledByUser))
        //    {
        //        StopGPS();
        //        StopCoroutine("StartGPS");
        //    }
        //}

        // Input.location = LocationService  
        // LocationService.lastData = LocationInfo   

        /// <summary>开启定位协同程序
        /// </summary>
        public void StartGPS_IE()
        {
            //这里需要启动一个协同程序  
            StartCoroutine("StartGPS");
        }

        /// <summary>停止定位
        /// </summary>
        public void StopGPS()
        {
            Input.location.Stop();
        }

        IEnumerator StartGPS()
        {
            // Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置  
            // LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
            if (!Input.location.isEnabledByUser)
            {
                this.gps_info = "isEnabledByUser value is:" + Input.location.isEnabledByUser.ToString() + " Please turn on the GPS";
                yield return false;
            }

            // LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
            Input.location.Start(10.0f, 10.0f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                // 暂停协同程序的执行(1秒)  
                yield return GameManager.wait1;
                maxWait--;
            }

            if (maxWait < 1)
            {
                this.gps_info = "Init GPS service time out初始化GPS服务时间";
                yield return false;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                this.gps_info = "Unable to determine device location无法确定设备位置";
                yield return false;
            }
            else
            {
                this.gps_info = "纬度N:" + Input.location.lastData.latitude + "经度E:" + Input.location.lastData.longitude;
                this.gps_info = this.gps_info + " 时间戳Time:" + Input.location.lastData.timestamp;

                v2NE.x = Input.location.lastData.latitude;
                v2NE.y = Input.location.lastData.longitude;
                if (onGPSSucc_ != null)
                {
                    onGPSSucc_(v2NE);
                }
                StopGPS();
                yield return new WaitForSeconds(100);
            }
        }


        private const double EARTH_RADIUS = 6378.137;//地球半径

        /*
        地球上任意两点距离计算公式为 ： D＝R* arccos(siny1siny2+cosy1cosy2cos(x1-x2) )  其中:R为地球半径，均值为6370km. A点经、纬度分别为x1和y1,，东经为正，西经为负 B点经、纬度分别为x2和y2，北纬为正，南纬为负 用上述公式算得两点的距离为30.4km,与googleearth的基本一致。 注意的是经纬度是角度，算sin,cos值时先将其换算成弧度。
        */
        /// <summary>A点经、纬度分别为x1和y1,  B点经、纬度分别为x2和y2，
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double GetDistance_A1(double x1, double y1, double x2, double y2)
        {
            double s = EARTH_RADIUS * Math.Acos(Math.Sin(y1) * Math.Sin(y2) + Math.Cos(y1) * Math.Cos(y2) * Math.Cos(x1 - x2));
            s = System.Math.Round(s, 1);
            return s;
        }




    }
}


