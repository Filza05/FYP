using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerName : MonoBehaviour
{
    public TMP_InputField DisplayNameInput;
    public NotificationManager NotificationManager;

    public void HandleEnterBTNPress()
    {
        if(DisplayNameInput.text != "")
        {
            PlayerPrefs.SetString("PlayerName", DisplayNameInput.text);
            PlayerPrefs.Save();

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        } else
        {
            NotificationManager.ExpandNotification();
        }
    }
}
