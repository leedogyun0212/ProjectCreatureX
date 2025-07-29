using UnityEngine;

public class CameraSpawner : MonoBehaviour
{
    [SerializeField] private CameraChange cameraChangePrefab;
    [SerializeField] private Camera mainCamera; //  유니티에서 직접 넣거나 Camera.main으로 가져와도 됨

    public void SpawnEnemy(Vector3 position)
    {
        CameraChange cameraChangeObj = Instantiate(cameraChangePrefab, position, Quaternion.identity);
        CameraChange cameraChange = cameraChangeObj.GetComponent<CameraChange>();
        cameraChange.Initialize(mainCamera);  //  여기서 카메라 참조 넘겨줌
    }

    private void Start()
    {
        SpawnEnemy(transform.position);
    }
}
