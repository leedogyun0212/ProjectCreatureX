using UnityEngine;

public class KeyItem : MonoBehaviour, IDoorKey
{
    public bool Key { get; set; }

    private GameObject Player;

    private Vector3 KeyPos;

    private void Start()
    {
        Player = GameObject.Find("Test Player");

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("≈∞ æ∆¿Ã≈€ »πµÊ!!!!!!!!!!!!!!!");
            Key = true;
            KeyPos = Player.transform.position;
            KeyPos.y += 2.0f;
            transform.position = Vector2.Lerp(transform.position, KeyPos, 4.0f * Time.deltaTime);
            Destroy(gameObject);
        }
    }
}
