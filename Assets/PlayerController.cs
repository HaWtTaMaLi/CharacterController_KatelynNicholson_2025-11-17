using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Speed")]
    public float speed = 3f; //low walk speed to notice >
    public float sprintSpeed = 8f; //> the difference in sprinting.
    public float smoothSpeed = 10f; //higher = faster
    public float currentVelocity = 0f;
    public Vector3 velocity;

    [Header("Jump Settings")]
    public float jumpHeight = 0.8f; //1.5 was to high imo
    public float airTime = 0f;
    public float gravity = -9.81f;
    public bool isGrounded = true;

    [Header("Crouch Settings")]
    public Animator crouch;
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 1f;
    public float cameraTransition = 5f;
    public Vector3 standCamera = new Vector3(-0.03908123f, 1.97f, 0.2218122f);
    public Vector3 crouchCamera = new Vector3(-0.03908123f, 1.01f, 0.32f);
    public bool isCrouching = false;

    [Header("Head Cheak")]
    public Transform headCheck;
    public float headCheckLength = 0.87f;
    public LayerMask groundMask;

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
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = groundSnap;
        }

        UseLook();
        UseMove();
        UseJump();
        HeadDetect();
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

        Vector2 input = Move.action.ReadValue<Vector2>();

        if (isCrouching) //if we are crouching
            currentVelocity = crouchSpeed; //force crouch speed
        else if (Sprint.action.IsPressed())
            currentVelocity = sprintSpeed; 
        else
            currentVelocity = speed;

        float speedFlightCap = 0.4f; //percent of speed used, only 40% of speed is used when in air

        if (!isGrounded)
            currentVelocity *= speedFlightCap;

        //acceleration/deceleration
        this.currentVelocity = Mathf.Lerp(this.currentVelocity, currentVelocity,
            Time.deltaTime * smoothSpeed);

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        characterController.Move(move * currentVelocity * Time.deltaTime);
    }

    public void UseJump()
    {
        float groundSnap = -2f;
         
        if (Jump.action.triggered && isGrounded && !isCrouching && OnFlatGround())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * groundSnap * gravity);
        }
    }

    public void UseCrouch()
    {
        float halfHeight = 2f;

        if (Crouch.action.triggered && isGrounded)
        {

            if (isCrouching)
            {
                if (HeadDetect()) // why does it work without '!' i dunno lol
                {
                    characterController.height = normalHeight;
                    characterController.center = new Vector3(0, normalHeight / halfHeight, 0);
                    speed = 3f;

                    isCrouching = false;
                }
            }
            else
            {
                characterController.height = crouchHeight;
                characterController.center = new Vector3(0, crouchHeight / halfHeight, 0);
                speed = crouchSpeed;

                isCrouching = true;
            }
            //play animation
            crouch.SetBool("isCrouching", isCrouching);

        }

        //camera transition 
        Vector3 targetPos = isCrouching ? crouchCamera : standCamera;
        transformCamera.localPosition = Vector3.Lerp
        (transformCamera.localPosition, targetPos,
        Time.deltaTime * cameraTransition);
    }

    public bool OnFlatGround()
    {
        RaycastHit hit;
        float rayDistance = characterController.height / 2 + 0.02f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            return slopeAngle <= characterController.slopeLimit;
        }
        return false;
    }

    public bool HeadDetect()
    {
        Vector3 start = transformCamera.position;
        float checkDistance = normalHeight - crouchHeight;
        return !Physics.Raycast(start, Vector3.up, checkDistance);
    }

    public void ApplyGravity()
    {
        //Debug.Log("Gravity Applied");
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
