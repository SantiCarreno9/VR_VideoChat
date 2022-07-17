using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FriendList : UserList
{
    [SerializeField]
    private VideoChatManager videoChatManager;
    [SerializeField]
    private SocialInteractionManager socialInteractionManager;

    public override void OnEnable()
    {
        base.OnEnable();
        FillList();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DeleteAll();
    }

    #region Photon Callback Methods

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckFriendsStatus();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckFriendsStatus();
    }

    #endregion

    #region Buttons Methods

    public void AddFriend(User newFriend)
    {
        UserInfoManager.Instance.AddFriend(newFriend);
        InstantiateUserButton(newFriend.userName, newFriend.userId, true);
    }

    public void RemoveFriend()
    {
        User friendToRemove = GetActiveToggleUser();
        RemoveFriend(friendToRemove);
        Player[] playerList = PhotonNetwork.PlayerListOthers;
        for (int j = 0; j < playerList.Length; j++)
        {
            if (friendToRemove.userId.Equals(playerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                socialInteractionManager.RemoveFriend(playerList[j]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
    }

    public void CallFriend()
    {
        User userToCall = GetActiveToggleUser();
        Player[] playerList = PhotonNetwork.PlayerListOthers;
        for (int j = 0; j < playerList.Length; j++)
        {
            if (userToCall.userId.Equals(playerList[j].UserId, System.StringComparison.Ordinal))
            {
                videoChatManager.CallFriend(playerList[j]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
    }

    #endregion

    public void RemoveFriend(User friendToRemove)
    {
        UserInfoManager.Instance.RemoveFriend(friendToRemove);
        DeleteUserButton(friendToRemove.userId);
        listButtonsCont.gameObject.SetActive(false);
    }

    public void FillList()
    {
        if (UserInfoManager.Instance.GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList.Count > 0)
            {
                for (int i = 0; i < userInfo.friendList.Count; i++)
                {
                    InstantiateUserButton(userInfo.friendList[i].userName, userInfo.friendList[i].userId, interactable: false);
                }
            }
            CheckFriendsStatus();
        }
    }

    public void CheckFriendsStatus()
    {
        if (UserInfoManager.Instance.GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList.Count > 0)
            {
                Player[] playerList = PhotonNetwork.PlayerListOthers;
                for (int i = 0; i < userInfo.friendList.Count; i++)
                {
                    bool isConnected = false;
                    for (int j = 0; j < playerList.Length; j++)
                    {
                        if (userInfo.friendList[i].userId.Equals(playerList[j].UserId, System.StringComparison.Ordinal))
                        {
                            isConnected = true;
                            break;
                        }
                    }
                    ChangeUserState(userInfo.friendList[i].userId, isConnected);
                }
            }
        }
    }

}
