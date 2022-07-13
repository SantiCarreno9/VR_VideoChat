using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXRRig;
    public GameObject MainAvatarGameobject;
    public GameObject AvatarHead;
    public GameObject AvatarBody;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI PlayerName_Text;
    public int avatarNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //The player is local
            //LocalXRRig.SetActive(true);

            //Getting the avatar selection data so that the correct avatar model can be Instantiated.
            //object avatarSelectionNumber = avatarNumber;
            //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            //{
            //    Debug.Log("Avatar selection number: " + (int)avatarSelectionNumber);
            //    photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            //}
            photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, avatarNumber);
            SetLayerRecursively(AvatarHead, 6);
            SetLayerRecursively(AvatarBody, 7);

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleportation areas");
                foreach (var item in teleportationAreas)
                {
                    item.teleportationProvider = LocalXRRig.GetComponent<TeleportationProvider>();
                }
            }
            MainAvatarGameobject.AddComponent<AudioListener>();
        }
        else
        {
            //The player is remote            
            SetLayerRecursively(AvatarHead, 0);
            SetLayerRecursively(AvatarBody, 0);
        }

        if (PlayerName_Text != null)
        {
            PlayerName_Text.text = photonView.Owner.NickName;
        }
    }

    private void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber]);
        if (photonView.IsMine)
        {
            selectedAvatarGameobject.transform.SetParent(LocalXRRig.transform);
            AvatarInputConverter avatarInputConverter = LocalXRRig.GetComponent<AvatarInputConverter>();
            AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
            avatarInputConverter.AvatarHead = avatarHolder.HeadTransform;
            avatarInputConverter.AvatarBody = avatarHolder.BodyTransform;
            avatarInputConverter.AvatarHand_Left = avatarHolder.HandLeftTransform;
            avatarInputConverter.AvatarHand_Right = avatarHolder.HandRightTransform;
            SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
            SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
            SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
            SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
            avatarInputConverter.enabled = true;
        }
        //GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], LocalXRRig.transform);        
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
