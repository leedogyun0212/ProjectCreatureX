using UnityEngine;

public class KeyDoor : MonoBehaviour, IDoor, IDoorKey
{
    public bool Key { get; set; }

    public bool DoorTouch { get; set; }

    public Camera Camera;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(Key);
        if (collision.gameObject.CompareTag("Player")&&Key)
        {
            DoorTouch = true;
            Destroy(gameObject);
        }
    }

}
