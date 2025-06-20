using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    private void Update()
    {
        MovingCamera();
    }

    private void MovingCamera()
    {
        Vector3 Pl = Player.transform.position;
        Pl.y = transform.position.y;
        Pl.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, Pl, 3.0f * Time.deltaTime);

        //if (transform.position.x - Player.transform.position.x < 0)
        //    transform.position = Vector3.Lerp(transform.position, Pl, 3.0f * Time.deltaTime);
    }

}
