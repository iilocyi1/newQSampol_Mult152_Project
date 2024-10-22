using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnives : MonoBehaviour
{
    public float speed = 15.0f; // Speed at which the knife moves forward

    // Update is called once per frame
    void Update()
    {
        // Move the knife forward based on its current rotation and speed
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
