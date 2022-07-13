using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField PlayerName_InputName;

    private const string NICKNAME_KEY = "NICKNAME_KEY";
    private UserInfoManager userInfoManager = new UserInfoManager();

    #region Unity Methods

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region UI Callback Methods

    public void ConnectAnonymously()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToPhotonServer()
    {
        if (PlayerName_InputName != null)
        {
            PhotonNetwork.NickName = PlayerName_InputName.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ConnectToGame()
    {
        string playerNickname;
        playerNickname = "User" + Random.Range(0, 10000);
        string userID = Random.Range(0, 10000).ToString();
        if (userInfoManager.GetLocalUserInfo(out UserInfo localuserInfo))
        {
            playerNickname = localuserInfo.localPlayer.userName;
            userID = localuserInfo.localPlayer.userId;
        }
        else
        {
            UserInfo userInfo = new UserInfo();
            userInfo.localPlayer = new User();
            userInfo.localPlayer.userName = playerNickname;
            userInfo.localPlayer.userId = userID;
            userInfo.friendList = new List<User>();
            Debug.Log(userInfo.localPlayer.userName);
            userInfoManager.SaveLocalUserInfo(userInfo);
        }
        PhotonNetwork.NickName = playerNickname;
        Debug.Log(PhotonNetwork.NickName);
        AuthenticationValues authenticationValues = new AuthenticationValues();
        authenticationValues.UserId = userID;
        PhotonNetwork.AuthValues = authenticationValues;
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region Photon Callback Methods

    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server with player name: " + PhotonNetwork.NickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.LoadLevel("HomeScene");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        PhotonNetwork.LoadLevel("World_School");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room_" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion
}
