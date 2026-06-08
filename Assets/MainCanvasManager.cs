using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance { get; private set; }  

    public TMP_Text stallInteractionText;
    public NotificationManager notificationManager;
    public GameObject pressButtonToInteract;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
