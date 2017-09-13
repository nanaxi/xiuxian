using UnityEngine;
using System.Collections;

public class TouZi_Rotate : MonoBehaviour
{

    public Transform tRotateObject;
    public uint i_PointsV;
    const int rotateSAngles = 32;
    // Use this for initialization
    void Start()
    {
        if (tRotateObject==null)
        {
            tRotateObject = transform;
        }
    }

    IEnumerator TouZiRotate()
    {
        float rotateTime = 1;
        while (rotateTime > 0.0f && tRotateObject != null )
        {
            tRotateObject.Rotate(rotateSAngles, rotateSAngles, rotateSAngles);
            yield return new WaitForSeconds(0.02f);
            rotateTime -= 0.08f;
        }
        if (tRotateObject!=null)
        {
            tRotateObject.localEulerAngles = TouZiEulerAngles.GetAngles()[i_PointsV];
        }
        yield return null;
    }

    public void TouZiStartRotate(uint i_Points)
    {
        i_PointsV = i_Points;
        StopCoroutine("TouZiRotate");
        StartCoroutine("TouZiRotate");
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(128, 128, 128, 128), "Start Rotate"))
    //    {
    //        StopCoroutine("TouZiRotate");
    //        StartCoroutine("TouZiRotate");
    //    }
    //}
}

/// <summary>//骰子点数与旋转值：
/// //1点：(180,0,0)//2点：(0,0,-90)//3点：(90,0,0)//4点：(0,0,90)//5点：(270,0,0)//6点：(0,0,0)
/// </summary>
public struct TouZiEulerAngles
{
    public Vector3[] anglesAry;
    //public TouZiEulerAngles(int i_1 = 0)
    //{
    //}
    static public Vector3[] GetAngles()
    {
        Vector3[] anglesAry = new Vector3[7];
        anglesAry[0] = Vector3.zero;
        anglesAry[1] = new Vector3(180, 0, 0);
        anglesAry[2] = new Vector3(0, 0, -90);
        anglesAry[3] = new Vector3(90, 0, 0);
        anglesAry[4] = new Vector3(0, 0, 90);
        anglesAry[5] = new Vector3(270, 0, 0);
        anglesAry[6] = new Vector3(0, 0, 0);
        return anglesAry;
    }
}