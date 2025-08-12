using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeGameManager : MonoBehaviour
{
    public static RuntimeGameManager Instance { get; private set; }

    [SerializeField] private TestPlayer player;
    [SerializeField] private ClearDoor door;
    private bool isGameCleared = false;
    private bool isGamePaused = false;
    private bool isGameOvered = false;

    [SerializeField] private GamePauseUI pauseUI;
    [SerializeField] private GameOverUI overUI;

    // Find 사용 안하면서 재등록 하는 방법 (게임 요소들이 생성될때 등록하기)
    public void Regist_Player(TestPlayer p) => player = p;
    public void Regist_Door(ClearDoor d) => door = d;
    public void Regist_PauseUI(GamePauseUI ui) => pauseUI = ui;
    public void Regist_OverUI(GameOverUI ui) => overUI = ui;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 Player, Door, UI 다시 등록
        //player = FindObjectOfType<TestPlayer>();
        //door = FindObjectOfType<ClearDoor>();
        //pauseUI = FindObjectOfType<GamePauseUI>();
        //overUI = FindObjectOfType<GameOverUI>();

        // 씬이 로드되면 한 프레임 기다렸다가 등록 확인
        StartCoroutine(WaitAndInitialize());
    }

    private IEnumerator WaitAndInitialize()
    {
        // 한 프레임 대기 (모든 Awake/Start가 실행될 시간 확보)
        yield return null;

        if (player == null)
            Debug.LogWarning("플레이어가 등록 X");
        if (pauseUI == null)
            Debug.LogWarning("Pause UI가 등록 X");
        if (overUI == null)
            Debug.LogWarning("Over UI가 등록 X");
    }

    // Update is called once per frame
    void Update()
    {
        GameSave();
        if (CheckGameManager())
        {
            Check_StageGameClear();
            Check_StageGameOver();
        }
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
            // Debug.LogError("게임매니저에 무언가가 설정되지 x");
            return false;
        }
        else
        {
            return true;
        }
    }


    private void Check_StageGameClear()
    {
        if (isGameCleared || isGamePaused) return;
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


    private void ToggleChildren(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    //private void GameReset()
    //{
    //    Debug.Log($"{Time.deltaTime}: 현재 게임 초기화 시도");
    //    if (player != null)
    //        player.PlayerRevive();
    //    if (overUI != null)
    //    {
    //        ToggleChildren(overUI.gameObject, false);
    //    }
    //    isGameCleared = false;
    //    isGameOvered = false;
    //    isGamePaused = false;
    //}


    private void GameReset()
    {
        StartCoroutine(DelayedReset());
    }

    private IEnumerator DelayedReset()
    {
        yield return null;

        if (player != null)
            player.PlayerRevive();

        if (overUI != null)
            ToggleChildren(overUI.gameObject, false);

        isGameCleared = false;
        isGameOvered = false;
        isGamePaused = false;
    }

    // 얘는 추후 UI나 버튼으로 구현
    public void NewGameStart()
    {
        GameReset();
        StageManager.Instance.ReStartStage();
    }

    public void GotoTitle()
    {
        GameReset();
        SceneManager.LoadScene("TestTitle");
    }


    private void Check_StageGameOver()
    {
        if (isGameOvered || isGamePaused) return;
        if (!player.isPlayerLive)
        {
            Debug.LogWarning("플레이어가 죽었습니다!!");
            isGameOvered = true;
            RunGameOver();
        }
    }


    public void Check_FallDeath()
    {
        if (isGameOvered || isGamePaused) return;
        isGameOvered = true;
        RunGameOver();
    }

    private void RunGameOver()
    {
        if (overUI != null)
        {
            // overUI.gameObject.SetActive(true);
            ToggleChildren(overUI.gameObject, true);
        }
        if (player != null)
            player.PlayerDie();

        Debug.LogError($"{Time.deltaTime}: 현재 체력: {player.playerCurHP} 게임오버!!");
    }

    public void Check_GamePause()
    {
        isGamePaused = !isGamePaused;
        if (pauseUI != null)
        {
            // pauseUI.gameObject.SetActive(isGamePaused);
            ToggleChildren(pauseUI.gameObject, isGamePaused);
        }
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            player.PlayerPause();
        }
        else
        {
            Time.timeScale = 1f;
            player.PlayerContinue();
        }
    }

}