using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player; //player's  position
    public Transform cam; //camera's position
    public Vector3 offset = new Vector3(0,0,-10); // vector containing the offset for the camera
    public float currentYaw = 0; // tracks camera yaw
    public float yawSpeed = 100; // sets the desired yaw speed
    public float pitch = 2; // sets the pitch
    
    //called every frame
    void Update() 
    {
        currentYaw -= Input.GetAxis("CamHorizontal") * yawSpeed * Time.deltaTime; //takes in player input for the camera, CamHorizontal defiined in editor through input manager
    }

    //called after Update()
    void LateUpdate() 
    {
        transform.position = player.position - offset;// guarantees offset from player is maintained
        transform.LookAt(player.position + Vector3.up * pitch);// makes sure player is focal point
        cam.RotateAround(player.position, Vector3.up, currentYaw);//rotates camera around player
    }
}
