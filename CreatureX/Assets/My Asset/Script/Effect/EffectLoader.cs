using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectLoader : MonoBehaviour
{
    public List<EffectEntry> effectList;  // ����Ƽ �����Ϳ��� ����� ����Ʈ

    void Awake()
    {
        EffectManager.Initialize(effectList);
    }
}