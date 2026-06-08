using Michsky.UI.Heat;
using System.IO;
using TMPro;
using UnityEngine;

public class StallDetailsManager : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] StallManager stallManager;

    [field: SerializeField] public Canvas StallDetailsCanvas { get; private set; }

    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private TMP_InputField detailsInputField;

    [HideInInspector] public StallData stallData;

    public bool isGettingDetails;
    
    public void OpenStallDetailsModal()
    {
        isGettingDetails = true;
        StallDetailsCanvas.gameObject.SetActive(true);

        //inputManager.playerControls.Disable();
    }

    public void HandleStallDetailsSubmit()
    {
        string title = titleInputField.text;
        string details = detailsInputField.text;

        string playerId = PlayerPrefs.GetString("PlayerID");

        if (!titleInputField.text.Equals("") && !detailsInputField.text.Equals(""))
        {
            stallData.description = details;
            stallData.title = title;
            stallData.playerId = playerId;
            stallManager.playerManager.playerStallPlacement.StallPlacementServerRpc(stallData);
        }
        else
        {
            MainCanvasManager.Instance.notificationManager.notificationText = "You Must Fill All The Fields Before Proceeding!";
            MainCanvasManager.Instance.notificationManager.ExpandNotification();
        }


        titleInputField.text = "";
        detailsInputField.text = "";

        CloseStallDetailsModal();
    }

    public void CloseStallDetailsModal()
    {
        isGettingDetails = false;
        StallDetailsCanvas.gameObject.SetActive(false);

        //inputManager.playerControls.Enable();
    }
    
}
