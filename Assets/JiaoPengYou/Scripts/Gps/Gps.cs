using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Lang
{
    public class Gps
    {
        public const float EarthRadius = 6378.137F;

        Action<GpsData> onGpsData;
        Action<string> onGetCity;

        void OnGpsData(GpsData result)
        {
            if (onGpsData != null)
            {
                onGpsData(result);
            }
        }

        void OnGetCity(string result)
        {
            if (onGetCity != null)
            {
                onGetCity(result);
            }
        }

        public IEnumerator StartGps(Action<GpsData> onComplete)
        {
            this.onGpsData = onComplete;

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                OnGpsData(new GpsData() { result = GpsType.NotSupport });
                yield break;
            }

            if (!Input.location.isEnabledByUser)
            {
                OnGpsData(new GpsData() { result = GpsType.NotEnabled });
                yield break;
            }

            Input.location.Start(10.0f, 10.0f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return GameManager.wait1;
                maxWait--;
            }

            if (maxWait < 1)
            {
                OnGpsData(new GpsData() { result = GpsType.InitTimeout });
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                OnGpsData(new GpsData() { result = GpsType.Failed });
                yield break;
            }

            var info = Input.location.lastData;
            Vector2 v = new Vector2(info.latitude, info.longitude);
            OnGpsData(new GpsData() { latlng = v, result = GpsType.Success });

            Input.location.Stop();
        }

        /// <summary>
        /// <para>
        /// 获取地球上任意两点距离
        /// </para>
        /// 地球上任意两点距离计算公式为 ： D＝R* arccos(siny1siny2+cosy1cosy2cos(x1-x2) )
        /// <para>
        /// 其中:R为地球半径，均值为6370km. A点经、纬度分别为x1和y1,，东经为正，西经为负 B点经、纬度分别为x2和y2，北纬为正，南纬为负
        /// </para>
        /// <para>
        /// 用上述公式算得两点的距离为30.4km,与googleearth的基本一致。 注意的是经纬度是角度，算sin,cos值时先将其换算成弧度。
        /// </para>
        /// </summary>
        public static double CalcDistance(Vector2 from, Vector2 to)
        {
            double s = EarthRadius * Math.Acos(Math.Sin(from.y) * Math.Sin(to.y) + Math.Cos(from.y) * Math.Cos(to.y) * Math.Cos(from.x - to.x));
            return Math.Round(s, 1);
        }

        public IEnumerator GetCity(Vector2 latlng, Action<string> onGetCity)
        {
            // 例子
            // 山西 http://maps.google.cn/maps/api/geocode/json?latlng=36.0860410097,112.3409636363&language=CN
            // 重庆 http://maps.google.cn/maps/api/geocode/json?latlng=29.5687,106.5459&language=CN

            this.onGetCity = onGetCity;

            if (latlng == Vector2.zero)
            {
                OnGetCity("");
                yield break;
            }

            // 根据纬度、经度获取地址
            WWW www = new WWW("http://maps.google.cn/maps/api/geocode/json?latlng=" + latlng.x + "," + latlng.y + "&language=CN");
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                OnGetCity("");
                yield break;
            }

            string json = www.text;

            if (string.IsNullOrEmpty(json))
            {
                OnGetCity("");
                yield break;
            }

            Dictionary<string, object> jsonData = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;

            if (!jsonData["status"].Equals("OK"))
            {
                OnGetCity("");
                yield break;
            }

            string city = "";
            try
            {
                var results = jsonData["results"] as List<object>;
                var result = results[0] as Dictionary<string, object>;
                var addressComs = result["address_components"] as List<object>;
                var names = addressComs[2] as Dictionary<string, object>;
                city = (string)names["short_name"];
            }
            catch (Exception e)
            {
                Debug.LogError("get city data failed : " + e.Message);
                yield break;
            }

            OnGetCity(city);
        }
    }

    public enum GpsType
    {
        NotSupport,
        NotEnabled,
        InitTimeout,
        Failed,
        Success
    }

    public struct GpsData
    {
        public GpsType result;

        public Vector2 latlng;
    }
}