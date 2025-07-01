using UnityEngine;

public class Car : BaseEnemy
{
    protected bool isMove = true;

    [SerializeField]
    private CapsuleCollider2D capsule;

    protected override void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.Move)
        {
            Move();
        }
        else if (monsterState == MonsterState.Dead)
        {
            EnemyDead();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&isMove)
        {
            Getdirection();
            MoveSpeed = 8.0f;
            capsule.isTrigger = true;
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&!isMove)
        {
            capsule.isTrigger = false;
            isMove =true;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            capsule.isTrigger = true;
            isMove = false;
        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            monsterState = MonsterState.idle;
            isMove = false;
            animWalk = 0;
        }
    }
}

