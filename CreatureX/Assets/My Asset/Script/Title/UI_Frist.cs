using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Frist : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene("TestTitle");
    }
}
