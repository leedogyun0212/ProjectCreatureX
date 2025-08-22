using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Researcher : BaseEnemy
{

    public float helpRange = 10f;

    private bool isHelp = true;

    [SerializeField]
    private CapsuleCollider2D capsule;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        animEnemy();
        if(Mathf.Abs(player.transform.position.x - transform.position.x) > 10)
        {
            animWalk = 0;
        }
        else if (monsterState == MonsterState.Move&& Mathf.Abs(player.transform.position.x - transform.position.x) < 10)
        {
            Getdirection();
            EffectManager.Play("Dust",gameObject, direction.x<0, new Vector3(-0.4f, -0.6f, 0f));
            Move();
        }
        else if (monsterState == MonsterState.Dead)
            EnemyDead();
        if (EnemyFailed)
        {
            capsule.size = new Vector2(capsule.size.x, 1.3f);
            animDead = 100;
            monsterState = MonsterState.failed;
            StartCoroutine(DeadOn());
        }
    }

    protected override void Getdirection() // 플레이어한테 벗어나는 시야를
    {
        direction = (transform.position - player.transform.position).normalized;
        direction.y = 0;
        animWalk = 1.0f;
    }

    public void RequestHelp()
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, helpRange, LayerMask.GetMask("Enemy"));
        foreach (var ally in allies)
        {
            IAllyResponder responder = ally.GetComponent<IAllyResponder>();
            if (responder != null)
            {
                responder.OnHelpRequested(this.transform);
            }
        }
    }

    public void AnimHelpStop()
    {
        animWalk = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
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
        if (collision.CompareTag("Player") && !player.isAttack&&isHelp && !EnemyFailed)
        {
            monsterState = MonsterState.Play;
            animWalk = 2;
            RequestHelp();
            isHelp = false;

        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //base.OnCollisionEnter2D(collision);
    }

    protected override IEnumerator HitRecover()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초간 피격 상태 유지
        HitOn = false;
        monsterState = MonsterState.Move;
    }

    protected override void ScaleChange() // 좌우반전
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
