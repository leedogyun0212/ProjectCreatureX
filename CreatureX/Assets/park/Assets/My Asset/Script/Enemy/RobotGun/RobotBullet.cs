using System.Collections;
using UnityEngine;

public class RobotBullet : MonoBehaviour, IRecyclableGameObject
{
    public bool isActive { get; set; }

    public BulletMovement BulletMove { get; private set; }

    private GameObject Robot;

    public Vector2 dir = Vector2.zero;

    private IEnumerator CheckBullet()
    {
        yield return new WaitUntil(() => (
        Vector3.Distance(Robot.transform.position, transform.position) >= 12.0f));
    }

    private void Update()
    {
        StartCoroutine(CheckBullet());

        gameObject.SetActive(false);
    }

    private void MoveBullet()
    {
        transform.Translate(dir * 30 * Time.deltaTime);
    }

    private void OnDisable()
    {
        isActive = false;
    }

    public void SetBullet(Vector2 direction)
    {
        Debug.Log(Robot + "fsfds");
        BulletMove = BulletMove ?? GetComponent<BulletMovement>();
        Robot = GameObject.Find("Robot");
    }
}
