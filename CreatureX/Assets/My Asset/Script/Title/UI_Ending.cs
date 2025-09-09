using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Ending : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            SceneManager.LoadScene("GameEnd");
    }
}
