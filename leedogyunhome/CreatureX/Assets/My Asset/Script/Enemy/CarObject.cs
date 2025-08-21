using System.Collections;
using UnityEngine;

public class CarObject : BaseEnemy
{
    protected bool isMove = true;

    [SerializeField]
    private PolygonCollider2D capsule;

    private Vector3 startPosition; // [변경] 시작 위치 저장
    private int moveDirX = 1; // [변경] 전진 방향(+1: 오른쪽, -1: 왼쪽)


    protected override void Start()
    {
        base.Start();
        // 시작 프레임에 현재 위치를 저장
        startPosition = transform.position;

        // 초기 방향에 따라 전진 방향을 결정
        moveDirX = transform.localScale.x >= 0 ? 1 : -1;

        monsterState = MonsterState.Move;
        isMove = true;
        Getdirection();
    }

    protected override void Update()
    {
        animEnemy();
        if (EnemyFailed)  // 차의 흡수 가능한 상태
        {
            monsterState = MonsterState.failed;
            isMove = false;
            animWalk = 0;
            animDead = 100;
            Debug.Log(monsterState + "죽음");
            EnemyFailed = false;
            StartCoroutine(DeadOn());
        }
        else if (monsterState == MonsterState.Move) //항상 전진
        {
            Move();
        }
        else if (monsterState == MonsterState.Dead) // 차가 완전한 죽음
        {
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // 데미지를 입으면 발동됨
    {
        if (collision.gameObject.CompareTag("Player") && player.isAttack)
        {
            monsterState = MonsterState.Hit;
            animHit = true;
            animWalk = 0;
            HpCheck();
            HitMove();
            StartCoroutine(HitRecover());
        }
        //벽/박스 충돌 시 리스폰
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Box")|| collision.gameObject.CompareTag("Ground"))
        {
            monsterState = MonsterState.idle;
            isMove = false;
            animWalk = 0;
            animDead = 100;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)  // 차에 닿으면 발동되는 콜라이더
    {
        //base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player") && isMove && !player.isAttack)  // 플레이어에 닿으면 통과되는 스크립트
        {
            capsule.isTrigger = true;
            if (capsule.isTrigger)
                StartCoroutine(TriggerFalse());
        }
        //벽/박스 충돌 시 리스폰
        //else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Box"))
        //{
        //    monsterState = MonsterState.idle;
        //    isMove = false;
        //    animWalk = 0;
        //    animDead = 100;
        //}
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
        FailTime = 5.0f;
    }

    protected override IEnumerator DeadOn()
    {
        yield return new WaitForSecondsRealtime(FailTime);
        Debug.Log(isMove+"실행");
        // 시작 위치로 즉시 복귀합니다. (화면 밖으로 밀려나거나 멈추는 것을 방지) 멈추는 원인 가능성 높음
        transform.position = startPosition;
        if (rigidbody != null)
        {
            // 물리 속도를 초기화
            rigidbody.linearVelocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
        }

        // 전진 상태를 유지
        monsterState = MonsterState.Move;
        Getdirection();
        isMove = true;
        animWalk = 1;
        animDead = 0;
    }

    // [변경] 항상 전진하도록 방향 고정
    protected override void Getdirection()
    {
        // 이동 벡터를 설정
        direction = new Vector3(moveDirX, 0, 0);
        animWalk = 1.0f;
    }

}

