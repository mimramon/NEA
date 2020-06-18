using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseCamera : MonoBehaviour
    
{

public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform cameraBody;
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        //horizontal movement
        playerBody.Rotate(Vector3.up * mouseX);

        //vertical movement
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
