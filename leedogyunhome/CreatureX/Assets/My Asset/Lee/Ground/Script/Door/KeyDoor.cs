using UnityEngine;

public class KeyDoor : MonoBehaviour, IDoor, IDoorKey
{
    public bool Key { get; set; }

    public bool DoorTouch { get; set; }

    public Camera Camera;

    private void Update()
    {
        Debug.Log(Key);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&Key)
        {
            DoorTouch = true;
            Destroy(gameObject);
        }
    }

}
