using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IDoor
{
    public bool DoorTouch { get; set; }

    public Camera Camera;

    public DoorDirection direction;

    private float CameraTime = 3.0f;

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
            ClampCameraImmediately(collision); // [변경] 카메라 BoxCollider2D 기준으로 즉시 보정
            StartCoroutine(DoorOpen()); // [변경] 한 번만 실행되도록 Enter 시점으로 이동
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // [변경] 트리거 내부에 있는 동안 프레임마다 보정하여 문 너머로 튀지 않도록 함
        if (collision.CompareTag("MainCamera") && CameraTime > Time.deltaTime)
        {
            ClampCameraImmediately(collision);
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

    // [변경] 카메라 BoxCollider2D 화면 크기 기준으로 문 경계에서 즉시 클램핑
    private void ClampCameraImmediately(Collider2D camCollider)
    {
        if (Camera == null) return;

        Vector3 camPos = Camera.transform.position;
        Vector3 doorPos = transform.position;
        Vector2 half = camCollider.bounds.extents;

        switch (direction)
        {
            case DoorDirection.Right: // 오른쪽 이동 제한: 카메라 x가 문 x를 넘지 않도록
                camPos.x = Mathf.Min(camPos.x, doorPos.x - half.x);
                break;
            case DoorDirection.Left: // 왼쪽 이동 제한
                camPos.x = Mathf.Max(camPos.x, doorPos.x + half.x);
                break;
            case DoorDirection.Up: // 위쪽 이동 제한
                camPos.y = Mathf.Min(camPos.y, doorPos.y - half.y);
                break;
            case DoorDirection.Down: // 아래쪽 이동 제한
                camPos.y = Mathf.Max(camPos.y, doorPos.y + half.y);
                break;
        }
    }
}
