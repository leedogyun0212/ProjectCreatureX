using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour, IDoor
{
    [SerializeField]
    private BoxCollider2D CamCollider;

    public CapsuleCollider2D PlayerCollider;

    [SerializeField]
    private float CameraSpeed = 4.0f;

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
    [SerializeField] private float cameraStepHeight = 4.0f; // y축 움직임 단위
    [SerializeField] private float climbYThreshold = 1.0f; // 사다리에서 이동할 최소 y 변화
    private float cameraHeight;

    private float targetY;

    private Vector3 Target;

    private Vector3 velocity = Vector3.zero;

    private bool IsMoving = false;

    private float moveDuration = 0.3f;

    private void Start()
    {
        targetY = transform.position.y;
        Target = transform.position;
        cameraHeight = CamCollider.bounds.size.y;
    }

    private void Update()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        Vector3 playerPos = PlayerCollider.bounds.center;
        Vector3 camPos = transform.position;

        GetCameraY();

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
        if (!IsMoving)
            CameraUp();
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
        Vector3 targetPos = new Vector3(
            PlayerCollider.bounds.center.x,
            Target.y,
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
        Vector3 target = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.15f);
    }

    private void GetCameraY()
    {
        float playerY = PlayerCollider.bounds.center.y;
        float camY = transform.position.y;

        if (playerY > camY + verticalDeadZoneHeight)
        {
            // 위로 한 단계 카메라 이동
            targetY = camY + cameraStepHeight;
        }
        else if (playerY < camY - verticalDeadZoneHeight)
        {
            // 아래로 한 단계 카메라 이동
            targetY = camY - cameraStepHeight;
        }
    }

    private void CameraUp()
    {
        float PlayerY = PlayerCollider.bounds.center.y;
        float camMaxY = CamCollider.bounds.max.y;
        float camMinY = CamCollider.bounds.min.y;
        if (PlayerY > camMaxY + verticalDeadZoneHeight)
        {
            Target += new Vector3(0, cameraHeight, 0);
            IsMoving = true;

            StartCoroutine(ReStartMoving());
        }
        else if(PlayerY < camMinY - verticalDeadZoneHeight)
        {
            Target -= new Vector3(0, cameraHeight, 0);
            IsMoving = true;

            StartCoroutine(ReStartMoving());
        }
    }

    private IEnumerator ReStartMoving()
    {
        yield return new WaitForSeconds(moveDuration + 0.05f);
        IsMoving = false;
    }

    public void DoorCam()
    {
        StartCoroutine(SlowReturnToPlayer());
    }

    private IEnumerator SlowReturnToPlayer()
    {
        float elapsedTime = 0f;
        float duration = 1.5f;

        GetCameraY();

        Vector3 start = transform.position;
        Vector3 end = new Vector3(PlayerCollider.bounds.center.x, targetY, transform.position.z);


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