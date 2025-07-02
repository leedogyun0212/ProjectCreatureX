using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    protected Animator animator;

    protected GameObject testplayer;

    protected new Rigidbody2D rigidbody;

    protected float MoveSpeed = 3.0f;

    protected float UAVPosY = 0;
    protected Vector3 UAVPos;

    [SerializeField]
    protected TestPlayer player;

    protected Transform helpTarget = null;
    
    protected float animWalk = 0.0f;
    
    protected bool animHit = false;

    protected int animDead = 0;

    protected enum MonsterState
    {
        idle = 0, // 발견전에 움직이는 코드
        Play = 1, // 발견후의 공격이나 무빙이외의 코드(추가 예정)
        Move = 2, // 움직이는 코드
        Hit = 3, // 맞았을 시 코드
        Dead = 4, // 죽었을 시 코드
        Help = 5 // Researcher의 도움요청 코드

    }

    protected MonsterState monsterState = MonsterState.idle;

    protected Vector3 direction;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        testplayer = GameObject.Find("Test Player");
        UAVPos = transform.position;
    }

    protected virtual void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.Move)
        {
            Getdirection();
            Move();
        }
        else if (monsterState == MonsterState.Dead)
            EnemyDead();
    }

    protected virtual void animEnemy() // Enemy의 애니메이션 코드
    {
        animator.SetFloat("Walk", animWalk);
        if (!player.isAttack && animHit)
            animHit = false;
        animator.SetBool("Hit", animHit);
        animator.SetInteger("Dead", animDead);
    }

    protected virtual void Move() // 움직이는 코드
    {
        ScaleChange();
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    protected virtual void Getdirection() // 플레이어를 추적하기위한 방향 지정
    {
        direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        animWalk = 1.0f;
    }

    protected virtual void ScaleChange() // 좌우반전
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

    protected virtual void EnemyDead()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) //isTrigger 켜진 콜라이더 감지, Enemy의 플레이어 포착하는 코드
    {
        if(collision.CompareTag("Player") && !player.isAttack)
        {
            monsterState = MonsterState.Play;
            animWalk = 0;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision) //isTrigger 켜진 콜라이더 감지, Enemy의 플레이어가 시야에서 벗어날시 나오는 코드
    {
        if (collision.CompareTag("Player"))
        {
        }
    }

    protected virtual void HitMove() // 피격시 뒤로 밀리는 코드
    {
        Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
        float knockbackPower = 30.0f;
        transform.position += (Vector3)(knockbackDir * knockbackPower * Time.deltaTime);
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision) //isTrigger 꺼진 콜라이더 감지, 플레이어가 Enemy에게 닿았을시 나오는 코드
    {
        if (collision.gameObject.CompareTag("Player") && player.isAttack)
        {
            monsterState = MonsterState.Hit;
            animHit = true;
            animWalk = 0;
            HitMove();
            StartCoroutine(HitRecover());
        }
    }

    protected virtual IEnumerator HitRecover() // 피격 코드
    {
        yield return new WaitForSeconds(0.5f);  // 0.5초간 피격 상태 유지
        monsterState = MonsterState.idle;
    }
}
