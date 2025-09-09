using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopIcon : MonoBehaviour
{

    [SerializeField] private Button shopButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("shopScene");
            });
        }
    }
}
