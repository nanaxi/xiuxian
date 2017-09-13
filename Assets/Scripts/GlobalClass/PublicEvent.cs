using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine.EventSystems;
using UnityEngine.Events;
//GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage();提示窗口
public class PublicEvent
{
    static PublicEvent _INS;
    public static PublicEvent GetINS
    {
        get
        {
            if (_INS == null)
            {
                _INS = new PublicEvent();
            }
            return _INS;
        }
    }
    public PublicEvent()
    {
        _INS = this;
    }
    #region 登录模块

    public Action LoginRest;

    /// <summary>
    /// 玩家登录转发
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ID"></param>
    public void AppLogin(string Name, string ID, string unionid = "")
    {
        GlobalSettings.LoginUserName = Name;
        GlobalSettings.LoginUserId = ID;
        GlobalSettings.LoginChannel = unionid;
        LoginProcessor.Inst().Login();
    }
    public void AppLoginOut()
    {
        AnySDKManager.SendLogout();
        LoginProcessor.Inst().ApplyLogout();
    }


    /// <summary>
    /// 登录的返回结果的委托
    /// </summary>
    /// <param name="Result"></param>
    public delegate void Delegate_LoginResult(ProtoBuf.AccountLoginRsp Result);
    public event Delegate_LoginResult Event_LoginResult;
    public void Fun_LoginResult(ProtoBuf.AccountLoginRsp Result)
    {
        if (Result.result == ProtoBuf.AccountLoginRsp.Result.ACCOUNT_LOGIN_SUCCESS)
        {
            GameManager.GM.MyPlayerData(Result);

            if (BaseProto.playerInfo.m_atRoomId != 0)
            {
                ///直接进入游戏
                Debug.Log("直接进入游戏");
                if (Result.inGame == GameType.GT_Poker)
                {
                    BaseProto.playerInfo.m_inGame = GameType.GT_Poker;
                    BaseProto.Inst().EnterGameRequest(ProtoBuf.GameType.GT_Poker);
                }
                if (Result.inGame == GameType.GT_MJ)
                {
                    BaseProto.playerInfo.m_inGame = GameType.GT_MJ;
                    BaseProto.Inst().EnterGameRequest(ProtoBuf.GameType.GT_MJ);
                }
                AppJoin(BaseProto.playerInfo.m_atRoomId);

            }
            else
            {
                BaseProto.Inst().EnterGameRequest(ProtoBuf.GameType.GT_GC);
                Debug.Log("登陆成功，并创建大厅");
                SoundMag.GetINS.PlayPopCard("kaichang", 0, 0);
                //PublicEvent.GetINS.LoginRest();
                if (GameManager.GM.DS.Login != null)
                    GameManager.GM.DS.Login.GetComponent<UI_Login>().turnToMain();
                else
                    ParticleManager.GetIns.SwitchSence(1);
            }

        }
        else
        {
            switch (Result.result)
            {
                case AccountLoginRsp.Result.ACCOUNT_LOGIN_SUCCESS:
                    break;
                case AccountLoginRsp.Result.CHANNEL_VERIFY_FAILED:
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("认证失败！");
                    break;
                case AccountLoginRsp.Result.ACCOUNT_IS_LOGINING:
                    break;
                case AccountLoginRsp.Result.ACCOUNT_LOGIN_TIMEOUT:
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("登陆超时！");
                    break;
                case AccountLoginRsp.Result.GAME_FULL_WAITTING:
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("服务器已满，请稍后再登陆！");
                    break;
                case AccountLoginRsp.Result.ACCOUNT_IN_BLACK_LIST:
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("您已被列为黑名单！");
                    break;
                case AccountLoginRsp.Result.SOMETHING_ERROR:
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("服务器出现错误！");
                    break;
                default:
                    break;
            }

        }

    }

    #endregion
    #region 进入大厅

    /// <summary>
    /// 进入大厅成功 触发方法引用
    /// </summary>
    public Action Event_SuccessIntoHall;
    /// <summary>
    /// 进入大厅失败 触发方法引用
    /// </summary>
    public Action Event_FaillIntoHall;
    /// <summary>
    /// 进入大厅成功 触发方法
    /// </summary>
    public void Fun_SuccessIntoHall()
    {
        // Event_SuccessIntoHall();
    }
    /// <summary>
    /// 进入大厅失败 触发方法
    /// </summary>
    public void Fun_FaillIntoHall()
    {
        Event_FaillIntoHall();
    }






