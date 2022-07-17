using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXRRig;
    [SerializeField]
    private GameObject MainAvatarGameobject;
    [SerializeField]
    private GameObject AvatarHead;
    [SerializeField]
    private GameObject AvatarBody;
    [SerializeField]
    private GameObject AvatarHand_Left;
    [SerializeField]
    private GameObject AvatarHand_Right;

    [SerializeField]
    private GameObject[] AvatarModelPrefabs;

    [SerializeField]
    private TextMeshProUGUI PlayerName_Text;

    [SerializeField]
    private AudioSource speaker;

    void Start()
    {
        if (photonView.IsMine)
        {
            #region SetXRRig targets
            AvatarInputConverter avatarInputConverter = LocalXRRig.GetComponent<AvatarInputConverter>();
            avatarInputConverter.AvatarHead = AvatarHead.transform;
            avatarInputConverter.AvatarBody = AvatarBody.transform;
            avatarInputConverter.AvatarHand_Left = AvatarHand_Left.transform;
            avatarInputConverter.AvatarHand_Right = AvatarHand_Right.transform;
            avatarInputConverter.MainAvatarTransform = MainAvatarGameobject.transform;
            #endregion

            #region Avatar Setup
            int avatarNumber = Random.Range(0, AvatarModelPrefabs.Length);
            photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, avatarNumber);
            SetLayerRecursively(AvatarHead, 6);
            SetLayerRecursively(AvatarBody, 6);
            MainAvatarGameobject.AddComponent<AudioListener>();
            #endregion            

            FindObjectOfType<VideoChatManager>().localPlayer = this;
        }
        else
        {
            SetLayerRecursively(AvatarHead, 0);
            SetLayerRecursively(AvatarBody, 0);
            FindObjectOfType<VideoChatManager>().AddUser(photonView.Owner.UserId, AvatarBody.transform);
        }

        if (PlayerName_Text != null)
            PlayerName_Text.text = photonView.Owner.NickName;
    }

    #region Avatar Setup

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
            LocalXRRig.GetComponent<AvatarInputConverter>().enabled = true;
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }

    private void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
    #endregion

    #region Voice    
    public void ChangeSpatialBlend(int value, Player target)
    {
        photonView.RPC("ChangeSpatialBlendRPC", target, value);
        speaker.spatialBlend = value;
    }

    [PunRPC]
    public void ChangeSpatialBlendRPC(int value)
    {
        speaker.spatialBlend = value;
    }

    #endregion
}
