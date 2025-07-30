using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    TestPlayer player;

    private bool EffectScale = false;

    private void Update()
    {
        PlayerBasicControl();
    }


    private void PlayerBasicControl()
    {
        ControlMove();
        ControlRealese();
        if (player.curTrans is Player_Trans_Drone)
        {
            ControlFly();
        }
        else {             
            ControlJump();
        }
        ControlAttack();
    }

    private void ControlMove() 
    {
        float dir = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) { dir = -1;  EffectScale = true; }
        if (Input.GetKey(KeyCode.RightArrow)) { dir = 1; EffectScale = true; }

        //if (dir !=0 ) EffectManager.Play("Dust", player.collider.bounds.min);

        // player.OrderMove(dir);
        player.curTrans.Move(dir);

        if (dir != 0) EffectManager.Play("Dust", player.collider.bounds.min,dir<0);
    }

    private void ControlJump() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // player.OrderJump();
            player.curTrans.Jump(0);
        }
    }

    private void ControlFly()
    {
        float dir = 0;
        if (Input.GetKey(KeyCode.UpArrow)) dir = 1;
        if (Input.GetKey(KeyCode.DownArrow)) dir = -1;

        player.curTrans.Jump(dir);
    }

    private void ControlRealese()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (player.curRelease != null)
            {
                Debug.LogWarning($"{Time.deltaTime}: {player.curTrans.Name} 방출 시도!");
                player.curRelease.Realese();
            }
            else 
            {
                Debug.LogWarning($"{Time.deltaTime}: {player.curTrans.Name}은 방출 불가!");
            }

            player.Test_FireBlackObj();
        }

    }

    private void ControlAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.LogWarning($"{Time.deltaTime}: {player.curTrans.Name} 공격 시도!");
            // player.curTrans.Attack();
            player.OrderAttack();
        }
    }


}