    #endregion
    #region 创建房间
    /// <summary>
    /// 向服务器发送当前的房间规则
    /// </summary>
    /// <param name="RoomRule"></param>
    public void NewRoom(int[] RoomRule, string type)
    {
        CreateRoomReq reqPack = new CreateRoomReq();
        if (type == "poker")
        {

            reqPack.pokerRoom = new PokerRoomRuleInfo();

            BaseProto.playerInfo.m_inGame = GameType.GT_Poker;
            reqPack.gameType = GameType.GT_Poker;
            reqPack.pokerRoom.gameRule = PokerRoomRule.POKER_ROOM_RULE;
            reqPack.pokerRoom.pokerRule = new PokerRule();
            reqPack.pokerRoom.pokerRule.roundNum = (uint)RoomRule[0];

            //.ga.Add(GARule.GA.NGM);
            if (RoomRule[1] == 1)//大三元
            {
                Debug.Log("冠军支付");
                reqPack.pokerRoom.pokerRule.pokerrules.Add(PokerRule.Poker.ChampionPay);

            }
            if (RoomRule[2] == 1)//大三元
            {
                Debug.Log("平局双倍");
                reqPack.pokerRoom.pokerRule.pokerrules.Add(PokerRule.Poker.PingJuDouble);
            }
            if (RoomRule[3] == 1)//大三元
            {
                Debug.Log("抢第一");
                reqPack.pokerRoom.pokerRule.pokerrules.Add(PokerRule.Poker.QiangFirst);
            }
            if (RoomRule[4] == 1)//大三元
            {
                Debug.Log("一条龙");
                reqPack.pokerRoom.pokerRule.pokerrules.Add(PokerRule.Poker.YiTiaoLong);
            }

        }
        else
        {

            reqPack.mjRoom = new MJRoomRuleInfo();

            BaseProto.playerInfo.m_inGame = GameType.GT_MJ;
            reqPack.gameType = GameType.GT_MJ;
            reqPack.mjRoom.gameRule = MJRoomRule.MJ_ROOM_RULE_XZDD;
            reqPack.mjRoom.xzddRule = new XZDDRule();
            Debug.Log("数量:" + RoomRule[0]);
            reqPack.mjRoom.xzddRule.roundNum = (uint)RoomRule[0];
            //番数
            reqPack.mjRoom.xzddRule.maxFan = (uint)RoomRule[1];

            if (RoomRule[2] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.H3Z);//换三张


            if (RoomRule[3] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.TDH);//天地胡

            if (RoomRule[4] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.HJZY);//呼叫转移

            if (RoomRule[5] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.MQZZ);//门清中张

            if (RoomRule[6] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.DYJJD);//将对


            if (RoomRule[7] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.ZMJF);//自摸加番
            if (RoomRule[8] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.ZMJD);//自摸家底
            if (RoomRule[9] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.DGH_ZM);//点杠花
            if (RoomRule[10] == 1)
                reqPack.mjRoom.xzddRule.flags.Add(XZDDRule.XZDD.DGH_FP);//点杠炮
        }
        BaseProto.Inst().CreateRoomRequest(reqPack);
    }
    /// <summary>
    /// 接收当前是否创建房间成功的委托
    /// </summary>
    public delegate void ReciveCreatRoomSuccess();
    public delegate void ReciveCreatRoomFail();
    public delegate void ReciveCreatRoomHas();
    /// <summary>
    /// 没钱了
    /// </summary>
    public delegate void ReciveCreatRoomNEM();
    /// <summary>
    /// 接收当前是否创建房间成功的委托方法
    /// </summary>
    public event ReciveCreatRoomSuccess Event_reciveCreatRoomSuccess;
    public event ReciveCreatRoomFail Event_reciveCreatRoomFail;
    //public event ReciveCreatRoomHas Event_reciveCreatRoomHas;
    //public event ReciveCreatRoomNEM Event_reciveCreatRoomNEM;
    /// <summary>
    /// 接收当前是否创建房间的委托事件的触发方法，有创建成功和创建失败
    /// </summary>
    public void Fun_reciveIsCreatRoom(CreateRoomRsp crp)
    {
        switch (crp.result)
        {
            case CreateRoomRsp.Result.SUCC:
                Debug.Log("创建房间成功！");
                break;
            case CreateRoomRsp.Result.FAIL:
                Debug.Log("创建失败");
                GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("房间创建失败！");
                //Event_reciveCreatRoomFail();
                break;
            case CreateRoomRsp.Result.HAS:
                GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("房间已存在，创建失败！" + crp.roomId + "gameType" + crp.gameType);

                //Event_reciveCreatRoomFail();
                //Event_reciveCreatRoomHas();
                Debug.Log("房间已经存在" + crp.roomId);
                break;
            case CreateRoomRsp.Result.NOT_ENOUGH_MONEY:
                Debug.Log("余额不足");
                GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("该账号余额不足，创建失败！");
                break;
            default:
                break;
        }
    }
    #endregion
    #region 加入房间
    int JoinTime = 0;
    uint LastRoomNum = 0;
    public void AppJoin(uint RoomNum)
    {
        BaseProto.playerInfo.m_cdRoomId = RoomNum;
        BaseProto.Inst().EnterRoomRequest();

        //返回房间号码
    }
    public delegate void JoinRoomSuccess(ProtoBuf.EnterRoomRsp rsp);
    public event JoinRoomSuccess Event_joinRoomSuccess;

    public void Fun_joinRoomSuccess(ProtoBuf.EnterRoomRsp rsp)
    {
        if (Event_joinRoomSuccess != null)
            Event_joinRoomSuccess(rsp);
    }

    public delegate void JoinRoomFail();
    public event JoinRoomFail Event_joinRoomFail;
    public delegate void JoinRoomFull();
    public event JoinRoomFull Event_joinRoomFull;
    public delegate void JoinRoomHaSin();
    public event JoinRoomFull Event_joinRoomHaSin;

