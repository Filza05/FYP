using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InfoDeskCollider : ColliderDetection
{
    [SerializeField] TMP_Text pressButtonText;
    [SerializeField] Canvas npcCanvas;

    const string playerTagName = "Player";

    public bool isPlayerNearDesk = false;

    protected override void Update()
    {
        base.Update();

        if (isPlayerNearDesk)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if(npcCanvas.isActiveAndEnabled)
                {
                    npcCanvas.gameObject.SetActive(false);
                }
                else
                {
                    npcCanvas.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(playerTagName)) { return; }

        if(other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }

            pressButtonText.gameObject.SetActive(true);
            isPlayerNearDesk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTagName)) { return; }

        if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }

            pressButtonText.gameObject.SetActive(false);
            isPlayerNearDesk = false;
        }
    }
}
