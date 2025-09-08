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

    [SerializeField]
    private AudioSource audioSource;

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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            DoorTouch = true;
            animator.SetBool("Key", Key);
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

    public void DoorSound()
    {
        audioSource.Play();

        Invoke(nameof(StopAllSound), audioSource.clip.length);
    }

    private void StopAllSound()
    {
        audioSource.Stop();
    }
}
