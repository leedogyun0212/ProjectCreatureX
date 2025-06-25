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
            if(UAVPosY - transform.position.y < 0.1)
                monsterState = MonsterState.idle;
            Debug.Log("???");
        }
    }

    protected override void Getdirection()
    {
        direction = (player.transform.position - transform.position).normalized;
        animWalk = 1;
    }

    protected void SimpleMove()
    {
        float xOffset = Mathf.Sin(Time.time * 2f) * 4.0f;
        Vector2 Posmove = new Vector2(UAVPos.x + xOffset, UAVPos.y);
        if (xOffset > 3.9f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xOffset < -3.9f)
            transform.localScale = new Vector3(1, 1, 1);
        rigidbody.MovePosition(Posmove);
    }

    private void FlyUAV()
    {
        direction = (UAVPos - transform.position).normalized;
        
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

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            FlyUAV();
            MoveSpeed = 5.0f;
            Debug.Log(direction);

        }
    }
}
