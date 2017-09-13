using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class InterfaceAll : MonoBehaviour {

//}

public interface IFC_Login_
{
    void Login_Success();
    void Login_Fail();
}

public interface IFCMjGameNet_Update
{
    void UpdateStartHandCards(List<uint> all_SP, int seat);
    void UpdateMopai(uint charid, uint cardId, List<uint> handCards);
    void UpdateMopai(uint charid, uint cardId);

    void UpdateChuPai(uint charid, uint cardId);

    void UpdatePengPai(uint charid, uint cardId);

    void UpdateGangPai(uint charid, uint cardId, uint oriCharid);

    void UpdateHule(uint charid, uint cardId, uint oriCharid);

}

public interface IFCMjGameNet_OpenMayOperation
{

    void Mj_OpenMay_Peng(uint card);

    void Mj_OpenMay_Gang(List<uint> card, uint orCharid_);

    void Mj_OpenMay_Hu(uint cardid, uint orCharid_);

    void Mj_OpenMay_Guo();
}

