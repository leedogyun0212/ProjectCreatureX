using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    protected Animator animator;

    protected float dir;

    protected new Rigidbody2D rigidbody;

    protected GameObject Player;

    [SerializeField]
    protected GameObject test;

    protected bool isMove = false;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Test Player");
        Debug.Log(Player);
    }

    protected void SeePlayer()
    {
        if (test.transform.position.x == Player.transform.position.x)
            isMove = true;
    }

    protected void Move()
    {
        Vector2 Pl = Player.transform.position;
        Pl.y = transform.position.y;
        transform.position = Vector2.Lerp(transform.position, Pl,3.0f*Time.deltaTime);
    }

}
