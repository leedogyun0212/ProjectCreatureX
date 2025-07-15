using UnityEngine;

public class FallenStool : MonoBehaviour
{
    [SerializeField] private float respawnTime = 3f;

    [SerializeField] private GameObject visual;
    [SerializeField] private Collider2D col;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip breakSFX;

    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Vector3 originalVisualPos;
    [SerializeField] private int cnt = 2;
    private int curCnt = 0;
    private bool isBroken = false;

    private void Start()
    {
        if (visual != null)
        {
            originalVisualPos = visual.transform.localPosition;
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
            originColor = spriteRenderer.material.color;
        }
        curCnt = cnt;
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (isBroken) return;
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
                    curCnt--;
                    Debug.LogWarning($"{Time.deltaTime}: 플레이어가 발판에 올라감. 남은횟수: {curCnt}");
                    if (curCnt == 1 && spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.red;
                    }
                    if (curCnt <= 0)
                        BreakStool();
                }
                else
                {
                    return;
                }
            }
        }
    }


    private void BreakStool()
    {
        if (breakSFX != null)
            audioSource.PlayOneShot(breakSFX);

        Debug.LogWarning($"{Time.deltaTime}: 발판 부서짐");
        isBroken = true;
        visual.SetActive(false);
        col.enabled = false;

        Invoke(nameof(RespawnGround), respawnTime);
    }

    private void RespawnGround()
    {
        Debug.LogWarning($"{Time.deltaTime}: 발판 복구됨");
        isBroken = false;
        curCnt = cnt;
        visual.SetActive(true);
        col.enabled = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originColor;
            visual.transform.localPosition = originalVisualPos;
        }
    }
}
