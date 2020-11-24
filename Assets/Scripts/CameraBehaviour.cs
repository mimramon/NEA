using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player; //player's  position
    public Transform cam; //camera's position
    public Vector3 offset = new Vector3(0,0,-10); //
    public float currentYaw = 0; //
    public float yawSpeed = 100; //
    public float pitch =2; //
    
    void Update() 
    {
        currentYaw -= Input.GetAxis("CamHorizontal") * yawSpeed * Time.deltaTime;
    }

    void LateUpdate() 
    {
        transform.position = player.position - offset;
        transform.LookAt(player.position + Vector3.up * pitch);

        cam.RotateAround(player.position, Vector3.up, currentYaw);//rotates camera around player
    }
}
