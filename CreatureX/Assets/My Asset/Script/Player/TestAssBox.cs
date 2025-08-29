using System.Collections.Generic;
using UnityEngine;

public class TestAssBox : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    private CircleCollider2D col;
    private List<BaseEnemy> enemyList = new List<BaseEnemy>();

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }


    private void Update()
    {
        CheckAssEnemy();
    }

    private void CheckAssEnemy()
    {
        bool canAss = false;

        foreach (var enemy in enemyList)
        {
            if (enemy != null && enemy.Able_Assimilated())
            {
                canAss = true;
                // Debug.Log($"{Time.time}: {enemy.enemyID} 흡수 가능! 값: {canAss}");
                EnemyManager.Instance.RegisterEnemy(enemy);
            }
        }        
        player.ableAss = canAss; // 이벤트 바로 실행?
    }


    public void SetRange(float range)
    {
        if (col != null)
            col.radius = range;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null && !enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
                //Debug.Log($"{Time.time}: 몬스터 진입");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null && enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
                EnemyManager.Instance.UnregisterEnemy(enemy);
                //Debug.Log($"{Time.time}: 몬스터 탈출");
            }
        }
    }

    public BaseEnemy GetFirstAssEnemy()
    {
        foreach (var enemy in enemyList)
        {
            if (enemy != null && enemy.Able_Assimilated())
                return enemy;
        }
        return null;
    }


}
