using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NPC_Manager : NetworkBehaviour
{
    public static NPC_Manager Instance;

    [SerializeField] GameObject NPCPrefab;
    [SerializeField] List<Transform> spawnPointTransforms;

    private List<SpawnPoint> spawnPoints;

    public PlayerManager playerManager;
    [SerializeField] NotificationManager notificationManager;

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

        spawnPoints = new List<SpawnPoint>();
    }

    private void Start()
    {
        playerManager = ApplicationController.Instance.localPlayer;

        int index = 0;
        foreach (Transform transform in spawnPointTransforms)
        {
            spawnPoints.Add(new SpawnPoint(spawnPointTransforms[index].position));
            index++;
        }
    }

    public void SpawnNpc(string title, string description)
    {
        if (!IsServer) return;

        SpawnPoint freeSpawnPoint =  new SpawnPoint(Vector3.zero);

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.isFree)
            {
                freeSpawnPoint = spawnPoint;
                break;
            }
        }

        if(freeSpawnPoint.spawnPosition == Vector3.zero) 
        { 
            Debug.Log("No free spawn points");
            notificationManager.notificationText = "All NPCs are Busy Right Now, Please Wait Some Time.";
            notificationManager.ExpandNotification();
            return;
        }

        GameObject NPCInstance = Instantiate(
            NPCPrefab,
            freeSpawnPoint.spawnPosition,
            Quaternion.identity);

        NPCInstance.GetComponent<NetworkObject>().Spawn(true);

        NPC npc = NPCInstance.GetComponent<NPC>();

        npc.infoTitle.Value = title;
        npc.infoDescription.Value = description;

        freeSpawnPoint.isFree = false;
    }

    #region Client
    public void SendInfoBroadcastToServer(string title, string description)
    {
        playerManager.playerInfoBroadcast.BroadcastDetailsSubmitServerRpc(title, description);
    }
    #endregion
}
