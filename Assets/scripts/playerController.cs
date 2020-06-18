using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public CharacterController controller;

    public float gravity = -9.81f;
    public float speed = 5f;
    public float jumpHeight = 5f;

    public Transform groundCheck;
    public float groundDistance=0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        //ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //movement
        Vector3 movement = transform.right * x + transform.forward * z;
        controller.Move(movement*speed*Time.deltaTime);

        //jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //gravity
        velocity.y += gravity*Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);

       
    }
}
