using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUIManager : MonoBehaviour
{
    public GameObject ConnectOptionsPanel;
    public GameObject ConnectWithNamePanel;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ConnectOptionsPanel.SetActive(true);
        ConnectWithNamePanel.SetActive(false);
    }

    #endregion
}
