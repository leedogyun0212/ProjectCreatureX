using UnityEngine;

public class Ladder : MonoBehaviour
{
    private Collider2D Hit;

    private BoxCollider2D box;

    [SerializeField]
    private LayerMask Playerlayer;

    [SerializeField] private float climbSpeed = 3f;

    private Rigidbody2D playerRb;

    private bool isOnLadder = false;

    TestPlayer testPlayer;

    private bool ClimbOn;

    private void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        PlayerEnter();
        Climb();
    }

    private void PlayerEnter()
    {
        Vector2 ladderPos = transform.position;
        Hit = Physics2D.OverlapBox(ladderPos, box.bounds.size, 0f, Playerlayer);

        if (Hit != null)
        {
            // 첫 진입 시 null 방어: playerRb/testPlayer 캐싱
            if (playerRb == null)
            {
                playerRb = Hit.attachedRigidbody;
                testPlayer = playerRb != null ? playerRb.gameObject.GetComponent<TestPlayer>() : null;
            }

            // Robot/Drone이면 ClimbOn=true (사다리 금지)
            ClimbOn = (testPlayer != null) && (testPlayer.curForm == TestPlayer.AssimilateForm.Robot || testPlayer.curForm == TestPlayer.AssimilateForm.Drone);

            // 로봇/드론 상태에선 아무것도 하지 않음(중력/상태 불변)
            if (ClimbOn) return;

            // 일반 상태에서만 사다리 진입 허용
            if (!isOnLadder && Input.GetKey(KeyCode.UpArrow))
            {
                isOnLadder = true;
            }

            // 일반 상태에서만 중력 0/속도 초기화
            if (isOnLadder && playerRb != null)
            {
                // 중력 해제
                playerRb.gravityScale = 0f;
                // 수직 속도 초기화
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
            }
        }
        else
        {
            // 사다리를 벗어나면 일반 상태일 때만 중력 복구(ClimbOn=true면 건드리지 않음)
            if (isOnLadder && playerRb != null && !ClimbOn)
            {
                playerRb.gravityScale = 8f;
                playerRb = null;
            }
            isOnLadder = false;
        }
    }

    private void Climb()
    {
        // 로봇/드론 상태에선 사다리 입력 처리 안 함
        if (ClimbOn) return;

        // 일반 상태에서만 등반 입력 처리
        if (isOnLadder && playerRb != null)
        {
            float verticalInput = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                verticalInput = -1f;
            else
                verticalInput = 0f;

            // 위 아래 움직임 적용  
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, verticalInput * climbSpeed);
        }
    }
}
