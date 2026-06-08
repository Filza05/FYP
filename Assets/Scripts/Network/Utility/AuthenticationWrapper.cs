using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    private static int authenticationTries = 5;
    public static AuthState AuthState {  get; private set; }

    public static async Task<AuthState> DoAuth() //  just like do math
    {
        if(AuthState == AuthState.Authenticated)
        {
            Debug.Log("Already Authenticated.");
            return AuthState;
        }

        if(AuthState == AuthState.Authenticating)
        {
            Debug.Log("Already Authenticating.");
            await Authenticating();
            return AuthState;
        }

        await GetAuthenticatedAnonymouslyAsync(authenticationTries);
        return AuthState;
    }

    public static async Task<AuthState> Authenticating()
    {
        while(AuthState == AuthState.Authenticating 
            || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    } 

    private static async Task GetAuthenticatedAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;
        int tries = 0;

        while(tries < maxTries)
        {
            //  Always wrap sensitive code in try catch
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if(AuthenticationService.Instance.IsSignedIn 
                    && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException ex)
            {
                Debug.LogError(ex.Message);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError(ex.Message);
                AuthState = AuthState.Error;
            }

            tries++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning("Authentication request timed out.");
            AuthState = AuthState.Timeout;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}
