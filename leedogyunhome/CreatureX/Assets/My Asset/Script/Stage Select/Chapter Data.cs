using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterData : MonoBehaviour
{

    [SerializeField] private Button chapterButton;
    public int chapterIndex;
    [SerializeField] private RectTransform chapterCanvas;


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
        //if (chapterIndex == 1)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 2)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 3)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 4)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 5)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 6)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 7)
        //    isChapterUnlocked = true;
        //if (chapterIndex == 8)
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
        if (isChapterUnlocked)
        {
            foreach (var chapter in allChapters)
            {
                if (chapter.chapterCanvas != null)
                    chapter.chapterCanvas.gameObject.SetActive(false);
            }
            chapterCanvas.gameObject.SetActive(true);
        }
    }


    public void UnlockChapter()
    {
        if (!isChapterUnlocked)
        {
            isChapterUnlocked = true;
            // UpdateChapterVisual();

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
