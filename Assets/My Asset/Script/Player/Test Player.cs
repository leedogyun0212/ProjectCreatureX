using System.Runtime.CompilerServices;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;
    private bool isJump = false;

    private void Update()
    {
        
    }


    public void OrderMove(float direction) 
    {
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
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }
    }


}
