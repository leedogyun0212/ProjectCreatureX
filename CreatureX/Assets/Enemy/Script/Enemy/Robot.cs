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
        animEnemy();
        if (monsterState == MonsterState.Move && helpTarget == null)
        {
            Getdirection();
            Move();
        }
        else if (monsterState == MonsterState.Play && isAttacking && !isCoolingDown)
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
        else if (monsterState == MonsterState.Help)
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
        else if (monsterState == MonsterState.Dead)
            EnemyDead();
    }

    public void OnHelpRequested(Transform targetResearcher)
    {
        helpTarget = targetResearcher;
        monsterState = MonsterState.Help;
    }

    protected override void Getdirection()
    {
        if (helpTarget != null)
        {
            direction = (helpTarget.position - transform.position).normalized;
            animWalk = 1;
        }
        else
            base.Getdirection();
    }

    public void Attack()
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
        Debug.Log(lookdir);
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

    private IEnumerator AttackCooldown()
    {
        isAttacking = false;
        isCoolingDown = true;

        yield return new WaitForSeconds(0.5f); // 발사 간격을 0.5초로 조절

        isAttacking = true;
        isCoolingDown = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !player.isAttack)
        {
            monsterState = MonsterState.Play;
            animWalk = 0;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsterState = MonsterState.Move;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.CompareTag("Researcher"))
        {
            monsterState = MonsterState.idle;
            helpTarget = null;
            animWalk = 0;
        }
    }
}
