using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxWrapper
{
    public static bool isJoiningChannel;

    public static async void LoginToVivox(string playerName)
    {
        var correctedDisplayName = Regex.Replace(playerName, "[^a-zA-Z0-9_-]", "");
        playerName = correctedDisplayName.Substring(0, Math.Min(correctedDisplayName.Length, 30));
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Please enter a display name.");
            return;
        }

        var loginOptions = new LoginOptions()
        {
            DisplayName = playerName,
            ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond
        };

        await VivoxService.Instance.LoginAsync(loginOptions);
    }

    public static async Task JoinLobbyChannel(string channelName = "testChannel")
    {
        await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.TextAndAudio);
    }

    public static async void LogoutOfVivoxAsync()
    {
        await VivoxService.Instance.LogoutAsync();
    }
}
