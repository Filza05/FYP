using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StallCollider : MonoBehaviour
{
    [SerializeField] Stall stall;
    [SerializeField] ModalWindowManager stallModalManager;
    [SerializeField] Canvas stallModalCanvas;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;

    private bool isPlayerNear;
    const string playerTagName = "Player";

    [SerializeField] InputManager inputManager;


    private void Update()
    {
        if(isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                    stallModalCanvas.gameObject.SetActive(true);
                    stallModalManager.gameObject.SetActive(true);
                    stallModalManager.OpenWindow();

                    Debug.Log(stall.title.Value.ToString());

                    title.text = stall.title.Value.ToString();
                    description.text = stall.description.Value.ToString();
                    //inputManager.playerControls.Disable();     
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTagName)) { return; }

        if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }
            
            MainCanvasManager.Instance.stallInteractionText.gameObject.SetActive(true);
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTagName)) { return; }

        if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (!playerManager.IsLocalPlayer) { return; }

            MainCanvasManager.Instance.stallInteractionText.gameObject.SetActive(false);
            isPlayerNear = false;
        }
    }

    public void HandleOKPressed()
    {
        stallModalManager.CloseWindow(); //
        stallModalCanvas.gameObject.SetActive(false);
        //inputManager.playerControls.Enable();
    }
}
