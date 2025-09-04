using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ladder : MonoBehaviour
{
    private Collider2D Hit;

    private BoxCollider2D box;

    [SerializeField]
    private LayerMask Playerlayer;

    [SerializeField] private float climbSpeed = 3f;

    private Rigidbody2D playerRb;

    private bool isOnLadder = false;

    private TestPlayer testPlayer = null;

    private void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        PlayerEnter();
        if (testPlayer != null && (!testPlayer.unableRel || testPlayer.curForm != TestPlayer.AssimilateForm.None))
        {
            Climb();
        }
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
            testPlayer = playerRb != null ? playerRb.gameObject.GetComponent<TestPlayer>() : null;
            LadderEnd();

            if (testPlayer == null || testPlayer.unableRel || testPlayer.curForm == TestPlayer.AssimilateForm.None) return;

            isOnLadder = true;
            // �߷� ����
            playerRb.gravityScale = 0f;
            // �ӵ� �ʱ�ȭ (�߷¿� ���� �������� �ʵ���)
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
        }
        else
        {
            if (isOnLadder && playerRb != null)
            {
                // ��ٸ����� �������� �߷� ����
                playerRb.gravityScale = 8f;
            }
            isOnLadder = false;
            playerRb = null;
            testPlayer = null;
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

            // ���Ʒ� ������ ����
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, verticalInput * climbSpeed);
        }
    }

    private void LadderEnd()
    {
        if (playerRb != null && Mathf.Abs(box.bounds.max.y - playerRb.position.y) < 0.1f)
        {
            Debug.Log("ī�޶� ����!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
