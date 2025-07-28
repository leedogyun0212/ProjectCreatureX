using UnityEngine;

public class CameraChange : MonoBehaviour
{
    private Camera camera;

    private BoxCollider2D Box;

    private void Start()
    {
        Box = GetComponent<BoxCollider2D>();
    }

    // Initialize로 카메라 참조를 받음
    public void Initialize(Camera Maincamera)
    {
        camera = Maincamera;
    }

    private void Update()
    {
        if (camera == null) return;
    }
}
