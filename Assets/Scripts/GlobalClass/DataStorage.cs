using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class DataStorage : MonoBehaviour
{
    public static DataStorage GetIns = null;
    public GameObject UsedChat;
    public GameObject CreateRoom_Hoom;
    public GameObject CreateRoom_Ga;
    public GameObject CreateRoom_Xz;
    public GameObject Roll;
    public GameObject Share;
    public GameObject Rule;
    public GameObject Main;
    public GameObject JoinRoom;
    public GameObject Call;
    public GameObject PlayerInfo;
    public GameObject Login;
    public GameObject CheckPoint;
    public GameObject Setting;
    public GameObject Store;
    public GameObject Message;
    public GameObject Combat;
    public GameObject IsVote;
    public GameObject Voting;
    //public GameObject PlayRoom;
    public GameObject UI_UsedChat;
    public GameObject PlayEnd;
    public GameObject Notic;
    public GameObject Quit;
    public GameObject GameOver;
    public GameObject voteQuit;
    public GameObject PlayBack;
    public GameObject CombatItems;
    public GameObject InGameManage_3DMjUI;
    public GameObject UiWin_ZJHF;
    public GameObject MJZJHF;
    public GameObject MJGameController;
    public GameObject uiWin_Prompt_QuitRoom;
    void Start()
    {
        if (GetIns == null)
            GetIns = this;
      //  Main = transform.Find("Main").gameObject;
    }
}
