using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject GenericVRPlayerPrefab;

    public GameObject localXRRig;

    public Vector3 spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Instantiating players");
            GameObject playerAvatar = PhotonNetwork.Instantiate(GenericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
            playerAvatar.GetComponent<PlayerNetworkSetup>().LocalXRRig = localXRRig;
            localXRRig.GetComponent<AvatarInputConverter>().MainAvatarTransform = playerAvatar.transform;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
