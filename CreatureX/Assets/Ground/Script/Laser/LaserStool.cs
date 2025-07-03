using System.Collections;
using UnityEngine;

public class LaserStool : MonoBehaviour
{
    public Laser LaserPrefab;
    private Laser currentLaser;

    private Vector3 LaserPos;

    private bool isCreate = true;
    private bool isRestarting = false;

    private void Start()
    {
        LaserPos = transform.position;
        LaserPos.y += 0.6f;
    }

    private void Update()
    {
        //if (currentLaser != null && currentLaser.LaserDead && !isRestarting)
        //{
        //    StartCoroutine(LaserReStart());
        //}
    }

    private void Lasercreate()
    {
        currentLaser = Instantiate(LaserPrefab, LaserPos, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")&&isCreate)
        {
            isCreate = false;
            StartCoroutine(CreateWarning());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCreate)
        {
            StartCoroutine(LaserReStart());
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
        isRestarting = true;
        yield return new WaitForSecondsRealtime(2.0f); // 꺼진 후 2초 대기
        currentLaser.gameObject.SetActive(true);
        isRestarting = false;
    }

}
