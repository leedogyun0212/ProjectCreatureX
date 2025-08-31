using UnityEngine;
using UnityEngine.UI;

public class UI_GameEnd : MonoBehaviour
{
    void Awake()
    {
        if (TitleUIManager.Instance != null)
            TitleUIManager.Instance.Regist_GameEndUI(this);
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
                if (TitleUIManager.Instance != null)
                    TitleUIManager.Instance.GameEnd();
            });
        }
        if (NoButton != null)
        {
            // titleButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(() =>
            {
                if (TitleUIManager.Instance != null)
                    TitleUIManager.Instance.Cancel_GameEnd();
            });
        }
    }
}
