using UnityEngine;

public class WorldText : MonoBehaviour
{
    //put this on the Cavas Item you want Facing the camera always

    [Header("References")]
    public Transform playerHandler; //the object you want the canvas to face

    void LateUpdate()
    {
        transform.forward = playerHandler.forward;
    }
}
