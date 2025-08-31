using System.Collections;
using UnityEngine;

public class FakeStool : MonoBehaviour
{
    [SerializeField] private float shakeTime = 2.5f;
    [SerializeField] private float respawnTime = 3.5f;
    [SerializeField] private GameObject visual;
    [SerializeField] private Collider2D col;

    // [SerializeField] private GameObject breakParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip breakSFX;
    [SerializeField] private Animator animator;

    private Vector3 originalVisualPos;
    private float curShakeTime;
    private bool isBroking = false;
    private bool isBroken = false;
    private bool isPlayerOn = false;

    [SerializeField]
    private bool Respawn = true;

    private void Start()
    {
        originalVisualPos = visual.transform.localPosition;
    }

    private void Update()
    {
        CheckStoolState();
    }

    private void CheckStoolState()
    {
        if (isBroken || !isPlayerOn) return;
        curShakeTime -= Time.deltaTime;
        // 흔들리는 효과
        float shakeAmount = 0.03f;
        visual.transform.localPosition = originalVisualPos + (Vector3)UnityEngine.Random.insideUnitCircle * shakeAmount;

        animator.SetBool("PlayerOn", isPlayerOn);

        if (curShakeTime <= 0f)
        {
            BreakStool();
        }
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (isBroken || isBroking) return;
        // if (!collision.gameObject.CompareTag("Player")) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;
            TestPlayer player = collision.gameObject.GetComponent<TestPlayer>();

            if (player != null && !player.isTransDrone)  // 드론폼이 아닐 때만 추가
            {
                // 플레이어가 위에서 밟았을 때만 인정 (거의 수직으로)
                if (Vector2.Dot(normal, Vector2.up) < 0.9f && contact.point.y > transform.position.y)
                {
                    Debug.LogWarning($"{Time.deltaTime}: 플레이어가 발판에 올라감.");
                    isPlayerOn = true;
                    isBroking = true;
                    curShakeTime = shakeTime;
                }
            }
        }
    }

    private IEnumerator BreakEnd()
    {
        yield return new WaitForSecondsRealtime(0.8f);

        Debug.LogWarning($"{Time.deltaTime}: 발판 부서짐");
        visual.SetActive(false);
        col.enabled = false;

        Invoke(nameof(RespawnStool), respawnTime);
    }

    private void BreakStool()
    {
        if (breakSFX != null)
            audioSource.PlayOneShot(breakSFX);

        //if (breakParticle != null)
        //    Instantiate(breakParticle, transform.position, Quaternion.identity);
        isBroken = true;
        isPlayerOn = false;

        animator.SetBool("PlayerOn", isPlayerOn);

        StartCoroutine(BreakEnd());
    }

    private void RespawnStool()
    {
        if (Respawn)
        {
            Debug.LogWarning($"{Time.deltaTime}: 발판 복구됨");
            isBroken = false;
            isBroking = false;
            visual.SetActive(true);
            col.enabled = true;
            visual.transform.localPosition = originalVisualPos;
        }
    }
}
