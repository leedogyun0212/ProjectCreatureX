using UnityEngine;

public class Car : BaseEnemy
{
    protected bool isMove = true;

    protected override void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.Move)
        {
            Move();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&isMove)
        {
            Getdirection();
            MoveSpeed = 8.0f;
            Debug.Log("¿€µøs");
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            monsterState = MonsterState.idle;
            isMove = false;
            animWalk = 0;
        }
    }
}

