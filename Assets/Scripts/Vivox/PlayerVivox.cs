using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class PlayerVivox : NetworkBehaviour
{
    int m_PermissionAskedCount;

    bool IsMicPermissionGranted()
    {
        bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
        return isGranted;
    }

    void AskForPermissions()
    {
        string permissionCode = Permission.Microphone;
        m_PermissionAskedCount++;
        Permission.RequestUserPermission(permissionCode);
    }

    bool IsPermissionsDenied()
    {
        return m_PermissionAskedCount == 1;
    }


    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            StartCoroutine(waitTillInitialized());

            string playerName = PlayerPrefs.GetString("PlayerName");

            Debug.Log(playerName);

            LoginToVivoxService(playerName);

            VivoxService.Instance.LoggedIn += OnVivoxLoggedIn;
            VivoxService.Instance.LoggedOut += OnVivoxLoggedOut;
        }
    }
    void LoginToVivoxService(string playerName)
    {
        if (IsMicPermissionGranted())
        {
            // The user authorized use of the microphone.
            VivoxWrapper.LoginToVivox(playerName);
        }
        else
        {
            // We do not have the needed permissions.
            // Ask for permissions or proceed without the functionality enabled if they were denied by the user
            if (IsPermissionsDenied())
            {
                m_PermissionAskedCount = 0;
                VivoxWrapper.LoginToVivox(playerName);
            }
            else
            {
                AskForPermissions();
            }
        }
    }

    private async void OnVivoxLoggedIn()
    {
        Debug.Log(ClientSingleton.Instance.channelToJoin);
        await VivoxWrapper.JoinLobbyChannel(ClientSingleton.Instance.channelToJoin);
    }

    public IEnumerator waitTillInitialized()
    {
        yield return new WaitUntil(() => VivoxVoiceManagerTemp.isInitialized);
    }

    private void OnVivoxLoggedOut()
    {
        Debug.Log("leaving all channels");
    }
}
