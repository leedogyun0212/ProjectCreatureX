using UnityEngine;

public interface IPlayerTrans
{
    string Name { get; }

    void Enter(
        // player.Collider.size = new Vector2(w, h);
        // player.Collider.offset = new Vector2(x, y);
        // 이런식으로 나중에 피격 판정 바꿔줘야 할듯?     

        );   // 변신 할 때
    void Exit();    // 변신 해제

    void Move(float direction);    // 이동
    void Jump(float vertical);    // 점프

    void Attack();  // 일단 공격만

    void TransSkill();

}

public interface IPlayerRelease
{
    void Realese();  // 방출
}

// 기본 상태
public class Player_Trans_Basic : IPlayerTrans 
{ 
    private TestPlayer player;
    public string Name => "일반형";

    public Player_Trans_Basic(TestPlayer player) 
    { 
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: 기본 상태 돌입");
        // player.TransApply(8, 25, player.BasicSprite, player.BasicAnimController);
        player.TransApply(8, 25, player.BasicAnimController);

        player.collider.size = new Vector2(1.0f, 2.25f);    
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: 기본 상태 해제!");
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
        // player.OrderAttack();  
        if (!player.isJump)
            player.BasicAttack();
        else
            player.JumpAttack();
    }

    public void TransSkill()
    {
        // 기본상태 변신 스킬 없음
    }
}

// 자동차 상태
public class Player_Trans_Car : IPlayerTrans, IPlayerRelease
{
    private TestPlayer player;
    public string Name => "자동차";

    public Player_Trans_Car(TestPlayer player) { 
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: {Name}로 변신!");
        // player.TransApply(16, 0, player.CarSprite, player.CarAnimController);
        player.TransApply(16, 0, player.CarAnimController);

        player.collider.size = new Vector2(4.75f, 2.25f);
        player.collider.offset = new Vector2(0.02f, -0.05f);
        player.collider.direction = CapsuleDirection2D.Horizontal;
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: {Name} 변신 해제!");
        player.collider.offset = new Vector2(0, 0);
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {
        // 자동차는 점프가 없음
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        Debug.Log($"{Time.deltaTime}: {Name} 능력 방출!");
        player.OrderRealese();
    }

    public void Attack() 
    {
        // 추후 자동차만의 공격 구현
        player.Trans_DashAttack(30f, 0.3f, new Vector3(2f, 0.5f, 0f), 2f);
    }

    public void TransSkill()
    {
        player.TransSkill_Car();
    }
}

// 드론 상태
public class Player_Trans_Drone : IPlayerTrans, IPlayerRelease
{
    private TestPlayer player;
    public string Name => "드론";

    public Player_Trans_Drone(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: {Name}으로 변신!");
        // player.TransApply(6, 0, player.DroneSprite, player.DroneAnimController);
        player.TransApply(6, 0, player.DroneAnimController);

        player.rigid.gravityScale = 0;  // 중력 해제    
        player.collider.size = new Vector2(2.15f, 0.75f);
        player.collider.offset = new Vector2(0, -0.01f);
        player.collider.direction = CapsuleDirection2D.Horizontal;
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: {Name}  변신 해제!");

        player.rigid.gravityScale = 8f;  // 중력 적용
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Jump(float vertical)
    {
        // 드론은 점프 대신 자유 비행이 가능하도록
        player.OrderFly(vertical);
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }

    public void Realese()
    {
        Debug.Log($"{Time.deltaTime}: {Name} 능력 방출!");
        player.OrderRealese();
    }

    public void Attack() {
        // 추후 드론만의 공격 필요        
        player.Trans_DashAttack(25f, 0.2f, new Vector3(2f, 0.5f, 0f), 1f);
    }

    public void TransSkill()
    {
        player.TransSkill_Drone();
    }
}

// 로봇 상태
public class Player_Trans_Robot : IPlayerTrans, IPlayerRelease
{
    private TestPlayer player;
    public string Name => "로봇";

    public Player_Trans_Robot(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: {Name}로 변신!");
        player.TransApply(8, 0, player.RobotAnimController);

        player.collider.size = new Vector2(1f, 2.25f);
        player.collider.offset = new Vector2(-0.2f, 0);
        player.collider.direction = CapsuleDirection2D.Vertical;
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: {Name} 변신 해제!");
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
        Debug.Log($"{Time.deltaTime}: {Name} 능력 방출!");
        player.OrderRealese();
    }

    public void Attack()
    {
        // player.TransForm_DashAttack(30f, 0.3f, new Vector3(2f, 0.5f, 0f), 2f);
        player.Trans_RobotAttack();
    }

    public void TransSkill()
    {
        player.TransSkill_Robot();
    }
}

// 연구원
public class Player_Trans_Res : IPlayerTrans, IPlayerRelease
{
    private TestPlayer player;
    public string Name => "연구원";

    public Player_Trans_Res(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: {Name}으로 변신!");
        player.TransApply(6, 0, player.ResAnimController);
        player.collider.size = new Vector2(1f, 2.05f);

        // 캐릭터 좌우 반전
        //Vector3 scale = player.transform.localScale;
        //scale.x = Mathf.Abs(scale.x) * -1f;
        //player.transform.localScale = scale;

        player.collider.offset = new Vector2(0.08f, 0);
        player.collider.size = new Vector2(0.85f, 2.15f);
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: {Name}  변신 해제!");

        Vector3 scale = player.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * 1f;
        player.transform.localScale = scale;

        // player.rigid.gravityScale = 8f;  // 중력 적용
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
        Debug.Log($"{Time.deltaTime}: {Name} 능력 방출!");
        player.OrderRealese();
    }

    public void Attack()
    {
        // 연구놈은 공격 x       

    }

    public void TransSkill()
    {

    }
}







public class PlayerTrans : MonoBehaviour
{

}
