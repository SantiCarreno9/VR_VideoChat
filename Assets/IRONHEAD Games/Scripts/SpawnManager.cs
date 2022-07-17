using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject genericVRPlayerPrefab;

    [SerializeField]
    private GameObject localXRRig;

    [SerializeField]
    private Vector2 xSpawnPositionRange;
    [SerializeField]
    private Vector2 zSpawnPositionRange;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Instantiating players");
            float xPosition = Random.Range(xSpawnPositionRange.x, xSpawnPositionRange.y);
            float zPosition = Random.Range(zSpawnPositionRange.x, zSpawnPositionRange.y);
            Vector3 spawnPosition = new Vector3(xPosition, 0, zPosition);
            localXRRig.transform.position = spawnPosition;
            GameObject playerAvatar = PhotonNetwork.Instantiate(genericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
            playerAvatar.GetComponent<PlayerNetworkSetup>().LocalXRRig = localXRRig;
        }
    }
}
