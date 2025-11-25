using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject Player;

    public int speed = 5;
    public float jumpHeight = 1.5f;
    public float jumpPower;
    public float gravity = -9.81f;

    public CharacterController playerController; //get component
    public Vector3 playerVelocity;

    [Header("Input Actions")]
    public InputActionReference Move; //vector2
    public InputActionReference Look; //vector2
    public InputActionReference Jump; //button
    public InputActionReference Sprint; //button
    public InputActionReference Interact; //button
    public InputActionReference Crouch; //button

    public void Update()
    {

        float x = Input.GetAxis("Horizontal"); //W && S
        float z = Input.GetAxis("Vertical"); //A && D

        Vector3 move = transform.right * x + transform.forward * z;

        playerController.Move(move * speed * Time.deltaTime); //change transform to Move character controller methods

        if (playerController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; //constant gounding
        }

        playerVelocity.y += gravity * Time.deltaTime;
        playerController.Move(playerVelocity * Time.deltaTime);
    }

    public void Jumping(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!playerController.isGrounded) return;

        playerVelocity.y += jumpHeight;
    }

}

