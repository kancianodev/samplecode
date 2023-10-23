using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionDistance = 2.0f;

    private Camera playerCamera;
    private InputManager inputManager;

    private IInteractable lastInteractable;

    private void Start()
    {
        playerCamera = Camera.main;
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        IInteractable interactable = null;
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            interactable = hit.collider.GetComponent<IInteractable>();
        }

        Debug.Log(interactable);

        if (lastInteractable != interactable && interactable != null)
        {
            interactable.OnHover();
            if (lastInteractable != null)
            {
                lastInteractable.OnStopHover();
            }
            lastInteractable = interactable;
        }
        else if (interactable == null && lastInteractable != null)
        {
            lastInteractable.OnStopHover();
            lastInteractable = null;
        }

        if (inputManager.Interact && interactable != null)
        {
            interactable.Interact();
        }
    }
    private void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
