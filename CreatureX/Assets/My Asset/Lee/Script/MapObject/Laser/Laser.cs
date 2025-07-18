using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool isLaser = false;

    public bool LaserDead = false;

    private float LaserTime = 3.0f;

    private float ColorA = 0f;

    private SpriteRenderer Lasersprite;

    private void Start()
    {
        Lasersprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Lasersprite.color = new Color(1f, 0, 0, ColorA);
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

        while (ColorA < 1f)
        {
            ColorA += Time.deltaTime * 2f;
            ColorA = Mathf.Clamp01(ColorA);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(LaserTime);

        while (ColorA > 0f)
        {
            ColorA -= Time.deltaTime * 2f;
            ColorA = Mathf.Clamp01(ColorA);
            yield return null;
        }

        isLaser = false;
        LaserDead = true;
        gameObject.SetActive(false);
    }
}
