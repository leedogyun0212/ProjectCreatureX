using System.Collections;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public bool Left = true;

	private Vector2 MouseVec;
	[SerializeField]
	private Transform _startPosition;

    [SerializeField]
    private Transform _endPosition;

    [SerializeField]
    private float time = 3.0f; // 도착 후 대기 시간(초)

    [SerializeField]
    private float Speed = 5.0f;

	private void Start()
	{
        if (_startPosition != null)
        {
            transform.position = _startPosition.position;
        }
        StartCoroutine(MoveOn());
	}



    private IEnumerator MoveOn()
    {
        if (_startPosition == null || _endPosition == null)
        {
            yield break;
        }

        while (true)
        {
            // 출발점으로 이동(즉시 세팅)
            transform.position = _startPosition.position;

            // 출발점 -> 도착점으로 이동
            while (Vector3.SqrMagnitude(transform.position - _endPosition.position) > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _endPosition.position,
                    Speed * Time.deltaTime
                );
                yield return null;
            }

            // 도착 후 대기
            if (time > 0f)
            {
                yield return new WaitForSecondsRealtime(time);
            }
        }
    }
}
