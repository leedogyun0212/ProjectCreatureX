using UnityEngine;

public class Strike : MonoBehaviour
{
    private TestPlayer player = null;

    [SerializeField]
    private int Damage = 5;

    private void Start()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            player = PlayerObj.GetComponent<TestPlayer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeHit(Damage);
        }
    }
}
