using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCCollider : MonoBehaviour
{
    private bool isPlayerNear;
    [SerializeField]
    private ModalWindowManager NPCModalWindow;
    NPC npcScript;
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text description;

    [SerializeField] InputManager inputManager;

    const string playerTagName = "Player";

    private void Start()
    {
        npcScript = GetComponent<NPC>();
    }

    private void Update()
    {
        if (isPlayerNear)
        {
            if(ApplicationController.Instance.localPlayer.playerState != PlayerState.IsFree) { return; }
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (NPCModalWindow.isOn)
                {
                    NPCModalWindow.CloseWindow();
                    inputManager.playerControls.Enable();
                }
                else
                {
                    NPCModalWindow.OpenWindow();    
                    title.text = npcScript.infoTitle.Value.ToString();
                    description.text = npcScript.infoDescription.Value.ToString();
                    inputManager.playerControls.Disable();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (!other.CompareTag(playerTagName)) { return; }
        Debug.Log("Player Triggered");

        if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }
            Debug.Log("local player Triggered");

            MainCanvasManager.Instance.pressButtonToInteract.gameObject.SetActive(true);
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTagName)) { return; }

        if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }

            MainCanvasManager.Instance.pressButtonToInteract.gameObject.SetActive(false);
            isPlayerNear = false;
        }
    }

    private void OnDestroy()
    {
        if(isPlayerNear)
            MainCanvasManager.Instance.pressButtonToInteract.gameObject.SetActive(false);
    }
    public void HandleOKPressed()
    {
        NPCModalWindow.CloseWindow();
        inputManager.playerControls.Enable();
    }

}
