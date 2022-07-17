using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject connectOptionsPanel;
    [SerializeField]
    private GameObject connectWithExistingUserPanel;

    [SerializeField]
    private TMP_InputField playerName_InputName;
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    private void Start()
    {
        if (UserInfoManager.Instance.GetLocalUserInfo(out UserInfo localuserInfo))
        {
            connectWithExistingUserPanel.SetActive(true);
            playerNameText.text = localuserInfo.localPlayer.userName;
        }
        else connectOptionsPanel.SetActive(true);
    }

    #region UI Methods

    public void ConnectWithRandomName()
    {
        string playerNickname = "User" + Random.Range(0, 10000);
        string userId = Random.Range(0, 10000).ToString();
        SaveUserInfo(playerNickname, userId);
        ConnectToGame(playerNickname, userId);
    }

    public void ConnectWithName()
    {
        if (!string.IsNullOrEmpty(playerName_InputName.text) && !string.IsNullOrWhiteSpace(playerName_InputName.text))
        {
            string userId = Random.Range(0, 10000).ToString();
            SaveUserInfo(playerName_InputName.text, userId);
            ConnectToGame(playerName_InputName.text, userId);
        }
    }

    public void ConnectWithExistingUser()
    {
        if (UserInfoManager.Instance.GetLocalUserInfo(out UserInfo localuserInfo))
        {
            ConnectToGame(localuserInfo.localPlayer.userName, localuserInfo.localPlayer.userId);
        }
    }

    public void ConnectToGame(string userName, string userId)
    {
        PhotonNetwork.NickName = userName;
        Debug.Log(PhotonNetwork.NickName);
        AuthenticationValues authenticationValues = new AuthenticationValues();
        authenticationValues.UserId = userId;
        PhotonNetwork.AuthValues = authenticationValues;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void SaveUserInfo(string userName, string userId)
    {
        UserInfo userInfo = new UserInfo();
        userInfo.localPlayer = new User();
        userInfo.localPlayer.userName = userName;
        userInfo.localPlayer.userId = userId;
        userInfo.friendList = new List<User>();
        UserInfoManager.Instance.SaveLocalUserInfo(userInfo);
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
