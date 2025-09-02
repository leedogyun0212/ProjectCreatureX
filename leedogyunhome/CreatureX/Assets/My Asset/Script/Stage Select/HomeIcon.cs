using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeIcon : MonoBehaviour
{
    [SerializeField] private Button homeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TestSelectScene");
            });
        }
    }
}
