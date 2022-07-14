using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UsersInRoom : UserList
{
    public static UsersInRoom Instance;
    public SocialInteractionManager socialInteractionManager;
    private List<User> usersInRoom = new List<User>();

    [Header("Friend Request UI")]
    [SerializeField]
    private GameObject friendRequestUI;
    [SerializeField]
    private TextMeshProUGUI friendRequestText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Friend Request

    public void SendFriendRequest()
    {
        User targetUser = GetActiveToggleUser();

        for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
        {
            if (targetUser.userId.Equals(PhotonNetwork.PlayerList[j].UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                socialInteractionManager.SendFriendRequest(PhotonNetwork.PlayerList[j]);
                break;
            }
        }
        usersToggleGroup.SetAllTogglesOff();
    }

    public void OpenFriendRequestUI(string otherName)
    {
        friendRequestUI.SetActive(true);
        friendRequestText.text = otherName + " wants to be your friend";
    }

    public void CloseFriendRequestUI()
    {
        friendRequestUI.SetActive(false);
        friendRequestText.text = null;
    }

    public void AcceptFriendRequest()
    {
        socialInteractionManager.AcceptFriendRequest();
        CloseFriendRequestUI();
    }

    public void DeclineFriendRequest()
    {
        CloseFriendRequestUI();
    }

    #endregion

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
        if (!UserInfoManager.Instance.CheckIfItIsAlreadyAFriend(user))
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
