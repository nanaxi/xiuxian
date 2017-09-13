using UnityEngine;
using System.Collections;

public class TestRank_Pq : MonoBehaviour {

    public GameObject gmRes_MJ;

    public Vector2 v2_MJPQ;
	// Use this for initialization
	void Start () {
        
	}

    public void PaiXu()
    {
        int i_Count = 0;
        for (int i = 0; i < 17; i++)
        {
            for (int i1 = 0; i1 < 2; i1++)
            {
                GameObject gmMj = Instantiate(gmRes_MJ);
                gmMj.transform.SetParent(transform);
                gmMj.transform.localEulerAngles = Vector3.zero;
                gmMj.transform.position = new Vector3(v2_MJPQ.x * i, 0, v2_MJPQ.y * i1);
                i_Count++;
                gmMj.name = "MJPQ_" + i_Count;
                //Debug.Log("");
            }
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("排序牌墙"))
        {
            PaiXu();
        }
    }
}
