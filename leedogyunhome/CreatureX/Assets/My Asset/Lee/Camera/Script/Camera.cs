using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour, IDoor
{
    [SerializeField]
    private BoxCollider2D CamCollider;

    public BoxCollider2D PlayerCollider;

    [SerializeField]
    private float CameraSpeed = 4.0f;

    private Vector3 SavePos;

    public bool DoorTouch { get; set; } = true;

    private void Update()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        if (!DoorTouch && PlayerCollider.bounds.center.x > transform.position.x)
        {
            return;
        }
        else
            MoveCam();
    }

    private void MoveCam()
    {
        Vector3 Player = new Vector3(PlayerCollider.bounds.center.x, GetVerticalFollowY(), transform.position.z);

        if (Mathf.Abs(PlayerCollider.bounds.center.x - transform.position.x) > 0.01)
            transform.position = Vector3.Lerp(transform.position, Player, CameraSpeed * Time.deltaTime);
    }

    private float GetVerticalFollowY()
    {
        float PlayerY = PlayerCollider.bounds.center.y;
        float CamY = transform.position.y;
        float PlayerBottomY = PlayerCollider.bounds.min.y;
        float CamBottomY = CamCollider.bounds.min.y;

        if (PlayerY > CamY + 0.25f)
            return PlayerY;
        else if (PlayerBottomY < CamBottomY-0.3f)
        {
            return PlayerY;
        }

        return CamY;
    }

    public void DoorCam()
    {
        StartCoroutine(SlowReturnToPlayer());
    }

    private IEnumerator SlowReturnToPlayer()
    {
        float elapsedTime = 0f;
        float duration = 1.5f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(PlayerCollider.bounds.center.x, GetVerticalFollowY(), transform.position.z);

        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1.5f, t); // ¿©±â!

            transform.position = Vector3.Lerp(start, end, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Door"))
        {
            DoorTouch = false;
            SavePos = CamCollider.bounds.max;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            Debug.Log(PlayerCollider.bounds.min.y + "sdas");
            Debug.Log(CamCollider.bounds.min.y - 0.3f);
            DoorTouch = true;
        }
    }

}
