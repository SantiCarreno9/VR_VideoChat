using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This class is in charge of the interactions with the UI
/// </summary>
public class UserList : MonoBehaviourPunCallbacks
{
    [Header("UserList")]
    [SerializeField]
    private GameObject userButtonPrefab;
    [SerializeField]
    private GameObject scrollViewCont;

    public ToggleGroup usersToggleGroup;
    public GameObject listButtonsCont;

    public void InstantiateUserButton(string userName, string userId, bool interactable = true)
    {
        GameObject userButtonInstance = Instantiate(userButtonPrefab, scrollViewCont.transform);
        userButtonInstance.name = userId;
        userButtonInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = userName;

        Toggle userButtonToggle = userButtonInstance.GetComponent<Toggle>();
        userButtonToggle.interactable = interactable;
        userButtonToggle.group = usersToggleGroup;
        userButtonToggle.onValueChanged.AddListener(state =>
        {
            if (usersToggleGroup.AnyTogglesOn())
            {
                listButtonsCont.SetActive(true);
                OnToggleSelected();
            }
            else listButtonsCont.SetActive(false);
        });
    }


    public void DeleteUserButton(string userId)
    {
        for (int i = 0; i < scrollViewCont.transform.childCount; i++)
        {
            if (scrollViewCont.transform.GetChild(i).name.Equals(userId, System.StringComparison.Ordinal))
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

    public virtual void OnToggleSelected()
    {

    }

    public User GetActiveToggleUser()
    {
        if (usersToggleGroup.AnyTogglesOn())
        {
            return GetToggleUser(usersToggleGroup.GetFirstActiveToggle().gameObject);
        }
        else return null;
    }

    private User GetToggleUser(GameObject toggle)
    {
        User user = new User();
        user.userId = toggle.name;
        user.userName = toggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        return user;
    }

}
