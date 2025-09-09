using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool isLaser = false;

    public bool LaserDead = false;

    private float LaserTime = 2.5f;

    private Animator animator;

    private TestPlayer player = null;

    [SerializeField]
    private int Damage = 5;

    public void SetLaser()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            player = PlayerObj.GetComponent<TestPlayer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        isLaser = true;
        LaserDead = false;
        StartCoroutine(LaserOn());
    }

    private void OnDisable()
    {
        LaserDead = true;
    }

    private IEnumerator LaserOn()
    {

        yield return new WaitForSecondsRealtime(LaserTime);


        LaserDead = true;
        animator.SetBool("LaserDead", LaserDead);
    }

    public void LaserEnd()
    {
        isLaser = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeHit(Damage);
            Debug.Log("공격 성공");
        }
    }
}
