using UnityEngine;
using System.Collections;

public class KeyDoor : MonoBehaviour, IDoor
{
    public bool Key { get; set; }

    public bool DoorTouch { get; set; }

    public Camera Camera;

    public DoorDirection direction;

    [SerializeField] private KeyData keyDataAsset;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Key = false;
    }

    private void Update()
    {
        // Debug.Log(Key);
        if (keyDataAsset.Key && !Key)
        {
            Key = true;
            animator.SetBool("Key", Key);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            DoorTouch = true;
            Camera.SetLockDirection(direction);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && keyDataAsset.Key)
        {
            DoorTouch = true;
            if (Camera != null)
            {
                Camera.DoorTouch = true;
            }
            Destroy(gameObject);
        }
    }

}
