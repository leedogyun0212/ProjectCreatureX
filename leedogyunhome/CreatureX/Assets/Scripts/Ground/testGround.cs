using UnityEngine;

public class testGround : MonoBehaviour
{
    private int GroundCheck = 2;

    public bool Check = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Check) return;
            GroundCheck--;
            Debug.Log(GroundCheck,this.gameObject);
        }
    }
}
