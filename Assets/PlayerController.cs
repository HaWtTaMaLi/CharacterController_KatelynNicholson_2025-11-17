using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Speed")]
    public float speed = 3f; //low walk speed to notice >
    public float sprintSpeed = 8f; //> the difference in sprinting.
    public Vector3 velocity;

    [Header("Jump Settings")]
    public float jumpHeight = 0.8f; //1.5 was to high imo
    public float airTime = 0f;
    public float gravity = -9.81f;
    public Vector3 lastPosition;
    public bool isGrounded = true;

    [Header("Crouch Settings")]
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 1f;
    public float cameraTransition = 5f;
    public Vector3 standCamera = new Vector3(-0.03908123f, 1.97f, 0.2218122f);
    public Vector3 crouchCamera = new Vector3(-0.03908123f, 0.8f, 0.2218122f);
    public bool isCrouching = false;

    [Header("Look Settings")]
    public float mouseSensitivity = 25f; //100 is way to high lol
    public float xRotation = 0f; //No limit
    public float upClamp = -90f; //Look Up
    public float downClamp = 30f; //Look Down 

    [Header("References")]
    public Transform transformCamera;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionReference Look;
    public InputActionReference Move;
    public InputActionReference Jump;
    public InputActionReference Sprint;
    public InputActionReference Crouch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        transformCamera = Camera.main.transform;

        Look.action.actionMap.Enable();


        //Enable actions
        Look.action.Enable();
        Move.action.Enable();
        Jump.action.Enable();
        Sprint.action.Enable();
        Crouch.action.Enable();
    }

    void Update()
    {
        float groundSnap = -2f;
        //Debug.Log($"Jump value: {Jump.action.ReadValue<float>()}");

        isGrounded = characterController.isGrounded;

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = groundSnap;
        }

        UseLook();
        UseMove();
        UseJump();
        ApplyGravity();
        UseCrouch();

    }

    public void UseLook()
    {
        //Debug.Log("Look Triggered");

        Vector2 lookInput = Look.action.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, upClamp, downClamp);

        transformCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void UseMove()
    {
        //Debug.Log("Move Triggered");

        Vector2 input = Move.action.ReadValue<Vector2>();
        float currentSpeed = Sprint.action.IsPressed() ? sprintSpeed : speed;
        //Debug.Log($"Sprint check: IsPressed={Sprint.action.IsPressed()}");

        float speedFlightCap = 0.4f; //percent of speed used, only 40% of speed is used when in flight

        if (!isGrounded)
        {
            currentSpeed *= speedFlightCap; 
        }

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }
    public void UseJump()
    {
        float groundSnap = -2f;
         
        if (Jump.action.triggered && isGrounded)
        {
            //Debug.Log("Jump Triggered");
            velocity.y = Mathf.Sqrt(jumpHeight * groundSnap * gravity);
        }
    }

    public void UseCrouch()
    {
        float halfHeight = 2f;

        if (Crouch.action.triggered)
        {
            if (isCrouching)
            {
                characterController.height = normalHeight;
                characterController.center = new Vector3(0, normalHeight / halfHeight, 0);
                speed = 3f;
                isCrouching = false;
            }
            else
            {
                characterController.height = crouchHeight;
                characterController.center = new Vector3(0, crouchHeight / halfHeight, 0);
                speed = crouchSpeed;
                isCrouching = true;
            }
        }

        Vector3 targetPos = isCrouching ? crouchCamera : standCamera;
        transformCamera.localPosition = Vector3.Lerp
        (transformCamera.localPosition, targetPos, 
        Time.deltaTime * cameraTransition);
    }

    public void ApplyGravity()
    {
        //Debug.Log("Gravity Applied");
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
