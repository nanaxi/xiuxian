using UnityEngine;
using gcloud_voice;
using System;

public class RealTimeVoice : MonoBehaviour
{
    public static RealTimeVoice Inst;

    IGCloudVoice voiceengine;

    bool isMicOpen = false;
    bool isSpeakerOpen = false;
    bool haveJoin = false;

    public string roomId;

    // 意外退出时，没有退出房间，当重新进入房间时，需要先退出，再重新进入(退出回调事件)
    Action rejoinRoom;

    public Action onJoinRoomSuccess;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string strTime = Convert.ToInt64(ts.TotalSeconds).ToString();
        string id = strTime;

        voiceengine = GCloudVoice.GetEngine();
        // TODO 这里是测试帐号
        //voiceengine.SetAppInfo("932849489", "d94749efe9fce61333121de84123ef9b", id);
        voiceengine.SetAppInfo("1830483713", "fdc643591ea5db8b688c44c588741555", id);
        voiceengine.Init();

        voiceengine.OnJoinRoomComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID) =>
        {
            Debug.Log("OnJoinRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);

            if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_JOINROOM_SUCC)
            {
                haveJoin = true;
                isMicOpen = false;
                isSpeakerOpen = false;

                if (onJoinRoomSuccess != null)
                {
                    onJoinRoomSuccess();
                }
            }
        };

        voiceengine.OnQuitRoomComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID) =>
        {
            Debug.Log("OnQuitRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);

            haveJoin = false;
            isMicOpen = false;
            isSpeakerOpen = false;
            roomId = "";

            if (rejoinRoom != null)
            {
                rejoinRoom();
                rejoinRoom = null;
            }
        };

        voiceengine.SetMode(GCloudVoiceMode.RealTime);
    }

    void Update()
    {
        if (voiceengine != null)
        {
            voiceengine.Poll();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (voiceengine == null)
        {
            return;
        }

        if (pauseStatus)
        {
            voiceengine.Pause();
        }
        else
        {
            voiceengine.Resume();
        }
    }

    void OnApplicationQuit()
    {
        QuitRoom(roomId);
    }

    public void JoinRoom(string roomId)
    {
        if (voiceengine == null)
        {
            return;
        }

        if (haveJoin)
        {
            return;
        }

        this.roomId = roomId;

        int ret = voiceengine.JoinTeamRoom(roomId, 15000);
        if (ret == (int)GCloudVoiceErr.GCLOUD_VOICE_REALTIME_STATE_ERR)
        {
            rejoinRoom = () => { JoinRoom(roomId); };
            QuitRoom(roomId);
        }
    }

    public void OpenMic(bool open)
    {
        if (voiceengine == null)
        {
            return;
        }

        if (!haveJoin)
        {
            return;
        }

        if (open)
        {
            var ret = voiceengine.OpenMic();
            isMicOpen = ret == 0;
        }
        else
        {
            var ret = voiceengine.CloseMic();
            isMicOpen = ret == 0;
        }
    }

    public void OpenSpeaker(bool open)
    {
        if (voiceengine == null)
        {
            return;
        }

        if (!haveJoin)
        {
            return;
        }

        if (open)
        {
            var ret = voiceengine.OpenSpeaker();
            isSpeakerOpen = ret == 0;
        }
        else
        {
            var ret = voiceengine.CloseSpeaker();
            isSpeakerOpen = ret == 0;
        }
    }

    public void QuitRoom(string roomId)
    {
        if (voiceengine == null)
        {
            return;
        }

        if (!haveJoin)
        {
            return;
        }

        voiceengine.QuitRoom(roomId, 6000);

        onJoinRoomSuccess = null;
    }
}