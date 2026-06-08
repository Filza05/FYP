using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint
{
    public Vector3 spawnPosition;
    public bool isFree;

    public SpawnPoint(Vector3 spawnPosition)
    {
        this.spawnPosition = spawnPosition;
        this.isFree = true;
    }
}
