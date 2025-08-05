using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private TestPlayer player;
    public float directionOffset = 2f;

    public float smoothTime = 0.2f; 

    [SerializeField] private Vector3 adjustCamPos;

    private Vector2 minCamLimit;
    private Vector2 maxCamLimit;

    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;

    private Vector3 velocity = Vector3.zero; // SmoothDamp는 velocity 필요

    void Start()
    {
        if (LeftWall != null)
            minCamLimit = new Vector2(LeftWall.transform.position.x, minCamLimit.y);

        if (RightWall != null)
            maxCamLimit = new Vector2(RightWall.transform.position.x, maxCamLimit.y);
    }

    void Update()
    {
        FollowPlayer();
    }


    private void FollowPlayer() {
        if (target == null) return;

        float offsetX = directionOffset * Mathf.Sign(player.curDirect);
        if (player.curDirect == 0) offsetX = 0;

        Vector3 targetPos = target.position + new Vector3(offsetX, 0, 0);

        // SmoothDamp로 부드럽게 이동
        Vector3 pos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        transform.position = new Vector3(
            Mathf.Clamp(pos.x, minCamLimit.x, maxCamLimit.x) + adjustCamPos.x,
            Mathf.Clamp(pos.y, minCamLimit.y, maxCamLimit.y) + adjustCamPos.y,
            -10f + adjustCamPos.z);
    }
}
