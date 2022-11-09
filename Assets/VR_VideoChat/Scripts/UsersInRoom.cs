using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UsersInRoom : UserList
{
    [SerializeField]
    private SocialInteractionManager socialInteractionManager;

    [SerializeField]
    private GameObject sendFriendRequestButton;
    [SerializeField]
    private GameObject waitingForResponseImage;

    [Space]
    [Header("Friend Request UI")]
    [SerializeField]
    private GameObject friendRequestUI;
    [SerializeField]
    private TextMeshProUGUI friendRequestText;

    private List<string> sentFriendRequests = new List<string>();

    /// <summary>
    /// Ir populates the user list with the room player list
    /// </summary>
    public void FillList()
    {
        if (PhotonNetwork.PlayerListOthers.Length > 0)
        {
            Player[] playerList = PhotonNetwork.PlayerListOthers;
            for (int i = 0; i < playerList.Length; i++)
            {
                Player currentPlayer = playerList[i];
                User currentUser = new User();
                currentUser.userName = currentPlayer.NickName;
                currentUser.userId = currentPlayer.UserId;
                InstantiateUserButton(currentUser.userName, currentUser.userId);
            }
        }
    }

    /// <summary>
    /// Gets the current selected user in the user list and sends him a friend request
    /// </summary>
    public void SendFriendRequest()
    {
        User targetUser = GetActiveToggleUser();
        Player[] playerList = PhotonNetwork.PlayerListOthers;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (targetUser.userId.Equals(playerList[i].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                sentFriendRequests.Add(playerList[i].UserId);
                socialInteractionManager.SendFriendRequest(playerList[i]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
    }

    /// <summary>
    /// As soon as the user selects a player from the user list, depending on whether that user is
    /// a friend or not, it will show the SendFriendRequest button or the WaitingForResponse image (if
    /// the user already sent a friend request to him)
    /// </summary>
    public override void OnToggleSelected()
    {
        User user = GetActiveToggleUser();
        if (!UserInfoManager.Instance.CheckIfItIsAlreadyAFriend(user))
        {
            listButtonsCont.SetActive(true);
            sendFriendRequestButton.SetActive(true);
            waitingForResponseImage.SetActive(false);
            if (sentFriendRequests.Count > 0)
            {
                for (int i = 0; i < sentFriendRequests.Count; i++)
                {
                    if (sentFriendRequests[i].Equals(user.userId, System.StringComparison.Ordinal))
                    {
                        sendFriendRequestButton.SetActive(false);
                        waitingForResponseImage.SetActive(true);
                        break;
                    }
                }
            }

        }
        else
        {
            listButtonsCont.SetActive(false);
        }
    }

    #region Friend Request UI

    public void OpenFriendRequestUI(string otherName)
    {
        friendRequestUI.SetActive(true);
        friendRequestText.text = otherName + " wants to be your friend";
    }

    /// <summary>
    /// Called when the user accepts or declines a friend request
    /// </summary>
    public void CloseFriendRequestUI()
    {
        friendRequestUI.SetActive(false);
        friendRequestText.text = null;
    }

    public void AcceptFriendRequest()
    {
        CloseFriendRequestUI();
        socialInteractionManager.AcceptFriendRequest();
    }

    /// <summary>
    /// It deletes the user that sent a friend request from the "sentFriendRequests" list
    /// after accepting or declining the request
    /// </summary>
    /// <param name="userId"></param>
    public void RemoveUserFromSentRequestId(string userId)
    {
        usersToggleGroup.SetAllTogglesOff();
        Debug.Log(sentFriendRequests.Count);
        if (sentFriendRequests.Count > 0)
        {
            sentFriendRequests.Remove(userId);
        }
        Debug.Log(sentFriendRequests.Count);
    }

    public void DeclineFriendRequest()
    {
        CloseFriendRequestUI();
        socialInteractionManager.DeclineFriendRequest();
    }

    #endregion       

    #region Photon Callback Methods

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom: " + newPlayer.NickName + " and UserId: " + newPlayer.UserId);
        InstantiateUserButton(newPlayer.NickName, newPlayer.UserId);
        usersToggleGroup.SetAllTogglesOff();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DeleteUserButton(otherPlayer.UserId);
        usersToggleGroup.SetAllTogglesOff();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("OnEnable");
        FillList();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DeleteAll();
    }

    #endregion
}
