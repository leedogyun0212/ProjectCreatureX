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
}
