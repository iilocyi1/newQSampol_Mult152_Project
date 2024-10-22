using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public GameObject player;
    private float heightIncrease = 0f;
    private bool isBossFight = false;

    void Start()
    {
        // Initial setup if needed
    }

    void LateUpdate()
    {
        if (isBossFight)
        {
            CameraFollow();
        }
        else
        {
            PlayerCamera();
        }
    }

    private void CameraFollow()
    {
        Vector3 desiredPosition = target.position + offset + new Vector3(0, heightIncrease, 0);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    private void PlayerCamera()
    {
        transform.position = player.transform.position + new Vector3(4, 6, 1);
    }

    public void IncreaseHeight(float amount)
    {
        heightIncrease += amount;
    }

    public void SetBossFight(bool isActive)
    {
        isBossFight = isActive;
    }
}
