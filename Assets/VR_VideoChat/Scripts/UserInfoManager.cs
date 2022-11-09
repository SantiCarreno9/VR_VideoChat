using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    public static UserInfoManager Instance;
    private string infoPath;    
    private string userInfoPath;

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
    
    void Start()
    {
#if !UNITY_EDITOR
        infoPath = Application.persistentDataPath + @"/GameData" + "/";
#else
        infoPath = Directory.GetCurrentDirectory() + @"/GameData" + "/";
#endif
        userInfoPath = infoPath + "userData.json";
        if (!Directory.Exists(infoPath))
        {
            Directory.CreateDirectory(infoPath);
        }
    }

    /// <summary>
    /// Retrieves all the information from the Json file
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public bool GetLocalUserInfo(out UserInfo userInfo)
    {
        if (!File.Exists(userInfoPath))
        {
            userInfo = null;
            return false;
        }
        else
        {
            string jsonToRead = File.ReadAllText(userInfoPath);
            userInfo = JsonUtility.FromJson<UserInfo>(jsonToRead);
            return true;
        }
    }

    /// <summary>
    /// Saves/Overwrites the json file information
    /// </summary>
    /// <param name="newUserInfo"></param>
    public void SaveLocalUserInfo(UserInfo newUserInfo)
    {
        string userInfoJson = JsonUtility.ToJson(newUserInfo);
        File.WriteAllText(userInfoPath, userInfoJson);
    }

    /// <summary>
    /// Adds a new friend to the json file
    /// </summary>
    /// <param name="friend"></param>
    public void AddFriend(User friend)
    {
        if (GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList == null)
                userInfo.friendList = new List<User>();

            if (!CheckIfItIsAlreadyAFriend(friend))
            {
                userInfo.friendList.Add(friend);
                SaveLocalUserInfo(userInfo);
            }
        }
    }

    /// <summary>
    /// Removes a given friend and updates the json file
    /// </summary>
    /// <param name="friend"></param>
    public void RemoveFriend(User friend)
    {
        if (GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList != null)
            {
                for (int i = 0; i < userInfo.friendList.Count; i++)
                {
                    if (userInfo.friendList[i].userId.Equals(friend.userId, System.StringComparison.OrdinalIgnoreCase))
                    {
                        userInfo.friendList.RemoveAt(i);
                        break;
                    }
                }
                SaveLocalUserInfo(userInfo);
            }
        }
    }

    /// <summary>
    /// Checks if the given user is already a friend
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool CheckIfItIsAlreadyAFriend(User user)
    {
        bool isAlreadyAFriend = false;
        if (GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList != null)
            {
                for (int i = 0; i < userInfo.friendList.Count; i++)
                {
                    if (userInfo.friendList[i].userName.Equals(user.userName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        isAlreadyAFriend = true;
                        break;
                    }
                }
            }
        }
        return isAlreadyAFriend;
    }

}
