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

    // Find ��� ���ϸ鼭 ���� �ϴ� ��� (���� ��ҵ��� �����ɶ� ����ϱ�)
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
        // ���� �ε�� ������ Player, Door, UI �ٽ� ���
        //player = FindObjectOfType<TestPlayer>();
        //door = FindObjectOfType<ClearDoor>();
        //pauseUI = FindObjectOfType<GamePauseUI>();
        //overUI = FindObjectOfType<GameOverUI>();

        // ���� �ε�Ǹ� �� ������ ��ٷȴٰ� ��� Ȯ��
        StartCoroutine(WaitAndInitialize());
    }

    private IEnumerator WaitAndInitialize()
    {
        // �� ������ ��� (��� Awake/Start�� ����� �ð� Ȯ��)
        yield return null;

        if (player == null)
            Debug.LogWarning("�÷��̾ ��� X");
        if (pauseUI == null)
            Debug.LogWarning("Pause UI�� ��� X");
        if (overUI == null)
            Debug.LogWarning("Over UI�� ��� X");
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
        // ÿ
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Instance.Save_PlayerData(player);
            SaveManager.Instance.Save_StageData(StageManager.Instance.curMainStage, StageManager.Instance.curSubStage);
            SaveManager.Instance.SaveToFile();
        }
        // ÿ ҷ
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
            // Debug.LogError("���ӸŴ����� ���𰡰� �������� x");
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
            StageGameClear();
            isGameCleared = true;
            // �� �� ���� �Ǵ��� ���߿� �� �߰� ����
        }
    }

    private void StageGameClear()
    {
        Debug.Log($"{Time.deltaTime}: �����մϴ� �������� Ŭ����! \n ���� ���������� �̵�");
        // stageManager.StageClear();       // �̷������� ���߿� �������� �Ŵ����� ����
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
    //    Debug.Log($"{Time.deltaTime}: ���� ���� �ʱ�ȭ �õ�");
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

    // ��� ���� UI�� ��ư���� ����
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
            Debug.LogWarning("�÷��̾ �׾����ϴ�!!");
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

        Debug.LogError($"{Time.deltaTime}: ���� ü��: {player.playerCurHP} ���ӿ���!!");
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