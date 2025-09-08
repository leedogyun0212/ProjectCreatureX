using UnityEngine;

public class FallZone : MonoBehaviour
{
    private Vector3 FallVec;

    private void Start()
    {
        FallVec = transform.position;
        FallVec.y -= 5;
        transform.position = FallVec;
    }

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
