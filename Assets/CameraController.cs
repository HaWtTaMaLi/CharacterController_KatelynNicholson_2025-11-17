using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float xRotation = 0f;
    public float mouseSensitivity = 100f;

    //Player Refference
    public Transform playerBody;

    void Start()
    {
        //hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 30); //min and max look

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        playerBody.Rotate(Vector3.up * mouseX);
    }
}
