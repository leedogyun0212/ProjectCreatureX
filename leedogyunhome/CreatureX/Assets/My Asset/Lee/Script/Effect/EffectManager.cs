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

            for (int i = 0; i < 2; i++)
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

    public static void Play(string effectName, Vector3 position)
    {
        if (!pools.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found.");
            return;
        }

        var pool = pools[effectName];
        var recyclable = pool.GetRecyclableObject();

        if (recyclable == null)
        {
            GameObject obj = GameObject.Instantiate(prefabMap[effectName]);
            recyclable = obj.GetComponent<IRecyclableGameObject>();
            pool.RegisterRecyclableObject(recyclable);
        }

        if (recyclable is EffectObject effectObj)
        {
            effectObj.PlayEffect(position);
        }
        else
        {
            Debug.LogWarning("Recyclable is not an EffectObject!");
        }
    }
}
