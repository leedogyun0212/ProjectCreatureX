using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour
{
    [SerializeField] private Button stageButton;
    public int mainStageIndex;
    public int subStageIndex;

    private bool isStageUnlocked = false;
    private bool isStageCleared = false;

    private Color oriColor;
    private Color lockColor = new Color(0.5f, 0.5f, 0.5f);

    private void Start()
    {
        if (mainStageIndex == 1 && subStageIndex == 1)
            isStageUnlocked = true;

        isStageUnlocked = true;

        // StageManager에 등록
        if (StageManager.Instance != null)
            StageManager.Instance.RegisterStage(this);

        if (stageButton != null)
        {
            stageButton.onClick.AddListener(OnStageButtonClicked);

            var image = stageButton.GetComponent<Image>();
            if (image != null)
                oriColor = image.color;
        }

        // 세이브 데이터에서 클리어/언락 정보 로드
        if (SaveManager.Instance != null)
        {
            string key = GetStageKey();
            isStageCleared = SaveManager.Instance.IsStageCleared(key);
            isStageUnlocked = isStageUnlocked || SaveManager.Instance.IsStageUnlocked(key);
        }

        UpdateStageVisual();
    }

    private void OnStageButtonClicked()
    {
        if (isStageUnlocked && StageManager.Instance != null)
            StageManager.Instance.LoadStage(mainStageIndex, subStageIndex);
    }

    private void UpdateStageVisual()
    {
        if (stageButton == null) 
        {
            Debug.LogWarning($"{mainStageIndex}-{subStageIndex} 스테이지 UI 버튼이 없음!!");
            return;
        }
        var image = stageButton.GetComponent<Image>();
        if (image == null) return;

        // Debug.LogWarning($"스테이지 UI 색깔 변경!");
        if (!isStageUnlocked)
        {
            image.color = lockColor;
            stageButton.interactable = false;
        }
        else
        {
            image.color = isStageCleared ? Color.green : oriColor;
            stageButton.interactable = true;
        }
    }

    public void ClearStage()
    {
        isStageCleared = true;
        // UpdateStageVisual();

        if (SaveManager.Instance != null)
            SaveManager.Instance.AddClearedStage(GetStageKey());
    }

    public void UnlockStage()
    {
        if (!isStageUnlocked)
        {
            isStageUnlocked = true;
            if (SaveManager.Instance != null)
                SaveManager.Instance.AddUnlockedStage(GetStageKey());
        }
    }

    private string GetStageKey()
    {
        return $"{mainStageIndex}-{subStageIndex}";
    }
}
