using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStallPlacement : NetworkBehaviour
{
    private GameObject tentPrefab;

    private void Start()
    {
        tentPrefab = StallManager.Instance.tentPrefab;
    }

    #region Network
    [ServerRpc]
    public void StallPlacementVerificationServerRpc(StallData stallData, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { senderClientId }
            }
        };

        StallPlacementVerificationClientRpc(stallData, clientRpcParams);
    }

    [ClientRpc]
    public void StallPlacementVerificationClientRpc(
        StallData stallData,
        ClientRpcParams clientRpcParams)
    {
        StallManager.Instance.StallDetailsManager.stallData = stallData;
        StallManager.Instance.StallDetailsManager.OpenStallDetailsModal();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StallPlacementServerRpc(StallData stallData)
    {
        // Save the stall data to the server's JSON file
        JSONHelper.SaveStallDataToJson(stallData); // Saving to Server's JSON File

        // Here spawn the stall as a network object, which should be synchronized across all clients
        Vector3 stallPosition = new Vector3(stallData.xzPosition.x, 0, stallData.xzPosition.y);
        Quaternion stallRotation = Quaternion.Euler(0, stallData.yRotation, 0);

        GameObject instance = Instantiate(tentPrefab, stallPosition, stallRotation);

        NetworkObject networkObject = instance.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        Stall stallComponent = instance.GetComponent<Stall>();
        stallComponent.title.Value = stallData.title;
        stallComponent.description.Value = stallData.description;
        stallComponent.stallData = stallData;
    }


    [ClientRpc]
    public void SpawnStallClientRpc(StallData stallData)
    {
        Vector3 stallPosition = new Vector3(stallData.xzPosition.x, 0, stallData.xzPosition.y);
        Quaternion stallRotation = Quaternion.Euler(0, stallData.yRotation, 0);

        GameObject instance = Instantiate(tentPrefab, stallPosition, stallRotation);
        instance.GetComponent<Stall>().stallData = stallData;
    }
    #endregion
}
