using ProtoBuf;
using UnityEngine;

namespace Lang
{
    public class PlayerInfo
    {
        /// <summary>
        /// 玩家唯一id
        /// </summary>
        public uint id;

        /// <summary>
        /// channel_user_id
        /// </summary>             
        public string account;

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string name;

        /// <summary>
        /// 玩家当前在哪里（比如玩家已经在玩麻将，此时强制杀掉进程，重新登录后，会再次进入上次的游戏内）
        /// </summary>
        public GameType inGame;

        /// <summary>
        /// 玩家所创建的房间号，如果没有则为0
        /// </summary>        
        public uint crRoomId;

        /// <summary>
        /// 玩家当前所在的房间号，如果没有则为0
        /// </summary> 
        public uint atRoomId;

        /// <summary>
        ///玩家剩余房卡
        /// </summary>
        public uint diamond;

        /// <summary>
        /// ip地址
        /// </summary>
        public string ip;

        /// <summary>
        /// 1男 2女
        /// </summary>
        public uint sex;

        /// <summary>
        /// 玩家头像链接
        /// </summary>
        public string avatarUrl;

        /// <summary>
        /// 纬经度
        /// </summary>
        public Vector2 latlng;

        /// <summary>
        /// 所在城市
        /// </summary>
        public string cityName;

        /// <summary>
        /// 设置玩家头像。获取完成后，自动设置到avatarImg上
        /// </summary>
        /// <param name="avatarImg">待设置的玩家头像Image</param>
        public void SetAvatar(UnityEngine.UI.Image avatarImg)
        {
            if (string.IsNullOrEmpty(avatarUrl))
            {
                Log.D("SetAvatar", string.Format("{0} avatar url is empty", name));
                return;
            }
            string url = avatarUrl;
            GameManager.GM.GetHead(url, (sprite, num) =>
            {
                if (avatarImg != null)
                {
                    avatarImg.sprite = sprite;
                }
            }, 0);
        }

        /// <summary>
        /// 获取城市名
        /// </summary>
        public string GetNormalizedCityName()
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return "未开启定位";
            }
            else
            {
                return cityName;
            }
        }
    }
}