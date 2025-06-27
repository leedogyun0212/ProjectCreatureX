using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField] private float StartSpeed = 500.0f;

    [SerializeField] private float MaxSpeed = 600.0f;

    public float MySpeed { get; set; } = 0.0f;

    public Vector2 lookdirection { get; set; } = Vector2.zero;

    private void Update()
    {
        Debug.Log(lookdirection + "look");
        Movement();
        if (!Mathf.Approximately(MySpeed, MaxSpeed))
            MySpeed = Mathf.Lerp(MySpeed, MaxSpeed, 0.2f * Time.deltaTime);
    }

    public void Movement()
    {
        transform.position += (Vector3)lookdirection * MySpeed * Time.deltaTime;
        Debug.Log(lookdirection+"look");
    }

    public void Initialize(Vector2 direction)
    {
        MySpeed = StartSpeed;
        lookdirection = direction;
    }
}
