using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool isLaser = false;

    public bool LaserDead = false;

    private float LaserTime = 3.0f;

    public SpriteRenderer Lasersprite;

    private void Update()
    {
        if (isLaser)
            StartCoroutine(LaserOn());
    }

    private IEnumerator LaserOn()
    {
        yield return new WaitForSecondsRealtime(LaserTime);
        isLaser = false;
        LaserDead = true;
        gameObject.SetActive(isLaser);
    }

    private void OnDisable()
    {
        LaserDead = true;
        //Destroy(gameObject);
    }

    private void OnEnable()
    {
        isLaser = true;
        LaserDead = false;  
    }

}
