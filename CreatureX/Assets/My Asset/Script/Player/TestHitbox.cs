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
            // Debug.Log($"{Time.deltaTime}: 몬스터 {name} 에게 데미지 입힘!");
            // 추후 TakeDamage 등 호출
            player.isAttack = true;
        }

        //if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        //{
        //    hitTargets.Add(other.gameObject);
        //    // 데미지 주기
        //    // Debug.Log($"{Time.deltaTime}: 몬스터 {name} 에게 데미지 입힘!");
        //    // 추후 TakeDamage 등 호출
        //}
    }



}
