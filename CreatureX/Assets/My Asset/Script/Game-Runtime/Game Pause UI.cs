using UnityEngine;

public class GamePauseUI : MonoBehaviour
{
    void Awake()
    {
        if (RuntimeGameManager.Instance != null)
            RuntimeGameManager.Instance.Regist_PauseUI(this);
        else
            Debug.LogError("아직 없음!");
    }

}
