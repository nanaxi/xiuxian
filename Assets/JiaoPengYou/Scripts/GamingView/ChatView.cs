namespace Lang
{
    public class ChatView : View
    {
        public PopChatText[] popChatTexts;

        void Start()
        {
            PublicEvent.GetINS.Fun_reciveMessagePreDefine += ShowAnim;
            PublicEvent.GetINS.Event_reciveMessageText += ShowPop;
        }

        void OnDestroy()
        {
            PublicEvent.GetINS.Fun_reciveMessagePreDefine -= ShowAnim;
            PublicEvent.GetINS.Event_reciveMessageText -= ShowPop;
        }

        void ShowAnim(uint id, string value)
        {
            int seat = (int)PlayerMgr.Inst.GetSeat(id);
            switch (value)
            {
                case "x0xxd0":
                    ShowFace.Ins.PlayAnim(Face.jiayou, seat, 2);
                    break;
                case "x0xxd1":
                    ShowFace.Ins.PlayAnim(Face.keai, seat, 2);
                    break;
                case "x0xxd2":
                    ShowFace.Ins.PlayAnim(Face.cry, seat, 2);
                    break;
                case "x0xxd3":
                    ShowFace.Ins.PlayAnim(Face.huaixiao, seat, 2);
                    break;
                case "x0xxd4":
                    ShowFace.Ins.PlayAnim(Face.weiqu, seat, 2);
                    break;
                case "x0xxd5":
                    ShowFace.Ins.PlayAnim(Face.baochou, seat);
                    break;
                case "x0xxd6":
                    ShowFace.Ins.PlayAnim(Face.fangle, seat);
                    break;
                case "x0xxd7":
                    ShowFace.Ins.PlayAnim(Face.yun, seat, 2);
                    break;
                case "x0xxd8":
                    ShowFace.Ins.PlayAnim(Face.han, seat, 2);
                    break;
                case "x0xxd9":
                    ShowFace.Ins.PlayAnim(Face.meiqianle, seat);
                    break;
                case "x0xxd10":
                    ShowFace.Ins.PlayAnim(Face.shengqi, seat);
                    break;
                case "x0xxd11":
                    ShowFace.Ins.PlayAnim(Face.shuile, seat);
                    break;
                case "x0xxd12":
                    ShowFace.Ins.PlayAnim(Face.zhuangbi, seat, 2);
                    break;
                case "x0xxd13":
                    ShowFace.Ins.PlayAnim(Face.leipi, seat);
                    break;
                case "x0xxd14":
                    ShowFace.Ins.PlayAnim(Face.ciya, seat, 2);
                    break;
                default:
                    break;
            }
        }

        void ShowPop(uint id, string value)
        {
            int seat = (int)PlayerMgr.Inst.GetSeat(id);
            var player = PlayerMgr.Inst.GetPlayer(id);
            popChatTexts[(int)seat].Show(value);
            DataManage.Instance.ChatRecord_Add(player.name, value);
            SoundMag.GetINS.ChatSound(value, (int)player.sex, seat);
        }
    }
}