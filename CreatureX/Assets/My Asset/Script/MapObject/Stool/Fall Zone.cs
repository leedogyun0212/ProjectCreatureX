using UnityEngine;

public class FallZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TestPlayer player = other.GetComponent<TestPlayer>();
        BaseEnemy baseEnemy = other.GetComponent<BaseEnemy>();
        if (player != null)
        {
            RuntimeGameManager.Instance.Check_FallDeath();
        }
        if (baseEnemy != null)
        {
            baseEnemy.FallDie();
        }

    }
}
