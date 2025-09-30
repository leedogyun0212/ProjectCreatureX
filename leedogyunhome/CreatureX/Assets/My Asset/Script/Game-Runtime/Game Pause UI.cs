using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    void Awake()
    {
        if (RuntimeGameManager.Instance != null)
            RuntimeGameManager.Instance.Regist_PauseUI(this);
        // else Debug.LogError("아직 없음!");
    }

    [SerializeField] private Button pauseButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Text StageText;

    private void Start()
    {
        // 버튼 이벤트 동적으로 등록
        if (restartButton != null)
        {
            // restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                    RuntimeGameManager.Instance.Check_GameRestart();
            });
        }
        if (titleButton != null)
        {
            // titleButton.onClick.RemoveAllListeners();
            titleButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                {
                    // RuntimeGameManager.Instance.GotoTitle();
                    RuntimeGameManager.Instance.Check_GoTitle();
                }
            });
        }
        if (pauseButton != null)
        {
            // titleButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                    RuntimeGameManager.Instance.Check_GamePause();
            });
        }
    }

    private void Update()
    {
        StageText.text = $"Stage: {StageManager.Instance.curMainStage}-{StageManager.Instance.curSubStage}";
    }

}
