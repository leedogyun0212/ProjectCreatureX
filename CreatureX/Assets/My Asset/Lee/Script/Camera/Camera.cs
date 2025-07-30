using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour, IDoor
{
    [SerializeField]
    private BoxCollider2D CamCollider; // 카메라의 콜라이더

    public CapsuleCollider2D PlayerCollider; // 플레이어 콜라이더

    private UnityEngine.Camera MainCamera; // 카메라 

    [SerializeField]
    private float CameraSpeed = 4.0f; // 카메라 속도 

    public bool DoorTouch { get; set; } = true; // 문에 닿았나 안 닿았나 관련 bool

    public enum LockDirection // 문 방향 지정
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
    private float cameraHeight; // 카메라 사이즈

    private Vector3 Target; // y움직임 관련 벡터

    private Vector3 velocity = Vector3.zero; // 카메라 이동 관련 벡터

    private bool IsMoving = false; // y 이동할때 쓰는 bool

    private float moveDuration = 0.3f; // IsMoving 바뀔때 잠깐의 대기 시간

    [SerializeField]
    private bool FollowYDirectly = false;

    private void Start()
    {
        MainCamera = GetComponent<UnityEngine.Camera>();
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

        YMovingUpdate();

        if (!IsMoving)
            CameraUp();
        if (!DoorTouch) // 이것으로 문의 이동이 제한
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

    private void YMovingUpdate()
    {
        if (!IsMoving)
        {
            if (FollowYDirectly)
                FollowPlayerY(); // 새로 만들 함수
            else
                CameraUp(); // 기존대로 작동
        }
    }

    public void SetLockDirection(DoorDirection doorDir) // 문에서 이동제한을 결정하는 코드
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
    
    private void MoveCam() // 카메라의 이동을 담당하는 코드
    {
        Vector3 targetPos = new Vector3(
            PlayerCollider.bounds.center.x,
            Target.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.15f); // velocity는 Vector3
    }

    private void MoveCamXOnly() // x밖에 못 움직일때 쓰는 코드
    {
        Vector3 xOnly = new Vector3(PlayerCollider.bounds.center.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, xOnly, CameraSpeed * Time.deltaTime);
    }

    private void MoveCamYOnly() // y밖에 못 움직일때 쓰는 코드
    {
        Vector3 target = new Vector3(transform.position.x, Target.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.15f);
    }

    private void CameraUp() // y 의 이동에 관여하는 Target의 벡터 설정
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

    private void FollowPlayerY()
    {
        // 제한된 범위 내에서만 따라감
        float clampedY = PlayerCollider.bounds.center.y;

        Vector3 PlayerPos = new Vector3(transform.position.x, clampedY, transform.position.z);

        // 스무딩 버전
        Target = Vector3.SmoothDamp(transform.position, PlayerPos, ref velocity, 0.15f);
    }

    private IEnumerator ReStartMoving() // IsMoving 풀릴때 잠깐의 대기시간
    {
        yield return new WaitForSeconds(moveDuration + 0.05f);
        IsMoving = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            DoorTouch = true;
            currentLock = LockDirection.None;
        }
    }

    public void EnableDirectYFollow()
    {
        FollowYDirectly = true;
    }

    public void DisableDirectYFollow()
    {
        FollowYDirectly = false;
    }

}