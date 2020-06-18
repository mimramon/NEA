using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
      void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("reception");
    }
}
