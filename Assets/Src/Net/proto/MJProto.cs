using System;
using UnityEngine;
using System.Collections;
using ProtoBuf;
using System.Collections.Generic;

public class MJProto : ISingleton<MJProto>
{
    public bool MJGameOPRequest(MJGameOpReq pack)
    {
        pack.charId = BaseProto.playerInfo.m_id;
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_MJ_GAME_OP;
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    public bool RequestIsDisConnect()
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.queryType = QueryInfoReq.QueryType.State;
        pack.charId = BaseProto.playerInfo.m_id;
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_QUERY_INFO;
        GameNetWork.Inst().SendDataToLoginServer(command, pack);
        return true;
    }
    // LGI_TO_CLI_MJ_HAND_CARDS
    public bool OnMJHandCardInfo(MJHandCardInfo rsp)
    {
        //调用发放手牌方法
        PublicEvent.GetINS.Fun_reciveGetHandCards(rsp.cards, 0);
        return true;
    }


    bool UpdatePaiShu = false;
    int CardNum = -1;


    // LGI_TO_CLI_MJ_ASK_GAME_OP
    /// <summary>
    /// 通知本地玩家操作或者通知其他玩家进行操作
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>

    public void OnAskMJGameOP(AskMJGameOP rsp)
    {
        //如果当前玩家里面的牌里面有可以碰杠胡的牌，就告诉玩家可以碰杠胡了
        Debug.Log("我收到Ask" + rsp.canOps.Count + "       " + rsp.canOps[0] + "Ask相关玩家Id" + rsp.oricharId);

        if (rsp.canOps.Count > 1)
        {
            for (int i = 0; i < rsp.canOps.Count; i++)
            {
                if (rsp.canOps[i] == MJGameOP.MJ_OP_PENG)
                {
                    PublicEvent.GetINS.Fun_KeYiPeng(rsp.doer, rsp.card[0]);
                }
                if (rsp.canOps[i] == MJGameOP.MJ_OP_GANG)
                {
                    PublicEvent.GetINS.Fun_KeYiGang(rsp.doer, rsp.gangCards, rsp.oricharId);
                }
                if (rsp.canOps[i] == MJGameOP.MJ_OP_HU)
                {
                    PublicEvent.GetINS.Fun_KeYiHu(rsp.doer, rsp.card[0], rsp.oricharId);
                }
            }
        }
        else
        {
            PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_CHUPAI, rsp.card, rsp.oricharId);
            //可以出牌
            //PublicEvent.GetINS.Fun_KeyiChuPai(rsp.doer);
        }
    }


    // LGI_TO_CLI_MJ_NOTIFY_GAME_OP
    /// <summary>
    /// 服务器广播消息
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>
    public bool OnNotifyMJGameOP(NotifyMJGameOP rsp)
    {
        Debug.Log("OnNotifyMJGameOP：" + rsp.op + rsp.doer + "   " + rsp.oricharId + "  " + rsp.card);

        if (rsp.doer == 0 ) //服务器直接告诉玩家谁是庄 没有发出的玩家charid参数
        {
            switch (rsp.op)
            {
                case MJGameOP.MJ_OP_ZJ:
                    // rsp.param //庄家ID
                    Debug.Log("收到庄家！");
                    PublicEvent.GetINS.Fun_DirLight(rsp.param);
                    PublicEvent.GetINS.Fun_reciveZhuang(rsp.param);
                    break;
                case MJGameOP.MJ_OP_X3Z_RESULT:
                    Debug.Log("返回换三张结果！");
                    PublicEvent.GetINS.Change3ZhangResult(rsp.x3zCardOut);    //换三张的结果

                    //   rsp.x3zCardOut //换三张的结果
                    break;
                case MJGameOP.MJ_OP_VOTE_RESULT:
                    PublicEvent.GetINS.Fun_ReciveVoteResult(rsp.param);
                    if (rsp.param != 1)
                    {
                        Debug.Log("投票结束，GAMEEND为true");
                        //GameEnd = true;
                    }
                    break;
                case MJGameOP.MJ_OP_ROUND_OVER:
                    Debug.Log("MJGameOP.MJ_OP_ROUND_OVER");
                    GameOver = true;
                    //PublicEvent.GetINS.Fun_ReciveGameOverResult();

                    break;
                case MJGameOP.MJ_OP_X3Z:
                    PublicEvent.GetINS.Change3ZhangOtherResult(rsp.doer);
                    break;
                case MJGameOP.MJ_OP_XQ:
                    //PublicEvent.GetINS.Fun_KeyiXQ();
                    break;
                case MJGameOP.MJ_OP_VOTE_JSROOM:
                    Debug.Log("收到有人投票");
                    PublicEvent.GetINS.Fun_ReciveOherVote(rsp.doer, rsp.param);
                    break;
                default:
                    break;
            }
        }
        else if (rsp.doer != 0)
        {
            switch (rsp.op)
            {
                case MJGameOP.MJ_OP_PREP:
                    PublicEvent.GetINS.Fun_recivePlayerReady(rsp.doer);
                    Debug.Log("收到准备");
                    //PublicEvent.GetINS.Fun_PlayerUpdata(GameManager.GM.GetPlayerNum(rsp.doer));
                    break;
                case MJGameOP.MJ_OP_XQ:
                    PublicEvent.GetINS.Fun_reciveSelectQue(rsp.doer, rsp.param);
                    Debug.Log("收到选缺");
                    break;
                case MJGameOP.MJ_OP_MOPAI:
                    PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                    PublicEvent.GetINS.Fun_reciveGetCard(rsp.doer, rsp.card);
                    if ((int)rsp.param != 0)
                    {
                        PublicEvent.GetINS.Fun_UpdatePaishu((int)rsp.param);
                        CardNum = (int)rsp.param;
                        if (!UpdatePaiShu)
                        {
                            UpdatePaiShu = true;
                        }
                    }
                    else if (!UpdatePaiShu)
                    {
                        UpdatePaiShu = true;
                        if (BaseProto.playerInfo.m_inGame == GameType.GT_Poker)
                        {
                            PublicEvent.GetINS.Fun_UpdatePaishu(67);//67需手动修改 此时服务器会传来0
                        }
                        else if (BaseProto.playerInfo.m_inGame == GameType.GT_MJ)
                        {
                            PublicEvent.GetINS.Fun_UpdatePaishu(55);//67需手动修改 此时服务器会传来0
                        }
                        CardNum = (int)rsp.param;
                    }
                    else if (UpdatePaiShu)
                    {
                        PublicEvent.GetINS.Fun_UpdatePaishu((int)rsp.param);
                        CardNum = (int)rsp.param;
                    }
                    break;
                case MJGameOP.MJ_OP_CHUPAI:
                    PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                    PublicEvent.GetINS.Fun_reciveOtherPopCard(rsp.doer, rsp.card);
                    break;
                case MJGameOP.MJ_OP_GUO:
                    PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                    break;
                case MJGameOP.MJ_OP_PENG:
                    PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                    PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                    break;
                case MJGameOP.MJ_OP_GANG:
                    PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                    PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                    break;
                case MJGameOP.MJ_OP_HU:
                    PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                    break;
                case MJGameOP.MJ_OP_VOTE_JSROOM:
                    Debug.Log("收到有人投票");
                    PublicEvent.GetINS.Fun_ReciveOherVote(rsp.doer, rsp.param);
                    break;
                case MJGameOP.MJ_OP_X3Z:
                    Debug.Log("返回谁换三张！");
                    PublicEvent.GetINS.Change3ZhangOtherResult(rsp.doer);    //谁换三张
                    break;
                case MJGameOP.MJ_OP_ON_LINE://param（0：离线，1：上线）通知别的玩家
                    if (rsp.param == 0)
                    {
                        Debug.Log("断线！"+ rsp.doer);
                        PublicEvent.GetINS.Fun_DisHead(rsp.doer, true);
                    }
                    else
                    {
                        Debug.Log("回来了！"+ rsp.doer);
                        PublicEvent.GetINS.Fun_DisHead(rsp.doer, false);
                    }
                    break;
                case MJGameOP.MJ_OP_TING:
                    //if (rsp.doer == BaseProto.playerInfo.m_id)
                    //    PublicEvent.GetINS.Fun_TingCard(rsp.doer, rsp.tingCards);
                    break;
                case MJGameOP.MJ_OP_COORD:// 定位s   02363813931   02368713929
                    DataManage.Instance.PData_SetAdress(rsp.doer, new Vector2(rsp.latitude, rsp.longitude));
                    PublicEvent.GetINS.PlayerDistance(rsp.doer);
                    break;
                default:
                    break;
            }
        }
        return true;
    }

    public bool GameOver = false;
    // LGI_TO_CLI_MJ_GAME_OVER
    public bool OnMJGameOver(MJGameOver rsp)
    {
        Debug.Log("这一回合结束！");
        PublicEvent.GetINS.Fun_ReciveRoundOverResult(rsp);
        //GameManager.GM.DS.PlayRoom.GetComponent<UI_PlayRoom>().NextJu();
        UpdatePaiShu = false;
        CardNum = -1;
        return true;
    }
}
