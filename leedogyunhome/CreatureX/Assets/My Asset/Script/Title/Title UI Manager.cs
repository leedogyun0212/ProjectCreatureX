using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{

    [SerializeField] Button startButton;

    private void Awake()
    {

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnClickStartGame);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClickStartGame()
    {
        // 예: 첫 스테이지로 이동
        // SceneManager.LoadScene("TestStage1-1");  // 씬 이름 정확히 입력!
        StageManager.Instance.LoadStage(StageManager.Instance.curMainStage, StageManager.Instance.curSubStage);
    }

}
