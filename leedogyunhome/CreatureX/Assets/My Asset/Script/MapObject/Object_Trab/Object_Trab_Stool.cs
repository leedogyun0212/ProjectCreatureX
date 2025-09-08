using System.Collections;
using UnityEngine;

public class Object_Trab_Stool : MonoBehaviour
{
    public Object_Trab TrabPrefab;
    private Object_Trab currentTrab;

    [SerializeField]
    private Transform TrabPos;

    [SerializeField]
    private int spawnCount = 1; // [변경] 소환 개수 (Inspector에서 조절)

    [SerializeField]
    private float horizontalSpacing = 2.0f; // [변경] 좌우 간격(유닛)

    //private Vector3 TrabPos;

    private bool Create = true;

    private void Start()
    {
        //TrabPos = transform.position;
        //TrabPos.y += 10;
    }

    private void TrabCreate()
    {
        // [변경] TrabPos를 중심으로 좌우 대칭으로 여러 개 소환
        int count = Mathf.Max(1, spawnCount); // 최소 1개 보장
        Vector3 center = TrabPos.position;
        float half = (count - 1) / 2f; // 홀수면 중앙 포함, 짝수면 반칸 기준 대칭

        int centerIndex = Mathf.RoundToInt(half);
        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - half) * horizontalSpacing; // 좌우 오프셋 계산
            Vector3 spawnPos = center + new Vector3(offsetX, 0f, 0f);

            Object_Trab spawned = Instantiate(TrabPrefab, spawnPos, Quaternion.identity);
            spawned.SetTrab();
            // currentTrab는 대표 참조로 중앙(또는 중앙에 가장 가까운) 개체를 기록
            if (i == centerIndex)
                currentTrab = spawned;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.CompareTag("Player")&&Create)
    //    {
    //        Create = false;
    //        StartCoroutine(Warning());
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Create)
        {
            Debug.Log("발동 성공!!!!");
            Create = false;
            StartCoroutine(Warning());
        }
    }

    private IEnumerator Warning()
    {
        Debug.Log("Ʈ�� �ߵ�!!!!!!!!");

        yield return new WaitForSecondsRealtime(1.0f);

        TrabCreate();
    }
}
