using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Vivox;
using UnityEngine;

public class ClientGameManager
{
    private const string ConnectionType = "dtls";
    private JoinAllocation allocation;

    public string joinCode = string.Empty;

    public async Task<bool> InitAsync()
    {
        /*  
            @Dev:
            Authentication is only on the client end not for host because host will also
            be a client and will be authenticated itself
         
        */

        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }  

    /*  @Description:
        This method joins the allocation using the join code and then start the game as client
    */
    public async Task JoinGameAsClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }

        NetworkManager networkManager = NetworkManager.Singleton;
        
        if(networkManager.TryGetComponent<UnityTransport>(out UnityTransport transport))
        {
            RelayServerData serverData = new RelayServerData(allocation, ConnectionType);
            transport.SetRelayServerData(serverData);

            this.joinCode = joinCode;
            networkManager.StartClient();
            ClientSingleton.Instance.channelToJoin = $"channel_{joinCode.ToUpper()}";
            /*  join the vivox channel here */
        }
    }
}
