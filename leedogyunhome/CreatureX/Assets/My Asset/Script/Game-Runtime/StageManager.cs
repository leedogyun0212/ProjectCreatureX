using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int curMainStage = 1;
    public int curSubStage = 1;
    public int ClearStageIndex = 10;

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

    public void LoadStage(int main, int sub)
    {
        curMainStage = main;
        curSubStage = sub;

        if (curMainStage >= ClearStageIndex)
        {
            Debug.Log("모든 스테이지 클리어!");
        }
        else
        {
            string sceneName = $"TestStage{main}-{sub}";
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
            RuntimeGameManager.Instance.GameReset();
        }
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

    public void CurMainStageClear()
    {
        MarkStageClear(curMainStage, curSubStage);
        curMainStage++;
        curSubStage = 1;
        UnlockStage(curMainStage, curSubStage);
        Load_NextStage();
    }

    public void Load_NextStage()
    {
        LoadStage(curMainStage, curSubStage);
    }

    public void MarkStageClear(int main, int sub)
    {
        string key = GetStageKey(main, sub);
        if (stageDict.TryGetValue(key, out var stage))
            stage.ClearStage();
        else
            Debug.LogWarning($"Stage {main}-{sub} 스테이지가 없다?!!");
    }

    // 스테이지 잠금 해제
    public void UnlockStage(int main, int sub)
    {
        string key = GetStageKey(main, sub);
        if (stageDict.TryGetValue(key, out var stage))
        {
            Debug.Log($"Stage {main}-{sub} 스테이지가 잠금 해제!");
            stage.UnlockStage();
        }
        else
        {
            Debug.LogWarning($"Stage {main}-{sub} 스테이지가 없다?!!");
        }
    }

    private string GetStageKey(int main, int sub)
    {
        return $"{main}-{sub}";
    }
}
