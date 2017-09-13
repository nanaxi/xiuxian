using UnityEngine;
using System.Collections;
using System;

namespace Lang
{
    public class GpsManager : MonoBehaviour
    {
        static GpsManager inst;

        public static GpsManager Inst
        {
            get
            {
                if (inst == null)
                {
                    var o = new GameObject("GpsManager");
                    inst = o.AddComponent<GpsManager>();
                }

                return inst;
            }
        }

        Action<Vector2> onGetGps;
        Action<string> onGetCity;

        void OnDestroy()
        {
            Destroy(inst.gameObject);
            inst = null;
        }

        public void GetGps(Action<Vector2> onGetGps)
        {
            this.onGetGps = onGetGps;
            StartCoroutine(StartGetGps());
        }

        IEnumerator StartGetGps()
        {
            Gps gps = new Gps();
            GpsData gpsData = new GpsData();

            yield return StartCoroutine(gps.StartGps(data =>
            {
                gpsData = data;
            }));

            Vector2 latlng = Vector2.zero;
            if (gpsData.result == GpsType.Success)
            {
                latlng = gpsData.latlng;
            }
            else
            {
                latlng = Vector2.zero;
                Log.D("GPS", "获取定位数据失败 : " + gpsData.result);
            }

#if UNITY_STANDALONE || UNITY_EDITOR
            // 编辑器下测试数据
            long tick = DateTime.Now.Ticks;
            int seed = (int)(tick & 0xffffffffL) | (int)(tick >> 32);
            UnityEngine.Random.seed = seed;

            var x = UnityEngine.Random.Range(28F, 37F);
            var y = UnityEngine.Random.Range(106F, 111F);
            latlng = new Vector2(x, y);
#endif

            if (onGetGps != null)
            {
                onGetGps(latlng);
            }
        }

        public void GetCity(Vector2 latlng, Action<string> onGetCity)
        {
            this.onGetCity = onGetCity;

            Gps gps = new Gps();
            StartCoroutine(gps.GetCity(latlng, s =>
            {
                if (onGetCity != null)
                {
                    onGetCity(s);
                }
            }));
        }
    }
}