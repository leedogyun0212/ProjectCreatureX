using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static TestPlayer;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    [SerializeField] private Ladder ladder;

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
    }

    private void ControlMove()
    {
        if (player.isTransDashing) return;

        bool leftPressed = Input.GetKey(moveLeftKey);
        bool rightPressed = Input.GetKey(moveRightKey);

        float dir;

        if (leftPressed && !rightPressed) dir = -1;
        else if (!leftPressed && rightPressed) dir = 1;
        else dir = 0;

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
            StartCoroutine(LateRealese());
            player.curTrans.Realese();
        }
    }

    private void ControlAttack()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Vector3 PlayerScale = player.transform.localScale;
            Vector3 Offset = new Vector3(1.95f, 0f, 0f);
            Debug.Log(player.curForm);
            if (!player.isJump&&!player.unableRel)
                EffectManager.Play("P_G_Attack", player.gameObject, PlayerScale.x < 0, Offset,-0.1f);
            else if (player.isJump && !player.unableRel)
                EffectManager.Play("P_J_Attack", player.gameObject, PlayerScale.x < 0,Vector3.zero,0f);
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

    private IEnumerator LateRealese()
    {
        yield return new WaitForSecondsRealtime(0.81f);

        EffectManager.Play("P_Release", player.gameObject, player.transform.localScale.x < 0, new Vector3(1.40f, 1.1f, 0f));
    }
}
