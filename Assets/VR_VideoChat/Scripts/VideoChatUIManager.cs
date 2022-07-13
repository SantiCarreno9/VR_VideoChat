using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VideoChatUIManager : MonoBehaviour
{
    public static VideoChatUIManager Instance;
    [HideInInspector]
    public UserVideoChat userVideoChat;

    public GameObject videoChatUI;

    [Header("Call Request")]
    public GameObject receiverCallRequestUI;
    public GameObject callerCallRequestUI;
    public TextMeshProUGUI callerNameText;
    public TextMeshProUGUI receiverNameText;

    [Header("In call UI")]
    public GameObject inCallUI;
    public TextMeshProUGUI otherNameText;
    public GameObject localPlayerCamera;
    public GameObject otherPlayerCamera;

    private string otherName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void CloseVideoChatUI()
    {
        videoChatUI.SetActive(false);
        CloseInCallUI();
        CloseCallerCallRequest();
        CloseReceiverCallRequest();
    }

    #region Call Request

    #region Caller

    public void OpenCallerCallRequest(string otherName)
    {
        this.otherName = otherName;
        videoChatUI.SetActive(true);
        callerCallRequestUI.SetActive(true);
        receiverNameText.text = otherName;
    }

    public void CloseCallerCallRequest()
    {
        callerCallRequestUI.SetActive(false);
        receiverNameText.text = null;
    }

    #endregion

    #region Receiver

    public void OpenReceiverCallRequest(string otherName)
    {
        this.otherName = otherName;
        videoChatUI.SetActive(true);
        receiverCallRequestUI.SetActive(true);
        callerNameText.text = otherName;
    }

    public void CloseReceiverCallRequest()
    {
        receiverCallRequestUI.SetActive(false);
        callerNameText.text = null;
    }

    #endregion

    #endregion

    #region In call UI

    public void OpenInCallUI()
    {
        videoChatUI.SetActive(true);
        inCallUI.SetActive(true);
        receiverNameText.text = otherName;
        localPlayerCamera.SetActive(true);
        otherPlayerCamera.SetActive(true);
    }

    public void CloseInCallUI()
    {
        inCallUI.SetActive(false);
        receiverNameText.text = otherName;
        localPlayerCamera.SetActive(false);
        otherPlayerCamera.SetActive(false);
    }

    #endregion

    public void AnswerCall()
    {
        userVideoChat.AcceptCall();
        CloseReceiverCallRequest();
        CloseCallerCallRequest();
        OpenInCallUI();
    }

    public void DeclineCall()
    {
        userVideoChat.DeclineCall();
        CloseReceiverCallRequest();
        CloseVideoChatUI();
    }

    public void CancelCall()
    {
        userVideoChat.DeclineCall();
        CloseCallerCallRequest();
        CloseVideoChatUI();
    }

    public void HangUp()
    {
        userVideoChat.HangUp();
        CloseInCallUI();
        CloseVideoChatUI();
    }
}
