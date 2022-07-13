using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject GenericVRPlayerPrefab;

    public GameObject localXRRig;
    public FriendList friendList;

    public Vector3 spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            GameObject playerAvatar = PhotonNetwork.Instantiate(GenericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                playerAvatar.GetComponent<PlayerNetworkSetup>().LocalXRRig = localXRRig;
                localXRRig.GetComponent<AvatarInputConverter>().MainAvatarTransform = playerAvatar.transform;
                friendList.userVideoChat = playerAvatar.GetComponent<UserVideoChat>();
                VideoChatUIManager.Instance.userVideoChat = playerAvatar.GetComponent<UserVideoChat>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
