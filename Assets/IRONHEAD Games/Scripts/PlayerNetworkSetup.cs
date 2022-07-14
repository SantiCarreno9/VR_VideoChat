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
    public GameObject AvatarHand_Left;
    public GameObject AvatarHand_Right;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI PlayerName_Text;
    public int avatarNumber = 0;
    //public Camera videoChatCamera;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //FindObjectOfType<VideoChatManager>().AddUser(photonView.Owner.UserId, MainAvatarGameobject.transform, true);
            AvatarInputConverter avatarInputConverter = LocalXRRig.GetComponent<AvatarInputConverter>();
            avatarInputConverter.AvatarHead = AvatarHead.transform;
            avatarInputConverter.AvatarBody = AvatarBody.transform;
            avatarInputConverter.AvatarHand_Left = AvatarHand_Left.transform;
            avatarInputConverter.AvatarHand_Right = AvatarHand_Right.transform;
            avatarInputConverter.MainAvatarTransform = MainAvatarGameobject.transform;
            avatarNumber = Random.Range(0, AvatarModelPrefabs.Length);
            photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, avatarNumber);
            SetLayerRecursively(AvatarHead, 6);
            SetLayerRecursively(AvatarBody, 7);

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
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

            FindObjectOfType<VideoChatManager>().AddUser(photonView.Owner.UserId, AvatarBody.transform);
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
        GameObject instantiatedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], MainAvatarGameobject.transform);
        AvatarHolder avatarHolder = instantiatedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, AvatarHead.transform);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, AvatarBody.transform);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, AvatarHand_Left.transform);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, AvatarHand_Right.transform);
        if (photonView.IsMine)
        {
            instantiatedAvatarGameobject.transform.SetParent(LocalXRRig.transform);
            LocalXRRig.GetComponent<AvatarInputConverter>().enabled = true;
        }
        //GameObject instantiatedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], LocalXRRig.transform);        
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
