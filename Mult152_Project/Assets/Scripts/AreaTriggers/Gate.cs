using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public float openHeight = 5f;
    public float openSpeed = 2f;
    private bool isOpen = false;

    void Update()
    {
        if (isOpen)
        {
            StartCoroutine(OpenAndDestroy());
            isOpen = false; // Ensure the coroutine is only started once
        }
    }

    public void OpenGate()
    {
        isOpen = true;
    }

    private IEnumerator OpenAndDestroy()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + openHeight, transform.position.z);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
            yield return null;
        }
        Destroy(gameObject);
    }
}
