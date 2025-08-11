using UnityEngine;

public class KeyDoor : MonoBehaviour, IDoor
{
    public bool Key { get; set; }

    public bool DoorTouch { get; set; }

    public Camera Camera;

    [SerializeField] private KeyData keyDataAsset;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Key = false;
    }

    private void Update()
    {
        Debug.Log(Key);
        if (keyDataAsset.Key && !Key)
        {
            Key = true;
            animator.SetBool("Key",Key);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&& keyDataAsset.Key)
        {
            DoorTouch = true;
            Destroy(gameObject);
        }
    }

}
