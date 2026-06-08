using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;

    private LobbiesList lobbiesList;
    private string lobbyId;

    public void Initialize(LobbiesList lobbiesList, Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobbyId = lobby.Id;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"Players: {lobby.Players.Count} / {lobby.MaxPlayers}";
    }

    public void JoinLobby()
    {
        Debug.Log("In the Lobby Join Func");

        try
        {
            lobbiesList.JoinLobbyAsync(lobbyId);
        }
        catch
        {
            lobbiesList.RefreshLobbiesListAsync();
        }
    }
}
