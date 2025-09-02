using UnityEngine;
using System.IO;
using System.Collections.Generic;

// 나중에 Vector3같은 Unity 타입 저장하는 방법
//[System.Serializable]
//public struct Vector3Data
//{
//    public float x, y, z;
//    public Vector3Data(Vector3 vec)
//    {
//        x = vec.x; y = vec.y; z = vec.z;
//    }
//    public Vector3 ToVector3() => new Vector3(x, y, z);
//}



[System.Serializable]
public class SaveData
{
    [Header("스테이지 & 챕터 Data")]
    public int saveCurMainStageIndex;
    public int saveCurSubStageIndex;

    // 클리어한 스테이지 목록 (예: "1-1", "1-2" 등)
    public List<string> unlockedChapter = new List<string>();
    public List<string> clearedStages = new List<string>();
    public List<string> unlockedStages = new List<string>();

    [Header("플레이어 Data")]
    public string savePlayerID;
    public float savePlayerHP;
    public float savePlayerSPD;
    // public Vector3 playerPosition;
    // public int playerATK;    // 이런식으로 나중에 추가
}


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private SaveData currentData = new SaveData();

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

    public void SaveToFile()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(Application.persistentDataPath + "/Gamesave.json", json);
        Debug.Log($"게임 저장: {Application.persistentDataPath} 성공!");
    }

    public void Save_PlayerData(TestPlayer player)
    {
        currentData.savePlayerID = player.playerID;
        currentData.savePlayerHP = player.playerMaxHP;
        currentData.savePlayerSPD = player.playerMoveSpeed;
    }

    public void Save_StageData(int stageMain, int stageSub)
    {
        currentData.saveCurMainStageIndex = stageMain;
        currentData.saveCurSubStageIndex = stageSub;
    }

    public void AddClearedStage(string stageKey)
    {
        if (!currentData.clearedStages.Contains(stageKey))
        {
            currentData.clearedStages.Add(stageKey);
            SaveToFile();
        }
    }

    public bool IsStageCleared(string stageKey)
    {
        return currentData.clearedStages.Contains(stageKey);
    }

    public void AddUnlockedStage(string stageKey)
    {
        if (!currentData.unlockedStages.Contains(stageKey))
        {
            currentData.unlockedStages.Add(stageKey);
            SaveToFile();
        }
    }

    public bool IsStageUnlocked(string stageKey)
    {
        return currentData.unlockedStages.Contains(stageKey);
    }

    public void AddUnlockedChapter(string capterKey)
    {
        if (!currentData.unlockedChapter.Contains(capterKey))
        {
            currentData.unlockedChapter.Add(capterKey);
            SaveToFile();
        }
    }

    public bool IsChapterUnlocked(string cahpterKey)
    {
        return currentData.unlockedChapter.Contains(cahpterKey);
    }

    public void LoadFromFile()
    {
        string path = Application.persistentDataPath + "/Gamesave.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        currentData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"게임 로드: {Application.persistentDataPath} 성공!");
    }

    public void Load_PlayerData(TestPlayer player)
    {
        player.playerID = currentData.savePlayerID;
        player.playerMaxHP = currentData.savePlayerHP;
        // player.transform.position = currentData.playerPosition;
    }

    public void Load_StageData()
    {
        StageManager.Instance.curMainStage = currentData.saveCurMainStageIndex;
        StageManager.Instance.curSubStage = currentData.saveCurSubStageIndex;
    }
}
