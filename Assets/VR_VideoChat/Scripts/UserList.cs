using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class UserList : MonoBehaviourPunCallbacks
{
    public GameObject userButtonPrefab;
    public GameObject scrollViewCont;
    public ToggleGroup usersToggleGroup;
    public GameObject listButtonsCont;


    public void InstantiateUserButton(string userName, string userID, bool interactable = true)
    {
        GameObject userButtonInstance = Instantiate(userButtonPrefab, scrollViewCont.transform);
        userButtonInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = userName;
        userButtonInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = userID;
        userButtonInstance.name = userName;
        Toggle currentToggle = userButtonInstance.GetComponent<Toggle>();
        currentToggle.interactable = interactable;
        currentToggle.group = usersToggleGroup;
        userButtonInstance.GetComponent<Toggle>().onValueChanged.AddListener(state =>
        {
            if (usersToggleGroup.AnyTogglesOn())
                OnToggleStateChanged(state);
            else listButtonsCont.SetActive(false);
        });
    }

    public void DeleteUserButton(string userName)
    {
        for (int i = 0; i < scrollViewCont.transform.childCount; i++)
        {
            if (scrollViewCont.transform.GetChild(i).name.Equals(userName, System.StringComparison.OrdinalIgnoreCase))
            {
                Destroy(scrollViewCont.transform.GetChild(i).gameObject);
                break;
            }
        }
    }

    public void DeleteAll()
    {
        for (int i = 0; i < scrollViewCont.transform.childCount; i++)
        {
            Destroy(scrollViewCont.transform.GetChild(i).gameObject);
        }
    }

    public void ChangeUserState(string userID, bool state)
    {
        for (int i = 0; i < scrollViewCont.transform.childCount; i++)
        {
            if (GetToggleUser(scrollViewCont.transform.GetChild(i).gameObject).userId.Equals(userID, System.StringComparison.Ordinal))
            {
                scrollViewCont.transform.GetChild(i).GetComponent<Toggle>().interactable = state;
                break;
            }
        }
    }

    public virtual void OnToggleStateChanged(bool state)
    {
        listButtonsCont.SetActive(true);
    }

    public User GetActiveToggleUser()
    {
        var activeToggle = usersToggleGroup.GetFirstActiveToggle();
        if (activeToggle != null)
        {
            return GetToggleUser(activeToggle.gameObject);
        }
        else return null;
    }

    public User GetToggleUser(GameObject toggle)
    {
        User user = new User();
        user.userName = toggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        user.userId = toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        return user;
    }

}
