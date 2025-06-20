using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    protected Animator animator;

    protected GameObject player;

    protected new Rigidbody2D rigidbody;

    protected float MoveSpeed = 3.0f;

    [SerializeField]
    protected GameObject see;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Test Player");
    }

    protected void Move()
    {
        Vector2 Pl = player.transform.position;
        Pl.y = transform.position.y;

        transform.position = Vector2.Lerp(transform.position, Pl, MoveSpeed * Time.deltaTime);
    }
}
