using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController player; //Player's character controller is assigned in editor

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
            player.Move(direction);//moves the player in the vector direction
        }
        
    }

    //Interaction handler
    public void Interaction()
    {
        
    }
}
