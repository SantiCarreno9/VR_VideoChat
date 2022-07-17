using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class SocialInteractionManager : MonoBehaviour
{
    [SerializeField]
    private UsersInRoom usersInRoom;
    [SerializeField]
    private FriendList friendList;

    private PhotonView photonView;
    private Player other;
    private Queue<string> requestersId = new Queue<string>();
    private bool isFriendRequestOpen;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    #region Methods
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

    private void SaveNewFriend()
    {
        User newFriend = new User();
        newFriend.userName = other.NickName;
        newFriend.userId = other.UserId;
        friendList.AddFriend(newFriend);
    }

    private void ShowNextFriendRequest()
    {
        if (requestersId.Count > 0)
        {
            other = GetPlayer(requestersId.Dequeue());
            string requesterName = other.NickName;
            usersInRoom.OpenFriendRequestUI(requesterName);
        }
        else isFriendRequestOpen = false;
    }

    #endregion


    #region Friend Request

    public void SendFriendRequest(Player targetPlayer)
    {
        other = targetPlayer;
        photonView.RPC("ShowFriendRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId);
    }

    [PunRPC]
    private void ShowFriendRequestRPC(string requesterId)
    {
        Debug.Log("ShowFriendRequest from " + requesterId);
        requestersId.Enqueue(requesterId);
        if (!isFriendRequestOpen)
        {
            ShowNextFriendRequest();
            isFriendRequestOpen = true;
        }
    }

    public void AcceptFriendRequest()
    {
        photonView.RPC("AcceptFriendRequestRPC", other, PhotonNetwork.LocalPlayer.UserId);
        SaveNewFriend();
        ShowNextFriendRequest();
    }

    [PunRPC]
    private void AcceptFriendRequestRPC(string userId)
    {
        other = GetPlayer(userId);
        usersInRoom.RemoveUserFromSentRequestId(userId);
        SaveNewFriend();
    }

    public void DeclineFriendRequest()
    {
        photonView.RPC("DeclineFriendRequestRPC", other, PhotonNetwork.LocalPlayer.UserId);
        ShowNextFriendRequest();
    }

    [PunRPC]
    private void DeclineFriendRequestRPC(string userId)
    {
        usersInRoom.RemoveUserFromSentRequestId(userId);
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
        friendList.RemoveFriend(friendToRemove);
    }

    #endregion




}
