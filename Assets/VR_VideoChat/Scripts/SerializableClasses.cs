using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string userId;
    public string userName;
}

[Serializable]
public class UserInfo
{
    public User localPlayer;
    public List<User> friendList;
}