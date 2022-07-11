using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UsersInRoom : UserList
{
    public FriendList friendList;
    private List<User> usersInRoom = new List<User>();



    public void AddFriend()
    {
        friendList.AddFriend(GetActiveToggleUser());
    }

    public void FillList()
    {
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player currentPlayer = PhotonNetwork.PlayerList[i];
                if (!currentPlayer.IsLocal)
                {
                    User currentUser = new User();
                    currentUser.userName = currentPlayer.NickName;
                    currentUser.userId = currentPlayer.UserId;
                    usersInRoom.Add(currentUser);
                    InstantiateUserButton(currentUser.userName, currentUser.userId);
                }
            }
        }
    }

    #region Photon Callback Methods


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom: " + newPlayer.NickName + " and UserId: " + newPlayer.UserId);
        InstantiateUserButton(newPlayer.NickName, newPlayer.UserId);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DeleteUserButton(otherPlayer.NickName);
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

    public override void OnToggleStateChanged(bool state)
    {
        Debug.Log("OnToggleStateChanged");
        var activeToggle = usersToggleGroup.GetFirstActiveToggle();
        User user = new User();
        user.userName = activeToggle.name;
        if (!friendList.userInfoManager.CheckIfItIsAlreadyAFriend(user))
        {
            listButtonsCont.SetActive(true);
        }
        else
        {
            listButtonsCont.SetActive(false);
        }
    }
    #endregion
}
