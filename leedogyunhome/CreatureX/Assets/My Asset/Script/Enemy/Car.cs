using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Car : BaseEnemy
{
    protected bool isMove = true; // 이동 관련 

    [SerializeField]
    private PolygonCollider2D capsule; // 차 겉의 콜라이더 플레이어 통과를 위함


    protected override void Update()
    {
        animEnemy();
        if (EnemyFailed) // 차의 흡수 가능한 상태
        {
            monsterState = MonsterState.failed;
            isMove = false;
            animWalk = 0;
            animDead = 100;
            StartCoroutine(DeadOn());
        }
        else if (monsterState == MonsterState.Move) // 차가 플레이어를 추격하는 상태,
        {
            Debug.Log("이동중");
            Move();
        }
        else if (monsterState == MonsterState.Dead) // 차가 완전한 죽음
        {
            EnemyDead();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // 차의 시야에 닿으면 발동되는 콜라이더
    {
        if (collision.gameObject.CompareTag("Player") && player.isAttack&&!HitOn)
        {
            monsterState = MonsterState.Hit;
            animHit = true;
            animWalk = 0;
            HitOn = true;
            HpCheck();
            HitMove();
            StartCoroutine(HitRecover());
        }
        if (collision.CompareTag("Player") && isMove)
        {
            Getdirection();
            MoveSpeed = 8.0f;
            isMove = false;
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) // 차에 닿으면 발동되는 콜라이더
    {
        //base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player") && !isMove && !player.isAttack) // 플레이어에 닿으면 통과되는 스크립트
        {
            capsule.isTrigger = true;
            if (capsule.isTrigger)
                StartCoroutine(TriggerFalse());
            isMove = false;
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Box")) // 벽에 닿으면 흡수 가능한 상태가 되는 콜라이더
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
        HitOn = false;
        monsterState = MonsterState.Move;
    }

    public void animDie() // 흡수 가능한 상태로 애니메이션을 바꾸는 스크립트
    {
        EnemyFailed = true;
        animDead = 150;
    }

    protected override IEnumerator DeadOn()
    {
        EffectManager.Play("E_Asl", this.gameObject, transform.localScale.x != -1, new Vector3(0.05f, 0.7f));
        return base.DeadOn();
    }

}

