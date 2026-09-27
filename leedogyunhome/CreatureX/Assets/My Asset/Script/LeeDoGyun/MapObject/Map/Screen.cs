using UnityEngine;

public class Screen : MonoBehaviour
{
    [Header("Animator Parameter Driving")]
    [SerializeField] private float changeIntervalSeconds = 2f;   // 한 상태가 유지되는 시간
    [SerializeField] private int stateCount = 4;                  // 화면 상태 개수 (01~04 => 4)

    private Animator _animator;
    private float _elapsed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_animator == null || changeIntervalSeconds <= 0f || stateCount <= 0) return;

        _elapsed += Time.deltaTime;
        float cycle = changeIntervalSeconds * stateCount;
        float repeated = Mathf.Repeat(_elapsed, cycle);
        _animator.SetFloat("Time", repeated);
    }
}
