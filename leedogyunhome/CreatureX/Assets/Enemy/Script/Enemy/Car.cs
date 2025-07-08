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
        if(EnemyFailed) // 차의 흡수 가능한 상태
        {
            monsterState = MonsterState.failed;
        }
        else if (monsterState == MonsterState.Move) // 차가 플레이어를 추격하는 상태,
        {
            Move();
        }
        else if (monsterState == MonsterState.Dead) // 차가 완전한 죽음
        {
            EnemyDead();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // 차의 시야에 닿으면 발동되는 콜라이더
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

    protected override void OnCollisionEnter2D(Collision2D collision) // 차에 닿으면 발동되는 콜라이더
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player")&&!isMove) // 플레이어에 닿으면 통과되는 스크립트
        {
            capsule.isTrigger = true;
            StartCoroutine(TriggerFalse());
            isMove = false;
        }
        else if(collision.gameObject.CompareTag("Wall")) // 벽에 닿으면 흡수 가능한 상태가 되는 콜라이더
        {
            monsterState = MonsterState.idle;
            isMove = false;
            animWalk = 0;
            animDead = 100;
        }
    }

    private IEnumerator TriggerFalse() // 플레이어에 닿은후 is Trigger가 꺼지게 만드는 스크립트
    {
        yield return new WaitForSecondsRealtime(1.0f);

        capsule.isTrigger = false;
        isMove = true;
    }

    protected override IEnumerator HitRecover() // 피격시 나오는 스크립트
    {
        yield return new WaitForSeconds(0.5f); // 0.5초간 피격 상태 유지
        monsterState = MonsterState.Move;
    }

    public void animDie() // 흡수 가능한 상태로 애니메이션을 바꾸는 스크립트
    {
        EnemyFailed = true;
        animDead = 150;
    }

}

