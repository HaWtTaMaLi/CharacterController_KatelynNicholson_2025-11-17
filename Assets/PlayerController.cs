using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject Player;

    public int speed = 5;
    public float jumpHeight = 1.5f;
    public float gravity = -9f;

    public CharacterController playerController; //get component
    public bool isGrounded = true;
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
        isGrounded = playerController.isGrounded;

        float x = Input.GetAxis("Horizontal"); //W && S
        float z = Input.GetAxis("Vertical"); //A && D

        Vector3 move = transform.right * x + transform.forward * z;

        Player.transform.position += move * speed * Time.deltaTime; //change transform to Move character controller methods

    }

}

