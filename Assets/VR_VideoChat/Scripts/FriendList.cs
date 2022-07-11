using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FriendList : UserList
{
    public UserInfoManager userInfoManager = new UserInfoManager();
    public UserVideoChat userVideoChat;

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

    #region PUN Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckFriendsStatus();
    }

    //public override void OnFriendListUpdate(List<FriendInfo> friendList)
    //{
    //    for (int i = 0; i < friendList.Count; i++)
    //    {
    //        ChangeUserState(friendList[i].UserId, friendList[i].IsOnline);
    //    }
    //}

    #endregion

    public void AddFriend(User newFriend)
    {
        userInfoManager.AddFriend(newFriend);
        InstantiateUserButton(newFriend.userName, newFriend.userId, true);
    }

    public void RemoveFriend()
    {
        User userToRemove = GetActiveToggleUser();
        userInfoManager.RemoveFriend(userToRemove);
        DeleteUserButton(userToRemove.userName);
    }

    public void CallFriend()
    {
        User userToCall = GetActiveToggleUser();
        for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
        {
            if (userToCall.userId.Equals(PhotonNetwork.PlayerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                userVideoChat.CallFriend(PhotonNetwork.PlayerList[j]);
                break;
            }
        }
    }

    public void FillList()
    {
        if (userInfoManager.GetLocalUserInfo(out UserInfo userInfo))
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
        if (userInfoManager.GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList.Count > 0)
            {
                for (int i = 0; i < userInfo.friendList.Count; i++)
                {
                    for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                    {
                        if (userInfo.friendList[i].userId.Equals(PhotonNetwork.PlayerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
                        {
                            ChangeUserState(userInfo.friendList[i].userId, true);
                            break;
                        }
                        else ChangeUserState(userInfo.friendList[i].userId, false);
                    }
                }
            }
        }

        //Find Friends Alternative
        //if (userInfoManager.GetLocalUserInfo(out UserInfo userInfo))
        //{
        //    if (userInfo.friendList.Count > 0)
        //    {
        //        string[] friends = new string[userInfo.friendList.Count];
        //        for (int i = 0; i < userInfo.friendList.Count; i++)
        //        {
        //            friends[i] = userInfo.friendList[i].userId;
        //        }
        //        PhotonNetwork.FindFriends(friends);
        //    }
        //}
    }
}
