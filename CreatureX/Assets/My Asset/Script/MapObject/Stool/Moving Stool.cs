using System.Collections.Generic;
using UnityEngine;

public class MovingStool : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private float speed = 2f;
    private Vector3 targetPos;

    private Vector3 lastPos;
    private Vector3 deltaPos;
    private HashSet<Transform> passengers = new HashSet<Transform>();

    private AudioSource audioSource = null;

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + offset;
        targetPos = endPos;
    }

    // Update is called once per frame
    void Update()
    {
        MovePassengers();
        GroundMove();
    }

    private void GroundMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            // 반대로 전환
            targetPos = (targetPos == endPos) ? startPos : endPos;
        }

        deltaPos = transform.position - lastPos;
        lastPos = transform.position;
    }

    private void MovePassengers()
    {
        foreach (Transform passenger in passengers)
        {
            passenger.position += deltaPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TestPlayer player = collision.gameObject.GetComponent<TestPlayer>();

            if (player != null && !player.isTransDrone)  // 드론폼이 아닐 때만 추가
            {
                passengers.Add(collision.transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            passengers.Remove(collision.transform);
        }
    }

}
