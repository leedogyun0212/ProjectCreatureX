using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{

    public static TitleUIManager Instance { get; private set; }
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


    [SerializeField] private UI_GameEnd gameEndUI;
    [SerializeField] private UI_TitleButton gameTitleUI;

    public void Regist_GameEndUI(UI_GameEnd ui) => gameEndUI = ui;
    public void Regist_TitleButtonUI(UI_TitleButton ui) => gameTitleUI = ui;


    private void ToggleChildren(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }


    public void OnClickStartGame()
    {
        SceneManager.LoadScene("TestSelectScene");
    }

    public void Check_GameEnd()
    {
        ToggleChildren(gameEndUI.gameObject, true);
    }

    public void Cancel_GameEnd()
    {
        ToggleChildren(gameEndUI.gameObject, false);
    }

    public void GameEnd()
    {
        Debug.Log($"{Time.deltaTime}: 게임을 종료합니다");
        Application.Quit();
    }

}
