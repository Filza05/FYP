using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Stall : NetworkBehaviour
{
    [HideInInspector] public StallData stallData;

    public NetworkVariable<FixedString128Bytes> title = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString4096Bytes> description = new NetworkVariable<FixedString4096Bytes>();
}
