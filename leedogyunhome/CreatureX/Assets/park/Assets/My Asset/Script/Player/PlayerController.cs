using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    TestPlayer player;

    void Start()
    {
    }

    private void Update()
    {
        PlayerBasicControl();
    }


    private void PlayerBasicControl()
    {
        ControlJump();
        ControlMove();
    }

    private void ControlMove() 
    {
        float dir = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) dir = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) dir = 1;

        player.OrderMove(dir);
    }

    private void ControlJump() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.OrderJump();
        }
    }
}
