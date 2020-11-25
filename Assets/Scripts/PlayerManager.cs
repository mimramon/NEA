using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController player; //Player's character controller is assigned in editor
    bool isGrounded; //in the movement function is used to check if the player is on the ground
    public Vector3 velocity; //used to calculate the acceleration due to gravity
    public Transform groundCheck; //assigned in editor, is the object used to check that the player is grounded
    public float groundDistance = 0.4f;
    public LayerMask groundMask; //is assigned in the editor and will hold the ground
    public float gravity = -9.81f; //sets value of g
    public float speed = 5f; //sets player speed
    public Transform cam; //the transform of the camera object, set in editor
    public float turnSmoothTime = 0.1f;//used to smooth out the rotation of the player
    public float jumpHeight = 5; //stores jump height
    float turnSmoothVelocity;//reference variable that holds turning velocity


    //Update is called once per frame
    void Update()
    {
        Gravity();

        Movement();

        Interaction();
    }

    //handles gravity
    public void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Uses the unity physics system to check that the groundcheck object is on the ground and returns bool
        if (isGrounded && velocity.y < 0) //checks if player is grounded
        {
            velocity.y = -2f; //resets the downwards velocity if player is on the ground, the value is -2 to offset marginal problems with the physics calculations, should be 0
        }
        if(Input.GetKeyDown("space") && isGrounded) //checks if the player is grounded and if the pressed spacebar
        {
            velocity.y = jumpHeight; //sets the y velocity to the jump speed
        }
        velocity.y += gravity*Time.deltaTime; //adds g to the y value of the vector, Time.deltaTime is the ammount of time between frames and makes it so that framerate doesnt affect speed and also smoothes out the falling
        player.Move(velocity*Time.deltaTime); //moves player in the vector, Time.deltaTime is the ammount of time between frames and makes it so that framerate doesnt affect speed
    }

    //Movement handler
    public void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");//stores horizontal value
        float z = Input.GetAxisRaw("Vertical");//stores vertical value
        
        Vector3 direction = new Vector3(x,0,z).normalized;//makes a vector based off the horizontal and vertical values, the .normalized makes the magnitude = 1
        
        if(direction.magnitude > 0.1f) //checks that the magnitude of the vector is bigger than 0.1
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;// the angle we want to rotate our player to with respect to the movement direction and the camera position, converted form radeons to degrees
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);//smooths out the rotation of the player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);//rotates player to the angle
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;//fixes the direction vector to conform with the rotation
            player.Move(moveDir.normalized*Time.deltaTime*speed);//moves the player in the vector moveDir, Time.deltaTime is the ammount of time between frames and makes it so that framerate doesnt affect speed
        } 
    }

    //Interaction handler
    public void Interaction()
    {
        
    }
}
