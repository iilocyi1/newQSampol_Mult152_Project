using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCamera : MonoBehaviour
{
    public playerCamera playerCameraScript;
    public Camera bridgeCamera;

    void Start()
    {
        // Ensure the player camera is active and the bridge camera is inactive at the start
        playerCameraScript.SetActive(true);
        bridgeCamera.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch to the bridge camera
            playerCameraScript.SetActive(false);
            bridgeCamera.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch back to the player camera
            bridgeCamera.gameObject.SetActive(false);
            playerCameraScript.SetActive(true);
        }
    }
}
