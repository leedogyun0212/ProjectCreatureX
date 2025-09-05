using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeGameManager : MonoBehaviour
{
    public static RuntimeGameManager Instance { get; private set; }

    [SerializeField] private TestPlayer player;
    [SerializeField] private ClearDoor door;
    [SerializeField] private HiddenDoor door_Hidden;
    private bool isGameCleared = false;
    private bool isGamePaused = false;
    private bool isGameOvered = false;

    [SerializeField] private GamePauseUI pauseUI;
    [SerializeField] private GameOverUI overUI;
    [SerializeField] private UI_GoTitle goToTitleUI;
    [SerializeField] private UI_GameRestart restartUI;

    // 오브젝트 객체 재등록
    public void Regist_Player(TestPlayer p) => player = p;
    public void Regist_Door(ClearDoor d) => door = d;
    public void Regist_Door(HiddenDoor d) => door_Hidden = d;
    public void Regist_PauseUI(GamePauseUI ui) => pauseUI = ui;
    public void Regist_OverUI(GameOverUI ui) => overUI = ui;
    public void Regist_TitleUI(UI_GoTitle ui) => goToTitleUI = ui;
    public void Regist_ReStartUI(UI_GameRestart ui) => restartUI = ui;


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


    private void Start()
    {
        StartCoroutine(Delayed_GameReset());
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndInitialize());
    }

    private IEnumerator WaitAndInitialize()
    {
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
        if (player == null)
        {
            // Debug.LogError("게임매니저에 무언가가 설정되지 x");
            return false;
        }
        else {
            return true;
        }
    }


    private void Check_StageGameClear()
    {
        if (isGameCleared || isGamePaused) return;
        if (door.isInPlayer)
        {
            isGameCleared = true;
            StageGameClear();
        }
        else if(door_Hidden.isInPlayer) 
        {
            isGameCleared = true;
            StageGameClear_Hidden();
        }
    }

    private void StageGameClear()
    {
        if (StageManager.Instance.curSubStage >= StageManager.Instance.ClearIndex)
        {
            StageManager.Instance.CurMainStageClear();
        }
        else 
        {
            StageManager.Instance.CurSubStageClear();
        }
    }

    private void StageGameClear_Hidden()
    {
        if (StageManager.Instance.curSubStage >= StageManager.Instance.ClearIndex)
        {
            StageManager.Instance.CurMainStageClear();
        }
        else
        {
            StageManager.Instance.CurSubStageClear_Hidden();
        }
    }


    private void ToggleChildren(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }   

    public void Check_GameRestart()
    {
        if (restartUI != null)
        {
            ToggleChildren(restartUI.gameObject, true);
        }
    }

    public void Cancel_Restart()
    {
        if (goToTitleUI != null)
        {
            ToggleChildren(restartUI.gameObject, false);
        }
    }


    public void NewGameStart()
    {
        GameReset();
        StageManager.Instance.ReStartStage();
    }

    public void GameReset()
    {
        StartCoroutine(Delayed_GameReset());
    }

    private IEnumerator Delayed_GameReset()
    {
        yield return null;

        if (player != null) player.PlayerRevive();

        if (pauseUI != null) ToggleChildren(pauseUI.gameObject, false);
        if (overUI != null) ToggleChildren(overUI.gameObject, false);
        if (goToTitleUI != null) ToggleChildren(goToTitleUI.gameObject, false);
        if (restartUI != null) ToggleChildren(restartUI.gameObject, false);


        Time.timeScale = 1f;
        player.PlayerContinue();

        isGameCleared = false;
        isGameOvered = false;
        isGamePaused = false;
    }


    public void Check_GoTitle()
    {
        if (goToTitleUI != null)
        {
            ToggleChildren(goToTitleUI.gameObject, true);
        }
    }

    public void Cancel_GoTitle()
    {
        if (goToTitleUI != null)
        {
            ToggleChildren(goToTitleUI.gameObject, false);
        }
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
            isGameOvered = true;
            RunGameOver();
        }
    }


    public void Check_FallDeath()
    {
        if (isGameOvered || isGamePaused) return;
        isGameOvered = true;
        RunGameOver_Fall();
    }

    private void RunGameOver()
    {
        if (overUI != null)
        {
            ToggleChildren(overUI.gameObject, true);
        }
    }

    private void RunGameOver_Fall()
    {
        if (overUI != null)
        {
            ToggleChildren(overUI.gameObject, true);
        }
    }


    public void Check_GamePause()
    {
        isGamePaused = !isGamePaused;
        if (pauseUI != null)
        {          
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