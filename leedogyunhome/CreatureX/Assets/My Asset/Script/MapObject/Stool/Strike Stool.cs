using System.Collections.Generic;
using UnityEngine;

public class StrikeStool : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private float speed = 2f;
    private Vector3 targetPos;

    private Vector3 lastPos;
    private Vector3 deltaPos;
    private HashSet<Transform> passengers = new HashSet<Transform>();

    [SerializeField]
    private int Damage = 5;

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
        if (!collision.gameObject.CompareTag("Player")) return;
        ContactPoint2D contact = collision.contacts[0];
        Vector2 normal = contact.normal;
        TestPlayer player = collision.gameObject.GetComponent<TestPlayer>();

        if (player != null && !player.isTransDrone) // 드론폼이 아닐 때만 추가
        {
            // 위에서 닿았는지 확인 (즉, 플레이어가 발판 위에 올라탐)
            if (Vector2.Dot(normal, Vector2.up) < 0.5f)
            {
                passengers.Add(collision.transform);
                Debug.Log($"{Time.deltaTime}: 플레이어가 위에서 발판에 탑승했습니다.");
            }
            else
            {
                // 아래에서 닿음
                Debug.LogWarning($"{Time.deltaTime}:플레이어가 위험한 방향(아래/옆)에서 충돌했습니다!");
                player.TakeHit(Damage);
                // TODO: PlayerHit() 또는 Player.TakeDamage() 호출
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
