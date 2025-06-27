using Unity.VisualScripting;
using UnityEngine;

public class Robot : BaseEnemy
{
    [SerializeField]
    private RobotBullet BulletPrefab;

    [SerializeField] 
    private Transform BulletShotPoint = null;

    private ObjectPool<RobotBullet> BulletPool = null;

    private Vector2 lookdir = Vector2.zero;

    protected override void Start()
    {
        base.Start();
        BulletPool = new ObjectPool<RobotBullet>();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.Z))
            Attack();
    }

    public void Attack()
    {
        RobotBullet bullet = BulletPool.GetRecyclableObject() ??
            BulletPool.RegisterRecyclableObject(Instantiate(BulletPrefab));

        if (bullet.gameObject.activeSelf)
            bullet.isActive = true;
        else
            bullet.gameObject.SetActive(bullet.isActive = true);

        bullet.SetBullet(lookdir);

        // 발사 위치
        //lookdir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        bullet.transform.position = BulletShotPoint.position;

        //bullet.BulletMove.Initialize(lookdir);
        Debug.Log(lookdir);
    }

    private void Test()
    {
        if(transform.localScale.x == 1)
        {

        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsterState = MonsterState.Move;
        }
    }
}
