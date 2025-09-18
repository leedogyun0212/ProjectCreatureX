using UnityEngine;

public interface IPlayerTrans
{
    string Name { get; }

    void Enter();

    void TransValueApply();

    void Realese();

    void Exit();

    void Move(float direction);
    void Jump(float vertical);

    void Attack();

    void TransSkill();
}

public class Player_Trans_Basic : IPlayerTrans
{ 
    private TestPlayer player;
    public string Name => "PlayerBasic";

    public Player_Trans_Basic(TestPlayer player) 
    { 
        this.player = player;
    }

    public void Enter()
    {
        player.TransApply(8, 25, player.BasicAnimController);
        player.collider.size = new Vector2(1f, 2.2f);
        player.collider.offset = new Vector2(0.03f, -0.4f);
    }

    public void TransValueApply()
    {

    }

    public void Realese()
    {
        player.OrderRealese();
    }

    public void Exit()
    {

    }

    public void Jump(float vertical)
    {
        player.OrderJump();
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Attack() 
    { 
        if (!player.isJump)
            player.BasicAttack_anim();
        else
            player.JumpAttack();
    }

    public void TransSkill()
    {

    }
}

public class Player_Trans_Car : IPlayerTrans
{
    private TestPlayer player;
    public string Name => "PlayerCar";

    public Player_Trans_Car(TestPlayer player) { 
        this.player = player;
    }

    public void Enter()
    {
        //if (player.isJump || player.isAttack) { return; }

        player.TransApply(16, 0, player.CarAnimController);
    }

    public void TransValueApply()
    {
        player.collider.size = new Vector2(4.75f, 2.25f);
        player.collider.offset = new Vector2(0.02f, -0.05f);
        player.collider.direction = CapsuleDirection2D.Horizontal;
    }

    public void Exit()
    {
        player.collider.offset = new Vector2(0, 0);
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {

    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        player.OrderRealese();
    }

    public void Attack() 
    {
        player.Trans_DashAttack(60f, 0.15f, new Vector3(2f, 0.5f, 0f), 2f);
    }

    public void TransSkill()
    {
        player.TransSkill_Car();
        player.TransChange(player.playerTransBasic);
    }
}

public class Player_Trans_Drone : IPlayerTrans
{
    private TestPlayer player;
    public string Name => "PlayerDrone";

    public Player_Trans_Drone(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        //if (player.isJump || player.isAttack) { return; }
        player.TransApply(6, 0, player.DroneAnimController);
    }
    public void TransValueApply()
    {
        player.rigid.gravityScale = 0;   
        player.rigid.linearVelocity = new Vector2(player.rigid.linearVelocity.x, 0f);
        player.collider.size = new Vector2(2.15f, 0.75f);
        player.collider.offset = new Vector2(0, -0.01f);
        player.collider.direction = CapsuleDirection2D.Horizontal;
    }

    public void Exit()
    {
        Debug.Log("중력 원래대로!");
        player.rigid.gravityScale = 8f;
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {
        player.OrderFly(vertical);
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        player.OrderRealese();
    }

    public void Attack() {    
        player.Trans_DashAttack(50f, 0.1f, new Vector3(2f, 0.5f, 0f), 1f);
    }

    public void TransSkill()
    {
        player.TransSkill_Drone();
        player.TransChange(player.playerTransBasic);
    }
}

public class Player_Trans_Robot : IPlayerTrans
{
    private TestPlayer player;
    public string Name => "PlayerRobot";

    public Player_Trans_Robot(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        //if (player.isJump || player.isAttack) { return; }
        player.TransApply(8, 0, player.RobotAnimController);
    }

    public void TransValueApply()
    {
        player.collider.size = new Vector2(1f, 2.25f);
        player.collider.offset = new Vector2(-0.2f, 0);
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Exit()
    {
        player.collider.offset = new Vector2(0, 0);
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {
        
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        player.OrderRealese();
    }

    public void Attack()
    {
        player.Trans_RobotAttack();
    }

    public void TransSkill()
    {
        player.TransSkill_Robot();
        player.TransChange(player.playerTransBasic);
    }
}

public class Player_Trans_Res : IPlayerTrans
{
    private TestPlayer player;
    public string Name => "PlayerRes";

    public Player_Trans_Res(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        //if (player.isJump || player.isAttack) { return; }
        player.TransApply(6, 0, player.ResAnimController);
    }

    public void TransValueApply()
    {
        player.collider.size = new Vector2(1f, 2.05f);
        player.collider.offset = new Vector2(0.08f, 0);
        player.collider.size = new Vector2(0.85f, 2.15f);
    }


    public void Exit()
    {
        Vector3 scale = player.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * 1f;
        player.transform.localScale = scale;
        // player.rigid.gravityScale = 8f;
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {
        player.OrderJump();
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        player.OrderRealese();
    }

    public void Attack()
    {     

    }

    public void TransSkill()
    {
        player.TransChange(player.playerTransBasic);
    }
}







public class PlayerTrans : MonoBehaviour
{

}
