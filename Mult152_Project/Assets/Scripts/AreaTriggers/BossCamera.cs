using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    public playerCamera playerCameraScript;
    public Camera bossCamera;

    void Start()
    {
        // Ensure the player camera is active and the bridge camera is inactive at the start
        playerCameraScript.SetActive(true);
        bossCamera.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch to the bridge camera
            playerCameraScript.SetActive(false);
            bossCamera.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch back to the player camera
            bossCamera.gameObject.SetActive(false);
            playerCameraScript.SetActive(true);
        }
    }
}
