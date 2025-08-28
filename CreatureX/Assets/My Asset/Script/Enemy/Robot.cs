using System.Collections;
using Unity.Burst.CompilerServices;
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

    private Transform currentMovingStool = null; // [변경] 이동식 발판 추적용 참조(부모 미사용)
    private Vector3 lastMovingStoolPos = Vector3.zero; // [변경] 발판 이동량 계산용

    protected override void Start()
    {
        base.Start();
        BulletPool = new ObjectPool<RobotBullet>();
    }

    protected override void Update()
    {
        animEnemy(); // �ִϸ��̼�
        if (EnemyFailed) // ���� ������ ����,
        {
            animDead = 100;
            monsterState = MonsterState.failed;
            StartCoroutine(DeadOn());
        }
        else if (monsterState == MonsterState.Move && helpTarget == null) // �þ� ����� �÷��̾� �߰� 
        {
            Getdirection();
            if(Mathf.Abs(player.transform.position.x - transform.position.x)>15)
                MoveSpeed +=6;
            EffectManager.Play("E_Dirt", this.gameObject, transform.localScale.x != -1, new Vector3(0.7f, -0.83f));
            Move();
        }
        else if (monsterState == MonsterState.Play && isAttacking && !isCoolingDown) // �þ� ������ ����
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
        else if (monsterState == MonsterState.Help&& helpTarget != null) // �������� ���� ��û�ϸ� ������������ �̵�
        {
            Getdirection();
            EffectManager.Play("E_Dirt", this.gameObject, transform.localScale.x != -1, new Vector3(0.7f, -0.83f));
            Move();

            if (Vector2.Distance(transform.position, helpTarget.position) < 0.3f)
            {
                monsterState = MonsterState.idle;
                helpTarget = null;
                animWalk = 0;
            }
        }
        else if (monsterState == MonsterState.Dead || Input.GetKey(KeyCode.A)) // ������ ����
            EnemyDead();
    }



    public void OnHelpRequested(Transform targetResearcher) // �������� �߰��ϰ� �ϴ� ��ũ��Ʈ
    {
        helpTarget = targetResearcher;
        monsterState = MonsterState.Help;
    }

    protected override void Getdirection() // �̵��ҷ��� �þ� ����
    {
        if (helpTarget != null)
        {
            direction = (helpTarget.position - transform.position).normalized;
            animWalk = 1;
        }
        else
            base.Getdirection();
    }

    public void Attack() // �Ѿ� ����, �Ѿ� ���� ����
    {
        RobotBullet bullet = BulletPool.GetRecyclableObject() ??
            BulletPool.RegisterRecyclableObject(Instantiate(BulletPrefab));

        if (bullet.gameObject.activeSelf)
            bullet.isActive = true;
        else
            bullet.gameObject.SetActive(bullet.isActive = true);
        //Test();
        bullet.SetBullet();

        // �߻� ��ġ
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

    private IEnumerator AttackCooldown() // ���� ��ٿ�
    {
        isAttacking = false;
        isCoolingDown = true;

        yield return new WaitForSeconds(0.5f); // �߻� ������ 0.5�ʷ� ����

        isAttacking = true;
        isCoolingDown = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) //�þ� �ݶ��̴�, �þ߿� ������ ����
    {
        if (collision.gameObject.CompareTag("Player") && player.isAttack&& !HitOn)
        {
            monsterState = MonsterState.Hit;
            animHit = true;
            animWalk = 0;
            HitOn = true;
            HpCheck();
            HitMove();
            StartCoroutine(HitRecover());
        }
        if (collision.CompareTag("Player")&& !player.isAttack&&!EnemyFailed)
        {
            helpTarget = null;

            monsterState = MonsterState.Play;
            if(MoveSpeed>5)
                MoveSpeed -= 6;
            animWalk = 2;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) // �þ� �ݶ��̴�, ����� �߰�
    {
        if (collision.CompareTag("Player") && !EnemyFailed)
        {
            monsterState = MonsterState.Move;
            animWalk = 1;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) // �κ� ���� ������ �߻��ϴ� �ݶ��̴�, ������ ������ ���� ���� 
    {
        //base.OnCollisionEnter2D(collision);
        if(collision.gameObject.CompareTag("Researcher") && !EnemyFailed)
        {
            monsterState = MonsterState.idle;
            helpTarget = null;
            animWalk = 0;
        }

        // [변경] MovingStool 위에 있을 때 발판 이동량을 적용하기 위한 참조 설정
        var movingStool = collision.gameObject.GetComponent<MovingStool>();
        if (movingStool != null)
        {
            bool onTop = false;
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    onTop = true;
                    break;
                }
            }

            if (onTop)
            {
                currentMovingStool = movingStool.transform;
                lastMovingStoolPos = currentMovingStool.position;
            }
        }
    }

    // [변경] MovingStool에서 떨어지면 참조 해제
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (currentMovingStool != null && collision.transform == currentMovingStool)
        {
            currentMovingStool = null;
            lastMovingStoolPos = Vector3.zero;
        }
    }

    protected override IEnumerator HitRecover()
    {
        EffectManager.Play("E_Hit", this.gameObject, transform.localScale.x != -1, new Vector3(1.03f, 0.78f));
        yield return new WaitForSeconds(0.5f); // 0.5초간 피격 상태 유지
        HitOn = false;
        monsterState = MonsterState.idle;
    }

    protected override IEnumerator DeadOn()
    {
        EffectManager.Play("E_Asl", this.gameObject, transform.localScale.x != -1, new Vector3(0.2f, 0.5f));
        return base.DeadOn();
    }

    // [변경] 발판이 이동한 만큼 로봇도 이동시키기(부모 설정 없이 동기화)
    private void LateUpdate()
    {
        if (currentMovingStool == null)
            return;

        // 발판이 비활성화되었으면 안전하게 참조 해제
        if (!currentMovingStool.gameObject.activeInHierarchy)
        {
            currentMovingStool = null;
            lastMovingStoolPos = Vector3.zero;
            return;
        }

        Vector3 stoolDelta = currentMovingStool.position - lastMovingStoolPos;
        if (stoolDelta.sqrMagnitude > 0f)
        {
            transform.position += stoolDelta;
        }
        lastMovingStoolPos = currentMovingStool.position;
    }
}
