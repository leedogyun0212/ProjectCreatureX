using System.Collections.Generic;
using UnityEngine;

public static class EffectManager
{
    private static Dictionary<string, ObjectPool<IRecyclableGameObject>> pools = new();
    private static Dictionary<string, GameObject> prefabMap = new();
    private static Dictionary<(GameObject, string), float> effectCooldowns = new();
    private static bool initialized = false;

    public static void Initialize(List<EffectEntry> entries)
    {
        //if (initialized) return;

        foreach (var entry in entries)
        {
            prefabMap[entry.effectName] = entry.prefab;
            var pool = new ObjectPool<IRecyclableGameObject>();

            for (int i = 0; i < entry.effectCount; i++)
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

    public static void Play(string key, GameObject caller, bool scaleLeft = false, Vector3 offset = default, float cooldown = 0.5f)
    {
        // 1. key + caller 조합이 이미 썼는지 체크
        var effectKey = (caller, key);
        if (cooldown > 0 && effectCooldowns.TryGetValue(effectKey, out float nextAvailableTime))
        {
            if (Time.time < nextAvailableTime)
                return; // 쿨타임 중이면 무시
        }

        // 2. 다음 호출 가능 시간 저장
        if (cooldown > 0)
            effectCooldowns[effectKey] = Time.time + cooldown;
        // 3. 이펙트 실행
        if (!pools.TryGetValue(key, out var pool))
        {
            // Debug.LogWarning($"Effect {key} not found in pool.");
            return;
        }

        Vector3 finalOffset = offset;
        if (scaleLeft)
            finalOffset.x *= -1f;

        var effectObject = pool.GetInactiveObject() as EffectObject;
        if (effectObject != null)
        {
            effectObject.PlayEffect(caller.transform.position + offset, scaleLeft, caller.transform, offset);
        }
    }
}
