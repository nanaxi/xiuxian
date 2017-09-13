using ProtoBuf;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class CreatePokerRoom : MonoBehaviour
    {
        public Toggle guanJun;
        public Toggle junTan;

        public Toggle chuTou;
        public Toggle buChuTou;
        public Toggle yiTiaoLong;
        public Toggle pingJuFanBei;

        public Toggle chuTouDiFen1;
        public Toggle chuTouDiFen2;
        public Toggle buChuTouDiFen1;
        public Toggle buChuTouDiFen2;

        public Slider round;
        public Text roundText;

        public Button createRoomNow;

        uint selectRoundRate = 1;
        uint basicRound = 6;

        void Start()
        {
            ChangeRound(0);

            createRoomNow.onClick.AddListener(CreateRoom);
            round.onValueChanged.AddListener(x => ChangeRound(x));

            LoadConfig();
        }

        void CreateRoom()
        {
            PokerRule rule = new PokerRule();
            // 默认为均摊房费
            if (guanJun.isOn)
            {
                rule.pokerrules.Add(PokerRule.Poker.ChampionPay);
            }
            if (chuTou.isOn)
            {
                rule.pokerrules.Add(PokerRule.Poker.QiangFirst);
                if (chuTouDiFen1.isOn)
                {
                    rule.baseScore = 2;
                }
                else if (chuTouDiFen2.isOn)
                {
                    rule.baseScore = 3;
                }
            }
            if (buChuTou.isOn)
            {
                if (buChuTouDiFen1.isOn)
                {
                    rule.baseScore = 2;
                }
                else if (buChuTouDiFen2.isOn)
                {
                    rule.baseScore = 3;
                }
            }
            if (yiTiaoLong.isOn)
            {
                rule.pokerrules.Add(PokerRule.Poker.YiTiaoLong);
            }
            if (pingJuFanBei.isOn)
            {
                rule.pokerrules.Add(PokerRule.Poker.PingJuDouble);
            }
            rule.roundNum = selectRoundRate * basicRound;

            NetSender.Inst.CreateRoom(rule);

            StartCoroutine(ClickInterval());

            SaveConfig();
        }

        void ChangeRound(float x)
        {
            // 0 ~ 9

            if (round.value <= 2)
            {
                roundText.text = "六局（40张房卡）";
                selectRoundRate = 1;
            }
            else if (round.value <= 4)
            {
                roundText.text = "十二局（80张房卡）";
                selectRoundRate = 2;
            }
            else if (round.value <= 6)
            {
                roundText.text = "十八局（120张房卡）";
                selectRoundRate = 3;
            }
            else if (round.value <= 8)
            {
                roundText.text = "二十四局（160张房卡）";
                selectRoundRate = 4;
            }
            else
            {
                roundText.text = "三十局（200张房卡）";
                selectRoundRate = 5;
            }
        }

        IEnumerator ClickInterval()
        {
            createRoomNow.interactable = false;
            yield return new WaitForSeconds(3);
            createRoomNow.interactable = true;
        }

        void SaveConfig()
        {
            PlayerPrefs.SetString("junTan", junTan.isOn.ToString());

            PlayerPrefs.SetString("chuTou", chuTou.isOn.ToString());
            PlayerPrefs.SetString("chuTouDiFen1", chuTouDiFen1.isOn.ToString());
            PlayerPrefs.SetString("chuTouDiFen2", chuTouDiFen2.isOn.ToString());

            PlayerPrefs.SetString("buChuTou", buChuTou.isOn.ToString());
            PlayerPrefs.SetString("buChuTouDiFen1", buChuTouDiFen1.isOn.ToString());
            PlayerPrefs.SetString("buChuTouDiFen2", buChuTouDiFen2.isOn.ToString());

            PlayerPrefs.SetString("yiTiaoLong", yiTiaoLong.isOn.ToString());

            PlayerPrefs.SetString("pingJuFanBei", pingJuFanBei.isOn.ToString());

            PlayerPrefs.SetString("round", round.value.ToString());

            PlayerPrefs.Save();
        }

        void LoadConfig()
        {
            try
            {
                junTan.isOn = bool.Parse(PlayerPrefs.GetString("junTan", junTan.isOn.ToString()));

                chuTou.isOn = bool.Parse(PlayerPrefs.GetString("chuTou", chuTou.isOn.ToString()));
                chuTouDiFen1.isOn = bool.Parse(PlayerPrefs.GetString("chuTouDiFen1", chuTouDiFen1.isOn.ToString()));
                chuTouDiFen2.isOn = bool.Parse(PlayerPrefs.GetString("chuTouDiFen2", chuTouDiFen2.isOn.ToString()));

                buChuTou.isOn = bool.Parse(PlayerPrefs.GetString("buChuTou", buChuTou.isOn.ToString()));
                buChuTouDiFen1.isOn = bool.Parse(PlayerPrefs.GetString("buChuTouDiFen1", buChuTouDiFen1.isOn.ToString()));
                buChuTouDiFen2.isOn = bool.Parse(PlayerPrefs.GetString("buChuTouDiFen2", buChuTouDiFen2.isOn.ToString()));

                yiTiaoLong.isOn = bool.Parse(PlayerPrefs.GetString("yiTiaoLong", yiTiaoLong.isOn.ToString()));

                pingJuFanBei.isOn = bool.Parse(PlayerPrefs.GetString("pingJuFanBei", pingJuFanBei.isOn.ToString()));

                round.value = float.Parse(PlayerPrefs.GetString("round", round.value.ToString()));
            }
            catch (System.Exception e)
            {
                Log.D("创建房间配置", "加载默认创建房间配置错误" + e.Message);
            }
        }
    }
}