using System.Collections.Generic;
using UnityEngine;

public class TestAssBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            Debug.Log($"{Time.deltaTime}: 몬스터가 닿았습니다");
            if (enemy != null && enemy.Able_Assimilated())
            {
                EnemyManager.Instance.RegisterEnemy(enemy);
            }
        }
    }

    //private void OnCollisionExit2D(Collider2D other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
    //        if (enemy != null)
    //        {
    //            Debug.Log($"{Time.deltaTime}: 몬스터 탈출");
    //            EnemyManager.Instance.UnregisterEnemy(enemy);
    //        }
    //    }
    //}

    private void OnTriggerExit2D(Collider2D other)  // 트리거로 나갈 때 처리
    {
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                //Debug.Log($"{Time.time}: 몬스터 탈출");
                EnemyManager.Instance.UnregisterEnemy(enemy);
            }
        }
    }

}
