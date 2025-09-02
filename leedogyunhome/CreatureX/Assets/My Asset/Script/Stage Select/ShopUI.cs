using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button shopButton;
    [SerializeField] private Button reinforceButton;

    [SerializeField] private RectTransform shopUI;
    [SerializeField] private RectTransform reinforceUI;


    private void Start()
    {
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(() =>
            {
                ToggleUI(shopUI);
            });
        }
        if (reinforceButton != null)
        {
            reinforceButton.onClick.AddListener(() =>
            {
                ToggleUI(reinforceUI);
            });
        }
    }

    private void ToggleUI(RectTransform targetUI)
    {
        // 이미 켜져 있으면 끄기
        if (targetUI.gameObject.activeSelf)
        {
            targetUI.gameObject.SetActive(false);
        }
        else
        {
            // 다른 UI는 끄고 자기만 켜기
            HideUI();
            targetUI.gameObject.SetActive(true);
        }
    }

    private void HideUI()
    {
        shopUI.gameObject.SetActive(false);
        reinforceUI.gameObject.SetActive(false);
    }


}
