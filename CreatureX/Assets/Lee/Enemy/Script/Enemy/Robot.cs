using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Robot : BaseEnemy, IAllyResponder
{
    [SerializeField]
    private RobotBullet BulletPrefab;

    [SerializeField] 
    private Transform BulletShotPoint = null;

    private ObjectPool<RobotBullet> BulletPool = null;

    private Vector2 lookdir = Vector2.zero;

    public bool isAttacking { get; set; } = true;

    private bool isCoolingDown = false;

    protected override void Start()
    {
        base.Start();
        BulletPool = new ObjectPool<RobotBullet>();
    }

    protected override void Update()
    {
        animEnemy(); // 애니메이션
        if (EnemyFailed) // 흡수 가능한 상태,
        {
            animDead = 100;
            monsterState = MonsterState.failed;

        }
        else if (monsterState == MonsterState.Move && helpTarget == null) // 시야 벗어나면 플레이어 추격 
        {
            Getdirection();
            if(Mathf.Abs(player.transform.position.x - transform.position.x)>15)
                MoveSpeed +=6;
            Move();
        }
        else if (monsterState == MonsterState.Play && isAttacking && !isCoolingDown) // 시야 들어오면 공격
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
        else if (monsterState == MonsterState.Help&& helpTarget != null) // 연구원이 도움 요청하면 연구원쪽으로 이동
        {
            Getdirection();
            Move();

            if (Vector2.Distance(transform.position, helpTarget.position) < 0.3f)
            {
                monsterState = MonsterState.idle;
                helpTarget = null;
                animWalk = 0;
            }
        }
        else if (monsterState == MonsterState.Dead || Input.GetKey(KeyCode.A)) // 완전한 죽음
            EnemyDead();
    }



    public void OnHelpRequested(Transform targetResearcher) // 연구원을 추격하게 하는 스크립트
    {
        helpTarget = targetResearcher;
        monsterState = MonsterState.Help;
    }

    protected override void Getdirection() // 이동할려는 시야 설정
    {
        if (helpTarget != null)
        {
            direction = (helpTarget.position - transform.position).normalized;
            animWalk = 1;
        }
        else
            base.Getdirection();
    }

    public void Attack() // 총알 생성, 총알 방향 설정
    {
        RobotBullet bullet = BulletPool.GetRecyclableObject() ??
            BulletPool.RegisterRecyclableObject(Instantiate(BulletPrefab));

        if (bullet.gameObject.activeSelf)
            bullet.isActive = true;
        else
            bullet.gameObject.SetActive(bullet.isActive = true);
        //Test();
        bullet.SetBullet();

        // 발사 위치
        lookdir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        bullet.transform.position = BulletShotPoint.position;

        bullet.BulletMove.Initialize(lookdir);
    }
    
    private void Test()
    {
        if(transform.localScale.x == 1)
        {
            lookdir = Vector2.right;
        }
        else if (transform.localScale.x == -1)
        {
            lookdir = Vector2.left;
        }
    }

    private IEnumerator AttackCooldown() // 공격 쿨다운
    {
        isAttacking = false;
        isCoolingDown = true;

        yield return new WaitForSeconds(0.5f); // 발사 간격을 0.5초로 조절

        isAttacking = true;
        isCoolingDown = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) //시야 콜라이더, 시야에 들어오면 공격
    {
        if (collision.CompareTag("Player") && !player.isAttack&&!EnemyFailed)
        {
            helpTarget = null;

            monsterState = MonsterState.Play;
            if(MoveSpeed>5)
                MoveSpeed -= 6;
            animWalk = 2;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) // 시야 콜라이더, 벗어나면 추격
    {
        if (collision.CompareTag("Player") && !EnemyFailed)
        {
            monsterState = MonsterState.Move;
            animWalk = 1;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) // 로봇 몸에 닿으면 발생하는 콜라이더, 연구원 닿으면 대기로 돌입 
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.CompareTag("Researcher") && !EnemyFailed)
        {
            monsterState = MonsterState.idle;
            helpTarget = null;
            animWalk = 0;
        }
    }
}
