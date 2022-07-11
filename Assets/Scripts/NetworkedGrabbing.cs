using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkedGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    private PhotonView m_PhotonView;
    private Rigidbody m_Rigidbody;

    private bool isBeingHold = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PhotonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHold)
        {
            //Object is being grabbed
            m_Rigidbody.isKinematic = true;
            gameObject.layer = 11;
        }
        else
        {
            //Object is not being grabbed
            m_Rigidbody.isKinematic = false;
            gameObject.layer = 9;
        }
    }

    private void TransferOwnership()
    {
        m_PhotonView.RequestOwnership();
    }

    public void OnSelectEntered()
    {
        Debug.Log("Grabbed");
        m_PhotonView.RPC("StartNetworkGrabbing", RpcTarget.AllBuffered);
        if (m_PhotonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("We do not request ownership, Already mine");
        }
        else
        {
            TransferOwnership();
        }
    }

    public void OnSelectedExited()
    {
        Debug.Log("Released");
        m_PhotonView.RPC("StopNetworkGrabbing", RpcTarget.AllBuffered);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != m_PhotonView)
        {
            return;
        }
        Debug.Log("Ownership Requested for: " + targetView.name + " from" + requestingPlayer.NickName);
        m_PhotonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("OnOwnership Transferred to " + targetView.name + " from" + previousOwner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {

    }

    [PunRPC]
    public void StartNetworkGrabbing()
    {
        isBeingHold = true;
    }

    [PunRPC]
    public void StopNetworkGrabbing()
    {
        isBeingHold = false;
    }
}
