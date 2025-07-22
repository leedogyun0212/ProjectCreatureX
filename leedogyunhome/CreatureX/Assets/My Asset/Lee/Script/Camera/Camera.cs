using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour, IDoor
{
    [SerializeField]
    private BoxCollider2D CamCollider;

    public CapsuleCollider2D PlayerCollider;

    [SerializeField]
    private float CameraSpeed = 4.0f;

    private Vector3 SavePos;

    public bool DoorTouch { get; set; } = true;

    public enum LockDirection
    {
        None = 0,
        Right = 1,
        Left = 2,
        Up = 3, 
        Down = 4,
    }

    private LockDirection currentLock;

    [SerializeField]
    private float verticalDeadZoneHeight = 1.5f; // 데드존 높이
    private float targetY;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        targetY = transform.position.y;
    }

    private void Update()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        Vector3 playerPos = PlayerCollider.bounds.center;
        Vector3 camPos = transform.position;

        if (!DoorTouch)
        {
            switch (currentLock)
            {
                case LockDirection.Right:
                    if (playerPos.x > camPos.x) { MoveCamYOnly(); return; }
                    break;
                case LockDirection.Left:
                    if (playerPos.x < camPos.x) { MoveCamYOnly(); return; }
                    break;
                case LockDirection.Up:
                    if (playerPos.y > camPos.y) { MoveCamXOnly(); return; }
                    break;
                case LockDirection.Down:
                    if (playerPos.y < camPos.y) { MoveCamXOnly(); return; }
                    break;
            }
        }

        MoveCam();
    }

    public void SetLockDirection(DoorDirection doorDir)
    {
        {
            // DoorDirection -> LockDirection 변환
            switch (doorDir)
            {
                case DoorDirection.Left:
                    currentLock = LockDirection.Left;
                    break;
                case DoorDirection.Right:
                    currentLock = LockDirection.Right;
                    break;
                case DoorDirection.Up:
                    currentLock = LockDirection.Up;
                    break;
                case DoorDirection.Down:
                    currentLock = LockDirection.Down;
                    break;
            }

            DoorTouch = false;
        }
    }

    private void MoveCam()
    {
        float playerY = PlayerCollider.bounds.center.y;
        float camY = transform.position.y;

        // Y축 데드존 검사
        if (playerY > camY + verticalDeadZoneHeight)
        {
            targetY = playerY - verticalDeadZoneHeight;
        }
        else if (playerY < camY - verticalDeadZoneHeight)
        {
            targetY = playerY + verticalDeadZoneHeight;
        }

        Vector3 targetPos = new Vector3(
            PlayerCollider.bounds.center.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.15f); // velocity는 Vector3
    }

    private void MoveCamXOnly()
    {
        Vector3 xOnly = new Vector3(PlayerCollider.bounds.center.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, xOnly, CameraSpeed * Time.deltaTime);
    }

    private void MoveCamYOnly()
    {
        Vector3 target = new Vector3(transform.position.x, GetVerticalFollowY(), transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.15f);
    }

    private float GetVerticalFollowY()
    {
        float PlayerY = PlayerCollider.bounds.center.y;
        float CamY = transform.position.y;
        float PlayerBottomY = PlayerCollider.bounds.min.y;
        float CamBottomY = CamCollider.bounds.min.y;

        if (PlayerY > CamY + 0.25f)
            return PlayerY;
        else if (PlayerBottomY < CamBottomY-0.3f)
        {
            return PlayerY;
        }

        return CamY;
    }

    public void DoorCam()
    {
        StartCoroutine(SlowReturnToPlayer());
    }

    private IEnumerator SlowReturnToPlayer()
    {
        float elapsedTime = 0f;
        float duration = 1.5f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(PlayerCollider.bounds.center.x, GetVerticalFollowY(), transform.position.z);

        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1.5f, t); // 여기!

            transform.position = Vector3.Lerp(start, end, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            Debug.Log(PlayerCollider.bounds.min.y + "sdas");
            Debug.Log(CamCollider.bounds.min.y - 0.3f);
            DoorTouch = true;
            currentLock = LockDirection.None;
        }
    }

}