using System.Collections;
using UnityEngine;

public class KeyItem : MonoBehaviour, IDoorKey
{
    public bool Key { get; set; }

    private GameObject Player;

    private Vector3 KeyPos;

    [SerializeField]
    private KeyDoor doorsToUnlock;

    private BoxCollider2D Keycollider;

    private void Start()
    {
        Player = GameObject.Find("Test Player");
        Keycollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Key)
            MoveKey();
    }

    private void MoveKey()
    {
        KeyPos = Player.transform.position;
        KeyPos.y += 2.0f;
        transform.position = Vector2.Lerp(transform.position, KeyPos, 4.0f * Time.deltaTime);
        StartCoroutine(KeyEnd());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Å° ¾ÆÀÌÅÛ È¹µæ!!!!!!!!!!!!!!!");
            Key = true;

            doorsToUnlock.Key = true;

            Keycollider.isTrigger = true;
        }
    }

    private IEnumerator KeyEnd()
    {
        yield return new WaitUntil(() => Mathf.Abs(transform.position.y - KeyPos.y) < 0.01);
        Destroy(gameObject);
    }
}
