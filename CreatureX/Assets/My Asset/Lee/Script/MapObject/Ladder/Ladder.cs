using Unity.VisualScripting;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    private Collider2D Hit;

    private BoxCollider2D box;

    [SerializeField]
    private LayerMask Playerlayer;

    [SerializeField] private float climbSpeed = 3f;

    private float Playergravity;

    private Rigidbody2D playerRb;

    private bool isOnLadder = false;

    private TestPlayer player;

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

        if (Hit != null && Input.GetKey(KeyCode.UpArrow) && !isOnLadder)
            isOnLadder = true;
        else if (Hit != null && isOnLadder)
        {
            if (playerRb == null)
                playerRb = Hit.attachedRigidbody;
            
            player = Hit.gameObject.GetComponent<TestPlayer>();
            //isOnLadder = true;
            // 중력 끄기
            playerRb.gravityScale = 0f;
            // 속도 초기화 (중력에 의해 내려가지 않도록)
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
        }
        else
        {
            if (isOnLadder && playerRb != null)
            {
                // 사다리에서 떨어지면 중력 복구
                playerRb.gravityScale = 8f;
                playerRb = null;
            }
            isOnLadder = false;
        }
    }

    private void Climb()
    {
        if (isOnLadder && playerRb != null)
        {
            float verticalInput = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                verticalInput = -1f;
            else
                verticalInput = 0f;

            // 위아래 움직임 적용
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, verticalInput * climbSpeed);
        }
    }
}
