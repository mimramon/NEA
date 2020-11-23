using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player; //player's  position
    public Transform cam; //camera's position
    
    void Update() 
    {
        Rotate(); 
    }

    void Rotate()
    {
        if(Input.GetKey("q"))
        {
            cam.RotateAround(player.position, cam.up, 20 * Time.deltaTime);//rotates camera around player
        }
    }
}
