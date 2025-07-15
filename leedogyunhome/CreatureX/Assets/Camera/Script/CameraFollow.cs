using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private BoxCollider2D CamCollider;
    public BoxCollider2D PlayerCollider;
    [SerializeField] private float CameraSpeed = 4.0f;

    public bool MoveStop = false;
    private bool wasMoveStopped = false;
    private Vector3 velocity = Vector3.zero; // 부드러운 움직임용

    private Vector3 targetPosition;

    private void Update()
    {
        if (!MoveStop)
        {
            if (wasMoveStopped)
            {
                wasMoveStopped = false;
                StartCoroutine(SlowReturnToPlayer());
            }
            else
            {
                FollowPlayerSmooth();
            }
        }
        else
        {
            wasMoveStopped = true;
        }
    }

    private void FollowPlayerSmooth()
    {
        // 플레이어 위치 기준으로 타겟 위치 설정 (z는 그대로 유지)
        Vector3 desiredPosition = new Vector3(
            PlayerCollider.bounds.center.x,
            PlayerCollider.bounds.center.y,
            transform.position.z
        );

        // 부드럽게 따라감
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.15f);
    }

    private IEnumerator SlowReturnToPlayer()
    {
        float elapsedTime = 0f;
        float duration = 1.0f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(
            PlayerCollider.bounds.center.x,
            PlayerCollider.bounds.center.y,
            transform.position.z
        );

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            MoveStop = true;
        }
    }
}
