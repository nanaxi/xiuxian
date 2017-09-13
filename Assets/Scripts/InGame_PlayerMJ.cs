using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>代表一个玩家在操作麻将
/// </summary>
public class InGame_PlayerMJ : MonoBehaviour
{
    public Transform t_ShouPai_Parent;//拖拽赋值
    public Transform t_PaiQiang_Parent;//拖拽赋值
    public Transform t_ChuPai_Parent;//拖拽赋值
    public Transform t_PengGang_Parent;//拖拽赋值
    public List<MJInfos> list_ShouPai;
    public List<MJInfos> list_ChuPai;
    public List<GameObject> list_PaiQiang;
    public List<MJInfos[]> list_PengGang;
    [SerializeField]
    private int paiQ_Count;
    public MJInfos mj_MoPai;
    public MJInfos mj_HuPaiPosition;
    public bool isInit = false;

    public GameObject gm_MoPaiMove;
    // Use this for initialization
    void Awake()
    {
        if (!isInit)
        {
            Init_SetVar();
        }
    }

    public void Init_SetVar()
    {
        isInit = false;
        list_ShouPai = t_ShouPai_Parent != null ? new List<MJInfos>(t_ShouPai_Parent.GetComponentsInChildren<MJInfos>()) : new List<MJInfos>();
        list_ChuPai = t_ChuPai_Parent != null ? new List<MJInfos>(t_ChuPai_Parent.GetComponentsInChildren<MJInfos>()) : new List<MJInfos>();

        if (t_PaiQiang_Parent != null)
        {
            list_PaiQiang = new List<GameObject>();
            for (int i = 0; i < t_PaiQiang_Parent.childCount; i++)
            {
                GameObject gm_Pq = t_PaiQiang_Parent.GetChild(i).gameObject;
                list_PaiQiang.Add(gm_Pq);
                gm_Pq.SetActive(false);
            }
        }
        paiQ_Count = 0;
        if (t_PengGang_Parent != null)
        {//碰杠数组
            list_PengGang = new List<MJInfos[]>();
            for (int i = 0; i < t_PengGang_Parent.childCount; i++)
            {
                MJInfos[] pgAry = t_PengGang_Parent.GetChild(i).GetComponentsInChildren<MJInfos>();
                list_PengGang.Add(pgAry);
            }
            Debug.Log("Length :" + list_PengGang.Count);
        }

        isInit = true;
    }

