using UnityEngine;
using System.Collections;

public class HandCard_T : MonoBehaviour {

    public Vector3 v3_LZ_Position;
    public Vector3 v3_ZT_Position;

    public Vector3 v3_LZ_Rotation;
    public Vector3 v3_ZT_Rotation;
    //rotation
    // Use this for initialization
    void Start () {

	}

    /// <summary>立着
    /// </summary>
    public void TS_LiZhe()
    {
        transform.position = v3_LZ_Position;
        transform.localEulerAngles = v3_LZ_Rotation;
    }

    /// <summary>正躺
    /// </summary>
    public void TS_ZhengTang()
    {
        transform.position = v3_ZT_Position;
        transform.localEulerAngles = v3_ZT_Rotation;
    }
}
