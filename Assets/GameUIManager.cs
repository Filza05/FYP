using Michsky.UI.Heat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] GameObject ChatUI;
    [SerializeField] InputManager inputManager;
    [SerializeField] ButtonManager leaveDltlobbyBTN;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStopped += OnClientDisconnected;
        if (NetworkManager.Singleton.IsServer)
        {
            leaveDltlobbyBTN.onClick.AddListener(HandleDeleteLobby);
            leaveDltlobbyBTN.buttonText = "Delete Lobby";
        }
        else
        {
            leaveDltlobbyBTN.onClick.AddListener(HandleLeaveLobby);
        }
    }

    private void OnClientDisconnected(bool obj)
    {
        Debug.Log("Disconnected from server, switching to main menu...");
        SceneManager.LoadScene(ApplicationController.mainMenuSceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapePressed();
        }
    }

    private void HandleEscapePressed()
    {
        if (ChatUI.activeInHierarchy)
        {
            ChatUI.SetActive(false);
            inputManager.
                playerControls.Enable();
        }
        else
        {
            ChatUI.SetActive(true);
            inputManager.playerControls.Disable();
        }
    }

    public async void HandleLeaveLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(
            ApplicationController.Instance.currentLobbyId,
            AuthenticationService.Instance.PlayerId);

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(ApplicationController.mainMenuSceneName);
    }

    public async void HandleDeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(
                ApplicationController.Instance.currentLobbyId);
            Debug.Log("Lobby successfully deleted.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Error deleting lobby: {e.Message}");
        }

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(ApplicationController.mainMenuSceneName);
    }
}
