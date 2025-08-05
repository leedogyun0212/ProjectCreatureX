using UnityEngine;
using System.IO;

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
    [Header("스테이지 Data")]
    public int saveCurMainStageIndex;
    public int saveCurSubStageIndex;

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
    private SaveData currentData = new SaveData();      // 현재 세이브 데이터

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
        Debug.LogWarning($"{Time.deltaTime}: 게임 저장: {Application.persistentDataPath} 성공적으로 성공!");
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
        Debug.LogWarning($"{Time.deltaTime}: 게임 로드: {Application.persistentDataPath} 성공적으로 성공!");
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