    public void Init_MjAll(bool b_ = false)
    {
        for (int i = 0; i < list_PaiQiang.Count; i++)
        {
            list_PaiQiang[i].gameObject.SetActive(false);
        }
        paiQ_Count = 0;
        for (int i = 0; i < list_ChuPai.Count; i++)
        {
            if (list_ChuPai[i].transform.childCount > 0)
            {
                list_ChuPai[i].MjINFO_ = new Mj_Sx_();
                GameObject mjCP = list_ChuPai[i].transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(mjCP);
            }
        }
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                list_ShouPai[i].MjINFO_ = new Mj_Sx_();
                GameObject mjSP = list_ShouPai[i].transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(mjSP);
            }
        }

        for (int i = 0; i < list_PengGang.Count; i++)
        {
            for (int i1 = 0; i1 < list_PengGang[i].Length; i1++)
            {
                list_PengGang[i][i1].MjINFO_ = new Mj_Sx_();
                if (list_PengGang[i][i1].transform.childCount > 0)
                {
                    GameObject mjPG = list_PengGang[i][i1].transform.GetChild(0).gameObject;
                    MemoryPool_3D.Instance.MJ3D_Recycle(mjPG);
                }
            }
        }
        if (mj_MoPai != null)
        {
            mj_MoPai.MjINFO_ = new Mj_Sx_();
            if (mj_MoPai.transform.childCount > 0)
            {
                GameObject mjPG = mj_MoPai.transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(mjPG);
            }
        }

        if (mj_HuPaiPosition != null && mj_HuPaiPosition.transform.childCount > 0)
        {
            mj_HuPaiPosition.MjINFO_ = new Mj_Sx_();
            GameObject mjPG = mj_HuPaiPosition.transform.GetChild(0).gameObject;
            MemoryPool_3D.Instance.MJ3D_Recycle(mjPG);
            mj_HuPaiPosition.gameObject.SetActive(false);
        }
    }
    public void Init_MjAll_ShouPai()
    {
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                list_ShouPai[i].MjINFO_ = new Mj_Sx_();
                GameObject mjSP = list_ShouPai[i].transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(mjSP);
            }
        }
    }

    IEnumerator Intantiate_MJ(GameObject gmIns)
    {
        gmIns.transform.SetParent(ChuPai_GetParent());
        gmIns.transform.localEulerAngles = Vector3.zero;
        gmIns.transform.localPosition = new Vector3(0, 0, 0.08f);
        gmIns.transform.localScale = Vector3.one;

        //设置麻将的材质灯光层级 麻将材质跟父物体一样
        gmIns.GetComponent<MeshRenderer>().material = gmIns.transform.parent.GetComponentInParent<MeshRenderer>().material;
        gmIns.layer = gmIns.transform.parent.gameObject.layer;

        yield return new WaitForSeconds(0.05f);
        gmIns.transform.localPosition = Vector3.zero;
        yield return GameManager.wait01;
        yield return null;
    }
    IEnumerator ChuPai_IfParent()
    {
        Vector3 f_Dic = list_ChuPai[0].transform.position - list_ChuPai[1].transform.position;
        if (list_ChuPai[list_ChuPai.Count - 3].transform.childCount > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject gmChuPaiInit = new GameObject();
                MJInfos newMJ = gmChuPaiInit.AddComponent<MJInfos>();
                newMJ.mjPType = MJPositionType.MJChuPai;
                gmChuPaiInit.transform.SetParent(list_ChuPai[list_ChuPai.Count - 1].transform.parent);
                gmChuPaiInit.layer = gmChuPaiInit.transform.parent.gameObject.layer;
                gmChuPaiInit.transform.position = list_ChuPai[list_ChuPai.Count - 1].transform.position - f_Dic;
                gmChuPaiInit.transform.localScale = list_ChuPai[0].transform.localScale;
                gmChuPaiInit.name = list_ChuPai[0].gameObject.name;
                gmChuPaiInit.transform.rotation = list_ChuPai[0].transform.rotation;
                list_ChuPai.Add(gmChuPaiInit.GetComponent<MJInfos>());
            }
        }
        yield return null;
    }
    public Transform ChuPai_GetParent()
    {
        Transform t_ChuPaiP = null;
        for (int i = 0; i < list_ChuPai.Count; i++)
        {
            if (list_ChuPai[i].transform.childCount == 0)
            {
                t_ChuPaiP = list_ChuPai[i].transform;
                break;
            }
        }
        StartCoroutine(ChuPai_IfParent());
        return t_ChuPaiP;
    }


    public GameObject ChuPai_ADD(string cardName)
    {
        GameObject gmRes = MemoryPool_3D.Instance.MJ3D_GetModel(cardName);
        if (gmRes.GetComponent<MeshRenderer>() != null)
        {
            gmRes.GetComponent<MeshRenderer>().enabled = true;
        }
        gmRes.SetActive(true);
        StartCoroutine(Intantiate_MJ(gmRes));
        return gmRes;
    }

    void ChuPai_DeleteAll()
    {
        for (int i = 0; i < list_ChuPai.Count; i++)
        {
            //if (list_ChuPai[i].coun )
            //{
            //}
        }
    }

    public void PaiQiang_Create()
    {

    }

    public void ShouPai_Add_My(Mj_Sx_ mjSx)
    {
        MJInfos t_SpParent = ShouPai_GetPosition();
        t_SpParent.MjINFO_ = mjSx;
        ShouPai_Instantiate(mjSx.mj_SpriteName, t_SpParent.transform);
    }

    public void ShouPai_Add_OtherPlayer(Mj_Sx_ cardName)
    {
        //Transform t_Parent = ShouPai_GetPosition();
        GameObject gmIns = MemoryPool_3D.Instance.MJ3D_GetRandomModel("1W");
        gmIns.SetActive(true);
        MJInfos t_Parent = ShouPai_GetPosition();
        //Debug.Log(t_Parent == null);
        //Debug.Log(list_ShouPai.Count);
        gmIns.transform.SetParent(t_Parent.transform, false);
        gmIns.transform.localEulerAngles = Vector3.zero;
        gmIns.transform.localScale = Vector3.one;
        gmIns.transform.localPosition = Vector3.zero;
        //设置麻将的材质灯光层级 麻将材质跟父物体一样
        gmIns.GetComponent<MeshRenderer>().material = gmIns.transform.parent.GetComponentInParent<MeshRenderer>().material;
        gmIns.layer = gmIns.transform.parent.gameObject.layer;
    }

    private MJInfos ShouPai_GetPosition()
    {
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount == 0)
            {
                return list_ShouPai[i];
            }
        }
        return null;
    }
    public void ShouPai_RecycleAll()
    {
        Debug.Log("ShouPai_RecycleAll()");
        MeshFilter[] allHnadCards = t_ShouPai_Parent.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < allHnadCards.Length; i++)
        {
            if (allHnadCards[i] != null)
            {
                GameObject mjSP = allHnadCards[i].gameObject;
                if (mjSP.transform.parent != null && mjSP.transform.parent.GetComponent<MJInfos>() != null)
                {
                    mjSP.transform.parent.GetComponent<MJInfos>().MjINFO_ = new Mj_Sx_();
                }
                MemoryPool_3D.Instance.MJ3D_Recycle(mjSP);
            }
        }

    }
    public GameObject ShouPai_ChuPai_My(Mj_Sx_ mj_Sx)
    {

        ShouPai_MY_InitMJINFO();

        if (mj_Sx.mJCardID == mj_MoPai.MjINFO_.mJCardID)
        {
            GameObject mj_Gm = mj_MoPai.transform.GetChild(0).gameObject;
            MemoryPool_3D.Instance.MJ3D_Recycle(mj_Gm);
            mj_MoPai.MjINFO_ = new Mj_Sx_();
            mj_MoPai.gameObject.name = "NULL";
            return ChuPai_ADD(mj_Sx.mj_SpriteName);
        }

        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].gameObject.activeInHierarchy && list_ShouPai[i].MjINFO_.mJCardID == mj_Sx.mJCardID)
            {
                if (list_ShouPai[i].transform.childCount > 0)
                {
                    GameObject mj_Gm = list_ShouPai[i].transform.GetChild(0).gameObject;
                    MemoryPool_3D.Instance.MJ3D_Recycle(mj_Gm);
                }
                //ShouPai_Rank_My(i);
                ShouPai_RankRight_MY();

                if (mj_MoPai.transform.childCount > 0)
                {
                    ShouPai_MY_InitMJINFO();
                    gm_MoPaiMove = mj_MoPai.transform.GetChild(0).gameObject;
                    int i1Index = ForTrim_My_MaJaing(gm_MoPaiMove.transform);
                    i1Index = i1Index == 0 ? i1Index : i1Index - 1;

                    i1Index = Mathf.Clamp(i1Index, 0, list_ShouPai.Count - 2);
                    StartCoroutine(Move_1(i1Index, gm_MoPaiMove.transform));

                }

                return ChuPai_ADD(mj_Sx.mj_SpriteName);
            }
        }



        Debug.Log("<color=red> ShouPai Find MJ_SX==NULL </color>");
        return null;
    }


    public GameObject ShouPai_ChuPai_Other(Mj_Sx_ mj_Sx)
    {

        if (list_ShouPai[list_ShouPai.Count - 1].transform.childCount > 0)
        {
            GameObject mj_Gm = list_ShouPai[list_ShouPai.Count - 1].transform.GetChild(0).gameObject;
            MemoryPool_3D.Instance.MJ3D_Recycle(mj_Gm);
            return ChuPai_ADD(mj_Sx.mj_SpriteName);
        }
        else
        {
            for (int i = 0; i < list_ShouPai.Count - 1; i++)
            {
                if (list_ShouPai[i].gameObject.activeInHierarchy && list_ShouPai[i].transform.childCount > 0)
                {
                    //if (list_ShouPai[i].transform.childCount > 0)
                    //{
                    Debug.Log("Child Count????" + list_ShouPai[i].transform.childCount);
                    GameObject mj_Gm = list_ShouPai[i].transform.GetChild(0).gameObject;
                    MemoryPool_3D.Instance.MJ3D_Recycle(mj_Gm);
                    //}

                    return ChuPai_ADD(mj_Sx.mj_SpriteName);
                }
            }
        }


        Debug.Log("<color=red> ShouPai Find MJ_SX==NULL </color>");
        return null;
    }

    public GameObject MoPai_Add_MY(Mj_Sx_ mjSx)
    {
        //GameObject gmHandCard = null;

        if (mj_MoPai != null)
        {
            mj_MoPai.MjINFO_ = mjSx;
            //显示摸牌
            return ShouPai_Instantiate(mjSx.mj_SpriteName, mj_MoPai.transform);
        }
        Debug.Log("<color=red>Instantiate HandCard Error </color>");
        if (mj_MoPai.transform.childCount > 0)
        {
            for (int i = 0; i < mj_MoPai.transform.childCount; i++)
            {
                GameObject mj_Gm = mj_MoPai.transform.GetChild(i).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(mj_Gm);
                mj_MoPai.MjINFO_ = new Mj_Sx_();
                mj_MoPai.gameObject.name = "NULL";
            }
        }
        return ShouPai_Instantiate(mjSx.mj_SpriteName, mj_MoPai.transform); ;
    }

    public void MoPai_Add_OtherPlayer(Mj_Sx_ mjSx)
    {
        MJInfos t_Parent = list_ShouPai[list_ShouPai.Count - 1];
        t_Parent.MjINFO_ = mjSx;
        if (t_Parent.transform.childCount == 0)
        {//防止其他玩家重连多刷新牌面
            ShouPai_Instantiate(mjSx.mj_SpriteName, t_Parent.transform);
        }
    }

    #region/*———My MJ———*/
    public GameObject ShouPai_Instantiate(string strName, Transform tParent)
    {
        //Debug.Log("" + strName);
        //GameObject gmRes = MemoryPool_3D.Instance.MJ3D_GetModel(strName);// Resources.Load<GameObject>("Prefabs/MJ/" + strName);
        GameObject gmIns = MemoryPool_3D.Instance.MJ3D_GetModel(strName); // Instantiate(gmRes) as GameObject;
        gmIns.SetActive(true);
        gmIns.name = strName;
        tParent.gameObject.name = strName;
        gmIns.transform.SetParent(tParent);
        gmIns.layer = tParent.gameObject.layer;
        gmIns.transform.localEulerAngles = Vector3.zero;
        gmIns.transform.localScale = Vector3.one;
        gmIns.transform.localPosition = Vector3.zero;
        //设置麻将的材质灯光层级 麻将材质跟父物体一样
        gmIns.GetComponent<MeshRenderer>().material = gmIns.transform.parent.GetComponentInParent<MeshRenderer>().material;
        gmIns.layer = gmIns.transform.parent.gameObject.layer;
        return gmIns;
    }

    public void Tsfm_SetInit(GameObject gmIns)
    {
        gmIns.layer = gmIns.transform.parent.gameObject.layer;
        gmIns.transform.localEulerAngles = Vector3.zero;
        gmIns.transform.localScale = Vector3.one;
        gmIns.transform.localPosition = Vector3.zero;
    }

    /// <summary>获取摸牌将要插到的 手牌位置
    /// </summary>
    /// <param name="my_Mj"></param>
    /// <returns></returns>
    //public int ForTrim_My_MaJaing(Transform my_Mj)
    //{
    //    int i_EndIndex = 0;
    //    int i_MjCount = 0;
    //    uint iValue = DataManage.Instance.MJSX_Get(my_Mj.gameObject.name).mJCardID;
    //    //if (iValue == 53)
    //    //{
    //    //    for (int i = 0; i <= list_ShouPai.Count - 2; i++)
    //    //    {
    //    //        if (list_ShouPai[i].transform.childCount > 0)
    //    //        {
    //    //            if (list_ShouPai[i].MjINFO_.mJCardID > 0)
    //    //            {
    //    //                //int iResult = i - 1;
    //    //                return i;//Mathf.Clamp(i, 0, list_ShouPai.Count - 2);
    //    //            }
    //    //        }
    //    //    }
    //    //    //return i_EndIndex;
    //    //}
    //    for (int i = list_ShouPai.Count - 2; i >= 0; i--)
    //    {//排序寻找位置，
    //        if (list_ShouPai[i].transform.childCount > 0)
    //        {
    //            i_EndIndex = i;
    //            i_MjCount++;
    //            if (iValue >= list_ShouPai[i].MjINFO_.mJCardID)//|| list_ShouPai[i].MjINFO_.mJCardID == 53
    //            {
    //                //Debug.Log(DataManage.Instance.PData_GetSeatID(0) + "IndexG1:" + i);
    //                return i + 1;
    //            }
    //        }
    //    }

    //    if (i_EndIndex > 1)
    //    {
    //        //Debug.Log(DataManage.Instance.PData_GetSeatID(0) + "IndexG2:" + (i_EndIndex));
    //        return i_EndIndex;
    //    }
    //    else if (i_MjCount == 0)
    //    {
    //        //Debug.Log(DataManage.Instance.PData_GetSeatID(0) + "IndexG3:" + (i_EndIndex - 1));
    //        return list_ShouPai.Count - 2;
    //    }
    //    else
    //    {
    //        //Debug.Log(DataManage.Instance.PData_GetSeatID(0) + "IndexG4:" + (i_EndIndex - 1));
    //        return 0;
    //    }

    //}
    public int ForTrim_My_MaJaing(Transform my_Mj)
    {
        int i_EndIndex = 0;
        int i_MjCount = 0;
        uint iValue = DataManage.Instance.MJSX_Get(my_Mj.gameObject.name).mJCardID;
        for (int i = list_ShouPai.Count - 2; i >= 0; i--)
        {//排序寻找位置，
            if (list_ShouPai[i].transform.childCount > 0)
            {
                i_EndIndex = i;
                i_MjCount++;
                if (iValue >= DataManage.Instance.MJSX_Get(list_ShouPai[i].gameObject.name).mJCardID)
                {
                    return i + 1;
                }
            }
        }
        if (i_EndIndex > 1)
        {
            return i_EndIndex;
        }
        else if (i_MjCount == 0)
        {
            return list_ShouPai.Count - 2;
        }
        else
        {
            return 0;
        }
    }

    IEnumerator Move_1(int i_Index, Transform t_)
    {
        //float jlDic = Vector3.Distance(list_ShouPai[0].transform.position, list_ShouPai[1].transform.position);

        list_ShouPai[i_Index].gameObject.name = "NULL";
        Transform tParenJL1 = list_ShouPai[i_Index].transform;
        yield return GameManager.wait01;
        for (int i = i_Index; i >= 0 /*list_ShouPai.Count - 2*/; i--)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                int iCount1 = list_ShouPai[i - 1].transform.childCount;
                Transform t_ChildMJ = list_ShouPai[i].transform.GetChild(0);
                //Mj_Sx_ jlMJSX = list_ShouPai[i].MjINFO_;
                t_ChildMJ.SetParent(list_ShouPai[i - 1].transform);
                list_ShouPai[i - 1].GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(t_ChildMJ.gameObject.name);

                t_ChildMJ.localPosition = Vector3.zero;
                //yield return new WaitForEndOfFrame();
                t_ChildMJ.parent.name = t_ChildMJ.gameObject.name;
                if (iCount1 == 0)
                {
                    break;
                }
                //yield return new WaitForSeconds(1f);
            }

        }
        //yield return new WaitForEndOfFrame();
        gm_MoPaiMove.transform.SetParent(tParenJL1);
        gm_MoPaiMove.transform.parent.gameObject.name = gm_MoPaiMove.name;
        tParenJL1.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(gm_MoPaiMove.name);
        Vector3 jlPosition1 = gm_MoPaiMove.transform.position;//摸牌先抬起来
        Vector3 jlPosition2 = gm_MoPaiMove.transform.parent.position;
        jlPosition1.y += 0.2f;
        jlPosition2.y += 0.1f;
        gm_MoPaiMove.transform.position = jlPosition2;
        //yield return new WaitForEndOfFrame();
        //while (Vector3.Distance(gm_MoPaiMove.transform.position, jlPosition2) > 0.0001f)
        //{//摸牌移向找到的空位上方
        //    gm_MoPaiMove.transform.position = Vector3.MoveTowards(gm_MoPaiMove.transform.position, jlPosition2, Time.deltaTime * 5);
        //    yield return new WaitForEndOfFrame();
        //}
        jlPosition2 = gm_MoPaiMove.transform.parent.position;
        //yield return new WaitForEndOfFrame();
        //while (Vector3.Distance(gm_MoPaiMove.transform.position, jlPosition2) > 0.0001f)
        //{//摸牌移向找到的空位落下
        //    gm_MoPaiMove.transform.position = Vector3.MoveTowards(gm_MoPaiMove.transform.position, jlPosition2, Time.deltaTime);
        //    yield return new WaitForEndOfFrame();
        //}
        gm_MoPaiMove.transform.localPosition = Vector3.zero;
        //yield return new WaitForEndOfFrame();
        if (DataManage.Instance.MjXuanQue > 0)
        {
            Que_UpdateHandCards();
        }
        yield return null;
    }
    #endregion

    public void ShouPai_Rank_My(int i_Index)
    {
        for (int i = i_Index; i >= 0 /*list_ShouPai.Count-1*/; i--)
        {
            if (list_ShouPai[i].gameObject.activeInHierarchy && list_ShouPai[i].MjINFO_.mJCardID != 0)
            {
                if (list_ShouPai[i].transform.childCount > 0)
                {
                    Transform mj_Gm = list_ShouPai[i].transform.GetChild(0);
                    Mj_Sx_ jLMjInfo = list_ShouPai[i].MjINFO_;
                    mj_Gm.SetParent(list_ShouPai[i + 1].transform);
                    Tsfm_SetInit(mj_Gm.gameObject);
                    list_ShouPai[i + 1].MjINFO_ = jLMjInfo;
                    //mj_Gm.transform.parent.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(mj_Gm.gameObject.name);
                    mj_Gm.transform.parent.gameObject.name = jLMjInfo.mj_SpriteName;
                }
            }
        }
    }

    /// <summary>本地手牌//麻将靠右
    /// </summary>
    public void ShouPai_RankRight_MY()
    {
        List<Transform> otherSP = new List<Transform>();
        for (int i = list_ShouPai.Count - 2; i >= 0; i--)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                otherSP.Add(list_ShouPai[i].transform);
            }
        }

        //int iCenter_I1 = 13 - otherSP.Count;

        //iCenter_I1 = iCenter_I1 / 2;

        for (int i = 0; i < otherSP.Count; i++)
        {
            Transform t_ChildMj = otherSP[i].GetChild(0);
            t_ChildMj.SetParent(list_ShouPai[(list_ShouPai.Count - 2) - (i)].transform);
            //list_ShouPai[iCenter_I1 + i].GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(t_ChildMj.gameObject.name);

            t_ChildMj.parent.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(t_ChildMj.gameObject.name);
            t_ChildMj.parent.gameObject.name = t_ChildMj.gameObject.name;
            Tsfm_SetInit(t_ChildMj.gameObject);
        }
        ShouPai_MY_InitMJINFO();
    }

    /// <summary>初始化空位
    /// </summary>
    void ShouPai_MY_InitMJINFO()
    {
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount == 0)
            {
                list_ShouPai[i].MjINFO_ = new Mj_Sx_();
                list_ShouPai[i].gameObject.name = "NULL";
            }
        }
    }

    public void ShouPai_Rank_OtherPlayer()
    {

        List<Transform> otherSP = new List<Transform>();
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (/*list_ShouPai[i].MjINFO_.mJCardID>0 &&*/ list_ShouPai[i].transform.childCount > 0)
            {
                otherSP.Add(list_ShouPai[i].transform);
            }
        }

        int iCenter_I1 = list_ShouPai.Count - otherSP.Count;
        iCenter_I1 = iCenter_I1 / 2;

        for (int i = 0; i < otherSP.Count; i++)
        {
            Transform t_ChildMj = otherSP[i].GetChild(0);
            t_ChildMj.SetParent(list_ShouPai[iCenter_I1 + i].transform);
            Tsfm_SetInit(t_ChildMj.gameObject);
        }
    }


    public int ShouPai_Destroy_My(int iCount, Mj_Sx_ mjSX_, bool b_ = false)
    {
        int i_Value = 0;
        if (mj_MoPai.transform.childCount > 0 && mj_MoPai.MjINFO_.mJCardID == mjSX_.mJCardID)
        {
            mj_MoPai.MjINFO_ = new Mj_Sx_();
            i_Value++;
            GameObject gmSP = mj_MoPai.transform.GetChild(0).gameObject;
            MemoryPool_3D.Instance.MJ3D_Recycle(gmSP);
            if (i_Value == iCount)
            {
                ShouPai_RankRight_MY();
                return i_Value;
            }
        }

        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0 && list_ShouPai[i].MjINFO_.mJCardID == mjSX_.mJCardID)
            {
                if (!b_)
                {
                    //list_ShouPai[i].transform.SetAsLastSibling();
                    list_ShouPai[i].MjINFO_ = new Mj_Sx_();
                }

                GameObject gmSP = list_ShouPai[i].transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(gmSP);
                i_Value++;
                if (i_Value == iCount)
                {
                    ShouPai_RankRight_MY();

                    /*防止起手杠的手牌排序*/
                    if (mj_MoPai.transform.childCount > 0)
                    {
                        ShouPai_MY_InitMJINFO();
                        gm_MoPaiMove = mj_MoPai.transform.GetChild(0).gameObject;
                        int i1Index = ForTrim_My_MaJaing(gm_MoPaiMove.transform);
                        i1Index = i1Index == 0 ? i1Index : i1Index - 1;

                        i1Index = Mathf.Clamp(i1Index, 0, list_ShouPai.Count - 2);
                        StartCoroutine(Move_1(i1Index, gm_MoPaiMove.transform));

                    }
                    /**/
                    return i_Value;
                }
            }
        }

        return i_Value;
    }

    public int ShouPai_Destroy_Other(int iCount, bool b_ = false)
    {
        int i_Value = 0;
        if (iCount == 1)
        {
            if (list_ShouPai[list_ShouPai.Count - 1].transform.childCount > 0)
            {//如果是销毁1张其他玩家的手牌。首先删除摸牌位置
                list_ShouPai[list_ShouPai.Count - 1].MjINFO_ = new Mj_Sx_();
                GameObject gmSp = list_ShouPai[list_ShouPai.Count - 1].transform.GetChild(0).gameObject;
                MemoryPool_3D.Instance.MJ3D_Recycle(gmSp);
                return 1;
            }
        }

        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                if (!b_)
                {
                    //all_ShouPai[i].transform.SetAsLastSibling();
                    list_ShouPai[i].MjINFO_ = new Mj_Sx_();

                    GameObject gmSp = list_ShouPai[i].transform.GetChild(0).gameObject;
                    MemoryPool_3D.Instance.MJ3D_Recycle(gmSp);
                }

                i_Value++;
                if (i_Value == iCount)
                {
                    return i_Value;
                }
            }
        }

        return i_Value;
    }



    public Transform MJ_Op_Other(MJ_OpType opType_, Mj_Sx_ mjSx_)
    {
        Mj_Sx_ h5MjSx = DataManage.Instance.MJSX_Get(53);
        switch (opType_)
        {
            case MJ_OpType.ChuPai:
                break;
            case MJ_OpType.MoPai:
                break;
            case MJ_OpType.FeiPeng:
                MJInfos[] feiPengShow = PengGang_GetParent();
                for (int i = 0; i < 2; i++)
                {
                    feiPengShow[i].MjINFO_ = mjSx_;
                    GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx_.mj_SpriteName);
                    pPaiGm.transform.SetParent(feiPengShow[i].transform);
                    //设置麻将的材质灯光层级 麻将材质跟父物体一样
                    pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                    pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;
                    pPaiGm.transform.parent.GetComponent<MJInfos>().MjINFO_ = mjSx_;
                    Tsfm_SetInit(pPaiGm);
                    pPaiGm.gameObject.SetActive(true);
                }

                feiPengShow[2].MjINFO_ = h5MjSx;
                GameObject hongZhongH5 = MemoryPool_3D.Instance.MJ3D_GetModel(h5MjSx.mj_SpriteName);
                hongZhongH5.transform.SetParent(feiPengShow[2].transform);
                //设置麻将的材质灯光层级 麻将材质跟父物体一样
                hongZhongH5.GetComponent<MeshRenderer>().material = hongZhongH5.transform.parent.GetComponentInParent<MeshRenderer>().material;
                hongZhongH5.layer = hongZhongH5.transform.parent.gameObject.layer;
                hongZhongH5.transform.parent.GetComponent<MJInfos>().MjINFO_ = h5MjSx;
                Tsfm_SetInit(hongZhongH5);
                hongZhongH5.gameObject.SetActive(true);

                if (feiPengShow[0] != null)
                {
                    feiPengShow[0].transform.parent.gameObject.name = "Peng|" + mjSx_.mj_SpriteName;
                }
                break;

            case MJ_OpType.TiPai:

                for (int i = 0; i < list_PengGang.Count; i++)
                {
                    if (list_PengGang[i][0].MjINFO_.mJCardID != 0)
                    {
                        if (list_PengGang[i][0].MjINFO_.mJCardID == mjSx_.mJCardID)
                        {
                            if (list_PengGang[i][2].transform.childCount > 0)
                            {
                                MemoryPool_3D.Instance.MJ3D_Recycle(list_PengGang[i][2].transform.GetChild(0).gameObject);
                            }
                            list_PengGang[i][2].MjINFO_ = mjSx_;
                            GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx_.mj_SpriteName);
                            pPaiGm.transform.SetParent(list_PengGang[i][2].transform);
                            Tsfm_SetInit(pPaiGm);

                            //设置麻将的材质灯光层级 麻将材质跟父物体一样
                            pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                            pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;

                            pPaiGm.gameObject.SetActive(true);
                            //list_PengGang[i][0].transform.parent.gameObject.name = "MingGang|" + mjSx.mj_SpriteName;
                        }
                    }
                }
                break;

            case MJ_OpType.PengPai:
                MJInfos[] pengShow = PengGang_GetParent();
                for (int i = 0; i < 3; i++)
                {
                    pengShow[i].MjINFO_ = mjSx_;
                    GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx_.mj_SpriteName);
                    pPaiGm.transform.SetParent(pengShow[i].transform);
                    //设置麻将的材质灯光层级 麻将材质跟父物体一样
                    pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                    pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;

                    pPaiGm.transform.parent.GetComponent<MJInfos>().MjINFO_ = mjSx_;
                    Tsfm_SetInit(pPaiGm);
                    pPaiGm.gameObject.SetActive(true);
                }
                if (pengShow[0] != null)
                {
                    pengShow[0].transform.parent.gameObject.name = "Peng|" + mjSx_.mj_SpriteName;
                }

                break;
            case MJ_OpType.MingGang:
                MJInfos[] gangShow = PengGang_GetParent();
                for (int i = 0; i < 4; i++)
                {
                    gangShow[i].MjINFO_ = mjSx_;
                    GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx_.mj_SpriteName);
                    pPaiGm.transform.SetParent(gangShow[i].transform);
                    //设置麻将的材质灯光层级 麻将材质跟父物体一样
                    pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                    pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;

                    pPaiGm.transform.parent.GetComponent<MJInfos>().MjINFO_ = mjSx_;
                    Tsfm_SetInit(pPaiGm);
                    pPaiGm.gameObject.SetActive(true);
                }
                if (gangShow[0] != null)
                {
                    gangShow[0].transform.parent.gameObject.name = "MingGang|" + mjSx_.mj_SpriteName;
                }
                break;
            case MJ_OpType.AnGang:
                MJInfos[] anGangShow = PengGang_GetParent();
                for (int i = 0; i < 4; i++)
                {
                    anGangShow[i].MjINFO_ = mjSx_;
                    GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx_.mj_SpriteName);
                    pPaiGm.transform.SetParent(anGangShow[i].transform);
                    //设置麻将的材质灯光层级 麻将材质跟父物体一样
                    pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                    pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;

                    pPaiGm.transform.parent.GetComponent<MJInfos>().MjINFO_ = mjSx_;
                    Tsfm_SetInit(pPaiGm);
                    if (i < 3)
                    {
                        pPaiGm.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    pPaiGm.gameObject.SetActive(true);
                }
                if (anGangShow[0] != null)
                {
                    anGangShow[0].transform.parent.gameObject.name = "AnGang|" + mjSx_.mj_SpriteName;
                }
                break;
            default:
                break;
        }
        return null;
    }

    public Transform MJ_Op_My(MJ_OpType opType_, Mj_Sx_ mjSx_)
    {
        switch (opType_)
        {
            case MJ_OpType.ChuPai:
                break;
            case MJ_OpType.MoPai:
                break;
            case MJ_OpType.PengPai:

                break;
            case MJ_OpType.MingGang:
                break;
            case MJ_OpType.AnGang:
                break;
            default:
                break;
        }
        return null;
    }

    public MJInfos[] PengGang_GetParent()
    {
        for (int i = 0; i < list_PengGang.Count; i++)
        {
            if (list_PengGang[i].Length > 0 &&
                list_PengGang[i][0].MjINFO_.mJCardID == 0
                )
            {
                return list_PengGang[i];
            }
        }
        return null;
    }

    /// <summary>玩家是否碰过该麻将
    /// </summary>
    public int Peng_IsExist(Mj_Sx_ mjSx)
    {
        for (int i = 0; i < list_PengGang.Count; i++)
        {
            if (list_PengGang[i][0].MjINFO_.mJCardID != 0)
            {
                if (list_PengGang[i][0].MjINFO_.mJCardID == mjSx.mJCardID)
                {

                    list_PengGang[i][3].MjINFO_ = mjSx;
                    GameObject pPaiGm = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx.mj_SpriteName);
                    pPaiGm.transform.SetParent(list_PengGang[i][3].transform);
                    Tsfm_SetInit(pPaiGm);

                    //设置麻将的材质灯光层级 麻将材质跟父物体一样
                    pPaiGm.GetComponent<MeshRenderer>().material = pPaiGm.transform.parent.GetComponentInParent<MeshRenderer>().material;
                    pPaiGm.layer = pPaiGm.transform.parent.gameObject.layer;

                    pPaiGm.gameObject.SetActive(true);
                    list_PengGang[i][0].transform.parent.gameObject.name = "MingGang|" + mjSx.mj_SpriteName;
                    return 1;
                }
            }
        }
        return 0;
    }
    #region /*———战绩回放———*/
    public GameObject ZJHF_ChuPai(Mj_Sx_ mjSx)
    {
        return ShouPai_ChuPai_My(mjSx);
    }

    public void ZJHF_MoPai(Mj_Sx_ mjSx)
    {
        //MJInfos zjMopAi = list_ShouPai[list_ShouPai.Count-1];

    }
    #endregion

    public void Hu_AddShow(Mj_Sx_ mjSx)
    {
        if (mj_HuPaiPosition.transform.childCount <= 0)
        {
            GameObject huGM = MemoryPool_3D.Instance.MJ3D_GetModel(mjSx.mj_SpriteName);
            huGM.SetActive(true);
            huGM.transform.SetParent(mj_HuPaiPosition.transform);
            mj_HuPaiPosition.gameObject.SetActive(true);
            Tsfm_SetInit(huGM);
            mj_HuPaiPosition.MjINFO_ = mjSx;
            huGM.gameObject.name = "HU_" + mjSx.mj_SpriteName;
            huGM.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }

    /// <summary>初始化显示牌墩。发手牌之前执行
    /// </summary>
    public List<GameObject> PaiQ_InitShow(int i_Count)
    {
        paiQ_Count = i_Count;
        List<GameObject> listPaiQ = new List<GameObject>();
        for (int i = 0; i < list_PaiQiang.Count && i < i_Count; i++)
        {
            list_PaiQiang[i].gameObject.SetActive(true);
            listPaiQ.Add(list_PaiQiang[i]);
        }
        return listPaiQ;
    }

    /// <summary>用List从结束到开始的地方清除牌墙
    /// </summary>
    public int PaiQ_Reduce_EndStart(int i_Count)
    {
        int i_C1 = 0;
        for (int i = list_PaiQiang.Count - 1; i >= 0; i--)
        {
            if (list_PaiQiang[i].gameObject.activeInHierarchy)
            {
                list_PaiQiang[i].gameObject.SetActive(false);
                i_C1++;
                if (i_C1 == i_Count)
                {
                    break;
                }
            }
        }
        return i_C1;
    }

    /// <summary>用List从开始到结束的地方清除牌墙
    /// </summary>
    public int PaiQ_Reduce_StartEnd(int i_Count)
    {
        int i_C1 = 0;
        bool isUp = paiQ_Count % 2 == 0;

        for (int i = 0; i < list_PaiQiang.Count; i++)
        {
            if (i % 2 == (isUp ? 1 : 0))
            {
                if (list_PaiQiang[i].gameObject.activeInHierarchy)
                {
                    list_PaiQiang[i].gameObject.SetActive(false);
                    paiQ_Count--;
                    isUp = paiQ_Count % 2 == 0;
                    i_C1++;

                    if (i_C1 == i_Count)
                    {
                        break;
                    }
                }
            }
        }
        return i_C1;
    }

    public bool DragRank_IFEqualMoPai(GameObject mjGmParent)
    {
        if (mj_MoPai.gameObject == mjGmParent)
        {
            return true;
        }
        //Debug.Log("XXXXXXXXXXXXXXXX");
        return false;
    }

    public int DragRank_GetIndex(GameObject mjGmParent)
    {
        int i_Result = 0;
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i].gameObject == mjGmParent)
            {
                i_Result = i;
                return i_Result;
            }
        }
        Debug.Log("XXXXXXXXXXXXXXXX");
        return i_Result;
    }

    /// <summary>麻将拖动排序。 i_Index = 被拖动的麻将当前位置；indexTarget=被拖到到的麻将位置
    /// </summary>
    public int DragRank_MoveOtherMJ(int i_Index, int indexTarget)
    {
        Debug.Log("XXXXXXXXXXXXXXXX");
        int i_My1 = i_Index;//被拖动的麻将
        int i_My2 = indexTarget;//麻将想要拖动到的位置
        MJInfos[] iAry_X1 = new MJInfos[Mathf.Abs(i_My1 - i_My2)];

        int i_X111 = 0;
        if (i_My1 > i_My2)
        {
            for (int i = i_My2; i < i_My1 && i < list_ShouPai.Count - 1; i++)
            {//X=5？ MoveIndex = 2?
                iAry_X1[i_X111] = list_ShouPai[i];
                i_X111++;
            }
        }
        else if (i_My1 < i_My2)
        {
            for (int i = i_My1 + 1; i <= i_My2 && i < list_ShouPai.Count - 1; i++)
            {//X=5？ MoveIndex = 2?
                iAry_X1[i_X111] = list_ShouPai[i];
                i_X111++;
            }
        }


        i_X111 = 0;
        //if (i_My1 > i_My2)
        //{
        //    for (int i = i_My2 + 1; i <= i_My1 && i < iAry.Length - 1; i++)
        //    {
        //        iAry[i] = iAry_X1[i_X111];
        //        i_X111++;
        //    }
        //}
        //else if (i_My1 < i_My2)
        //{
        for (int i = (i_My1 < i_My2) ? i_My1 : (i_My2 + 1); (i_My1 < i_My2 ? (i < i_My2) : (i <= i_My1)) && i < list_ShouPai.Count - 1; i++)
        {//判断两个索引来看出麻将想要拖动到元素位子的左边还是右边，如果是左边就要预防>=0 
            if (iAry_X1[i_X111].transform.childCount > 0)
            {
                GameObject mjGm = iAry_X1[i_X111].transform.GetChild(0).gameObject;

                mjGm.transform.SetParent(list_ShouPai[i].transform);
                DragRank_SetINFO(mjGm);
            }

            i_X111++;
        }
        //}

        //list_ShouPai[i_My2] = i_Index;

        return i_My2;
    }
    public void DragRank_SetINFO(GameObject gm_Mj)
    {
        //gm_Mj.transform.SetParent(list_ShouPai[i_Index].transform);
        Tsfm_SetInit(gm_Mj);
        gm_Mj.GetComponentInParent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(gm_Mj.name);
        gm_Mj.GetComponentInParent<MJInfos>().gameObject.name = gm_Mj.name;
    }

    public void DragRank_EndSetParent(int i_Index, GameObject gm_Mj)
    {
        if (list_ShouPai[i_Index].transform.childCount > 0)
        {
            Debug.Log("<color=red>DRAG Set Mj Parent ERROR</color>");
            return;
        }
        gm_Mj.transform.SetParent(list_ShouPai[i_Index].transform);
        DragRank_SetINFO(gm_Mj);
    }

    public void ShouPai_RankMYAll()
    {
        List<MJInfos> shengYuMJ = new List<MJInfos>();
        for (int i = 0; i < list_ShouPai.Count - 1; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                shengYuMJ.Add(list_ShouPai[i]);
            }
        }

        shengYuMJ = (from mj in shengYuMJ
                     orderby mj.MjINFO_.mJCardID
                     select mj).ToList();
        int i_Count1 = 0;

        for (int i = shengYuMJ.Count - 1; i >= 0 && i <= 12; i--)
        {
            if (shengYuMJ[i].transform.childCount > 0)
            {
                Transform mjChild = shengYuMJ[i].transform.GetChild(0);
                mjChild.SetParent(list_ShouPai[12 - i_Count1].transform);
                DragRank_SetINFO(mjChild.gameObject);
            }
            i_Count1++;
        }

        Rank_LeftHZ();
    }

    /// <summary>手牌排序红中靠左
    /// </summary>
    public void Rank_LeftHZ()
    {//cardid = 53 ;//== 红中；
        List<MJInfos> shengYuMJ = new List<MJInfos>();
        for (int i = list_ShouPai.Count - 2; i >= 0; i--)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                shengYuMJ.Add(list_ShouPai[i]);
            }
        }

        //选出非红中
        List<MJInfos> listFeiHZ = new List<MJInfos>();
        listFeiHZ = (from mjCard in shengYuMJ
                     where mjCard.MjINFO_.mJCardID != 53
                     select mjCard).ToList();

        //选出红中
        List<MJInfos> listHongZhong = new List<MJInfos>();
        listHongZhong = (from hongZhong in shengYuMJ
                         where hongZhong.MjINFO_.mJCardID == 53
                         select hongZhong).ToList();
        //ShouPai_RankRight_MY
        for (int i = 0; i < listHongZhong.Count; i++)
        {
            listFeiHZ.Add(listHongZhong[i]);
        }
        for (int i = 0; i < listFeiHZ.Count; i++)
        {
            Transform t_ChildMj = listFeiHZ[i].transform.GetChild(0);
            t_ChildMj.SetParent(list_ShouPai[(list_ShouPai.Count - 2) - (i)].transform);
            //list_ShouPai[iCenter_I1 + i].GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(t_ChildMj.gameObject.name);
            t_ChildMj.parent.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(t_ChildMj.gameObject.name);
            t_ChildMj.parent.gameObject.name = t_ChildMj.gameObject.name;
            Tsfm_SetInit(t_ChildMj.gameObject);
        }
        ShouPai_MY_InitMJINFO();
    }

    /// <summary>1、干掉1张手牌
    /// 2、干掉1张红中
    /// 3、添加飞碰
    /// </summary>
    public void Peng_FeiPengADD(Mj_Sx_ mjSx)
    {

    }

    /// <summary>根据选缺，将手牌排序。
    /// </summary>
    public void Que_UpdateHandCards()
    {
        List<MJInfos> shengYuMJ = new List<MJInfos>();
        for (int i = 0; i < list_ShouPai.Count - 1; i++)
        {
            if (list_ShouPai[i].transform.childCount > 0)
            {
                shengYuMJ.Add(list_ShouPai[i]);
            }
        }
        string queResult = Que_TYPE();
        Debug.Log("UpdateQue :" + queResult);
        List<MJInfos> shengYuMJ_FlaseQUE = new List<MJInfos>();
        List<MJInfos> shengYuMJ_QUE = new List<MJInfos>();
        //非缺排在前面
        shengYuMJ_FlaseQUE = (from mj in shengYuMJ
                              where (mj.MjINFO_.mj_SpriteName.IndexOf(queResult) < 0)
                              orderby mj.MjINFO_.mJCardID
                              select mj).ToList();

        //缺排在后面
        shengYuMJ_QUE = (from mj in shengYuMJ
                         where (mj.MjINFO_.mj_SpriteName.IndexOf(queResult) >= 0)
                         orderby mj.MjINFO_.mJCardID
                         select mj).ToList();
        //合并List
        shengYuMJ = shengYuMJ_FlaseQUE;//
        shengYuMJ = shengYuMJ.Concat(shengYuMJ_QUE).ToList<MJInfos>();

        int i_Count1 = 0;

        for (int i = shengYuMJ.Count - 1; i >= 0 && i <= 12; i--)
        {
            if (shengYuMJ[i].transform.childCount > 0)
            {
                Transform mjChild = shengYuMJ[i].transform.GetChild(0);
                mjChild.SetParent(list_ShouPai[12 - i_Count1].transform);
                DragRank_SetINFO(mjChild.gameObject);
            }
            i_Count1++;
        }

        Rank_LeftHZ();
    }
    public string Que_TYPE()
    {
        string queResult = "W";
        switch (DataManage.Instance.MjXuanQue)
        {
            case 1:
                queResult = "W";
                break;
            case 2:
                queResult = "B";
                break;
            case 3:
                queResult = "T";
                break;
            default:
                break;
        }
        return queResult;
    }
    public List<MJInfos> Que_GetAllQue()
    {
        List<MJInfos> listResult = new List<MJInfos>();
        string queResult = Que_TYPE();
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i] != null
                && list_ShouPai[i].transform.childCount > 0
                && list_ShouPai[i].MjINFO_.mj_SpriteName.IndexOf(queResult) >= 0)
            {
                listResult.Add(list_ShouPai[i]);
            }
        }
        return listResult;
    }
    public List<MJInfos> Que_GetNotQueCards()
    {
        List<MJInfos> listResult = new List<MJInfos>();
        string queResult = Que_TYPE();
        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i] != null
                && list_ShouPai[i].transform.childCount > 0
                && list_ShouPai[i].MjINFO_.mj_SpriteName.IndexOf(queResult) < 0)
            {
                listResult.Add(list_ShouPai[i]);
            }
        }
        return listResult;
    }



    /// <summary>
    /// 获取本地手牌
    /// </summary>
    /// <returns></returns>
    public List<MJInfos> GetMyHandCardValue()
    {
        List<MJInfos> listResult = new List<MJInfos>();

        for (int i = 0; i < list_ShouPai.Count; i++)
        {
            if (list_ShouPai[i] != null
                && list_ShouPai[i].transform.childCount > 0)
            {
                listResult.Add(list_ShouPai[i]);
            }
        }
        return listResult;
    }

    /// <summary>判断手牌是不是只剩下红中
    /// </summary>
    public bool IsAllHongZhong()
    {
        //选出手牌中不为空的麻将
        List<MJInfos> listHands = (from mjIns in list_ShouPai
                                   where mjIns.MjINFO_.mJCardID > 0 && mjIns.transform.childCount > 0
                                   select mjIns).ToList();
        for (int i = 0; i < listHands.Count; i++)
        {
            if (listHands[i].MjINFO_.mJCardID != 53)
            {
                return false;
            }
        }
        return true;
    }
}

public enum MJ_OpType
{
    ChuPai, MoPai, PengPai, MingGang, AnGang, FeiPeng, TiPai
}
