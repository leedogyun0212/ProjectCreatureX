using System.Collections;
using UnityEngine;

public class Object_Trab : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D box;

    [SerializeField] private AudioSource audioSource;

    public bool isTrab = false;

    public Vector3 Player;

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
            audioSource.Play();
            StartCoroutine(Trab());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trigger"))
        {
            StartCoroutine(Trab());
            audioSource.Play();
        }
    }
}
