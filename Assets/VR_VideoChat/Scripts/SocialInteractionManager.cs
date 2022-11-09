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
    /// <summary>
    /// Gets the Player object from the current room list by searching its userId
    /// </summary>
    /// <param name="otherId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds a User as a new friend in the friend list
    /// </summary>
    private void SaveNewFriend()
    {
        User newFriend = new User();
        newFriend.userName = other.NickName;
        newFriend.userId = other.UserId;
        friendList.AddFriend(newFriend);
    }

    /// <summary>
    /// It shows all the incoming friend requests one by one
    /// </summary>
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

    /// <summary>
    /// Sends to the selected user the command to show the friend request screen
    /// </summary>
    /// <param name="targetPlayer"></param>
    public void SendFriendRequest(Player targetPlayer)
    {
        other = targetPlayer;
        photonView.RPC("ShowFriendRequestRPC", targetPlayer, PhotonNetwork.LocalPlayer.UserId);
    }

    /// <summary>
    /// Receives the command from the requester to show the friend request screen
    /// </summary>
    /// <param name="requesterId"></param>
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

    /// <summary>
    /// Accepts the friend request and notifies the requester, then it shows the remnant requests
    /// </summary>
    public void AcceptFriendRequest()
    {
        photonView.RPC("AcceptFriendRequestRPC", other, PhotonNetwork.LocalPlayer.UserId);
        SaveNewFriend();
        ShowNextFriendRequest();
    }

    /// <summary>
    /// Receives the acceptance of the friend request and saves him in the friend list
    /// </summary>
    /// <param name="userId"></param>
    [PunRPC]
    private void AcceptFriendRequestRPC(string userId)
    {
        other = GetPlayer(userId);
        usersInRoom.RemoveUserFromSentRequestId(userId);
        SaveNewFriend();
    }

    /// <summary>
    /// Declines the friend request and notifies the requester, then it shows the remnant requests
    /// </summary>
    public void DeclineFriendRequest()
    {
        photonView.RPC("DeclineFriendRequestRPC", other, PhotonNetwork.LocalPlayer.UserId);
        ShowNextFriendRequest();
    }

    /// <summary>
    /// Receives the declination of the request and deletes the other user from the list
    /// </summary>
    /// <param name="userId"></param>
    [PunRPC]
    private void DeclineFriendRequestRPC(string userId)
    {
        usersInRoom.RemoveUserFromSentRequestId(userId);
    }

    public void RemoveFriend(Player friendToRemove)
    {
        photonView.RPC("RemoveFriendRPC", friendToRemove, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
    }

    /// <summary>
    /// Receives the command to delete a friend from the friend list
    /// </summary>
    /// <param name="friendId"></param>
    /// <param name="friendName"></param>
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
