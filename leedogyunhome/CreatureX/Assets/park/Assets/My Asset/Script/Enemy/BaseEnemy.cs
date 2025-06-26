using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    protected Animator animator;

    protected GameObject testplayer;

    protected new Rigidbody2D rigidbody;

    protected float MoveSpeed = 3.0f;

    protected float animWalk = 0.0f;

    protected float UAVPosY = 0;
    protected Vector3 UAVPos;

    [SerializeField]
    protected TestPlayer player;

    protected enum MonsterState
    {
        idle = 0,
        Play = 1,
        Move = 2,
        Hit = 3,
        Dead = 4
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
        //else if (monsterState == MonsterState.Hit)
        //{
        //    HitMove();
        //}
    }

    protected virtual void animEnemy()
    {
        animator.SetFloat("Walk", animWalk);
    }

    protected virtual void Move()
    {
        ScaleChange();
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
        Debug.Log("Move");
    }

    protected virtual void Getdirection()
    {
        direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        animWalk = 1.0f;
    }

    protected virtual void ScaleChange()
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !player.isAttack)
        {
            monsterState = MonsterState.Play;
            animWalk = 0;
            Debug.Log(monsterState);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        }
    }

    protected virtual void HitMove()
    {
        Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
        float knockbackPower = 30.0f;
        transform.position += (Vector3)(knockbackDir * knockbackPower * Time.deltaTime);
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player.isAttack)
        {
            monsterState = MonsterState.Hit;
            animWalk = 0;
            HitMove();
            Debug.Log("Hit!");
            StartCoroutine(HitRecover());
        }
    }

    protected virtual IEnumerator HitRecover()
    {
        yield return new WaitForSeconds(0.5f);  // 0.5초간 피격 상태 유지
        monsterState = MonsterState.idle;
    }
}
