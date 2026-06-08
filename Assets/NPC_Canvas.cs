using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC_Canvas : Interactable
{
    public static NPC_Canvas Instance { get; private set; }

    [SerializeField] TMP_InputField titleInput;
    [SerializeField] TMP_InputField descriptionInput;

    //public GameObject pressButtonToInteract;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleConfirmPressed()
    {
        string title = titleInput.text;
        string description = descriptionInput.text;
        if(!titleInput.text.Equals("") && !descriptionInput.text.Equals(""))
        {
            NPC_Manager.Instance.SendInfoBroadcastToServer(title, description);
        }
        else
        {
            MainCanvasManager.Instance.notificationManager.notificationText = "You Must Fill All The Fields Before Proceeding!";
            MainCanvasManager.Instance.notificationManager.ExpandNotification();
        }

        titleInput.text = "";
        descriptionInput.text = "";
        gameObject.SetActive(false);
    }

    public void HandleCancel()
    {
        gameObject.SetActive(false);
    }
}
