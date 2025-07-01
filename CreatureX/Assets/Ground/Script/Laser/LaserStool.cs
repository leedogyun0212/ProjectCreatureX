using System.Collections;
using UnityEngine;

public class LaserStool : MonoBehaviour
{
    public Laser LaserPrefab;

    private Vector3 LaserPos;

    private bool isCreate = true; 

    private void Start()
    {
        LaserPos = transform.position;
        LaserPos.y += 0.6f;
    }

    private void Update()
    {
        StartCoroutine(LaserReStart());
    }

    private void Lasercreate()
    {
        Instantiate(LaserPrefab, LaserPos, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")&&isCreate)
        {
            isCreate = false;
            StartCoroutine(CreateWarning());
        }
    }

    private IEnumerator CreateWarning()
    {
        Debug.Log("레이저 발사!!!!");

        yield return new WaitForSecondsRealtime(1.0f);

        Lasercreate();
    }

    private IEnumerator LaserReStart()
    {
        Debug.Log("통과 X"+ LaserPrefab.LaserDead);
        yield return new WaitUntil(() =>  LaserPrefab.LaserDead);
        Debug.Log("통과 1");
        yield return new WaitForSecondsRealtime(3.0f);
        Debug.Log("모두 통과");
        LaserPrefab.gameObject.SetActive(true);
    }

}
