using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialInteractionManager : MonoBehaviour
{
    private PhotonView photonView;
    private Player other;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //if (photonView.IsMine)
        {
            //UsersInRoom.Instance.socialInteractionManager = this;
        }
    }


    #region Friend Request

    public void SendFriendRequest(Player targetPlayer)
    {
        other = targetPlayer;
        Debug.Log(other.NickName);
        photonView.RPC("SendFriendRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    private void SendFriendRequestRPC(string requesterId, string requesterName)
    {
        UsersInRoom.Instance.OpenFriendRequestUI(requesterName);
        SaveOther(requesterId);
    }

    public void AcceptFriendRequest()
    {
        photonView.RPC("AcceptFriendRequestRPC", other);
        SaveNewFriend();
    }

    [PunRPC]
    private void AcceptFriendRequestRPC()
    {
        SaveNewFriend();
    }

    public void RemoveFriend(Player friendToRemove)
    {
        photonView.RPC("RemoveFriendRPC", friendToRemove, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    private void RemoveFriendRPC(string friendId, string friendName)
    {
        User friendToRemove = new User();
        friendToRemove.userName = friendName;
        friendToRemove.userId = friendId;
        FriendList.Instance.RemoveFriend(friendToRemove);
    }



    #endregion

    public void SaveOther(string otherId)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].UserId.Equals(otherId, System.StringComparison.Ordinal))
            {
                other = PhotonNetwork.PlayerList[i];
                break;
            }
        }
    }

    private void SaveNewFriend()
    {
        User newFriend = new User();
        newFriend.userName = other.NickName;
        newFriend.userId = other.UserId;
        FriendList.Instance.AddFriend(newFriend);
    }
}
