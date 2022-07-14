using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FriendList : UserList
{
    public static FriendList Instance;

    public VideoChatManager videoChatManager;
    public SocialInteractionManager socialInteractionManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

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

    public override void OnPlayerLeftRoom(Player otherPlayer)
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
        UserInfoManager.Instance.AddFriend(newFriend);
        InstantiateUserButton(newFriend.userName, newFriend.userId, true);
    }

    public void RemoveFriend()
    {
        User friendToRemove = GetActiveToggleUser();
        RemoveFriend(friendToRemove);
        for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
        {
            if (friendToRemove.userId.Equals(PhotonNetwork.PlayerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                socialInteractionManager.RemoveFriend(PhotonNetwork.PlayerList[j]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
    }

    public void RemoveFriend(User friendToRemove)
    {
        UserInfoManager.Instance.RemoveFriend(friendToRemove);
        DeleteUserButton(friendToRemove.userName);
        listButtonsCont.gameObject.SetActive(false);
    }

    public void CallFriend()
    {
        User userToCall = GetActiveToggleUser();

        for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
        {
            if (userToCall.userId.Equals(PhotonNetwork.PlayerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                videoChatManager.CallFriend(PhotonNetwork.PlayerList[j]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
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
