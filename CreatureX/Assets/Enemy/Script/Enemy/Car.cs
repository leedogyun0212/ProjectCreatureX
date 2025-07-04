using System.Collections;
using UnityEngine;

public class Car : BaseEnemy
{
    protected bool isMove = true;

    [SerializeField]
    private PolygonCollider2D capsule;

    protected override void Update()
    {
        animEnemy();
        if(EnemyFailed)
        {
            monsterState = MonsterState.failed;
        }
        else if (monsterState == MonsterState.Move)
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
            isMove = false;
            capsule.isTrigger = true;
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        //if(collision.CompareTag("Player")&&!isMove)
        //{
        //    capsule.isTrigger = false;
        //    isMove =true;
        //}
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player")&&!isMove)
        {
            capsule.isTrigger = true;
            StartCoroutine(TriggerFalse());
            isMove = false;
        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            monsterState = MonsterState.idle;
            isMove = false;
            animWalk = 0;
            animDead = 100;
        }
    }

    private IEnumerator TriggerFalse()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        capsule.isTrigger = false;
        isMove = true;
    }

    protected override IEnumerator HitRecover()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초간 피격 상태 유지
        monsterState = MonsterState.Move;
    }

    public void animDie()
    {
        EnemyFailed = true;
        animDead = 150;
    }

}

