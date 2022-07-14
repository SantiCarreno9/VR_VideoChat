using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class VideoChatManager : MonoBehaviour
{
    private Player otherPlayer;
    private PhotonView photonView;
    public GameObject localCamera;
    public GameObject otherCamera;
    private Dictionary<string, Transform> usersAvatars = new Dictionary<string, Transform>();
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        //if (photonView.IsMine)
        {
            //FriendList.Instance.videoChatManager = this;
            //VideoChatUIManager.Instance.videoChatManager = this;
        }
    }

    public void AddUser(string userId, Transform avatar, bool isLocal = false)
    {
        Debug.Log("User with Id: " + userId + " was added to VideoChatManager");
        usersAvatars.Add(userId, avatar);
        //if (isLocal)
        //{
        //    camera.targetTexture = localRenderTexture;
        //    localCamera = camera;
        //}
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
    }

    [PunRPC]
    public void HangUpRPC()
    {
        Debug.Log("HangUp");
        VideoChatUIManager.Instance.CloseInCallUI();
        VideoChatUIManager.Instance.CloseVideoChatUI();
        DeactivateCameras();
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
}
