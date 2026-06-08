using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    public static ApplicationController Instance { get; private set; }

    [SerializeField] private HostSingleton hostSingletonPrefab;
    [SerializeField] private ClientSingleton clientSingletonPrefab;

    public const string mainMenuSceneName = "Main Menu (NEW)";

    public PlayerManager localPlayer;
    public InputManager inputManager;

    public string currentLobbyId;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

        bool isDedicatedServer = 
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        LaunchInMode(isDedicatedServer);
    }

    private void LateUpdate()
    {
        if(localPlayer == null && SceneManager.GetActiveScene().name == "Game")
        {
            SceneManager.LoadScene(mainMenuSceneName);  
        }
    }

    private async void LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            //  implement later
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostSingletonPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientSingletonPrefab);
            bool isAuthenticated = clientSingleton.CreateClient();

            if(isAuthenticated)
            {
                SceneManager.LoadScene(mainMenuSceneName);

                var options = new InitializationOptions();
                await VivoxService.Instance.InitializeAsync();
            }
        }
    }
} 
