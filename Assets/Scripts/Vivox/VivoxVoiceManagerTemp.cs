using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxVoiceManagerTemp : MonoBehaviour
{
    static VivoxVoiceManagerTemp instance;
    static object m_Lock = new object();

    public static bool isInitialized = false;

    public static VivoxVoiceManagerTemp Instance
    {
        get
        {
            lock (m_Lock)
            {
                if (instance == null)
                {
                    // Search for existing instance.
                    instance = (VivoxVoiceManagerTemp)FindObjectOfType(typeof(VivoxVoiceManagerTemp));

                    // Create new instance if one doesn't already exist.
                    if (instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<VivoxVoiceManagerTemp>();
                        singletonObject.name = typeof(VivoxVoiceManager).ToString() + " (Singleton)";
                    }
                }
                // Make instance persistent even if its already in the scene
                DontDestroyOnLoad(instance.gameObject);
                return instance;
            }
        }
    }

    async void Awake()
    {
        if (instance != this && instance != null)
        {
            Debug.LogWarning(
                "Multiple VivoxVoiceManager detected in the scene. Only one VivoxVoiceManager can exist at a time. The duplicate VivoxVoiceManager will be destroyed.");
            Destroy(this);
        }

        var options = new InitializationOptions();
        await VivoxService.Instance.InitializeAsync();

        isInitialized = true;
    }
}
