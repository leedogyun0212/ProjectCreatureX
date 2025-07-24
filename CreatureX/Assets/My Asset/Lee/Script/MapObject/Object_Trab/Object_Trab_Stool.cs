using System.Collections;
using UnityEngine;

public class Object_Trab_Stool : MonoBehaviour
{
    public Object_Trab TrabPrefab;
    private Object_Trab currentTrab;

    private Vector3 TrabPos;

    private bool Create = true;

    private void Start()
    {
        TrabPos = transform.position;
        TrabPos.y += 10;
    }

    private void TrabCreate()
    {
        currentTrab = Instantiate(TrabPrefab, TrabPos, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")&&Create)
        {
            Create = false;
            StartCoroutine(Warning());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Create)
        {
            Create = false;
            StartCoroutine(Warning());
        }
    }

    private IEnumerator Warning()
    {
        Debug.Log("Æ®·¦ ¹ßµ¿!!!!!!!!");

        yield return new WaitForSecondsRealtime(1.0f);

        TrabCreate();
    }
}
