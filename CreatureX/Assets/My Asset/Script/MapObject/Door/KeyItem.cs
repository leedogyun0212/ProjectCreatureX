using System.Collections;
using UnityEngine;

public class KeyItem : MonoBehaviour, IDoorKey
{
    [SerializeField] private KeyData keyDataAsset;
    public KeyData KeyData { get => keyDataAsset; set => keyDataAsset = value; }

    private GameObject Player;

    private Vector3 KeyPos;

    [SerializeField]
    private KeyDoor doorsToUnlock;

    private BoxCollider2D Keycollider;


    private void Start()
    {
        Player = GameObject.Find("Test Player");
        Keycollider = GetComponent<BoxCollider2D>();
        keyDataAsset.Key = false;
    }

    private void Update()
    {
        EffectManager.Play("Key_Effect", this.gameObject, false, new Vector3(-0.18f, 0.05f, 0f), 1.0f);
        if (keyDataAsset.Key)
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
            Debug.Log("키 아이템을 획득!!!!!!!!!!!!!!!");
            keyDataAsset.Key = true;

            //doorsToUnlock.Key = true;

            Keycollider.isTrigger = true;
        }
    }

    private IEnumerator KeyEnd()
    {
        yield return new WaitUntil(() => Mathf.Abs(transform.position.y - KeyPos.y) < 0.01);
        Destroy(gameObject);
    }
}
