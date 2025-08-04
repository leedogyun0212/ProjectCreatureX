using UnityEngine;

public class TestCircle : MonoBehaviour
{
    public float lifeTime = 2.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
