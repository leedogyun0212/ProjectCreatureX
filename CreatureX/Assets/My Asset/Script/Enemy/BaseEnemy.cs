using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("몬스터 능력치")]
    [SerializeField] public int enemyID;

    protected Animator animator;

    protected new Rigidbody2D rigidbody;

    [SerializeField] // 이동속도
    protected float MoveSpeed = 3.0f;

    [SerializeField] // 체력
    protected float EnemyMaxHP = 40.0f;
    protected float EnemyCurHP;

    protected float UAVPosY = 0; // UAV 전용
    protected Vector3 UAVPos;

    protected TestPlayer player;  // 플레이어 상호작용을 위해
    protected Transform helpTarget = null; // Researcher 도움 요청 관련
    
    protected float animWalk = 0.0f; // 애니메이션 전용
    
    protected bool animHit = false; // 애니메이션 전용

    protected int animDead = 0; // 애니메이션 전용

    [SerializeField]
    protected bool EnemyFailed = false; // 체력이 일정이하로 떨어져서 흡수가능한 상태

    protected float FailTime = 5.0f; // 완전히 죽기전 흡수 가능상태 유지시간 

    protected bool HitOn = false; // 피격 상태중 데미지 안받게 하기 위함

    protected AudioSource audioSource = null;

    [SerializeField]
    protected int FailedLayer = 11;

    protected enum MonsterState
    {
        idle = 0, // 발견전에 움직이는 코드
        Play = 1, // 발견후의 공격이나 무빙이외의 코드(추가 예정)
        Move = 2, // 움직이는 코드
        Hit = 3, // 맞았을 시 코드
        Dead = 4, // 죽었을 시 코드
        Help = 5,// Researcher의 도움요청 코드
        failed = 6, // 흡수 가능한 상태
        fall = 7 // 밑으로 떨어졌을때
    }

    protected MonsterState monsterState = MonsterState.idle;

    protected Vector3 direction;

    protected virtual void Start()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            player = PlayerObj.GetComponent<TestPlayer>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        UAVPos = transform.position;

        // EnemyCurHP = EnemyMaxHP;
        EnemyCurHP = EnemyMaxHP * 0.5f;
    }

    protected virtual void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.Move)
        {
            Getdirection();
            Move();
        }
        else if (monsterState == MonsterState.Dead||Input.GetKey(KeyCode.A))
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

    protected virtual void EnemyDead()  // 완전한 죽음
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) //isTrigger 켜진 콜라이더 감지, Enemy의 플레이어 포착하는 코드
    {
        if (collision.CompareTag("Player") && !player.isAttack)
        {
            monsterState = MonsterState.Play;
            animWalk = 0;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) //isTrigger 꺼진 콜라이더 감지, 플레이어가 Enemy에게 닿았을시 나오는 코드
    {
        // 플레이어 공격 피격은 아래로 옮김
    }

    public virtual void GetHitByPlayer()
    {
        if (player.isAttack && !HitOn)
        {
            monsterState = MonsterState.Hit;
            animHit = true;
            animWalk = 0;
            HitOn = true;
            HpCheck();
            HitMove();
            StartCoroutine(HitRecover());
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




    protected virtual IEnumerator HitRecover() // 피격 코드
    {
        yield return new WaitForSeconds(0.5f); // 0.5초간 피격 상태 유지
        HitOn = false;
        monsterState = MonsterState.idle;
    }

    protected virtual void HpCheck()
    {
        Debug.Log("Enemy 다침ㅠㅠ" + this.gameObject);
        EnemyCurHP -= 10;
        if (EnemyCurHP <= 0) EnemyFailed = true;
    }

    protected virtual IEnumerator DeadOn()
    {
        yield return new WaitForSecondsRealtime(FailTime);
        EnemyFailed = false;
        monsterState = MonsterState.Dead;
    }

    public bool Able_Assimilated()  // 흡수 가능한 상태
    {
        return EnemyFailed && monsterState == MonsterState.failed;
    }

    protected void CheckAssimilated()
    {
        if (EnemyCurHP <= EnemyMaxHP * 0.2f)
        {
            monsterState = MonsterState.failed;
            EnemyFailed = true;
        }
    }

    public void FallDie()
    {
        monsterState = MonsterState.fall;
    }

}
