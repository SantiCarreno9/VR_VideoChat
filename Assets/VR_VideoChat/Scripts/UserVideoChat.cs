using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UserVideoChat : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallFriend(Player targetPlayer)
    {
        photonView.RPC("CallFriendRPC", targetPlayer);
    }

    [PunRPC]
    public void CallFriendRPC()
    {
        Debug.Log("Open Video chat call");
    }

}
