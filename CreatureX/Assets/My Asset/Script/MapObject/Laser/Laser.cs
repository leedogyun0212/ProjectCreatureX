using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool isLaser = false;

    public bool LaserDead = false;

    private float LaserTime = 6.0f;

    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        isLaser = true;
        LaserDead = false;
        StartCoroutine(LaserOn());
    }

    private void OnDisable()
    {
        LaserDead = true;
    }

    private IEnumerator LaserOn()
    {

        yield return new WaitForSecondsRealtime(LaserTime);


        LaserDead = true;
        animator.SetBool("LaserDead", LaserDead);
    }

    public void LaserEnd()
    {
        isLaser = false;
        gameObject.SetActive(false);
    }

}
