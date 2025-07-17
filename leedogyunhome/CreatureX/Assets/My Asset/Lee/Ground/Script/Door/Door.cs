using UnityEngine;

public class Door : MonoBehaviour, IDoor
{
    public bool DoorTouch { get; set; }

    public Camera Camera;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DoorTouch = true;
            Camera.DoorCam();
            Destroy(gameObject);
        }
    }
}
