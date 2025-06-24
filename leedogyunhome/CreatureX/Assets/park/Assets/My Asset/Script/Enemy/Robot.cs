using Unity.VisualScripting;
using UnityEngine;

public class Robot : BaseEnemy
{
    protected override void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.Z))
            HitMove();
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsterState = MonsterState.Move;
        }
    }
}
