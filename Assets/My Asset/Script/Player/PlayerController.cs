using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    TestPlayer player;

    private void Update()
    {
        PlayerBasicControl();
    }


    private void PlayerBasicControl()
    {
        ControlMove();
        if (player.curTrans is Player_Trans_Drone)
        {
            ControlFly();
        }
        else {             
            ControlJump();
        }
    }

    private void ControlMove() 
    {
        float dir = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) dir = -1;
        if (Input.GetKey(KeyCode.RightArrow)) dir = 1;

        // player.OrderMove(dir);
        player.curTrans.Move(dir);
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

}
