using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackHome : MonoBehaviour
{
    [SerializeField] private Button homeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TestTitle");
            });
        }
    }
}
