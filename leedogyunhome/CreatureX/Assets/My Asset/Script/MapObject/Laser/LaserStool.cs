using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserStool : MonoBehaviour
{
    public Laser LaserPrefab;
    private Laser currentLaser;

    [SerializeField]
    private Transform LaserPos;

    private Vector3 Offset;

    [SerializeField]
    private float LaserFloat;

    [SerializeField]
    private float LaserNum; 

    private bool isCreate = true;
    private bool isRestarting = false;

    private List<Laser> lasers = new List<Laser>();

    private void Start()
    {
        Offset = Vector3.zero;
        //LaserPos.y += 0.6f;
    }

    private void Update()
    {
        if ( lasers.Count>0 && !isRestarting)
        {
            bool allDead = true;
            foreach (var laser in lasers)
            {
                if (laser != null && !laser.LaserDead)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                StartCoroutine(LaserReStart());
            }

        }
    }

    private void Lasercreate()
    {
        Offset = Vector3.zero;
        for (int i = 0; i < LaserNum; i++)
        {
            Laser newlaser = Instantiate(LaserPrefab, LaserPos.position + Offset, Quaternion.Euler(0,0,90));
            lasers.Add(newlaser);
            Offset.x += LaserFloat; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isCreate)
        {
            isCreate = false;
            StartCoroutine(CreateWarning());
        }
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && !isCreate)
    //    {
    //        StartCoroutine(LaserReStart());
    //    }
    //}

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
        foreach (var laser in lasers)
        {
            if (laser != null)
                laser.gameObject.SetActive(true);
        }
        isRestarting = false;
    }

}
