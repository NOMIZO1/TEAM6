using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private float openHeight = 4f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        StartCoroutine(OpenAnimation());
    }

    IEnumerator OpenAnimation()
    {
        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition + Vector3.up * openHeight;

        while (Vector3.Distance(
            transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        Debug.Log("î‡Ç™äJÇ´Ç‹ÇµÇΩÅI");
    }
}
