using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float AnimBlendSpeed = 8.9f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform Camera;
    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottonLimit = 70f;
    [SerializeField] private float MouseSensivity = 21.9f;
    [SerializeField] private float JumpFactor = 150f;
    [SerializeField] private float Dis2Ground = 0.8f;
    [SerializeField] private float AirResistance = 0.8f;
    [SerializeField] private LayerMask GroundCheck;

    private Rigidbody playerRigidbody;
    private InputManager inputManager;
    private Animator animator;
    private bool grounded;
    private bool hasAnimator;
    private bool isDriving;
    private int xVelHash;
    private int yVelHash;
    private int jumpHash;
    private int groundHash;
    private int fallingHash;
    private int zVelHash;
    private float xRotation;

    private const float walkSpeed = 2f;
    private const float runSpeed = 6f;
    private Vector2 currentVelocity;

    private void Start()
    {
        hasAnimator = TryGetComponent<Animator>(out animator);
        playerRigidbody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();

        xVelHash = Animator.StringToHash("X_Velocity");
        yVelHash = Animator.StringToHash("Y_Velocity");
        jumpHash = Animator.StringToHash("Jump");
        groundHash = Animator.StringToHash("Grounded");
        fallingHash = Animator.StringToHash("Falling");
        zVelHash = Animator.StringToHash("Z_Velocity");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        /*Ray ray = Camera.ScreenPointToRay(inputManager.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
        {

        }*/
    }

    private void FixedUpdate()
    {
        if (!isDriving)
        {
            SampleGround();
            Move();
            HandleJump();
        }
    }

    private void LateUpdate()
    {
        if (!isDriving)
            CamMovements();
    }

    private void Move()
    {
        if (!hasAnimator) return;

        float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;
        if (inputManager.Move == Vector2.zero) targetSpeed = 0f;

        if (grounded)
        {

            Vector3 inputDirection = new Vector3(inputManager.Move.x, inputManager.Move.y).normalized;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputDirection.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputDirection.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            var xVelDifference = currentVelocity.x - playerRigidbody.velocity.x;
            var zVelDifference = currentVelocity.y - playerRigidbody.velocity.z;

            playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);

        }
        else
        {
            playerRigidbody.AddForce(transform.TransformVector(new Vector3(currentVelocity.x * AirResistance, 0, currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
        }

        animator.SetFloat(xVelHash, currentVelocity.x);
        animator.SetFloat(yVelHash, currentVelocity.y);
    }

    private void CamMovements()
    {
        if (!hasAnimator) return;

        var Mouse_X = inputManager.Look.x;
        var Mouse_Y = inputManager.Look.y;

        Camera.position = CameraRoot.position;

        xRotation -= Mouse_Y * MouseSensivity * Time.smoothDeltaTime;
        xRotation = Mathf.Clamp(xRotation, UpperLimit, BottonLimit);

        Camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensivity * Time.smoothDeltaTime, 0));
    }

    private void HandleJump()
    {
        if (!hasAnimator) return;
        if (!inputManager.Jump) return;
        //playerRigidbody.AddForce(1000 * Vector3.up, ForceMode.Impulse);
        animator.SetTrigger(jumpHash);
    }

    public void JumpAddForce()
    {
        playerRigidbody.AddForce(-playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
        playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
        animator.ResetTrigger(jumpHash);
    }

    private void SampleGround()
    {
        if (!hasAnimator) return;

        RaycastHit hitInfo;
        if (Physics.Raycast(playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
        {
            grounded = true;
            SetAnimationGrounding();
            return;
        }
        grounded = false;
        animator.SetFloat(zVelHash, playerRigidbody.velocity.y);
        SetAnimationGrounding();
        return;
    }

    private void SetAnimationGrounding()
    {
        animator.SetBool(fallingHash, !grounded);
        animator.SetBool(groundHash, grounded);
    }
}