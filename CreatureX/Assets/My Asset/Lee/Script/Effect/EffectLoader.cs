using System.Collections.Generic;
using UnityEngine;

public class EffectLoader : MonoBehaviour
{
    public List<EffectEntry> effectList;  // 유니티 에디터에서 등록할 리스트

    void Awake()
    {
        EffectManager.Initialize(effectList);
    }
}