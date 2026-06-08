using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;    
    public ClientGameManager ClientGameManager { get; private set; }

    public string channelToJoin = "";

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<ClientSingleton>();

            if (instance == null)
            {
                Debug.LogWarning("no object of type client singleton in the scene");
                return null;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool CreateClient()
    {
        ClientGameManager = new ClientGameManager();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            return true;
        }

        return false;
    }
}
