using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int curMainStage = 1;
    public int curSubStage = 1;
    [NonSerialized] public int ClearIndex = 7;

    private Dictionary<string, ChapterData> chapDict = new Dictionary<string, ChapterData>();
    private Dictionary<string, StageData> stageDict = new Dictionary<string, StageData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RegisterStage(StageData stage)
    {
        string key = GetStageKey(stage.mainStageIndex, stage.subStageIndex);
        if (!stageDict.ContainsKey(key))
            stageDict.Add(key, stage);
    }

    public void RegisterChapter(ChapterData chapter)
    {
        //string key = GetChapterKey(chpater.chapterIndex);
        //if (!chapDict.ContainsKey(key))
        //    chapDict.Add(key, chpater);

        string key = GetChapterKey(chapter.chapterIndex);
        chapDict[key] = chapter; // 있으면 덮어쓰기
    }

    public void LoadStage(int main, int sub)
    {
        curMainStage = main;
        curSubStage = sub;

        string sceneName = $"Stage{main}-{sub}";
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
        RuntimeGameManager.Instance.GameReset();
    }

    public void ReStartStage()
    {
        LoadStage(curMainStage, curSubStage);
    }

    public void CurSubStageClear()
    {
        MarkStageClear(curMainStage, curSubStage);
        curSubStage++;
        UnlockStage(curMainStage, curSubStage);
        Load_NextStage();
    }

    public void CurSubStageClear_Hidden()
    {
        MarkStageClear(curMainStage, curSubStage);
        curSubStage = curSubStage + 2;
        UnlockStage(curMainStage, curSubStage);
        Load_NextStage();
    }

    public void CurMainStageClear()
    {
        MarkStageClear(curMainStage, curSubStage);
        curMainStage++;
        curSubStage = 1;
        UnlockChapter(curMainStage);
        UnlockStage(curMainStage, curSubStage);
        SceneManager.LoadScene("TestSelectScene");
        // Load_NextStage();
    }

    public void Load_NextStage()
    {
        LoadStage(curMainStage, curSubStage);
    }

    public void MarkStageClear(int main, int sub)
    {
        string key = GetStageKey(main, sub);
        if (stageDict.TryGetValue(key, out var stage))
        {
            Debug.Log($"Stage {main}-{sub} 스테이지 클리어!!");
            stage.ClearStage();
        }
        else
            Debug.LogWarning($"Stage {main}-{sub} 스테이지가 없다?!!");
    }

    // 스테이지 잠금 해제
    private void UnlockStage(int main, int sub)
    {
        string key = GetStageKey(main, sub);
        if (stageDict.TryGetValue(key, out var stage))
        {
            stage.UnlockStage();
        }
        else
        {
            Debug.LogWarning($"Stage {main}-{sub} 스테이지가 없다?!!");
        }
    }

    private void UnlockChapter(int chapindex)
    { 
        string key = GetChapterKey(chapindex);
        if (chapDict.TryGetValue(key, out var chapter))
        {
            chapter.UnlockChapter();
        }
        else
        {
            Debug.LogWarning($"Stage {chapindex} 챕터 없음!!!");
        }
    }


    private string GetStageKey(int main, int sub)
    {
        return $"{main}-{sub}";
    }

    private string GetChapterKey(int chap)
    {
        return $"{chap}";
    }
}
