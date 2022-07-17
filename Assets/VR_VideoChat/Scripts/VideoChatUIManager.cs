using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoChatUIManager : MonoBehaviour
{
    [SerializeField]
    private VideoChatManager videoChatManager;
    [SerializeField]
    private GameObject videoChatUI;

    [Space]
    [Header("Call Request")]

    [Header("Receiver")]
    [SerializeField]
    private GameObject receiverCallRequestUI;
    [SerializeField]
    private GameObject callingUI;
    [SerializeField]
    private TextMeshProUGUI receiverNameText;
    [SerializeField]
    private GameObject callDeclinedUI;
    [SerializeField]
    private TextMeshProUGUI callDeclinedMessageText;

    [Header("Caller")]
    [SerializeField]
    private GameObject callerCallRequestUI;
    [SerializeField]
    private TextMeshProUGUI callerNameText;


    [Header("In call UI")]
    [SerializeField]
    private GameObject inCallUI;
    [SerializeField]
    private TextMeshProUGUI otherNameText;
    [SerializeField]
    private Image microphoneImage;
    [SerializeField]
    private Sprite microphoneNormalIcon;
    [SerializeField]
    private Sprite microphoneClosedIcon;

    private bool microphoneState = true;
    private string otherName;


    public void CloseVideoChatUI()
    {
        videoChatUI.SetActive(false);
    }

    #region Call Request

    #region Caller

    public void OpenCallerCallRequest(string otherName)
    {
        this.otherName = otherName;
        videoChatUI.SetActive(true);
        callingUI.SetActive(true);
        callerCallRequestUI.SetActive(true);
        receiverNameText.text = otherName;
    }

    public void CloseCallerCallRequest()
    {
        callerCallRequestUI.SetActive(false);
        callingUI.SetActive(false);
        callDeclinedUI.SetActive(false);
    }

    public IEnumerator ShowDeclinedCallRequest(string message)
    {
        callingUI.SetActive(false);
        callDeclinedUI.SetActive(true);
        callDeclinedMessageText.text = message;
        yield return new WaitForSeconds(2);
        CloseCallerCallRequest();
        CloseVideoChatUI();
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
        otherNameText.text = otherName;
    }

    public void CloseInCallUI()
    {
        inCallUI.SetActive(false);
        otherNameText.text = "Other";
    }

    #endregion

    #region Buttons Methods

    public void AnswerCall()
    {
        videoChatManager.JoinCall();
        CloseReceiverCallRequest();
        OpenInCallUI();
    }

    public void DeclineCall()
    {
        videoChatManager.DeclineCall();
        CloseReceiverCallRequest();
        CloseVideoChatUI();
    }

    public void CancelCall()
    {
        videoChatManager.CancelCallRequest();
        CloseCallerCallRequest();
        CloseVideoChatUI();
    }

    public void HangUp()
    {
        videoChatManager.HangUp();
        CloseInCallUI();
        CloseVideoChatUI();
    }

    public void ChangeMicrophoneState()
    {
        if (!microphoneState)
            microphoneImage.sprite = microphoneNormalIcon;
        else microphoneImage.sprite = microphoneClosedIcon;

        microphoneState = !microphoneState;
        videoChatManager.ChangeMicrophoneState(microphoneState);
    }
    #endregion
}
