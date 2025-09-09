using System.Collections;
using UnityEngine;

public class Object_Trab : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D box;

    [SerializeField] private AudioSource audioSource;

    public bool isTrab = false;

    public Vector3 PlayerPos;

    private TestPlayer player = null;

    [SerializeField]
    private int Damage = 5;

    // 정적 변수로 전역 무적 시간 관리
    private static float lastHitTime = 0f;
    private static float invulnerabilityDuration = 2.5f; // 무적 시간 (초)

    public void SetTrab()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            player = PlayerObj.GetComponent<TestPlayer>();
    }

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
        if (collision.CompareTag("Player"))
        {
            // 무적 시간 체크
            if (IsPlayerInvulnerable())
            {
                Debug.Log("무적 시간 중 - 데미지 무시");
                return;
            }

            // 데미지 적용 및 무적 시간 시작
            audioSource.Play();
            player.TakeHit(Damage);
            Debug.Log(player.playerCurHP + "트랩");
            lastHitTime = Time.time; // 마지막 히트 시간 기록
            StartCoroutine(Trab());
        }
    }

    // 플레이어가 무적 시간 중인지 체크
    private bool IsPlayerInvulnerable()
    {
        return Time.time - lastHitTime < invulnerabilityDuration;
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