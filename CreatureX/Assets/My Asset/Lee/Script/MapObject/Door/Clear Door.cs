using System;
using UnityEngine;

public class ClearDoor : MonoBehaviour
{
    [NonSerialized] public bool isInPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayer = false;
        }
    }


}
