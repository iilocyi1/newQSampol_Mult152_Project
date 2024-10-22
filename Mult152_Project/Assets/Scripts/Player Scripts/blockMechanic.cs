using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockMechanic : MonoBehaviour
{

    public GameObject shieldPrefab;
    private GameObject shieldInstance;
    private bool isBlocking = false;
    public float blockDamageReduction = .5f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartBlocking();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopBlocking();
        }
        
    }

    void StartBlocking()
    {

        isBlocking = true;
        shieldInstance = Instantiate(shieldPrefab, transform.position + transform.forward * 0.75f,
            transform.rotation, transform);
        //Adjust the shield position and rotation as needed
        GetComponent<playerController>().SetBlockingState(true);
    }

    void StopBlocking()
    {
        isBlocking = false;
        Destroy(shieldInstance);
        GetComponent<playerController>().SetBlockingState(false);
    }

    public float CalculateDamage(float incomingDamage)
    {
        if (isBlocking)
        {
            return incomingDamage * blockDamageReduction;
        }

        return incomingDamage;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }
}
