using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    [NonSerialized]
    public List<BaseEnemy> enemyList = new List<BaseEnemy>();


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    // 몬스터 등록
    public void RegisterEnemy(BaseEnemy enemy)
    {
        if (!enemyList.Contains(enemy))
        {
            Debug.Log($"{Time.deltaTime}: {enemy.enemyID}: 몬스터 등록 성공!");
            enemyList.Add(enemy);
        }
    }
    // 몬스터 등록 삭제
    public void UnregisterEnemy(BaseEnemy enemy)
    {
        if (enemyList.Contains(enemy))
        {
            Debug.Log($"{Time.deltaTime}: {enemy.enemyID}: 몬스터 삭제 성공!");
            enemyList.Remove(enemy);
        }
    }


    //public BaseEnemy GetAssimilateEnemy(Vector3 playerPos, float range)
    //{
    //    BaseEnemy closest = null;
    //    float minDist = Mathf.Infinity;

    //    foreach (var enemy in enemyList)
    //    {
    //        if (!enemy.Able_Assimilated()) continue;

    //        float dist = Vector3.Distance(playerPos, enemy.transform.position);
    //        if (dist < range && dist < minDist)
    //        {
    //            minDist = dist;
    //            closest = enemy;
    //        }
    //    }
    //    return closest;
    //}
}
