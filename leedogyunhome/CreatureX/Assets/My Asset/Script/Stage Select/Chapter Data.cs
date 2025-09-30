using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterData : MonoBehaviour
{

    [SerializeField] private Button chapterButton;
    public int chapterIndex;
    private static int curChapIndex;
    [SerializeField] private RectTransform chapterCanvas;
    public GearWheel gearWheel; 

    private bool isChapterUnlocked = false;

    private Color oriColor;
    private Color lockColor = new Color(0.85f, 0.85f, 0.85f);

    private static List<ChapterData> allChapters = new List<ChapterData>();

    private void Awake()
    {
        allChapters.Add(this);
    }

    private void OnDestroy()
    {
        allChapters.Remove(this);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        isChapterUnlocked = true;


        // StageManager에 등록
        if (StageManager.Instance != null)
            StageManager.Instance.RegisterChapter(this);

        if (chapterButton != null)
        {
            chapterButton.onClick.AddListener(OnChapterButtonClicked);

            var image = chapterButton.GetComponent<Image>();
            if (image != null)
                oriColor = image.color;
        }

        if (SaveManager.Instance != null)
        {
            string key = GetChapterKey();
            isChapterUnlocked = isChapterUnlocked || SaveManager.Instance.IsChapterUnlocked(key);
        }

        UpdateChapterVisual();
        LoadChapterScene(1);
    }


    public void LoadChapterScene(int index)
    { 
        if (chapterIndex == index)
            chapterCanvas.gameObject.SetActive(true);
        else
            chapterCanvas.gameObject.SetActive(false);
    }


    private void OnChapterButtonClicked()
    {
        if (curChapIndex == chapterIndex) return;
        if (isChapterUnlocked)
        {
            foreach (var chapter in allChapters)
            {
                if (chapter.chapterCanvas != null)
                    chapter.chapterCanvas.gameObject.SetActive(false);
            }
            chapterCanvas.gameObject.SetActive(true);
            curChapIndex = chapterIndex;
            // Debug.Log($"현재 챕터: {curChapIndex}");

            if (gearWheel != null)
                gearWheel.RotateGear();
        }
    }


    public void UnlockChapter()
    {
        if (!isChapterUnlocked)
        {
            isChapterUnlocked = true;
            if (SaveManager.Instance != null)
                SaveManager.Instance.AddUnlockedChapter(GetChapterKey());
        }
    }

    private void UpdateChapterVisual()
    {
        if (chapterButton == null)
        {
            Debug.LogWarning($"{chapterIndex} 챕터 UI 버튼이 없음!!");
            return;
        }
        var image = chapterButton.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning($"{chapterIndex} 챕터 UI 이미지가 없음!!");
            return;
        }
        if (!isChapterUnlocked)
        {
            image.color = lockColor;
            chapterButton.interactable = false;
        }
        else
        {
            image.color = oriColor;
            chapterButton.interactable = true;
        }
    }

    private string GetChapterKey()
    {
        return $"{chapterIndex}";
    }
}
