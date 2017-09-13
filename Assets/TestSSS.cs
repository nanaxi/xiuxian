using UnityEngine;
using UnityEngine.UI;
public class TestSSS : MonoBehaviour {

    public Slider _slider;
    public Text _butterryLevel;
    public Text _signalStrength;

    void Start()
    {
        // 开启协程 并保持更新  
       // InvokeRepeating("UpdateUI", 1, 10f);
    }

    void UpdateUI()
    {

#if !UNITY_EDITOR && UNITY_ANDROID
        // 电量相关的设置  
        _butterryLevel.text = "电量级别：" + BatteryState() + "电量：" + BatteryLevel();  
        _slider.value = BatteryLevel() *1f/100;  
  
  
        if (Application.internetReachability== NetworkReachability.NotReachable)                
        {   
            _signalStrength.text = "没有任何可用的网络！";  
        }      
          
        //当用户使用WiFi时    
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)                 
        {   
            var value = GetWIFISignalStrength();  
            _signalStrength.text = "wifi：" + value + "格子数： " + GetSignalLevel(value);  
        }                           
        //当用户使用移动网络时    
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)               
        {  
            var value = GetTeleSignalStrength();  
            _signalStrength.text = "信号：" + value + "格子数： " + GetSignalLevel(value);                 
        }  
 
#endif
    }

    /// <summary>  
    /// 返回应该显示的格子数（共5个）  
    /// </summary>  
    /// <param name="value"></param>  
    /// <returns></returns>  
    public static int GetSignalLevel(int value)
    {
        //1、当信号大于等于 - 85dBm时候，信号显示满格  
        //2、当信号大于等于 - 95dBm时候，而小于 - 85dBm时，信号显示4格  
        //3、当信号大于等于 - 105dBm时候，而小于 - 95dBm时，信号显示3格，不好捕捉到。  
        //4、当信号大于等于 - 115dBm时候，而小于 - 105dBm时，信号显示2格，不好捕捉到。  
        //5、当信号大于等于 - 140dBm时候，而小于 - 115dBm时，信号显示1格，不好捕捉到。  
        if (value > -85)
        {
            return 5;
        }
        else if (value < -85 && value > -95)
        {
            return 4;
        }
        else if (value < -95 && value > -105)
        {
            return 3;
        }
        else if (value < -105 && value > -115)
        {
            return 2;
        }
        else if (value < -115 && value > -140)
        {
            return 1;
        }

        return -1;
    }

    public static int CallStatic(string className, string methodName, params object[] args)
    {
//#if UNITY_ANDROID && !UNITY_EDITOR
        try  
        {  
            string CLASS_NAME = "com.mengxi.mm.GetBattery";     //  package.classname  
            AndroidJavaObject bridge = new AndroidJavaObject(CLASS_NAME);

            int value = bridge.CallStatic<int>(methodName, args);
            return value;

        }   
        catch (System.Exception ex)  
        {  
            Debug.LogWarning(ex.Message);  
        }  
 
 
//#endif
        return -1;
    }


    // 返回状态   是枚举-1,0,1,2   如下面  
    public static int BatteryState()
    {
        return CallStatic("GetBattery", "BatteryState");
    }

    // 返回剩余电量，  满电是100  
    public static int BatteryLevel()
    {
        return CallStatic("GetBattery", "BatteryLevel");
    }

    // 返回WIFI      返回的是负值  越靠近0 越强  
    public static int GetWIFISignalStrength()
    {
        return CallStatic("GetWIFIRssi", "GetWIFISignalStrength");
    }

    // 返回Telephone  
    public static int GetTeleSignalStrength()
    {
        return CallStatic("GetWIFIRssi", "GetTeleSignalStrength");
    }

}
