using UnityEngine;

public class RuntimeGameManager : MonoBehaviour
{

    [SerializeField] private TestPlayer player;
    [SerializeField] private ClearDoor door;
    private bool isGameCleared = false;


    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameSave();
        if (CheckGameManager())
            Check_StageGameClear();
        else 
            return;
    }

    private void GameSave() 
    {
        // 예시용 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Instance.Save_PlayerData(player);
            SaveManager.Instance.Save_StageData(StageManager.Instance.curMainStage, StageManager.Instance.curSubStage);
            SaveManager.Instance.SaveToFile();
        }
        // 예시용 불러오기
        if (Input.GetKeyUp(KeyCode.O))
        {
            SaveManager.Instance.Load_PlayerData(player);
            SaveManager.Instance.Load_StageData();
            SaveManager.Instance.LoadFromFile();
        }
    }

    private bool CheckGameManager()
    {
        if (player == null || door == null)
        {
            Debug.LogError("게임매니저에 무언가가 설정되지 x");
            return false;
        }
        else { 
            return true;
        }
    }


    private void Check_StageGameClear() 
    {
        if (isGameCleared) return;
        if (door.isInPlayer)
        {
            StageGameClear();
            isGameCleared = true;
            // 이 후 조건 판단은 나중에 더 추가 가능
        }
    }

    private void StageGameClear()
    {
        Debug.Log($"{Time.deltaTime}: 축하합니다 스테이지 클리어! \n 다음 스테이지로 이동");
        // stageManager.StageClear();       // 이런식으로 나중에 스테이지 매니저가 관리
        // StageManager.Instance.CurStageClear();
        // StageManager.Instance.CurMainStageClear();
        StageManager.Instance.CurSubStageClear();
    }

    private void NewGameStart() 
    {
        isGameCleared = false;
    }
}
