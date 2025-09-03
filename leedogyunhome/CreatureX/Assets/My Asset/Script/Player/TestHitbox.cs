using System.Collections.Generic;
using UnityEngine;

public class TestHitbox : MonoBehaviour
{
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public float lifetime;

    private TestPlayer player;

    private void Start()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            player = PlayerObj.GetComponent<TestPlayer>();
        Destroy(gameObject, lifetime);
    }

    private void OnDestroy()
    {
        player.isAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")||other.CompareTag("Researcher")) // 몬스터는 Enemy 태그
        {
            player.isAttack = true;
            var enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                Debug.Log($"{Time.time:F2}: 몬스터 {enemy.name} 감지 성공!");               
                enemy.GetHitByPlayer();
            }
        }

    }



}
