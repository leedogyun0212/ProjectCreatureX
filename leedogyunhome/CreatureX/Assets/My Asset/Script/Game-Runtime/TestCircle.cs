using UnityEngine;

public class TestCircle : MonoBehaviour
{
    public float lifeTime = 1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Researcher") ||
        other.CompareTag("Door") || other.CompareTag("Wall") ||
        other.CompareTag("Box"))
        {
            var enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.GetHitByPlayer();
            }
            Destroy(gameObject);
        }

    }
}
