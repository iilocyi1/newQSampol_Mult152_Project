using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int keyCount = 0;

    public void AddKey()
    {
        keyCount++;
        Debug.Log("Key added. Total keys: " + keyCount);
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            Debug.Log("Key used. Remaining keys: " + keyCount);
            return true;
        }
        else
        {
            Debug.Log("No keys to use.");
            return false;
        }
    }
}
