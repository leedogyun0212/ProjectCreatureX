using UnityEngine;
using UnityEngine.UI;

public class UI_TitleButton : MonoBehaviour
{
    void Awake()
    {
        if (TitleUIManager.Instance != null)
            TitleUIManager.Instance.Regist_TitleButtonUI(this);
    }

    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_exit;

    private void Start()
    {
        // 버튼 이벤트 동적으로 등록
        if (btn_start != null)
        {
            btn_start.onClick.AddListener(() =>
            {
                if (TitleUIManager.Instance != null)
                    TitleUIManager.Instance.OnClickStartGame();
            });
        }
        if (btn_exit != null)
        {
            btn_exit.onClick.AddListener(() =>
            {
                if (TitleUIManager.Instance != null)
                    TitleUIManager.Instance.Check_GameEnd();
            });
        }
    }



}
