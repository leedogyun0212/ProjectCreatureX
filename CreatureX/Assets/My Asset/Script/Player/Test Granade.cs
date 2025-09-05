using UnityEngine;

public class TestGranade : MonoBehaviour
{
    public float lifeTime = 1f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void InitializeDirection(float direction)
    {
        if (direction < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
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
