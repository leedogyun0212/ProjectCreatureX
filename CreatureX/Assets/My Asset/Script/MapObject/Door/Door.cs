using UnityEngine;

public class Door : MonoBehaviour, IDoor
{
    public bool DoorTouch { get; set; }

    public Camera Camera;

    public DoorDirection direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            Camera.SetLockDirection(direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Camera.DoorTouch = true;
            //Camera.DoorCam();
            Destroy(gameObject);
        }
    }
}