    public void Fun_JoinResult(ProtoBuf.EnterRoomRsp rsp)
    {
        switch (rsp.result)
        {
            case EnterRoomRsp.Result.SUCC:
                Debug.Log("是他创建的");
                if (BaseProto.playerInfo.m_atRoomId != 0)//那么就是他创建的
                {
                    if (rsp.gameType == GameType.GT_MJ)
                    {

                        Debug.Log("加入麻将房间成功！");
                        if (GameManager.GM.DS.MJGameController == null)
                            GameManager.GM.DS.MJGameController = GameManager.GM.PopUI(ResPath.MJGameController, false);
                        PublicEvent.GetINS.ReciveData(rsp);
                        Fun_joinRoomSuccess(rsp);
                    }
                    else
                    if (rsp.gameType == GameType.GT_Poker)
                    {
                        Debug.Log("加入扑克房间成功！");
                    }
                    ParticleManager.GetIns.SwitchSence(2);
                }
                break;
            case EnterRoomRsp.Result.FAIL:
                BaseProto.playerInfo.m_atRoomId = 0;
                BaseProto.playerInfo.m_cdRoomId = 0;
                Debug.Log("进入游戏失败");
                GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("进入房间失败,请检查房间号!");
                break;
            case EnterRoomRsp.Result.FULL:
                    Debug.Log("房间人数已满");
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("房间人数已满!");
                break;
            case EnterRoomRsp.Result.HASIN:
                    Debug.Log("该账号已经存在在该房间内");
                    GameManager.GM.PopUI(ResPath.Notic).GetComponent<UI_Notic>().SetMessage("该账号已经存在于房间内");
                break;
            default:
                break;
        }

    }
    int SwitchTimes = 0;
    GameType SwitchGameToContack()
    {
        ++SwitchTimes;
        switch (SwitchTimes)
        {
            case 1:
                return GameType.GT_MJ;
            case 2:
                return GameType.GT_Poker;
            case 3:
                SwitchTimes = 0;
                return GameType.GT_NULL;
            default:
                break;
        }
        return GameType.GT_NULL;
    }
    #endregion
    #region 加载的数据
    /// <summary>
    /// 进入房间之后要加载的数据
    /// </summary>
    /// <param name="mjRoom"></param>
    public void ReciveData(ProtoBuf.EnterRoomRsp rsp)
    {
        if (rsp.gameType == GameType.GT_MJ)
        {
            #region 麻将
            DataManage.Instance.SetRoomEnterRsp = rsp;
            DataManage.Instance.roomBoosId = rsp.mjRoom.charIds[0];
            Player_Data[] zjAllPlayerAry = new Player_Data[4];
            for (int i = 0; i < zjAllPlayerAry.Length; i++)
            {
                if (zjAllPlayerAry[i] == null)
                {
                    zjAllPlayerAry[i] = new Player_Data();
                }
            }
            for (int i = 0; i < rsp.mjRoom.charIds.Count; i++)
            {
                zjAllPlayerAry[i] = new Player_Data(rsp.mjRoom.charInfos[i].userName, rsp.mjRoom.charInfos[i].charId, rsp.mjRoom.charInfos[i].ip, rsp.mjRoom.charInfos[i].portrait, (int)rsp.mjRoom.charInfos[i].diamond, (int)rsp.mjRoom.charInfos[i].gold, (int)rsp.mjRoom.charInfos[i].sex, new Vector2(rsp.mjRoom.charStates[i].latitude, rsp.mjRoom.charStates[i].longitude));
                Debug.Log("玩家:" + rsp.mjRoom.charInfos[i].userName + "  玩家id" + rsp.mjRoom.charInfos[i].charId);
            }
            uint[] TempList = new uint[4];
            TempList = GameManager.GM.SortAllPlayerList(rsp.mjRoom.charIds);
            for (int i = 0; i < TempList.Length; i++)
            {
                for (int z = 0; z < rsp.mjRoom.charInfos.Count; z++)
                {
                    if (TempList[i] == rsp.mjRoom.charInfos[z].charId)
                    {
                        GameManager.GM.ReSetAllPlayerData(rsp.mjRoom.charInfos[z], i);
                        break;
                    }
                }
            }
            zjAllPlayerAry = DataManage.Instance.PData_Rank(zjAllPlayerAry);
            DataManage.Instance.PData_Update(zjAllPlayerAry);
            Fun_ReUpdateMj(rsp.mjRoom);
            if (PublicEvent.GetINS.Fun_SameIpTip != null)
            {
                PublicEvent.GetINS.Fun_SameIpTip(rsp.mjRoom.charIds.Count);
            }
            #endregion
        }
        else
            if (rsp.gameType == GameType.GT_Poker)
        {

        }
    }

    /// <summary>
    /// 刷新玩家信息的委托
    /// </summary>
    /// 
    public event JoinPlayerData Event_Join_PlayerData;
    public delegate void JoinPlayerData(ProtoBuf.CharacterInfo temp);

    /// <summary>
    /// 其它玩家加入时刷新玩家UI信息的委托事件
    /// </summary>

    public delegate void ExitPlayerData(ProtoBuf.CharacterInfo temp);
    public event ExitPlayerData Event_Exit_PlayerData;
    /// <summary>
    /// 本地玩家进入别人的房间的时候要把本地的玩家准备状态给初始化
    /// </summary>
    public delegate void JoinRoomPlayerUpdata(ProtoBuf.EnterRoomRsp value = null);
    public event JoinRoomPlayerUpdata Event_JoinRoomPlayerUpdata;
    /// <summary>
    /// 排序之后刷新首次进入当前房间内的玩家数据
    /// </summary>
    public void Fun_JoinRoomPlayerUpdata(ProtoBuf.EnterRoomRsp value = null)
    {
        //Event_JoinRoomPlayerUpdata(value);
    }

    /// <summary>
    /// 刷新玩家UI信息的委托事件的方法
    /// </summary>
    public void Fun_JoinPlayerData(ProtoBuf.CharacterInfo temp)
    {
        Debug.Log("加入进来的人的名字：" + temp.userName);
        Event_Join_PlayerData(temp);
    }
    public void Fun_ExitPlayerData(ProtoBuf.CharacterInfo temp)
    {
        Event_Exit_PlayerData(temp);
    }
    #endregion
    #region 音乐管理
    public delegate void ChangeEffectValue(float value);
    public delegate void ChangeBgValue(float value);
    public ChangeEffectValue EventChangeEffectValue;
    public ChangeBgValue EventChangeBgValue;
    public void Fun_ChangeBgValue(float value)
    {
        EventChangeBgValue(value);
    }
    public void Fun_ChangeEffectValue(float value)
    {
        EventChangeEffectValue(value);
    }

    #endregion
    /// <summary>
    /// 头像显示
    /// </summary>
    /// <param name="player"></param>
    /// <param name="dispear"></param>
    public delegate void DisHead(uint player, bool dispear);
    public event DisHead Event_DisHead;
    public void Fun_DisHead(uint player, bool dispear)
    {
        Event_DisHead(player, dispear);
    }
    public delegate void ShowSpeak(int n);
    public event ShowSpeak Event_ShowSpeak;
    public void Fun_ShowSpeak(int n)
    {
        Event_ShowSpeak(n);
    }
    #region 准备阶段客户端发送
    //客户端准备
    public void Fun_SentClientPre()
    {

        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_PREP;
        MJProto.Inst().MJGameOPRequest(pack);
        Debug.Log("发送准备");
    }

