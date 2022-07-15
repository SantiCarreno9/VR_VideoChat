using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Realtime;

public class VideoChatManager : MonoBehaviour
{
    private Player otherPlayer;
    private PhotonView photonView;
    public GameObject localCamera;
    public GameObject otherCamera;
    private Dictionary<string, Transform> usersAvatars = new Dictionary<string, Transform>();
    private bool isInACall;
    public Recorder photonVoiceRecorder;
    [HideInInspector]
    public PlayerNetworkSetup localPlayer;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    //photonVoiceNetwork.Client.OpChangeGroups(new byte[] { 0 }, new byte[] { 1 });            
        //    PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { 0 }, new byte[] { 1 });
        //    photonVoiceRecorder.InterestGroup = 1;
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    //photonVoiceNetwork.Client.OpChangeGroups(new byte[] { 0 }, new byte[] { 1 });
        //    PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { 1 }, new byte[] { 0 });
        //    photonVoiceRecorder.InterestGroup = 0;
        //}
    }

    public void AddUser(string userId, Transform avatar)
    {
        Debug.Log("User with Id: " + userId + " was added to VideoChatManager");
        usersAvatars.Add(userId, avatar);
    }

    public void SaveOther(string otherId)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].UserId.Equals(otherId, System.StringComparison.Ordinal))
            {
                otherPlayer = PhotonNetwork.PlayerList[i];
                break;
            }
        }
    }

    #region Call Request

    #region Caller Methods

    public void CallFriend(Player targetPlayer)
    {
        otherPlayer = targetPlayer;
        photonView.RPC("ShowCallRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
        VideoChatUIManager.Instance.OpenCallerCallRequest(otherPlayer.NickName);
    }

    /// <summary>
    /// The call request UI is shown in the receiver session
    /// </summary>
    /// <param name="callerUserId"></param>
    /// <param name="callerUserName"></param>
    [PunRPC]
    public void ShowCallRequestRPC(string callerUserId, string callerUserName)
    {
        Debug.Log("Call from " + callerUserName);
        VideoChatUIManager.Instance.OpenReceiverCallRequest(callerUserName);
        SaveOther(callerUserId);
    }

    public void CancelCallRequest()
    {
        photonView.RPC("CancelCallRequestRPC", otherPlayer);
    }

    [PunRPC]
    public void CancelCallRequestRPC()
    {
        Debug.Log("Call request canceled");
        VideoChatUIManager.Instance.CloseReceiverCallRequest();
        VideoChatUIManager.Instance.CloseVideoChatUI();
    }

    #endregion

    #region Receiver

    public void AcceptCall()
    {
        photonView.RPC("AcceptCallRPC", otherPlayer);
        ActivateCameras();
        SetInCallVoice();
    }

    /// <summary>
    /// The receiver accepts and sends this confirmation to the caller
    /// </summary>
    [PunRPC]
    public void AcceptCallRPC()
    {
        Debug.Log("Call request accepted");
        VideoChatUIManager.Instance.CloseCallerCallRequest();
        VideoChatUIManager.Instance.OpenInCallUI();
        ActivateCameras();
        SetInCallVoice();
    }

    public void DeclineCall()
    {
        photonView.RPC("DeclineCallRPC", otherPlayer);
    }

    /// <summary>
    /// The caller declines and closes the request UI in the caller session
    /// </summary>
    [PunRPC]
    public void DeclineCallRPC()
    {
        Debug.Log("Call declined");
        VideoChatUIManager.Instance.CloseCallerCallRequest();
        VideoChatUIManager.Instance.CloseVideoChatUI();
    }

    #endregion

    public void HangUp()
    {
        photonView.RPC("HangUpRPC", otherPlayer);
        DeactivateCameras();
        SetNormalVoice();
    }

    [PunRPC]
    public void HangUpRPC()
    {
        Debug.Log("HangUp");
        VideoChatUIManager.Instance.CloseInCallUI();
        VideoChatUIManager.Instance.CloseVideoChatUI();
        DeactivateCameras();
        SetNormalVoice();
    }

    public void ChangeMicrophoneState(bool state)
    {
        photonVoiceRecorder.TransmitEnabled = state;
    }

    #endregion

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
        localPlayer.ChangeSpatialBlend(0, otherPlayer);
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { 0 }, new byte[] { 1 });
        photonVoiceRecorder.InterestGroup = 1;
    }

    public void SetNormalVoice()
    {
        localPlayer.ChangeSpatialBlend(1, otherPlayer);
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { 1 }, new byte[] { 0 });
        photonVoiceRecorder.InterestGroup = 0;
    }
}
