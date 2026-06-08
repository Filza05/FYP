using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NPC : NetworkBehaviour
{
    [SerializeField] float fautgiTimer;

    public NetworkVariable<FixedString128Bytes> infoTitle = new NetworkVariable<FixedString128Bytes>(
        new FixedString128Bytes(""),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<FixedString4096Bytes> infoDescription = new NetworkVariable<FixedString4096Bytes>(
        new FixedString4096Bytes(""),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private float timer = 0f;

    private void Update()
    {
        if(!IsServer) { return; }

        if( timer > fautgiTimer)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
