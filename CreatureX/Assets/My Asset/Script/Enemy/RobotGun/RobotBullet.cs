using System.Collections;
using UnityEngine;

public class RobotBullet : MonoBehaviour, IRecyclableGameObject
{
    public bool isActive { get; set; }

    public BulletMovement BulletMove { get; private set; }

    private GameObject Robot;

    public Vector2 dir = Vector2.zero;

    public bool isAttacking { get; set; } = false;

    private IEnumerator CheckBullet()
    {
        yield return new WaitUntil(() => 
        Robot != null && Vector3.Distance(Robot.transform.position, transform.position) >= 12.0f);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //MoveBullet();

    }

    private void MoveBullet()
    {
        transform.Translate(dir * 30 * Time.deltaTime);
    }

    private void OnDisable()
    {
        isActive = false;
    }

    public void SetBullet()
    {
        BulletMove = BulletMove ?? GetComponent<BulletMovement>();
        Robot = Robot??GameObject.Find("Robot");
        //dir = direction;

        StopAllCoroutines(); // 혹시 남아있는 코루틴 방지
        StartCoroutine(CheckBullet());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
