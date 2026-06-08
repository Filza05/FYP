using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private ModalWindowManager JoinCodeModalManager;
    [SerializeField] private ModalWindowManager loadingModalManager;
    [SerializeField] private PanelManager panelManager;

    private void Start()
    {
        VivoxService.Instance.LeaveAllChannelsAsync();
        VivoxService.Instance.LogoutAsync();
    }

    public async void StartHost()
    {
        // Show the loading modal
        loadingModalManager.OpenWindow();

        await HostSingleton.Instance.HostGameManager.StartHostAsync();

        // Hide the loading modal after joining
        loadingModalManager.CloseWindow();
    }

    public async void JoinGame()
    {
        string joinCode = joinCodeInput.text;

        if(joinCode == "")
        {
            return;
        }

        // Show the loading modal
        loadingModalManager.OpenWindow();

        await ClientSingleton.Instance.ClientGameManager.JoinGameAsClientAsync(joinCode);

        // Hide the loading modal after joining
        loadingModalManager.CloseWindow();
    }

    public void HandleJoinWithJoinCodeBTN()
    {
        JoinCodeModalManager.OpenWindow();
    }

    public void HandleJoinCodeCancelBTN()
    {
        JoinCodeModalManager.CloseWindow();
    }

    public void HandleLobbiesListBTN()
    {
        panelManager.OpenPanel("Available Lobbies");
    }
}
