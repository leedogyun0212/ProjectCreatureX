using UnityEngine;
using UnityEngine.UI;

public class UI_GameRestart : MonoBehaviour
{
    void Awake()
    {
        if (RuntimeGameManager.Instance != null)
            RuntimeGameManager.Instance.Regist_ReStartUI(this);
        // else Debug.LogError("아직 없음!");
    }

    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton; 

    private void Start()
    {
        // 버튼 이벤트 동적으로 등록
        if (YesButton != null)
        {
            // restartButton.onClick.RemoveAllListeners();
            YesButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                    RuntimeGameManager.Instance.NewGameStart();
            });
        }
        if (NoButton != null)
        {
            // titleButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(() =>
            {
                if (RuntimeGameManager.Instance != null)
                    RuntimeGameManager.Instance.Cancel_Restart();
            });
        }
    }
}
