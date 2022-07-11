using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserInfoManager// : MonoBehaviour
{
    private string userInfoPath = Directory.GetCurrentDirectory() + @"/GameData" + "/" + "userData.json";
    //private string userInfoPath = Application.streamingAssetsPath + "/" + "userData.json";

    // Start is called before the first frame update
    void Start()
    {
        //if (!Directory.Exists(userInfoPath))
        //{
        //    Directory.CreateDirectory(userInfoPath);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    public void SaveLocalUserInfo(UserInfo newUserInfo)
    {
        string userInfoJson = JsonUtility.ToJson(newUserInfo);
        File.WriteAllText(userInfoPath, userInfoJson);
    }

    public void AddFriend(User friend)
    {
        if (GetLocalUserInfo(out UserInfo userInfo))
        {
            if (userInfo.friendList == null)
                userInfo.friendList = new List<User>();

            userInfo.friendList.Add(friend);
            SaveLocalUserInfo(userInfo);
        }
    }

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

    ///// <summary>
    ///// Crea el archivo json en caso de no existir y almacena la puntuaci�n actual
    ///// De lo contrario compara la puntuaci�n almacenada y la actual para almacenar la m�s alta
    ///// Finalmente muestra estos valores en la interfaz
    ///// </summary>
    //public void ManageJson()
    //{
    //    ScoreRecord currentScore = new ScoreRecord();
    //    currentScore.score = cookiesCount;
    //    string json = JsonUtility.ToJson(currentScore);

    //    string route = Application.streamingAssetsPath + "/" + "scoreData.json";

    //    if (!File.Exists(route))
    //    {
    //        File.WriteAllText(Application.streamingAssetsPath + "/" + "scoreData.json", json);
    //        prevScoreText.text = "0";
    //    }
    //    else
    //    {
    //        ScoreRecord prevScore = new ScoreRecord();
    //        string prevJson = File.ReadAllText(route);
    //        prevScore = JsonUtility.FromJson<ScoreRecord>(prevJson);
    //        if (prevScore.score < cookiesCount)
    //        {
    //            File.WriteAllText(Application.streamingAssetsPath + "/" + "scoreData.json", json);
    //        }
    //        prevScoreText.text = prevScore.score.ToString();
    //    }
    //    currentScoreText.text = currentScore.score.ToString();

    //}
}
