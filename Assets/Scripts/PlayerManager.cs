using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController player; //Player's character controller is assigned in editor
    public bool isGrounded; //in the movement function is used to check if the player is on the ground
    public Vector3 velocity; // used to calculate the acceleration due to gravity
    public Transform groundCheck; // assigned in editor, is the object used to check that the player iss grounded
    public float groundDistance = 0.4f;
    public LayerMask groundMask; // is assigned in the editor and will hold the ground
    public float gravity = -9.81f; //sets value of g
    public float speed = 5f; // sets player speed

    // Update is called once per frame
    void Update()
    {
        Movement();

        Interaction();
    }

    //Movement handler
    public void Movement()
    {

        float x = Input.GetAxisRaw("Horizontal");//stores horizontal value
        float z = Input.GetAxisRaw("Vertical");//stores vertical value
        
        Vector3 direction = new Vector3(x,0,z).normalized;//makes a vector based off the horizontal and vertical values, the .normalized makes the magnitude = 1
        
        if(direction.magnitude > 0.1f) //checks that the magnitude of the vector is bigger than 0.1
        {
            player.Move(direction*Time.deltaTime*speed);//moves the player in the vector direction, Time.deltaTime is the ammount of time between frames and makes it so that framerate doesnt affect speed
        }
        
    }

    //Interaction handler
    public void Interaction()
    {
        
    }
}
