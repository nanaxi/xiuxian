using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
/// <summary>
/// 单个小的
/// </summary>
public class Combat : MonoBehaviour
{
    public Text Time;
    public Text RoomNumber;
    public Text Player0, Player1, Player2, Player3;
    public Button Open;
    // Use this for initialization
    void Start()
    {
        //GameManager.GM.DS.Combat.GetComponent<UI_ComBat>().gameType = ProtoBuf.GameType.GT_Poker;
        Open.onClick.AddListener(delegate { OpenRound(); OpenLocalRound(); });
    }

    ProtoBuf.MJRoomRecord RecordRsp;
    ProtoBuf.QPRoomRecord qpRecordRsp;

    /// <summary>
    /// 总结算
    /// </summary>
    /// <param name="Record"></param>
    public void SetCombatInformation(ProtoBuf.MJRoomRecord Record)
    {
        RecordRsp = Record;
        RoomNumber.text = "房间号:" + Record.roomId.ToString();


        string time = Record.rounds[0].roundOverTime.ToString();
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(time + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        var TimeOut = dtStart.Add(toNow);

        Time.text = "时间：" + TimeOut.Month + "月" + TimeOut.Day + "日";
        int[] p = new int[4];
        //for (int i = 0; i < Record.rounds.Count; i++)
        //{
        //    for (int z = 0; z < Record.rounds[i].players.Count; z++)
        //    {
        //        p[z] += Record.rounds[i].players[z].;
        //    }
        //}
        Player0.text = GameManager.GM.ToName(Record.rounds[0].players[0].name) + "得分:" + Record.rounds[0].players[0].restGold;
        Player1.text = GameManager.GM.ToName(Record.rounds[0].players[1].name) + "得分:" + Record.rounds[0].players[1].restGold;
        Player2.text = GameManager.GM.ToName(Record.rounds[0].players[2].name) + "得分:" + Record.rounds[0].players[2].restGold;
        Player3.text = GameManager.GM.ToName(Record.rounds[0].players[3].name) + "得分:" + Record.rounds[0].players[3].restGold;
    }

    public void SetCombatInformation(ProtoBuf.QPRoomRecord Record)
    {
        qpRecordRsp = Record;
        RoomNumber.text = "房间号:" + Record.roomId.ToString();


        string time = Record.rounds[0].roundOverTime.ToString();
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(time + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        var TimeOut = dtStart.Add(toNow);

        Time.text = "时间：" + TimeOut.Month + "月" + TimeOut.Day + "日";
        int[] p = new int[4];
        //for (int i = 0; i < Record.rounds.Count; i++)
        //{
        //    for (int z = 0; z < Record.rounds[i].players.Count; z++)
        //    {
        //        p[z] += Record.rounds[i].players[z].;
        //    }
        //}
        Player0.text = GameManager.GM.ToName(Record.rounds[0].players[0].name) + "得分:" + Record.rounds[0].players[0].restGold;
        Player1.text = GameManager.GM.ToName(Record.rounds[0].players[1].name) + "得分:" + Record.rounds[0].players[1].restGold;
        Player2.text = GameManager.GM.ToName(Record.rounds[0].players[2].name) + "得分:" + Record.rounds[0].players[2].restGold;
        Player3.text = GameManager.GM.ToName(Record.rounds[0].players[3].name) + "得分:" + Record.rounds[0].players[3].restGold;
    }


    ProtoBuf.MJRoundRecord MjRoundRecordRsp;
    ProtoBuf.QPRoundRecord qpRoundRecordRsp;
    /// <summary>
    /// 设定当前的信息
    /// </summary>
    /// <param name="Record"></param>
    public void SetLocalCombatInformation(ProtoBuf.MJRoundRecord Record)
    {
        MjRoundRecordRsp = Record;
        RoomNumber.text = "房间号:" + Record.playBack.roomId.ToString();


        string time = Record.roundOverTime.ToString();
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(time + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        var TimeOut = dtStart.Add(toNow);

        Time.text = "时间：" + TimeOut.Month + "月" + TimeOut.Day + "日" + TimeOut.Hour + "时" + TimeOut.Minute + "分";

        Player0.text = GameManager.GM.ToName(Record.players[0].name) + "得分:" + Record.players[0].gold;
        Player1.text = GameManager.GM.ToName(Record.players[1].name) + "得分:" + Record.players[1].gold;
        Player2.text = GameManager.GM.ToName(Record.players[2].name) + "得分:" + Record.players[2].gold;
        Player3.text = GameManager.GM.ToName(Record.players[3].name) + "得分:" + Record.players[3].gold;
    }

    public void SetLocalCombatInformation(ProtoBuf.QPRoundRecord Record)
    {
        qpRoundRecordRsp = Record;
        RoomNumber.text = "房间号:" + Record.playBack.roomId.ToString();


        string time = Record.roundOverTime.ToString();
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(time + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        var TimeOut = dtStart.Add(toNow);

        Time.text = "时间：" + TimeOut.Month + "月" + TimeOut.Day + "日" + TimeOut.Hour + "时" + TimeOut.Minute + "分";

        Player0.text = GameManager.GM.ToName(Record.players[0].name) + "得分:" + Record.players[0].gold;
        Player1.text = GameManager.GM.ToName(Record.players[1].name) + "得分:" + Record.players[1].gold;
        Player2.text = GameManager.GM.ToName(Record.players[2].name) + "得分:" + Record.players[2].gold;
        Player3.text = GameManager.GM.ToName(Record.players[3].name) + "得分:" + Record.players[3].gold;
    }

    void OpenRound()
    {
        var gameType = GameManager.GM.DS.Combat.GetComponent<UI_ComBat>().gameType;
        Debug.Log("gameType:"+ gameType);
        if (gameType == ProtoBuf.GameType.GT_MJ && RecordRsp != null)
        {
            Debug.Log("参加局数：" + RecordRsp.rounds.Count);
            GameObject item = GameManager.GM.PopUI("Prefabs/CombatItems");
            GameManager.GM.DS.CombatItems = item;
            item.GetComponent<CombatItems>().SetComBat(RecordRsp);
        }

        if (gameType == ProtoBuf.GameType.GT_Poker && qpRecordRsp != null)
        {
            Debug.Log("参加局数：" + qpRecordRsp.rounds.Count);
            GameObject item = GameManager.GM.PopUI("Prefabs/CombatItems");
            GameManager.GM.DS.CombatItems = item;
            item.GetComponent<CombatItems>().SetComBat(qpRecordRsp);
        }
    }
    void OpenLocalRound()
    {
        var gameType = GameManager.GM.DS.Combat.GetComponent<UI_ComBat>().gameType;

        if (gameType == ProtoBuf.GameType.GT_MJ && MjRoundRecordRsp != null)
        {
            Debug.Log("进入麻将回放！");
            if (GameManager.GM.DS.PlayBack == null)
            {
                ParticleManager.GetIns.SwitchSence(2);

                GameManager.GM.GmType = GameSceneType.gm_MjZhanJiHuiFang;
                GameManager.GM.DS.MJZJHF = GameManager.GM.PopUI(ResPath.MJ_ZJHF, false);
                ZJHF_Nx3D zjhF_Mj = GameManager.GM.DS.MJZJHF.GetComponent<ZJHF_Nx3D>();
                zjhF_Mj.Init_();
                zjhF_Mj.mjRoundR_ = MjRoundRecordRsp;
                PublicEvent.GetINS.ReturnToMain();
            }
        }

        if (gameType == ProtoBuf.GameType.GT_Poker && qpRoundRecordRsp != null)
        {
            Debug.Log("进入扑克回放！");

            ParticleManager.GetIns.SwitchSence(2);

            Lang.GameMgr.Inst.Playback(qpRoundRecordRsp, transform.parent.childCount);

            PublicEvent.GetINS.ReturnToMain();
        }
    }
    
}
