using System.Collections;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public bool Left = true;

	private Vector2 MouseVec;
	[SerializeField]
	private Transform _startPosition;

    [SerializeField]
    private float time = 4.0f;

    [SerializeField]
    private float Speed = 5.0f;

	private void Start()
	{
		//_startPosition = transform.position;
		MouseVec = Left ? Vector2.left : Vector2.right;
		StartCoroutine(MoveOn());
	}



	private IEnumerator MoveOn()
	{
		while (true)
		{
			float elapsed = 0f;
			while (elapsed < time)
			{
				transform.Translate((Vector3)MouseVec * Speed * Time.deltaTime, Space.World);
				elapsed += Time.deltaTime;
				yield return null;
			}

			// 시작 위치로 리셋
			transform.position = _startPosition.position;
			yield return null;
		}
	}
}
