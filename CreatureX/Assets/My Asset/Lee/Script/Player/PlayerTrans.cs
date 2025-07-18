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
        player.TransApply(8, 25, player.BasicSprite, player.BasicAnimController);

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
        player.TransApply(16, 0, player.CarSprite, player.CarAnimController);

        player.collider.size = new Vector2(3.25f, 1.5f);
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
        player.CarDashAttack(25f, 0.25f);
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
        player.TransApply(6, 0, player.DroneSprite, player.DroneAnimController);

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
    }
}




public class PlayerTrans : MonoBehaviour
{

}
