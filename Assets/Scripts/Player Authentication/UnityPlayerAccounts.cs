using System;
using System.Text;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;


class UnityPlayerAccounts : MonoBehaviour
{
    [SerializeField]
    TMP_Text ErrorText;

    [SerializeField]
    TMP_Text StatusText;

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        //string randomString = GenerateRandomString(16);


        //AuthenticationService.Instance.SwitchProfile(randomString);
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();

        AuthenticationService.Instance.ClearSessionToken();

        //  If player has signed in on unity website, subscribe this method
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;

        if ((AuthenticationService.Instance.SessionTokenExists))
        {
            //  Switch Scene
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }

/*        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);*/
    }

    public async void StartSignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            //  Wait for player to sign in on website
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);
        }
    }

    public void OpenAccountPortal()
    {
        Application.OpenURL(PlayerAccountService.Instance.AccountPortalUrl);
    }

    private async void SignInWithUnity()
    {
        try
        {
            //  After player logs in unity player account, Authenticate the access token via auth service
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);

            //  Saving PlayerID in PlayerPrefs to be accessed later
            string playerID = AuthenticationService.Instance.PlayerId;
            string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

            Debug.Log(playerName);

            PlayerPrefs.SetString("PlayerID", playerID);
            PlayerPrefs.Save();


            getStatusText();

            if ((AuthenticationService.Instance.SessionTokenExists))
            {
                //  Switch Scene
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex + 1);
            }


        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);
        }
    }

    //additional text in unityauthentication scene to get current player status.
    private void getStatusText()
    {
        var statusBuilder = new StringBuilder();

        statusBuilder.AppendLine($"Player Accounts State: <b>{(PlayerAccountService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");
        statusBuilder.AppendLine($"Player Accounts Access token: <b>{(string.IsNullOrEmpty(PlayerAccountService.Instance.AccessToken) ? "Missing" : "Exists")}</b>\n");
        statusBuilder.AppendLine($"Authentication Service State: <b>{(AuthenticationService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");
        statusBuilder.AppendLine($"Authentication Service Session: <b>{(AuthenticationService.Instance.IsExpired ? "Session expired" : "Session isn't expired")}</b>");
        statusBuilder.AppendLine($"Authentication Service token: <b>{(string.IsNullOrEmpty(AuthenticationService.Instance.AccessToken) ? "Missing" : "Exists")}</b>\n");


        if (AuthenticationService.Instance.IsSignedIn)
        {
            statusBuilder.AppendLine($"PlayerId: <b>{AuthenticationService.Instance.PlayerId}</b>");
        }

        StatusText.text = statusBuilder.ToString();

    }

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    void SetException(Exception ex)
    {
        ErrorText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
    }
}