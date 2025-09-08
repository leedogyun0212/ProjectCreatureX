using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;

    void Awake()
    {
        if (RuntimeGameManager.Instance != null)
            RuntimeGameManager.Instance.Regist_OverUI(this);
    }

    private void Start()
    {
        // 버튼 이벤트 동적으로 등록
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                {
                    RuntimeGameManager.Instance.NewGameStart();
                }
            });
        }
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                {
                    RuntimeGameManager.Instance.Check_GoTitle();
                }
            });
        }
    }

}
