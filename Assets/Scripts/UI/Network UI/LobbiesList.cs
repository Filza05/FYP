using Michsky.UI.Heat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    private bool isJoining = false;

    [SerializeField] Transform lobbyItemsParent;
    [SerializeField] LobbyItem lobbyItemPrefab;
    [SerializeField] PanelManager panelManagerForLobbies;
    [SerializeField] ModalWindowManager loader;

    private void Awake()
    {
        panelManagerForLobbies.onPanelChanged.AddListener(HandlePanelChanged);
        //RefreshLobbiesListAsync();
    }

    private void HandlePanelChanged(int arg0)
    {
        if(panelManagerForLobbies.currentPanelIndex == 1)
        {
            RefreshLobbiesListAsync();   //THIS CODE NEEDS TO BE REFACTORED, RENDER LOBBIES LIST INITIALLY FIRST.
        }
    }

    public async void JoinLobbyAsync(string lobbyId)
    {
        if(isJoining) { return; }
        isJoining = true;
        loader.OpenWindow();

        try
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);

            string lobbyJoinCode = lobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.ClientGameManager.JoinGameAsClientAsync(lobbyJoinCode);
            ApplicationController.Instance.currentLobbyId = lobbyId;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        isJoining = false;
        loader.CloseWindow();
    }

    public async void RefreshLobbiesListAsync()
    {
        List<Lobby> lobbies = await GetLobbiesListAsync();

        if(lobbies == null) {  return; }

        foreach (Transform child in lobbyItemsParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbies)
        {
            Debug.Log("Instantiating Lobby Prefab...");
            LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemsParent);
            lobbyItem.Initialize(this, lobby);
        }
    }

    private async Task<List<Lobby>> GetLobbiesListAsync()
    {
        try
        {
            var queryOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"),

                    new QueryFilter(
                        field: QueryFilter.FieldOptions.IsLocked,
                        op: QueryFilter.OpOptions.EQ,
                        value: "0")
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            return queryResponse.Results;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }
}
