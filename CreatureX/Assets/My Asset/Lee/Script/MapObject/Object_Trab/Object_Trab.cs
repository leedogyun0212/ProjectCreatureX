using System.Collections;
using UnityEngine;

public class Object_Trab : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D box;

    public bool isTrab = false;

    private void OnDisable()
    {
        isTrab = true;
    }

    private IEnumerator Trab()
    {
        yield return new WaitForSecondsRealtime(2f);
        isTrab = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("¿€µø??");
            StartCoroutine(Trab());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Trab());
        }
    }
}
