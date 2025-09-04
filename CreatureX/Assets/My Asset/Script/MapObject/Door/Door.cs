using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IDoor
{
    public bool DoorTouch { get; set; }

    public Camera Camera;

    public DoorDirection direction;

    private float CameraTime = 3.0f;

    private Animator animator;

    // 카메라 클램프를 LateUpdate 타이밍에 일관되게 처리하기 위한 상태 보관
    private bool _shouldClampCamera = false;
    private Collider2D _cachedCameraCollider = null;

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
            // 즉시 1회 보정으로 초기 관통 방지
            ClampCameraImmediately(collision);
            // 이후 프레임에서는 LateUpdate에서 일관된 타이밍으로 보정
            _cachedCameraCollider = collision;
            _shouldClampCamera = true;
            StartCoroutine(DoorOpen()); // [변경] 한 번만 실행되도록 Enter 시점으로 이동
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 트리거 내부에 머무는 동안 LateUpdate에서만 보정하도록 플래그/콜라이더 유지
        if (collision.CompareTag("MainCamera"))
        {
            _cachedCameraCollider = collision;
            _shouldClampCamera = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            _shouldClampCamera = false;
            _cachedCameraCollider = null;
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

        Camera.transform.position = camPos;
    }

    private void LateUpdate()
    {
        // 카메라 이동(추적)이 끝난 뒤 최종 타이밍에 1회 보정하여 흔들림 방지
        if (_shouldClampCamera && _cachedCameraCollider != null)
        {
            ClampCameraImmediately(_cachedCameraCollider);
        }
    }
}
