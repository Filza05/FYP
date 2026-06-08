using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private int maxConnections = 20;
    private Allocation allocation;
    private const string ConnectionType = "dtls";
    private const string gameSceneName = "Game"; 
    private string lobbyId;
    private float lobbyHeartBeatTime = 15f;

    public string joinCode = string.Empty;

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message);
        }

        try
        {
            string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            this.joinCode = joinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message);
            Debug.LogError("Could not receive join code");
        }

        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager.TryGetComponent<UnityTransport>(out UnityTransport transport))
        {
            RelayServerData relayServerData =
                new RelayServerData(allocation, ConnectionType);

            transport.SetRelayServerData(relayServerData);


            /*  @Dev:
                Before we start the host we create a lobby
            */

            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
                lobbyOptions.IsPrivate = false;

                string lobbyChannelName = $"channel_{joinCode}";

                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "JoinCode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: joinCode
                        )
                    }
                };

                Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", maxConnections, lobbyOptions);
                lobbyId = lobby.Id;

                HostSingleton.Instance.StartCoroutine(HeartBeatLobby(lobbyHeartBeatTime));
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }

            networkManager.StartHost();
            ApplicationController.Instance.currentLobbyId = lobbyId;
            ClientSingleton.Instance.channelToJoin = $"channel_{joinCode.ToUpper()}";
            networkManager.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
        else
        {
            //  Use UI to show error
            Debug.LogWarning("Couldn't find unity transport component.");
        }
    }

    private IEnumerator HeartBeatLobby(float heartBeatTime)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(heartBeatTime);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
