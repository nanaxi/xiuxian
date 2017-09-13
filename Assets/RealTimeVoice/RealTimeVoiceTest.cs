using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class RealTimeVoiceTest : MonoBehaviour
    {
        public Button joinRoom;
        public Button exitRoom;

        void Start()
        {
            joinRoom.onClick.AddListener(() =>
            {
                RealTimeVoice.Inst.JoinRoom("100000");
                RealTimeVoice.Inst.onJoinRoomSuccess += () =>
                {
                    RealTimeVoice.Inst.OpenSpeaker(true);
                };
            });

            exitRoom.onClick.AddListener(() =>
            {
                RealTimeVoice.Inst.QuitRoom("100000");
            });
        }
    }
}