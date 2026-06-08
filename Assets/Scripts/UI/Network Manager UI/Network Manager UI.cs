using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;

    private void Start()
    {
        string joinCode = HostSingleton.Instance.HostGameManager.joinCode;

        if (joinCode != string.Empty)
        {
            joinCodeText.text = "JOIN CODE: " + joinCode;
        }
    }
}
