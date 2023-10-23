using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput PlayerInput;

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Interact { get; private set; }
    public bool LMB { get; private set; }
    public bool Drop { get; private set; }

    private InputActionMap currentMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction interactionAction;
    private InputAction lmbAction;
    private InputAction drop;

    private void Awake()
    {
        Cursor.visible = false;
        currentMap = PlayerInput.currentActionMap;
        moveAction = currentMap.FindAction("Move");
        lookAction = currentMap.FindAction("Look");
        runAction = currentMap.FindAction("Run");
        jumpAction = currentMap.FindAction("Jump");
        interactionAction = currentMap.FindAction("Interaction");
        lmbAction = currentMap.FindAction("LMB");
        drop = currentMap.FindAction("Drop");

        moveAction.performed += onMove;
        lookAction.performed += onLook;
        runAction.performed += onRun;
        jumpAction.performed += onJump;
        interactionAction.performed += onInteract;
        lmbAction.performed += onLeftMouseButton;
        drop.performed += onDrop;

        moveAction.canceled += onMove;
        lookAction.canceled += onLook;
        runAction.canceled += onRun;
        jumpAction.canceled += onJump;
        interactionAction.canceled += onInteract;
        lmbAction.canceled += onLeftMouseButton;
        drop.canceled += onDrop;
    }

    private void onMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void onLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    private void onRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }

    private void onJump(InputAction.CallbackContext context)
    {
        //if(context.interaction is TapInteraction)
        Jump = context.ReadValueAsButton();
    }

    private void onInteract(InputAction.CallbackContext context)
    {
        //if(context.interaction is TapInteraction)
        Interact = context.ReadValueAsButton();
    }

    private void onLeftMouseButton(InputAction.CallbackContext context)
    {
        //if(context.interaction is TapInteraction)
        LMB = context.ReadValueAsButton();
        Debug.Log("LMB Pressed: " + LMB);  // Dodaj tê liniê

    }

    private void onDrop(InputAction.CallbackContext context)
    {
        //if(context.interaction is TapInteraction)
        Drop = context.ReadValueAsButton();
    }

    private void onEnable()
    {
        currentMap.Enable();
    }

    private void onDisable()
    {
        currentMap.Disable();
    }
}