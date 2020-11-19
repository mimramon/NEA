using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject player;
    public GameObject cam;

    void Update() 
    {
        cam.Move(player.transform);
    }

}
