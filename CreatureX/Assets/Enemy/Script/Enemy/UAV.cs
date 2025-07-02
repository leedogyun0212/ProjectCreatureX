using System.Collections;
using UnityEngine;

public class UAV : BaseEnemy, IAllyResponder
{
    protected bool isMove = true;

    protected bool isReturning = false;

    [SerializeField]
    private BoxCollider2D capsule;

    private float SavePosX;

    [SerializeField]
    private LayerMask EnemyLayer;

    protected override void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.idle)
        {
            SimpleMove();
        }
        else if (monsterState == MonsterState.Move)
        {
            StateMove();
        }
        else if (monsterState == MonsterState.Help)
        {
            Getdirection();
            ResearcherMove();
        }
        else if (monsterState == MonsterState.Dead)
            EnemyDead();
    }

    private void StateMove()// 포착시 드론이 가져갈 움직임
    {

        if (!isReturning)
            Getdirection();
        else if (isReturning)
            FlyUAV();

        Move();
    }

    public void OnHelpRequested(Transform targetResearcher)
    {
        helpTarget = targetResearcher;
        monsterState = MonsterState.Help;
    }

    protected override void Getdirection()//드론이 플레이어 공격할때 드론이 움직일 방향
    {
        if (helpTarget != null)
        {
            direction = (helpTarget.position - transform.position).normalized;
        }
        else
            direction = (player.transform.position - transform.position).normalized;
        animWalk = 1;
    }

    private void FlyUAV() // 플레이어 공격후 드론이 잠시 위로 올라 가고 다시 내려올때 쓰는코드
    {
        if (Mathf.Abs(transform.position.y - player.transform.position.y) < 3.8f)
            direction = (UAVPos - transform.position).normalized;
        else if (Mathf.Abs(transform.position.y - player.transform.position.y) > 3.8f)
        {
            StartCoroutine(WaitUAV());
        }

    }

    private IEnumerator WaitUAV()
    {
        monsterState = MonsterState.Play;

        yield return new WaitForSecondsRealtime(2.0f);
        monsterState = MonsterState.Move;
        isReturning = false;
        capsule.isTrigger = false;
        Getdirection();
    }

    private void AttUAV() // 특정 지점 도착후 플레이어 공격후의 도착할 지점 지정하는 코드
    {
        if(capsule.transform.position.x-player.transform.position.x>0)
        {
            UAVPos = new Vector3(player.transform.position.x - 5.0f, player.transform.position.y+4.0f, 0);
        }
        else if(capsule.transform.position.x - player.transform.position.x < 0)
        {
            UAVPos = new Vector3(player.transform.position.x + 5.0f, player.transform.position.y + 4.0f, 0);
        }
    }

    protected void SimpleMove() // 포착 전에 드론이 움직이는 움직임 
    {
        float xOffset = Mathf.Sin(Time.time * 2f) * 18.0f;
        float yOffset = Mathf.Sin(Time.time * 2f) * 4.0f;
        Vector2 targetPos = new Vector2(UAVPos.x + xOffset, UAVPos.y);
        Vector2 newPos = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * MoveSpeed);
        rigidbody.MovePosition(newPos);
        if (xOffset > 3.9f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (xOffset < -3.9f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    protected override void Move() // 드론이 포착한후 움직이게 만드는 코드
    {
        ScaleChange();
        if (Mathf.Abs(transform.position.y - player.transform.position.y) > 3.8f)
        {
            transform.Translate(Vector2.down * MoveSpeed * Time.deltaTime);
            animWalk = 1;
        }
        else if (Mathf.Abs(transform.position.y - player.transform.position.y) < 3.8f)
        {
            transform.Translate(direction * MoveSpeed * Time.deltaTime);
            animWalk = 0;
        }

    }

    private void ResearcherMove()
    {
        ScaleChange();
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // isTrigger 켜진 콜라이더 감지, 드론의 플레이어 포착하는 코드
    {
        if (collision.CompareTag("Player") && isMove && !isReturning)
        {
            isMove = false;
            Getdirection();
            MoveSpeed = 8.0f;
            SavePosX = Mathf.Abs(player.transform.position.x - capsule.bounds.center.x);
            UAVPos = new Vector3(capsule.bounds.center.x - SavePosX * 2, capsule.bounds.center.y, 0);
            monsterState = MonsterState.Move;
        }
        else if (collision.gameObject.CompareTag("Researcher") && helpTarget != null)
        {
            monsterState = MonsterState.idle;
            helpTarget = null;
            UAVPos = transform.position;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) // 플레이어와 닿고 나갈때 드론의 isTrigger 끄는 코드
    {
        if (collision.CompareTag("Player"))
        {
            //capsule.isTrigger = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) // isTrigger 꺼진 드론 본체와 닿을때 쓰는 코드
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayerAttack());
            capsule.isTrigger = true;
            MoveSpeed = 8.0f;
        }
    }

    private IEnumerator PlayerAttack() // 플레이어에게 닿았을시 나오는 코드
    {
        isReturning = true;
        AttUAV();
        yield return new WaitForSeconds(0.5f);
        FlyUAV();
    }

    protected override IEnumerator HitRecover() // 피격시 발동하는 코드
    {
        yield return new WaitForSeconds(0.5f);  // 0.5초간 피격 상태 유지
        monsterState = MonsterState.Move;
    }
}
