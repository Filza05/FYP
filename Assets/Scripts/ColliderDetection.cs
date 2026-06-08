using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetection : MonoBehaviour
{
    protected virtual void Update()
    {
        if(ApplicationController.Instance.localPlayer.playerState != PlayerState.IsFree) { return; }
    }
}
