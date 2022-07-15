using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialInteractionManager : MonoBehaviour
{
    private PhotonView photonView;
    private Player other;
    private Queue<string> requestersId = new Queue<string>();
    private bool isFriendRequestOpen;

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
        photonView.RPC("SendFriendRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
    }

    /// <summary>
    /// Friend request target receives 
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="requesterName"></param>
    [PunRPC]
    private void SendFriendRequestRPC(string requesterId, string requesterName)
    {
        requestersId.Enqueue(requesterId);
        if (!isFriendRequestOpen)
        {
            ShowFriendRequest();
            isFriendRequestOpen = true;
        }
    }

    private void ShowFriendRequest()
    {
        if (requestersId.Count > 0)
        {
            other = GetPlayer(requestersId.Dequeue());
            string requesterName = other.NickName;
            UsersInRoom.Instance.OpenFriendRequestUI(requesterName);
        }
        else isFriendRequestOpen = false;
    }


    public void AcceptFriendRequest()
    {
        photonView.RPC("AcceptFriendRequestRPC", other, PhotonNetwork.LocalPlayer.UserId);
        SaveNewFriend();
    }


    /// <summary>
    /// Requester receives response
    /// </summary>
    /// <param name="userId"></param>
    [PunRPC]
    private void AcceptFriendRequestRPC(string userId)
    {
        other = GetPlayer(userId);
        SaveNewFriend();
    }

    public void DeclineFriendRequest()
    {
        ShowFriendRequest();
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

    public Player GetPlayer(string otherId)
    {
        Player player = null;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].UserId.Equals(otherId, System.StringComparison.Ordinal))
            {
                player = PhotonNetwork.PlayerList[i];
                break;
            }
        }
        return player;
    }

    private void SaveNewFriend()
    {
        User newFriend = new User();
        newFriend.userName = other.NickName;
        newFriend.userId = other.UserId;
        FriendList.Instance.AddFriend(newFriend);
    }


}
