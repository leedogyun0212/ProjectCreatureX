using UnityEngine;

public class UAV : BaseEnemy
{
    protected bool isMove = true;

    protected override void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.idle)
        {
            SimpleMove();
        }
        else if (monsterState == MonsterState.Move)
        {
            Move();
        }
    }

    protected override void Getdirection()
    {
        direction = (player.transform.position - transform.position).normalized;
        animWalk = 1;
    }

    protected void SimpleMove()
    {
        float yOffset = Mathf.Sin(Time.time * 2f) * 0.3f;
        Vector2 Posmove = new Vector2(transform.position.x, UAVPosY + yOffset);
        rigidbody.MovePosition(Posmove);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isMove)
        {
            Getdirection();
            MoveSpeed = 8.0f;
            monsterState = MonsterState.Move;
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Ground"))
        {
            monsterState = MonsterState.Play;
            isMove = false;
            animWalk = 0.0f;
            animator.SetFloat("Walk", Mathf.Abs(direction.x));
        }
    }
}
