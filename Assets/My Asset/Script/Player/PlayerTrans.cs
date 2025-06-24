using UnityEngine;

public interface IPlayerTrans
{
    void Enter(
        // player.Collider.size = new Vector2(w, h);
        // player.Collider.offset = new Vector2(x, y);
        // 이런식으로 나중에 피격 판정 바꿔줘야 할듯?
        
        );   // 변신 할 때
    void Exit();    // 변신 해제(방출?)

    void Move(float direction);    // 이동
    void Jump(float vertical);    // 점프
}


// 기본 상태
public class Player_Trans_Basic : IPlayerTrans 
{ 
    private TestPlayer player;

    public Player_Trans_Basic(TestPlayer player) 
    { 
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: 기본 상태 돌입");
        player.CurSpriteRenderer.sprite = player.BasicSprite;
        player.CurAnimator.runtimeAnimatorController = player.BasicAnimController;
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
}

public class Player_Trans_Car : IPlayerTrans
{
    private TestPlayer player;

    public Player_Trans_Car(TestPlayer player) { 
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: 자동차로 변신!");
        player.CurSpriteRenderer.sprite = player.CarSprite;
        player.CurAnimator.runtimeAnimatorController = player.CarAnimController;
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: 자동차 변신 해제!");
    }

    public void Jump(float vertical)
    {
        // 자동차는 점프가 없음
    }

    public void Move(float direction)
    {
        player.OrderMove(direction);
    }
}

public class Player_Trans_Drone : IPlayerTrans
{
    private TestPlayer player;

    public Player_Trans_Drone(TestPlayer player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log($"{Time.deltaTime}: 드론으로 변신!");
        player.CurSpriteRenderer.sprite = player.DroneSprite;
        player.CurAnimator.runtimeAnimatorController = player.DroneAnimController;

        player.rigid.gravityScale = 0;  // 중력 해제    
    }

    public void Exit()
    {
        Debug.Log($"{Time.deltaTime}: 드론 변신 해제!");

        player.rigid.gravityScale = 2.5f;  // 중력 적용
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
}




public class PlayerTrans : MonoBehaviour
{

}
