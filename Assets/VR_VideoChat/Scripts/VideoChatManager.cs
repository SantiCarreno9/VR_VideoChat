using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Realtime;

public class VideoChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private VideoChatUIManager videoChatUIManager;

    [Header("Video Chat")]
    [SerializeField]
    private GameObject localCamera;
    [SerializeField]
    private GameObject otherCamera;
    private Dictionary<string, Transform> usersAvatars = new Dictionary<string, Transform>();

    [Header("Voice Chat")]
    [SerializeField]
    private Recorder photonVoiceRecorder;
    private const byte globalVoiceRoom = 0;
    private List<byte> roomsInUse = new List<byte>();
    private byte currentVoiceRoom;

    [HideInInspector]
    public PlayerNetworkSetup localPlayer;
    private Player otherPlayer;
    private bool isInACall;

    #region Local Methods

    public void AddUser(string userId, Transform avatar)
    {
        Debug.Log("User with Id: " + userId + " was added to VideoChatManager");
        usersAvatars.Add(userId, avatar);
    }

    private Player GetPlayer(string otherId)
    {
        Player[] playerList = PhotonNetwork.PlayerListOthers;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].UserId.Equals(otherId, System.StringComparison.Ordinal))
            {
                return playerList[i];
            }
        }
        return null;
    }

    #endregion

    #region Call Methods

    #region Caller Methods    

    public void CallFriend(Player targetPlayer)
    {
        otherPlayer = targetPlayer;
        videoChatUIManager.OpenCallerCallRequest(otherPlayer.NickName);
        photonView.RPC("ShowCallRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
    }

    public void CancelCallRequest()
    {
        photonView.RPC("CancelCallRequestRPC", otherPlayer);
    }

    [PunRPC]
    public void ShowCallRequestRPC(string callerUserId, string callerUserName)
    {
        if (!isInACall)
        {
            videoChatUIManager.OpenReceiverCallRequest(callerUserName);
            otherPlayer = GetPlayer(callerUserId);
        }
        else DeclineCallAutomatically(GetPlayer(callerUserId));
    }

    [PunRPC]
    public void CancelCallRequestRPC()
    {
        Debug.Log("Call request canceled");
        videoChatUIManager.CloseReceiverCallRequest();
        if (!isInACall) videoChatUIManager.CloseVideoChatUI();
    }

    #endregion

    #region Receiver Methods   

    public void DeclineCall()
    {
        string message = PhotonNetwork.LocalPlayer.NickName + " has declined the call";
        photonView.RPC("DeclineCallRPC", otherPlayer, message);
    }

    private void DeclineCallAutomatically(Player requester)
    {
        string message = PhotonNetwork.LocalPlayer.NickName + " is in a call";
        photonView.RPC("DeclineCallRPC", requester, message);
    }

    [PunRPC]
    public void DeclineCallRPC(string reason)
    {
        StartCoroutine(videoChatUIManager.ShowDeclinedCallRequest(reason));
    }

    public void JoinCall()
    {
        byte voiceChatRoom = 1;

        if (roomsInUse.Count > 0)
        {
            while (roomsInUse.Contains(voiceChatRoom))
                voiceChatRoom++;
        }
        currentVoiceRoom = voiceChatRoom;
        isInACall = true;
        ActivateCameras();
        SetInCallVoice();
        photonView.RPC("AnswerCallRPC", otherPlayer, voiceChatRoom);
        photonView.RPC("AddRoomInUseRPC", RpcTarget.AllBuffered, voiceChatRoom);
    }

    [PunRPC]
    public void AnswerCallRPC(byte voiceRoom)
    {
        Debug.Log("Call request accepted");
        videoChatUIManager.CloseCallerCallRequest();
        videoChatUIManager.OpenInCallUI();
        ActivateCameras();
        currentVoiceRoom = voiceRoom;
        SetInCallVoice();
        isInACall = true;
    }

    [PunRPC]
    public void AddRoomInUseRPC(byte roomInUse)
    {
        roomsInUse.Add(roomInUse);
    }

    [PunRPC]
    public void DeleteRoomInUseRPC(byte roomInUse)
    {
        roomsInUse.Remove(roomInUse);
    }

    public void HangUp()
    {
        Debug.Log("HangUP");
        DeactivateCameras();
        SetNormalVoice();
        isInACall = false;
        photonView.RPC("HangUpRPC", otherPlayer);
        photonView.RPC("DeleteRoomInUseRPC", RpcTarget.AllBuffered, currentVoiceRoom);
    }

    [PunRPC]
    public void HangUpRPC()
    {
        Debug.Log("HangUp");
        videoChatUIManager.CloseInCallUI();
        videoChatUIManager.CloseVideoChatUI();
        DeactivateCameras();
        SetNormalVoice();
        isInACall = false;
    }


    #endregion

    public void ChangeMicrophoneState(bool state)
    {
        photonVoiceRecorder.TransmitEnabled = state;
    }

    private void ActivateCameras()
    {
        localCamera.gameObject.SetActive(true);
        otherCamera.GetComponent<CameraFollowUser>().target = usersAvatars[otherPlayer.UserId];
        otherCamera.gameObject.SetActive(true);
    }

    private void DeactivateCameras()
    {
        localCamera.gameObject.SetActive(false);
        otherCamera.gameObject.SetActive(false);
    }

    public void SetInCallVoice()
    {
        //2D Spatial Blend
        localPlayer.ChangeSpatialBlend(0, otherPlayer);
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { globalVoiceRoom }, new byte[] { currentVoiceRoom });
        photonVoiceRecorder.InterestGroup = 1;
    }

    public void SetNormalVoice()
    {
        //3D Spatial Blend
        localPlayer.ChangeSpatialBlend(1, otherPlayer);
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { currentVoiceRoom }, new byte[] { globalVoiceRoom });
        photonVoiceRecorder.InterestGroup = 0;
    }

    #endregion

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (this.otherPlayer != null)
        {
            if (otherPlayer.UserId.Equals(this.otherPlayer.UserId, System.StringComparison.Ordinal))
            {
                if (isInACall)
                {
                    HangUp();
                    videoChatUIManager.CloseInCallUI();
                    videoChatUIManager.CloseVideoChatUI();
                    this.otherPlayer = null;
                }
            }
        }
        PhotonNetwork.Destroy(usersAvatars[otherPlayer.UserId].gameObject);
        usersAvatars.Remove(otherPlayer.UserId);
    }

}
