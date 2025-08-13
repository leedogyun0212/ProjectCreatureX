using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IDoor
{
    public bool DoorTouch { get; set; }

    public Camera Camera;

    public DoorDirection direction;

    private bool Check = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        DoorTouch = false;
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
        if (collision.CompareTag("MainCamera")&&!Check)
        {
            Check = true;
            StartCoroutine(DoorOpen());
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

    private IEnumerator DoorOpen()
    {
        yield return new WaitForSecondsRealtime(1f);

        animator.SetBool("Open", DoorTouch);
    }
}
