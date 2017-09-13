using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MemoryPool_3D : MonoBehaviour {
    #region Instance Define
    static MemoryPool_3D inst = null;
    static public MemoryPool_3D Instance
    {
        get
        {
            return inst;
        }
    }
    #endregion
    private Transform t_MemoryPool_Parent;
    [SerializeField]private List<GameObject> list_PsPool = new List<GameObject>();
    [SerializeField]
    private List<GameObject> list_MJPool = new List<GameObject>();
    public float f_deletePSTime;//原来是2s
    // Use this for initialization
    void Awake () {

        inst = this;
        t_MemoryPool_Parent = transform.Find("MemoryPool_Parent");
        f_deletePSTime = 2.0f;
    
    }
    void Start()
    {
        StartCoroutine(Init_CreateAllCard());
    }
    IEnumerator Init_CreateAllCard()
    {
        GameObject[] mj3d = new GameObject[4];
        for (int i = 0; i < 27; i++)
        {
            mj3d[0] = MemoryPool_3D.Instance.MJ3D_GetModel(DataManage.Instance.mjName_Ary[i]);
            mj3d[1] = MemoryPool_3D.Instance.MJ3D_GetModel(DataManage.Instance.mjName_Ary[i]);
            mj3d[2] = MemoryPool_3D.Instance.MJ3D_GetModel(DataManage.Instance.mjName_Ary[i]);
            mj3d[3] = MemoryPool_3D.Instance.MJ3D_GetModel(DataManage.Instance.mjName_Ary[i]);
            MemoryPool_3D.Instance.MJ3D_Recycle(mj3d[0]);
            MemoryPool_3D.Instance.MJ3D_Recycle(mj3d[1]);
            MemoryPool_3D.Instance.MJ3D_Recycle(mj3d[2]);
            MemoryPool_3D.Instance.MJ3D_Recycle(mj3d[3]);
        }
        Debug.Log("Create MJ Pool Over");
        yield return null;
    }

    #region/*———Special Effects———*/

    public void PSPool_IfNull()
    {
        for (int i = 0; i < list_PsPool.Count; i++)
        {
            if (list_PsPool[i]==null)
            {
                list_PsPool.Remove(list_PsPool[i]);
            }
        }
    }

    /// <summary> Show   special effects
    /// </summary>
    public GameObject GetShowPS_Model(string psName ,Transform t_Praent)
    {
        Debug.Log("特效名字:"+ psName);
        PSPool_IfNull();
        GameObject gmValue=null;
        for (int i = 0; i < list_PsPool.Count; i++)
        {
            if (list_PsPool[i].transform.parent == t_MemoryPool_Parent && 
                list_PsPool[i].name == psName &&
                !list_PsPool[i].activeInHierarchy
                )
            {
                gmValue = list_PsPool[i];
                gmValue.SetActive(true);
                list_PsPool[i].transform.SetParent(t_Praent);
                list_PsPool[i].transform.position = t_Praent.position;
                gmValue.layer = gmValue.transform.parent.gameObject.layer;
                StartCoroutine(WaitTime_Invoke(0.1f, delegate ()
                {
                    gmValue.transform.SetParent(transform);
                }));
                StartCoroutine(WaitTime_Invoke(f_deletePSTime, delegate ()
                {
                    if (gmValue!=null && t_MemoryPool_Parent!=null)
                    {
                        gmValue.transform.SetParent(t_MemoryPool_Parent);
                        gmValue.SetActive(false);
                    }
                }));
                return list_PsPool[i];
            }      
        }
        GameObject gmRes = Resources.Load<GameObject>("Prefabs/SE/" + psName);
        if (gmRes==null)
        {
            Debug.LogError("Resources.Load<GameObject> :"+ "Prefabs/SE/" + psName+"== NULL");
        }
        GameObject gmIns = Instantiate(gmRes) as GameObject;
        gmIns.gameObject.name = gmRes.name;
        gmIns.transform.SetParent(t_MemoryPool_Parent);
        gmIns.transform.localScale = gmRes.transform.localScale;
        gmIns.transform.position = Vector3.zero;
        gmIns.gameObject.SetActive(false);
        list_PsPool.Add(gmIns);
        return GetShowPS_Model(psName, t_Praent);
    }

    public void PsPool_ClearAll()
    {
        for (int i = 0; i < list_PsPool.Count; i++)
        {
            if (list_PsPool[i]!=null)
            {
                Destroy(list_PsPool[i]);
            }
        }
        list_PsPool.Clear();
    }
    #endregion

    #region/*———MJ Pool———*/
    /// <summary> Show   special effects
    /// </summary>
    public GameObject MJ3D_GetModel(string mjName, Transform t_Praent = null, bool isActive = false)
    {
        GameObject gmValue = null;
        for (int i = 0; i < list_MJPool.Count; i++)
        {
            if (list_MJPool[i].transform.parent == t_MemoryPool_Parent &&
                list_MJPool[i].name == mjName &&
                !list_MJPool[i].activeInHierarchy
                )
            {
                gmValue = list_MJPool[i];
                gmValue.SetActive(isActive);
                list_MJPool[i].transform.SetParent(t_Praent);
                //list_MJPool[i].transform.position = t_Praent.position;

                return list_MJPool[i];
            }
        }
        GameObject gmRes = Resources.Load<GameObject>("Prefabs/MJ/" + mjName);
        if (gmRes == null)
        {
            Debug.LogError("Resources.Load<GameObject> :" + "Prefabs/MJ/" + mjName + "== NULL");
        }
        GameObject gmIns = Instantiate(gmRes) as GameObject;
        gmIns.gameObject.name = gmRes.name;
        gmIns.transform.SetParent(t_MemoryPool_Parent);
        gmIns.transform.localScale = gmRes.transform.localScale;
        gmIns.transform.position = Vector3.zero;
        gmIns.gameObject.SetActive(false);
        list_MJPool.Add(gmIns);
        return MJ3D_GetModel(mjName, t_Praent, isActive);
    }

    /// <summary>Other Player Random Get MJ Card
    /// </summary>
    public GameObject MJ3D_GetRandomModel(string mjName, Transform t_Praent = null, bool isActive = false)
    {
        GameObject gmValue = null;
        for (int i = 0; i < list_MJPool.Count; i++)
        {
            if (list_MJPool[i].transform.parent == t_MemoryPool_Parent &&
                !list_MJPool[i].activeInHierarchy
                )
            {
                gmValue = list_MJPool[i];
                gmValue.SetActive(isActive);
                list_MJPool[i].transform.SetParent(t_Praent);
                return list_MJPool[i];
            }
        }

        return MJ3D_GetModel("6H", t_Praent, isActive);
    }

    /// <summary>MJ3D 回收
    /// </summary>
    public void MJ3D_Recycle(GameObject mjGmObj)
    {
        if (mjGmObj != null)
        {
            mjGmObj.SetActive(false);
            mjGmObj.transform.SetParent(t_MemoryPool_Parent);
        }
        else
        {
            Debug.Log("<color=red>MJ3D_Recycle -> mjGmObj==Null</color>");
        }
    }

    /// <summary>MJ3D 回收
    /// </summary>
    public void MJ3D_RecycleALL()
    {
        for (int i = 0; i < list_MJPool.Count; i++)
        {
            if (list_MJPool[i] != null)
            {
                list_MJPool[i].SetActive(false);
                list_MJPool[i].transform.SetParent(t_MemoryPool_Parent);
            }
        }
    }

    public void MJ3D_ClearAll()
    {
        for (int i = 0; i < list_MJPool.Count; i++)
        {
            if (list_MJPool[i] != null)
            {
                Destroy(list_MJPool[i]);
            }
        }
        list_MJPool.Clear();
    }
    #endregion

    IEnumerator WaitTime_Invoke(float waitTime,UnityEngine.Events.UnityAction event_)
    {
        yield return new WaitForSeconds(waitTime);
        event_.Invoke();
        yield return null;
    }

}
