using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player; //player's  position
    public Transform cam; //camera's position
    public Vector3 offset = new Vector3(0f, 0f, -10f); //Vector for the camera offset

    void Update() 
    {
        cam.position = player.position + offset;//every frame the cameras position is updated to the player position + the offset
    }

}
