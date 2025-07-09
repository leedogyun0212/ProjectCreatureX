using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D CamCollider;

    public BoxCollider2D PlayerCollider;

    [SerializeField]
    private float CameraSpeed = 4.0f;

    public bool MoveStop = false;
    private Vector3 SavePos;

    private void Update()
    {
        if (!MoveStop)
        {
            MoveCam();
        }
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
        else if (PlayerBottomY < CamBottomY-0.25f)
        {
            return PlayerY;
        }

        return CamY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Door"))
        {
            MoveStop = true;
            SavePos = CamCollider.bounds.max;
        }
    }

}
