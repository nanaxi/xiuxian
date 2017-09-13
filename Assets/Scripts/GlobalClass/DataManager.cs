using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public static class DataManager
{

    /// <summary>
    /// 方法扩展模板
    /// </summary>
    /// <param name="str"></param>
    public static void SetData(this string str)
    {

    }
    public enum CardsType
    {
        Tiao,
        Tong,
        Wan,
        Null,//杠牌时暗杠显示的背牌
        Hua,//东南西北 中 发 白
    }
    public struct MJCardStruct
    {
        //public CardsType CT;
        public int Value;
        public uint MJCardID;
        public MJCardStruct(int value, uint CardID)
        {
            //this.CT = CT;
            this.Value = value;
            this.MJCardID = CardID;
        }
    }
    static public Mj_Sx_ ToCard(this uint cardID)
    {
        Mj_Sx_ cardStruct;

        uint color = (cardID & 0xF0) >> 4;
        uint value = cardID & 0x0F;

        CardsType CT = CardsType.Tong;
        string mj_Name = "";
        switch (color)
        {
            case 0:
                CT = CardsType.Wan;
                mj_Name = value + "W";
                break;
            case 1:
                CT = CardsType.Tiao;
                mj_Name = value + "T";
                break;
            case 2:
                CT = CardsType.Tong;
                mj_Name = value + "B";
                break;
            case 3:
                CT = CardsType.Hua;
                mj_Name = value + "H";
                break;
            default:
                CT = CardsType.Null;
                break;
        }

        cardStruct = new Mj_Sx_((int)value, cardID, mj_Name);
        return cardStruct;

    }
    //public static string ToName(this MJCardStruct mjCard)
    //{
    //    string name = mjCard.Value.ToString();
    //    switch (mjCard.CT)
    //    {
    //        case CardsType.Tiao:
    //            name += "t";
    //            break;
    //        case CardsType.Tong:
    //            name += "b";
    //            break;
    //        case CardsType.Wan:
    //            name += "w";
    //            break;
    //        case CardsType.Hua:
    //            name += "h";
    //            break;
    //        default:
    //            break;
    //    }
    //    return name;
    //}

    public static bool IsPeng(this List<ProtoBuf.MJGameOP> ops)
    {
        bool isTrue;
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == ProtoBuf.MJGameOP.MJ_OP_PENG)
            {
                isTrue = true;
                break;
            }
        }
        isTrue = false;
        return isTrue;
    }
    public static bool IsGang(this List<ProtoBuf.MJGameOP> ops)
    {

        bool isTrue;
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == ProtoBuf.MJGameOP.MJ_OP_GANG)
            {
                isTrue = true;
                break;
            }
        }
        isTrue = false;
        return isTrue;
    }
    public static bool IsHu(this List<ProtoBuf.MJGameOP> ops)
    {

        bool isTrue;
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == ProtoBuf.MJGameOP.MJ_OP_HU)
            {
                isTrue = true;
                break;
            }
        }
        isTrue = false;
        return isTrue;
    }
    public static bool IsChuPai(this List<ProtoBuf.MJGameOP> ops)
    {

        bool isTrue;
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == ProtoBuf.MJGameOP.MJ_OP_CHUPAI)
            {
                isTrue = true;
                break;
            }
        }
        isTrue = false;
        return isTrue;
    }
    //public static MJCardStruct[] Sort(this uint[] uints)
    //{
    //    Array.Sort(uints);
    //    MJCardStruct[] temps = new MJCardStruct[uints.Length];
    //    List<MJCardStruct> Wan = new List<MJCardStruct>();
    //    List<MJCardStruct> Tong = new List<MJCardStruct>();
    //    List<MJCardStruct> Tiao = new List<MJCardStruct>();
    //    List<List<MJCardStruct>> gos = new List<List<MJCardStruct>>();
    //    MJCardStruct go = new MJCardStruct();
    //    for (int i = 0; i < uints.Length; i++)
    //    {
    //        go = new MJCardStruct(ToCard(uints[i]).);//ToCard(uints[i])
    //        switch (go.CT)
    //        {
    //            case CardsType.Wan:
    //                Wan.Add(go);
    //                break;
    //            case CardsType.Tong:
    //                Tong.Add(go);
    //                break;
    //            case CardsType.Tiao:
    //                Tong.Add(go);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    gos.Add(Wan);
    //    gos.Add(Tong);
    //    gos.Add(Tiao);
    //    gos.Sort();
    //    int m = 0;
    //    for (int i = 0; i < gos.Count; i++)
    //    {
    //        List<MJCardStruct> temp = gos[i];
    //        for (int j = 0; j < temp.Count; j++)
    //        {
    //            temps[j + m] = temp[j];
    //        }
    //        if (i != gos.Count - 1)
    //            m += temp.Count;
    //    }
    //    return temps;
    //}

    //public static MJCardStruct[] Sort(this uint[] uints, CardsType[] CTs)
    //{
    //    Array.Sort(uints);
    //    MJCardStruct[] temps = new MJCardStruct[uints.Length];
    //    List<MJCardStruct> Wan = new List<MJCardStruct>();
    //    List<MJCardStruct> Tong = new List<MJCardStruct>();
    //    List<MJCardStruct> Tiao = new List<MJCardStruct>();
    //    MJCardStruct go = new MJCardStruct();
    //    for (int i = 0; i < uints.Length; i++)
    //    {
    //        go = ToCard(uints[i]);
    //        switch (go.CT)
    //        {
    //            case CardsType.Wan:
    //                Wan.Add(go);
    //                break;
    //            case CardsType.Tong:
    //                Tong.Add(go);
    //                break;
    //            case CardsType.Tiao:
    //                Tong.Add(go);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    int n = 0;
    //    for (int i = 0; i < CTs.Length; i++)
    //    {
    //        switch (CTs[i])
    //        {
    //            case CardsType.Wan:
    //                for (int j = 0; j < Wan.Count; j++)
    //                {
    //                    temps[j + n] = Wan[j];
    //                }
    //                n = Wan.Count;
    //                break;
    //            case CardsType.Tiao:
    //                for (int j = 0; j < Tiao.Count; j++)
    //                {
    //                    temps[j + n] = Tiao[j];
    //                }
    //                n += Tiao.Count;
    //                break;
    //            case CardsType.Tong:
    //                for (int j = 0; j < Tong.Count; j++)
    //                {
    //                    temps[j + n] = Tong[j];
    //                }
    //                n += Tong.Count;
    //                break;
    //        }
    //    }
    //    return temps;
    //}
}