    #endregion
    #region 准备阶段服务器广播
    //收到同Ip提示
    public Action<int> Fun_SameIpTip;
    //收到玩家准备
    public delegate void RecivePlayerReady(uint PlayerID);
    public event RecivePlayerReady Event_recivePlayerReady;
    public void Fun_recivePlayerReady(uint PlayerID)
    {
        if (Event_recivePlayerReady != null)
        {
            Event_recivePlayerReady(PlayerID);
        }
    }
    #endregion
    #region  游戏阶段客户端发送

    //客户端选缺 注:1是万，2是筒，3是条
    public void Fun_SentSelectQue(uint num)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_XQ;
        pack.charId = BaseProto.playerInfo.m_id;
        pack.param = num;
        MJProto.Inst().MJGameOPRequest(pack);
        Debug.Log("选缺");
    }
    //客户端换三张
    public void SentChange3Zhang(List<uint> CardId)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.op = MJGameOP.MJ_OP_X3Z;
        for (int i = 0; i < CardId.Count; i++)
        {
            pack.x3zCardId.Add(CardId[i]);
            Debug.LogError("本地发出去的 换三张的牌:" + CardId[i]);
        }
        MJProto.Inst().MJGameOPRequest(pack);
    }

    //客户端摸牌(仅第一次庄家摸牌时候使用)
    public void Fun_SentClientMo(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.cardId = CardNum;
        pack.op = MJGameOP.MJ_OP_MOPAI;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    //客户端出牌
    public void Fun_SentPopCard(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_CHUPAI;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
        if (OnSentPopCard != null)
        {
            OnSentPopCard();
        }
    }
    public Action OnSentPopCard;
    //客户端发过
    public void Fun_SentGuo(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_GUO;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = false;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    //客户端发碰
    public void Fun_SentPeng(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_PENG;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    //客户端发杠
    public void Fun_SentGang(uint CardNum, uint OriCharid)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_GANG;
        pack.oricharId = OriCharid;
        pack.cardId = CardNum;

        pack.charId = BaseProto.playerInfo.m_id;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = true;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    //客户端发胡
    public void Fun_SentHu(uint CardNum, uint OriCharid)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_HU;
        pack.oricharId = OriCharid;
        pack.charId = BaseProto.playerInfo.m_id;
        pack.cardId = CardNum;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = true;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    //客户端发吃
    //public void Fun_SentChi(uint CardNum, uint OriCharid,uint ChiType )
    //{
    //    MJGameOpReq pack = new MJGameOpReq();
    //    pack.op = MJGameOP.MJ_OP_CHI;
    //    pack.oricharId = OriCharid;
    //    pack.charId = BaseProto.playerInfo.m_id;
    //    pack.cardId = CardNum;
    //    pack.param = ChiType;
    //    MJProto.Inst().MJGameOPRequest(pack);
    //}

    #endregion 
    #region 游戏阶段服务器广播

    //收到庄家
    public delegate void ReciveZhuang(uint ZhuangID);
    public event ReciveZhuang Event_reciveZhuang;
    public void Fun_reciveZhuang(uint ZhuangID)
    {

        Debug.Log("庄家" + ZhuangID);
        if (Event_reciveZhuang != null)
        {
            Event_reciveZhuang(ZhuangID);
        }
        else
            Debug.Log("Event_reciveZhuang 没有注册!");
    }

    //收到第一次手牌
    public delegate void ReciveGetFirstCards(List<uint> Cards, int i);
    public event ReciveGetFirstCards Event_reciveGetFirstCards;
    public void Fun_reciveGetHandCards(List<uint> Cards, int seat)
    {
        string str = seat + "号玩家";
        for (int i = 0; i < Cards.Count; i++)
        {
            str += "  第" + i + "张牌" + Cards[i];
        }

        Debug.Log(str);

        if (Event_reciveGetFirstCards != null)
        {
            Event_reciveGetFirstCards(Cards, seat);

        }
        else
            Debug.Log("Event_reciveGetFirstCards 没有注册!");
    }

    /// 收到三张牌(换三张)
    public delegate void ReciveChange3ZhangResult(List<uint> Change3ZhangCard);
    public event ReciveChange3ZhangResult Event_ReciveChange3ZhangResult;
    public void Change3ZhangResult(List<uint> Change3ZhangCard)
    {
        if (Event_ReciveChange3ZhangResult != null)
        {
            Event_ReciveChange3ZhangResult(Change3ZhangCard);
        }
        else
            Debug.Log("Event_ReciveChange3ZhangResult 没有注册!");
    }
    public delegate void ReciveChange3ZhangOthercharid(uint charid);
    public event ReciveChange3ZhangOthercharid Evnet_ReciveChange3ZhangOthercharid;
    public void Change3ZhangOtherResult(uint charid)
    {
        if (Evnet_ReciveChange3ZhangOthercharid != null)
            Evnet_ReciveChange3ZhangOthercharid(charid);
    }

    //收到选缺.
    public delegate void ReciveSelectQue(uint PlayerID, uint CardType);
    public event ReciveSelectQue Event_reciveSelectQue;
    public void Fun_reciveSelectQue(uint PlayerID, uint CardType)
    {
        // 注: 1是万，3是条，2是筒
        if (Event_reciveSelectQue != null)
        {
            Event_reciveSelectQue(PlayerID, CardType);
        }
        else
            Debug.Log("Event_reciveSelectQue 没有注册!");
    }

    //收到摸牌
    public delegate void ReciveGetCard(uint Charid, uint card);
    public event ReciveGetCard Event_reciveGetCard;
    public void Fun_reciveGetCard(uint Charid, uint card)
    {
        if (Event_reciveGetCard != null)
        {
            Event_reciveGetCard(Charid, card);
        }
        else
            Debug.Log("Event_reciveGetCard 没有注册!");
    }

    //收到可操作Ask    
    #region 收到可操作Ask
    //客户端可过
    public delegate void ReciveGuo();
    public event ReciveGuo Event_reciveGuo;
    public void Fun_reciveGuo()
    {
        Event_reciveGuo();
    }

    ////客户端可碰
    //public delegate void RecivePeng(uint cardid);
    //public event RecivePeng Event_KeYiPeng;
    //public void Fun_KeYiPeng(uint charid, uint cardid, uint Oricharid)
    //{
    //    if (charid == GameManager.GM._AllPlayerData[0].ID)//如果是本地玩家再显示他的碰Button
    //        Event_KeYiPeng(cardid);
    //}

    //客户端可胡
    public delegate void ReciveHu(uint cardid, uint OriCharid);
    public event ReciveHu Event_KeYiHu;
    public void Fun_KeYiHu(uint charid, uint cardid, uint OriCharid)
    {
        if (charid == GameManager.GM._AllPlayerData[0].ID)//如果是本地玩家再显示他的胡Button

            Event_KeYiHu(cardid, OriCharid);
    }

    //客户端可杠
    public delegate void ReciveGangList(List<uint> cardid, uint OriCharid);
    public event ReciveGangList Event_KeYiGang;
    public void Fun_KeYiGang(uint charid, List<uint> cardid, uint OriCharid)
    {
        if (charid == BaseProto.playerInfo.m_id)
        {
            Event_KeYiGang(cardid, OriCharid);
        }//如果是本地玩家再显示他的杠Button
    }
    //客户端可吃
    //public delegate void ReciveChi(uint cardid, uint OriCharid,uint Card,List<ChiPaiOP> ChiPaiOP);
    //public event ReciveChi Event_KeYiChi;
    //public void Fun_KeYiChi(uint charid,uint Oricharid,uint Card,List<ChiPaiOP> ChiPaiOP)
    //{
    //    if (charid == GameManager.GM._AllPlayerData[0].ID)
    //    {
    //        //Debug.Log("可以吃！");
    //        Event_KeYiChi(charid, Oricharid, Card, ChiPaiOP);
    //    }
    //}

    //客户端可出牌
    public delegate void KeyiChuPai(uint Charid);
    public event KeyiChuPai Event_KeyiChuPai;
    public void Fun_KeyiChuPai(uint Charid)
    {
        Event_KeyiChuPai(Charid);
    }

    ////收到可以换三张
    //public delegate void KeyiH3z();
    //public event KeyiH3z Event_KeyiH3z;
    //public void Fun_KeyiH3z()
    //{
    //    Event_KeyiH3z();
    //}

    //public Action EnableH3zButton;
    //public Action DisableH3zButton;
    //public Action OnClickH3z;// UI 告诉 MJ


    //public delegate void KeyiXQ();
    //public event KeyiXQ Event_KeyiXQ;
    //public void Fun_KeyiXQ()
    //{
    //    if (Event_KeyiXQ != null)
    //    {
    //        Event_KeyiXQ();
    //    }
    //}

    #endregion

    public delegate void Event_ReciveOtherPeng(uint charid, uint card, uint Oricharid);
    public Event_ReciveOtherPeng Fun_ReciveOtherPeng;
    //杠
    public delegate void Event_ReciveOtherGang(uint charid, uint card, uint oricharid);
    public Event_ReciveOtherGang Fun_ReciveOtherGang;
    //胡
    public delegate void Event_ReciveOtherHu(uint charid, uint card, uint oricharid);
    public Event_ReciveOtherHu Fun_ReciveOtherHu;
    //吃
    public delegate void Event_ReciveOtherChi(uint charid, uint a, uint b, uint c, uint card, uint oricharid);
    public Event_ReciveOtherChi Fun_ReciveOtherChi;
    //过
    public delegate void Event_ReciveOtherGuo(uint charid);
    public Event_ReciveOtherGuo Fun_ReciveOtherGuo;
    public Action<uint> Event_ZhuangChuDiYiZhang;


    //收到出牌
    public delegate void ReciveOtherPopCard(uint PlayerId, uint CardId);
    public event ReciveOtherPopCard Event_ReciveOtherPopCard;
    public void Fun_reciveOtherPopCard(uint PlayerId, uint CardId)
    {
        Debug.Log(PlayerId + " 出牌" + CardId);
        if (Event_ReciveOtherPopCard != null)
        {
            Event_ReciveOtherPopCard(PlayerId, CardId);
        }
    }
    //指示灯
    public delegate void DirLight(uint charid);
    public event DirLight Event_DirLight;
    public void Fun_DirLight(uint charid)
    {
        if (Event_DirLight != null)
        {
            Event_DirLight(charid);
        }
    }
    //牌数
    public Action<int> Fun_UpdatePaishu;

    #endregion   
    #region 社交消息

    #region 社交消息客户端发送
    //玩家发送的预制消息
    public void SentMegssageId(uint MesId)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.PreDefine;
        pack.msgId = MesId;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    ///发送当前的自定义文字
    public void SentMegssageText(string MesText)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.InputText;
        pack.msgString = MesText;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    //玩家发送的图片
    public void SentMegssageImage(/*uint MesId*/string BqId)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.PreDefine;
        pack.msgString = BqId;
        //pack.msgId = MesId;
        Debug.Log("发送的图片ID:" + BqId);
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    //把玩家发送的声音
    public void SentMegssageVoice(byte[] Mesbyte)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.InputVoice;
        pack.msgBytes = Mesbyte;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    #endregion
    #region 社交消息服务器广播
    // 服务器提示其他玩家所发出的消息的委托
    public delegate void ReciveMessagePreDefine(uint Sender, string Value);
    public delegate void ReciveMessageText(uint Sender, string Value);

    // 服务器发给当前客户端，该玩家所发出的消息的委托事件
    public event ReciveMessagePreDefine Fun_reciveMessagePreDefine;
    public event ReciveMessageText Event_reciveMessageText;
    public event ReciveHuDongDaoJu Event_reciveHuDongDaoJu;
    public delegate void ReciveHuDongDaoJu(uint Sender, uint Reciver, uint Value);

    // 服务器发给当前客户端，该玩家所发出的消息的委托事件的方法
    public void Fun_reciveOtherMessage(ChatMessageRsp pack)
    {
        Debug.Log("发送的玩家" + pack.senderId + pack.msgType);

        switch (pack.msgType)
        {
            case ChatMessageRsp.MsgType.PreDefine:
                {
                    Fun_reciveMessagePreDefine(pack.senderId, pack.msgString);
                    Debug.Log("收到的图片ID:" + pack.msgString);
                }
                break;
            case ChatMessageRsp.MsgType.InputText:
                {

                    Event_reciveMessageText(pack.senderId, pack.msgString);

                }
                break;
            case ChatMessageRsp.MsgType.InputVoice:
                {
                    Debug.Log("收到别人发的语音");
                    //自定义语音直接播放
                    //Event.Inst().F_PlaySound(pack.senderId, pack.msgString);
                    F_PlaySound(pack.senderId, pack.msgString);
                }
                break;
            case ChatMessageRsp.MsgType.Throw:
                {
                    Debug.Log("收到别人发的道具");
                    //发的道具
                    //Event.Inst().F_PlaySound(pack.senderId, pack.msgString);
                    Event_reciveHuDongDaoJu(pack.senderId, pack.dirId, pack.param);
                }
                break;
            default:
                break;
        }
        //自己发出来的消息，在本地客户端自己发出来
    }
    /// <summary>
    /// 发送道具
    /// </summary>
    /// <param name="ReciveId"></param>
    /// <param name="BqId"></param>
    public void SentMegssageDaoju(uint ReciveId, uint BqId)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.Throw;
        pack.param = BqId;
        pack.dirId = ReciveId;
        //pack.msgId = MesId;
        Debug.Log("发送的道具ID:" + BqId);
        BaseProto.Inst().SendChatMsgRequest(pack);
    }

    public delegate void OnClick(bool isSend = true);
    public OnClick StartMic;
    public delegate void playSound(uint SendId, string url);
    public event playSound PlaySound;

    public void F_PlaySound(uint SendID, string url)
    {
        PlaySound(SendID, url);
        Events.Inst().F_PlaySound(url);
    }
    public delegate void playSoundEnd();
    public event playSoundEnd Event_playSoundEnd;
    public void F_playSoundEnd()
    {
        Event_playSoundEnd();
    }

    #endregion

    #endregion
    #region 退出及投票  结算及结束  网络断线


    //发起投票 
    public void VoteRequest(bool istongyi)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.op = MJGameOP.MJ_OP_VOTE_JSROOM;
        if (istongyi)
            pack.param = 2;
        else
            pack.param = 1;

        Debug.Log("投票发送了" + pack.param);
        MJProto.Inst().MJGameOPRequest(pack);
    }
    /// <summary>
    /// 是否是我发起的投票
    /// </summary>
    public bool IsMyVote = false;
    public int voteTime = 0;
    //接到别人投票的事件
    public delegate void ReciveOtherVote(uint charid, bool istongyi);
    public event ReciveOtherVote Event_ReciveOherVote;
    public void Fun_ReciveOherVote(uint charid, uint istongyi)
    {
        if (GameManager.GM.DS.Voting == null)
        {
            GameManager.GM.DS.Voting = GameManager.GM.PopUI(ResPath.Voting);
            GameManager.GM.DS.Voting.GetComponent<UI_Vote>().Default(GameManager.GM.GetPlayerName(charid));
        }
        if (IsMyVote)
            GameManager.GM.DS.Voting.GetComponent<UI_Vote>().DisableAllBtn();

        //这时候已经有人开始投票了，还没有存在投票界面
        if (istongyi == 1)
        {
            Debug.Log("收到别人投票拒绝");
            GameManager.GM.DS.Voting.GetComponent<UI_Vote>().SetVote(charid, false);
        }
        else if (istongyi == 2)
        {
            Debug.Log("收到别人投票确定");
            GameManager.GM.DS.Voting.GetComponent<UI_Vote>().SetVote(charid, true);
        }
    }
    public delegate void voteQuit1();
    public event voteQuit1 voteQuit;
    //接到投票结果
    public delegate void ReciveVoteResult(bool isjiesan);
    public event ReciveVoteResult Event_ReciveVoteResult;
    public bool VoteQuit = false;
    public void Fun_ReciveVoteResult(uint isjiesan)
    {
        PublicEvent.GetINS.voteTime = 0;
        PublicEvent.GetINS.IsMyVote = false;
        Debug.Log("投票成功！");
        //Debug.Log(isjiesan);
        if (isjiesan == 1)
        {
            voteQuit();
        }
        else
        {
            Debug.Log("投票解散成功！");
            VoteQuit = true;
            if (GameOverRsp == null)
                Fun_ExitRoomSucc();
        }
    }

    //请求退出房间
    public void OnExitRoom()
    {
        BaseProto.Inst().ExitRoomRequest();
    }
    // 接受服务器退出房间成功
    public delegate void ExitRoomSucc();
    public event ExitRoomSucc Event_ExitRoomSucc;
    public void Fun_ExitRoomSucc()
    {
        //UIManager.Instance.ShowUI("HomeLobbyUI");

        //关闭成功事件
        if (Event_ExitRoomSucc != null)
        {
            ParticleManager.GetIns.SwitchSence(1);
            Event_ExitRoomSucc();

        }
    }
    /// <summary>
    /// 用于点了战绩然后删掉战绩界面的委托，避免在麻将那边出现战绩依旧存在的问题出现
    /// </summary>
    public Action ReturnToMain;
    //接收每局结束结果
    public delegate void ReciveRoundOverResult(MJGameOver rsp);

    public event ReciveRoundOverResult Event_ReciveRoundOverResult;
    public MJGameOver GameOverRsp = null;
    public void Fun_ReciveRoundOverResult(MJGameOver rsp)
    {
        GameOverRsp = rsp;
        if (Event_ReciveRoundOverResult != null)
        {
            Event_ReciveRoundOverResult(rsp);
        }
        ThePlayerInfo[] playerinfo = new ThePlayerInfo[4];
        for (int i = 0; i < rsp.players.Count; i++)
        {
            int pos = DataManage.Instance.PData_GetIndex(rsp.players[i].charId);
            int[] tag = new int[] { 0, 0, 0, 0 };
            SetHead(DataManage.Instance.Head_GetSprite(rsp.players[i].charId), pos);
            // 0胡  1自摸   2放炮   3没有胡
            if (rsp.players[i].huInfos != null)
            {
                if (rsp.players[i].huInfos.Count > 0)
                {
                    if (rsp.players[i].huInfos[0].card != 216)
                    {
                        if (rsp.players[i].huInfos[0].oricharId != rsp.players[i].charId)//catag属性未名
                        {
                            //胡
                            tag[0] = 1;
                        }
                        else
                        {
                            //自摸
                            tag[1] = 1;
                        }
                    }
                }
            }
            else
            {
                if (rsp.players[i].fpInfos != null)
                {
                    for (int z = 0; z < rsp.players[i].fpInfos.Count; z++)
                    {
                        if (rsp.players[i].fpInfos[z].card != 216 && rsp.players[i].fpInfos[z].card != 0)
                        {
                            //放炮
                            tag[2] = 1;
                            Debug.Log("放炮有！");
                        }
                    }
                }
                else
                {
                    tag[3] = 1;
                }
            }
            playerinfo[i] = (new ThePlayerInfo(rsp.players[i].charId, rsp.players[i].restGold.ToString(), Temphead, rsp.players[i].changeGold.ToString(), tag));
            for (int z = 0; z < 4; z++)
            {
                if (rsp.players[i].charId == DataManage.Instance.PData_GetSeatID(z))
                {
                    DataManage.Instance.PData_GetData(rsp.players[i].charId).p_gold = rsp.players[i].restGold;
                    GameManager.GM._AllPlayerData[z].Money = rsp.players[i].restGold;
                }
            }
        }

        var t = GameManager.GM.PopUI(ResPath.PlayEnd);
        t.GetComponent<UI_PlayEnd>().Default(playerinfo);
        GameManager.GM.DS.PlayEnd = t;
        GameManager.GM.DS.PlayEnd.GetComponent<UI_PlayEnd>().SetCard(rsp);
    }
    Sprite Temphead = null;
    void SetHead(Sprite sprite, int num = 0)
    {
        Temphead = sprite;
    }


    ////玩家点击再来一局
    //public delegate void ReadyToPlayNew();

    //public event ReadyToPlayNew Event_ReadyToPlayNew;

    //public void Fun_ReadyToPlayNew()
    //{
    //    Event_ReadyToPlayNew();
    //}

    ////游戏结束了
    //public delegate void ReciveGameOverResult();

    //public event ReciveGameOverResult Event_ReciveGameOverResult;

    //public void Fun_ReciveGameOverResult()
    //{
    //    //Event_ReciveGameOverResult();
    //}
    //断线了
    public void DisconnectNet()
    {
        //回收游戏场景
        if (Event_ExitRoomSucc != null)
        {
            Event_ExitRoomSucc();
        }
        //退出服务器
        AppLoginOut();

        //退出微信服务器
        //AnySDKManager.SendLogout();

        //弹窗 "网络异常 请重新登录"
        //var window = GameManager.GM.SearchEmpty().AddComponent<Login>();
        //window.Ins();
        //UIManager.Instance.ShowUI("LoginUI");

        //var window = UIManager.Instance.ShowUI("NoticeUI").GetComponent<NoticeUI_C>();
        //window.InputText(false, "网络异常,请重新登录！");
        //window.GraduallyRest();
    }


    #endregion
    #region 断线重连
    //等待过程中重连更新UI
    public delegate void ReUpdateWatingUi(ProtoBuf.MJRoomInfo mjRoom);
    public event ReUpdateWatingUi Event_ReUpdateWatingUi;
    public void Fun_ReUpdateWatingUi(MJRoomInfo mjRoom)
    {
        if (Event_ReUpdateWatingUi != null)
        {
            Event_ReUpdateWatingUi(mjRoom);
        }
        else
        {
            Debug.Log("Event_ReUpdateWatingUi 等待阶段重连时间没有添加");
            //PublicEvent.GetINS.VoteRequest(true);
        }
    }
    // 游戏过程中重连更新麻将
    public delegate void ReUpdateMj(ProtoBuf.MJRoomInfo mjRoom);
    public event ReUpdateMj Event_ReUpdateMj;
    public void Fun_ReUpdateMj(ProtoBuf.MJRoomInfo mjRoom)
    {
        if (Event_ReUpdateMj != null)
        {
            Event_ReUpdateMj(mjRoom);
        }
        else
        {
            Debug.Log("Event_ReUpdateMj 游戏阶段重连时间没有添加");
            //PublicEvent.GetINS.VoteRequest(true);
        }
    }
    #endregion
    #region 接收公告、消息等
    // 接收公告信息的委托
    public delegate void RecivePublicText(string value);
    // 接收公告信息的委托事件
    public event RecivePublicText Event_recivePublicText;
    //接收服务器发送的公告信息
    public void Fun_recivePublicText(string value)
    {
        Event_recivePublicText(value);
    }
    //接收Message的委托
    public delegate void ReciveMessage(string value);
    //接收Message的委托方法
    public event ReciveMessage Event_reciveMessage;
    //接收Message
    public void Fun_reciveMessage(string value)
    {
        Event_reciveMessage(value);
    }
    #endregion
    #region 战绩以及回放
    public void ZhanjiHuiFangRequst(ProtoBuf.GameType gameType = GameType.GT_MJ)
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.queryType = QueryInfoReq.QueryType.ZhanJi;
        pack.gameType = (int)gameType;
        pack.param1 = 0;
        pack.param2 = 11;
        BaseProto.Inst().QueryInfoRequest(pack);
    }

    public Action<QueryInfoRsp> Fun_ReciveZhanJiHuiFang;
    public void ReciveZhanJiHuiFang(QueryInfoRsp rsp)
    {
        Debug.Log("收到战绩" + (GameType)rsp.gameType);

        if (rsp.mjRecords.Count > 0)
        {
            GameManager.GM.combatGainRsp = rsp;
            Debug.Log("麻将战绩记录：" + rsp.mjRecords.Count);
        }
        if (rsp.qpRecords.Count > 0)
        {
            GameManager.GM.qpcombatGainRsp = rsp;
            Debug.Log("扑克战绩记录：" + rsp.qpRecords.Count);
        }
        if (rsp.gameType == (int)GameType.GT_MJ)
        {
            ZhanjiHuiFangRequst(GameType.GT_Poker);
        }
    }
    //回放开始
    public Action ReViewStart;
    //回放结束
    public Action ReViewEnd;
    #endregion
    #region 玩家信息主动查询
    //查询钻石
    public void DiamondRequst()
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.queryType = QueryInfoReq.QueryType.CharInfo;
        BaseProto.Inst().QueryInfoRequest(pack);
    }
    public void ReciveDiamond(QueryInfoRsp rsp)
    {
        //if (ReciveDiamon != null)
        //{
        //ReciveDiamon((int)rsp.charInfoDy.diamond);
        if (GameManager.GM.DS.Main != null)
            GameManager.GM.DS.Main.GetComponent<UI_Main>().SetRoomCard(GameManager.GM._AllPlayerData[0].Diamond.ToString());

        // GameManager.GM.DS.Main.GetComponent<UI_Main>().SetInfo(GameManager.GM._AllPlayerData[0].Name, GameManager.GM._AllPlayerData[0].ID.ToString(), GameManager.GM._AllPlayerData[0].Diamond.ToString(), GlobalSettings.avatarUrl);
        GameManager.GM._AllPlayerData[0].Diamond = rsp.charInfoDy.diamond;
        //}
    }
    public Action<int> ReciveDiamon;
    //
    #endregion
    #region 移动端多触点
    public Action MoreThanOneTouch;
    #endregion

    /// <summary>
    /// 提示客户端可以‘碰’这个操作的委托
    /// </summary>
    public delegate void RecivePeng(uint cardid);
    /// <summary>
    /// RecivePeng类型，服务器发送客户端‘碰’这个操作的委托事件
    /// </summary>
    public event RecivePeng Event_KeYiPeng;
    /// <summary>
    /// 提示客户端‘碰’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiPeng(uint charid, uint cardid)
    {
        if (charid == BaseProto.playerInfo.m_id)//如果是本地玩家再显示他的碰Button
            Event_KeYiPeng(cardid);
    }
    public delegate void KeyiMoPai(uint Charid, uint OriCharid);
    public event KeyiMoPai Event_KeyiMoPai;

    public void Fun_KeyiMoPai(uint Charid, uint OriCharid)
    {
        if (Event_KeyiMoPai != null)
        {
            Event_KeyiMoPai(Charid, OriCharid);
        }
    }
    /// <summary>
    /// 接收其他玩家已经碰杠胡过的消息的委托
    /// </summary>
    /// <param name="MjOp"></param>
    /// <param name="PlayerID"></param>
    public delegate void ReciveOtherCanPlay(MJGameOP MjOp, uint PlayerID, uint card, uint oricharid);
    public event ReciveOtherCanPlay Event_ReciveOtherCanPlay;
    /// <summary>
    /// 接收其他玩家已经碰杠胡过的消息
    /// </summary>
    /// <param name="MjOp"></param>
    public void Fun_reciveOtherPengGangHu(MJGameOP MjOp, uint PlayerID, uint card, uint oricharid)
    {
        Event_ReciveOtherCanPlay(MjOp, PlayerID, card, oricharid);
    }
    /// <summary>
    /// 客户端现在可以选择的操作的委托
    /// </summary>
    public delegate void ReciveCanPlay(MJGameOP MjOp, List<uint> cards);
    /// <summary>
    /// 服务器发送给客户端现在可以选择的操作的委托事件 过碰杠胡出牌
    /// </summary>
    public event ReciveCanPlay Event_reciveCanPlay;
    /// <summary>
    /// 客户端现在可以选择 过碰杠胡出牌，委托事件的方法
    /// </summary>
    public void Fun_reciveCanPlay(uint charid, MJGameOP MjOp, List<uint> cards, uint OriCharid/*,uint cardid  可碰杠胡的牌*/ )
    {
        //Event_reciveCanPlay(MjOp, cards); //自己调用的方法
        //Debug.Log("调用了么？");


        switch (MjOp)
        {
            ////case MJGameOP.MJ_OP_XQ://暂时不需要
            ////break;
            //case MJGameOP.MJ_OP_GUO:
            //    Fun_reciveGuo();
            //    break;
            case MJGameOP.MJ_OP_CHUPAI:
                Event_ZhuangChuDiYiZhang(charid);
                Fun_KeyiMoPai(charid, OriCharid);

                break;
            case MJGameOP.MJ_OP_PENG:
                Debug.Log("玩家:" + charid + " cards[0]:" + cards[0]);
                Fun_KeYiPeng(charid, cards[0]);
                break;
            case MJGameOP.MJ_OP_GANG:
                Fun_KeYiGang(charid, cards, OriCharid);
                break;
            case MJGameOP.MJ_OP_HU:
                Fun_KeYiHu(charid, cards[0], OriCharid);
                break;
            case MJGameOP.MJ_OP_ROUND_OVER:
                break;
            case MJGameOP.MJ_OP_VOTE_JSROOM:
                break;
            case MJGameOP.MJ_OP_VOTE_RESULT:
                break;
            default:
                break;
        }
    }
    public delegate void TingCard(uint charid, List<uint> TingCards);
    public event TingCard Event_TingCard;
    public void Fun_TingCard(uint charid, List<uint> TingCard)
    {
        Event_TingCard(charid, TingCard);
    }
    public Action<uint> PlayerDistance;
}
