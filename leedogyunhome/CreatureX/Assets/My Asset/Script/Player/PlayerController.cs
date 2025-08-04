using Unity.VisualScripting;
using UnityEngine;
using static TestPlayer;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    TestPlayer player;

    // 키 바인딩 변수
    [Header("Movement Keys")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Trans Action Keys")]
    [SerializeField] private KeyCode flyUpKey = KeyCode.UpArrow;
    [SerializeField] private KeyCode flyDownKey = KeyCode.DownArrow;

    [SerializeField] private KeyCode transKey = KeyCode.T;
    [SerializeField] private KeyCode releaseKey = KeyCode.E;
    [SerializeField] private KeyCode attackKey = KeyCode.Z; 
    [SerializeField] private KeyCode transSkillKey = KeyCode.X; 
    [SerializeField] private KeyCode assimilateKey = KeyCode.R;

    [Header("System Keys")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Alpha9;
    // [SerializeField] private KeyCode reStartKey = KeyCode.Alpha8;






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
        else
        {
            ControlJump();
        }
        ControlAttack();
        ControlAssimilate();
        ControlGamePause();
        ControlTransSkill();
        ControlTrans();

        //if (Input.GetKey(reStartKey))
        //{
        //    RuntimeGameManager.Instance.NewGameStart();
        //}
    }

    private void ControlMove()
    {
        float dir = 0;
        if (player.isTransDashing) return;

        if (Input.GetKey(moveLeftKey)) dir = -1;
        if (Input.GetKey(moveRightKey)) dir = 1;

        // player.OrderMove(dir);
        player.curTrans.Move(dir);
    }

    private void ControlJump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            player.curTrans.Jump(0);
        }
    }

    private void ControlFly()
    {
        float dir = 0;
        if (Input.GetKey(flyUpKey)) dir = 1;
        if (Input.GetKey(flyDownKey)) dir = -1;

        player.curTrans.Jump(dir);
    }

    private void ControlTrans()
    {
        if (Input.GetKeyDown(transKey))
        {
            player.TryTransform();
        }
    }


    private void ControlRealese()
    {
        if (Input.GetKeyDown(releaseKey))
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
        if (Input.GetKeyDown(attackKey))
        {
            Debug.LogWarning($"{Time.deltaTime}: {player.curTrans.Name} 공격 시도!");
            // player.curTrans.Attack();
            player.OrderAttack();
        }
    }



    private void ControlAssimilate()
    {
        if (Input.GetKeyDown(assimilateKey))
        { 
            player.TryAssimilate();
        }
    }

    private void ControlGamePause()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            RuntimeGameManager.Instance.Check_GamePause();
        }        
    }

    private void ControlTransSkill()
    {
        if (Input.GetKeyDown(transSkillKey))
        {
            player.OrderTransSkill();
        }
    }
}
