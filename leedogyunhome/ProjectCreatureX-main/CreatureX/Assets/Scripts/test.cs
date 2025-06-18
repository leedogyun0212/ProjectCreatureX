using UnityEngine;

public class test : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private float moveInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        move();
        Jump();
    }

    private void move()
    {
        moveInput = Input.GetAxis("Horizontal");
        rigidbody.linearVelocity = new Vector2(moveInput * 5.0f, rigidbody.linearVelocity.y);
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x,  5.0f);
        }
    }
}
