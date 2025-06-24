using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

public class TestPlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;
    private bool isJump = false;
    [SerializeField]
    private LayerMask groundLayer; // 레이어

    public Animator animator;

    public bool isAttack = false;

    private void Update()
    {

    }


    public void OrderMove(float direction) 
    {
        if (direction == -1) transform.localScale = new Vector3(-1, 1, 1);
        if (direction == 1) transform.localScale = new Vector3(1, 1, 1);
        animator.SetFloat("Walk", Mathf.Abs(direction));
        Move(direction);
    }
    private void Move(float direction)
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    public void OrderJump()
    {
        if (!isJump)
        {
            Jump();
        }
    }

    private void Jump() 
    {
        isJump = true;
        animator.SetBool("Jump", true);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCheck();

        }
    }

    private void GroundCheck()
    {
        Vector2 Playertrans = transform.position;

        Playertrans.y -= 1.0f; // 플레이어 중심으로 놓으니 옆면도 인식이 되는것 같아서 중심에서 플레이어 발 밑으로 임시로 옮김.

        RaycastHit2D hit = Physics2D.Raycast(Playertrans, Vector2.down, 0.5f, groundLayer); //선을 아래방향으로 0.3f만큼 쏘아 레이어를 구분

        if (hit.collider != null) // 레이어가 groundLayer이라면 아래 의 것을 실행
        {
            isJump = false;
            animator.SetBool("Jump", false);
        }
    }
}
