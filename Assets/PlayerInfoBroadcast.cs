using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfoBroadcast : NetworkBehaviour
{
    [ServerRpc]
    public void BroadcastDetailsSubmitServerRpc(string title, string description)
    {
        NPC_Manager.Instance.SpawnNpc(title, description);
    }
}
