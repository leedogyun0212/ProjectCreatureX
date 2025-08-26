using UnityEngine;

public class Robot : BaseEnemy
{
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsterState = MonsterState.Move;
        }
    }
}
