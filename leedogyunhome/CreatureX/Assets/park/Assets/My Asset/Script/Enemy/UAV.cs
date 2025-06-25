using UnityEngine;

public class UAV : BaseEnemy
{
    protected bool isMove = true;

    protected bool isReturning = false;

    protected override void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.idle)
        {
            SimpleMove();
        }
        else if (monsterState == MonsterState.Move)
        {
            StateMove();
        }
    }

    private void StateMove()
    {

        Move();

        if(isReturning&&Mathf.Abs(UAVPos.y - transform.position.y) < 0.1f)
        {
            monsterState = MonsterState.idle;
            isReturning = false;
            MoveSpeed = 3.0f;
        }
    }

    protected override void Getdirection()
    {
        direction = (player.transform.position - transform.position).normalized;
        Debug.Log(direction);
        animWalk = 1;
    }

    protected void SimpleMove()
    {
        float xOffset = Mathf.Sin(Time.time * 2f) * 18.0f;
        Vector2 targetPos = new Vector2(UAVPos.x + xOffset, UAVPos.y);
        Vector2 newPos = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * MoveSpeed);
        rigidbody.MovePosition(newPos);
        if (xOffset > 3.9f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xOffset < -3.9f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void FlyUAV()
    {
        direction = (UAVPos - transform.position).normalized;
        isReturning = true;

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isMove&& !isReturning)
        {
            Getdirection();
            MoveSpeed = 8.0f;
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            FlyUAV();
            MoveSpeed = 8.0f;
            Debug.Log(direction);

        }
    }
}
