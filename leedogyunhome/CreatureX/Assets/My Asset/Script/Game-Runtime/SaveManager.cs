using UnityEngine;
using System.IO;

// ߿ Vector3 Unity Ÿ ϴ 
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
    [Header(" Data")]
    public int saveCurMainStageIndex;
    public int saveCurSubStageIndex;

    [Header("÷̾ Data")]
    public string savePlayerID;
    public float savePlayerHP;
    public float savePlayerSPD;
    // public Vector3 playerPosition;
    // public int playerATK;    // ̷ ߿ ߰
}


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private SaveData currentData = new SaveData();      // ̺ 

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
        Debug.LogWarning($"{Time.deltaTime}:: {Application.persistentDataPath} !");
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



    public void LoadFromFile()
    {
        string path = Application.persistentDataPath + "/Gamesave.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        currentData = JsonUtility.FromJson<SaveData>(json);
        Debug.LogWarning($"{Time.deltaTime}:ε: {Application.persistentDataPath} !");
    }


    public void Load_PlayerData(TestPlayer player)
    {
        player.playerID = currentData.savePlayerID;
        player.playerMaxHP = currentData.savePlayerHP;
        // player.transform.position = currentData.playerPosition;
    }

    public void Load_StageData()
    {
        // stage.curStage = currentData.saveCurStage;
        StageManager.Instance.curMainStage = currentData.saveCurMainStageIndex;
        StageManager.Instance.curSubStage = currentData.saveCurSubStageIndex;
    }
}
