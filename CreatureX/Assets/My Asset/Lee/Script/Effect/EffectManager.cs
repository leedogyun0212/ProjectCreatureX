using System.Collections.Generic;
using UnityEngine;

public static class EffectManager
{
    private static Dictionary<string, ObjectPool<IRecyclableGameObject>> pools = new();
    private static Dictionary<string, GameObject> prefabMap = new();
    private static bool initialized = false;

    public static void Initialize(List<EffectEntry> entries)
    {
        if (initialized) return;

        foreach (var entry in entries)
        {
            prefabMap[entry.effectName] = entry.prefab;
            var pool = new ObjectPool<IRecyclableGameObject>();

            for (int i = 0; i < 3; i++)
            {
                GameObject obj = GameObject.Instantiate(entry.prefab);
                obj.SetActive(false);
                var recyclable = obj.GetComponent<IRecyclableGameObject>();
                pool.RegisterRecyclableObject(recyclable);
            }

            pools[entry.effectName] = pool;
        }

        initialized = true;
    }

    public static void Play(string effectName, Vector3 position, bool ScaleLeft)
    {
        if (!pools.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found.");
            return;
        }

        var pool = pools[effectName];

        var effectObject = pool.GetInactiveObject() as EffectObject;

        if (pool.HasActiveObject())
        {
            var activeEffect = pool.GetActiveObject() as EffectObject;
            if (activeEffect != null)
            {
                activeEffect.UpdateEffect(position, ScaleLeft); // 위치만 갱신
            }
            return;
        }

        //if (effectObject != null)
        //{
        //    effectObject.PlayEffect(position,ScaleLeft);
        //}
        //else
        //{
        //    Debug.LogWarning($"{effectName} 이펙트가 전부 사용 중입니다.");
        //}

        var recyclable = pool.GetRecyclableObject();

        if (recyclable == null)
        {
            return;
        }

        if (recyclable is EffectObject effectObj)
        {
            effectObj.PlayEffect(position, ScaleLeft);
        }
        else
        {
            Debug.LogWarning("Recyclable is not an EffectObject!");
        }
    }
}
