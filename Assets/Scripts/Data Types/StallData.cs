using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

[Serializable]
public class StallData : INetworkSerializable
{
    public string playerId;
    public Vector2 xzPosition;
    public float yRotation;
    public string title;
    public string description;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref xzPosition);
        serializer.SerializeValue(ref yRotation);
        serializer.SerializeValue(ref title);
        serializer.SerializeValue(ref description);
    }
}

