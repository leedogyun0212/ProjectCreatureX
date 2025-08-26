using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    protected Animator animator;

    protected GameObject player;

    protected new Rigidbody2D rigidbody;

    protected float MoveSpeed = 3.0f;

    protected float animWalk = 0.0f;

    protected float UAVPosY = 0;

    protected enum MonsterState
    {
        idle = 0,
        Play = 1,
        Move = 2
    }

    protected MonsterState monsterState = MonsterState.idle;

    protected Vector3 direction;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Test Player");
        UAVPosY = transform.position.y;
    }

    protected virtual void Update()
    {
        animEnemy();
        if (monsterState == MonsterState.Move)
        {
            Getdirection();
            Move();
        }
    }

    protected virtual void animEnemy()
    {
        animator.SetFloat("Walk", animWalk);
    }

    protected virtual void Move()
    {
        ScaleChange();
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
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
        if(collision.CompareTag("Player"))
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
}
