using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int ClearStageIndex = 10;    // 한 10 스테이지 정도로 생각하면
    public int curMainStage = 1;
    public int runMaintStage = 1;

    public int curSubStage = 1;
    public int runSubStage = 1;

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


    public void LoadStage(int stage, int subStage)
    {
        curMainStage = stage;
        curSubStage = subStage;

        if (curMainStage >= ClearStageIndex)
        {
            Debug.Log("모든 스테이지 클리어!");
        }
        else {
            string sceneName = $"TestStage{stage}-{subStage}";
            Debug.Log($"{Time.deltaTime}: Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ReStartStage()
    {
        LoadStage(curMainStage, curSubStage);
    }


    public void CurSubStageClear() {
        curSubStage++;
        Load_NextStage();
    }

    public void CurMainStageClear() {
        curMainStage++;
        curSubStage = 1;
        Load_NextStage();
    }

    public void Load_NextStage()
    {
        LoadStage(curMainStage, curSubStage);
    }

    public void RetryStage()
    {
        LoadStage(curMainStage, curSubStage);
    }
}
