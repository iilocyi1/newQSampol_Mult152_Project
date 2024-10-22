using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOutOfBounds : MonoBehaviour
{
    private float leftOfScene = -50.0f;
    private float rightOfScene = 55.0f; 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < leftOfScene)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z > rightOfScene)
        {
            Destroy(gameObject);
        }
    }
}
