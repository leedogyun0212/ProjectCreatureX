using System;
using UnityEngine;

public class HiddenDoor : MonoBehaviour
{
    [NonSerialized] public bool isInPlayer = false;

    private void Start()
    {
        RuntimeGameManager.Instance.Regist_Door(this);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        TestPlayer player = other.GetComponent<TestPlayer>();
        if (player != null)
        {
            isInPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TestPlayer player = other.GetComponent<TestPlayer>();
        if (player != null)
        {
            isInPlayer = false;
        }
    }
}
