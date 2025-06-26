using System.Collections;
using UnityEngine;

public class UAV : BaseEnemy
{
    protected bool isMove = true;

    protected bool isReturning = false;

    [SerializeField]
    private CapsuleCollider2D capsule;

    private float SavePosX;

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
    }

    private void StateMove()// 포착시 드론이 가져갈 움직임
    {

        if (!isReturning)
            Getdirection();
        else if (isReturning)
            FlyUAV();

        Move();
    }

    protected override void Getdirection()//드론이 플레이어 공격할때 드론이 움직일 방향
    {
        direction = (player.transform.position - transform.position).normalized;
        animWalk = 1;
    }

    private void FlyUAV() // 플레이어 공격후 드론이 잠시 위로 올라 가고 다시 내려올때 쓰는코드
    {
        if (Mathf.Abs(transform.position.y - player.transform.position.y) < 4.0f)
            direction = (UAVPos - transform.position).normalized;
        else if (Mathf.Abs(transform.position.y - player.transform.position.y) > 4.0f)
        {
            isReturning = false;
            Debug.Log("작동");
            AttUAV();
            capsule.isTrigger = false;
            Getdirection();
        }

    }

    private void AttUAV()
    {
        if(capsule.transform.position.x-player.transform.position.x>0)
        {
            SavePosX = Mathf.Abs(player.transform.position.x - capsule.bounds.center.x);
            UAVPos = new Vector3(capsule.bounds.center.x - SavePosX * 2, capsule.bounds.center.y, 0);
        }
        else if(capsule.transform.position.x - player.transform.position.x < 0)
        {
            SavePosX = Mathf.Abs(player.transform.position.x - capsule.bounds.center.x);
            UAVPos = new Vector3(capsule.bounds.center.x + SavePosX * 2, capsule.bounds.center.y, 0);
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
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xOffset < -3.9f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    protected override void Move() // 드론이 포착한후 움직이게 만드는 코드
    {
        ScaleChange();
        if (Mathf.Abs(transform.position.y - player.transform.position.y) > 4.0f)
            transform.Translate(Vector2.down * MoveSpeed * Time.deltaTime);
        else if (Mathf.Abs(transform.position.y - player.transform.position.y) < 4.0f)
            transform.Translate(direction * MoveSpeed * Time.deltaTime);

    }

    protected override void OnTriggerEnter2D(Collider2D collision) // isTrigger 켜진 콜라이더 감지, 드론의 플레이어 포착하는 코드
    {
        if (collision.CompareTag("Player") && isMove&& !isReturning)
        {
            isMove = false;
            Getdirection();
            MoveSpeed = 8.0f;
            SavePosX = Mathf.Abs(player.transform.position.x - capsule.bounds.center.x);
            UAVPos = new Vector3(capsule.bounds.center.x - SavePosX * 2, capsule.bounds.center.y, 0);
            Debug.Log(UAVPos+"UAVPos");
            monsterState = MonsterState.Move;
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
            FlyUAV();
            MoveSpeed = 8.0f;
        }
    }

    private IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isReturning = true;
        FlyUAV();
    }

    protected override IEnumerator HitRecover()
    {
        yield return new WaitForSeconds(0.5f);  // 0.5초간 피격 상태 유지
        monsterState = MonsterState.Move;
    }
}
