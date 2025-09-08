using UnityEngine;

public class FakeKeyData : MonoBehaviour
{
    [SerializeField] private KeyData keyDataAsset;

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
