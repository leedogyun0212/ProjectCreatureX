using UnityEngine;

public class Door : MonoBehaviour
{
    public bool Key;

    public Camera Camera;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Camera.MoveStop = false;
            Destroy(gameObject);
        }
    }

}
