using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float curDirect { get; private set; }    // 현재 이동방향

    [Header("Player State")]
    [SerializeField] private float moveSpeed = 8;
    [SerializeField] private float flySpeed = 5;
    [SerializeField] private float jumpForce = 13;
    private bool isJump;

    [SerializeField] private LayerMask checkGroundLayer; // 레이어
    [NonSerialized] public Rigidbody2D rigid;

    // 현재 SpriteRenderer, Animator
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer CurSpriteRenderer => spriteRenderer;

    [SerializeField] private Animator animator;
    public Animator CurAnimator => animator;


    // 상태별 스프라이트 + 애니메이션 컨트롤러
    [Header("Basic State")]
    [SerializeField] private Sprite basicSprite;
    [SerializeField] private RuntimeAnimatorController basicAnimController;

    [Header("Car State")]
    [SerializeField] private Sprite carSprite;
    [SerializeField] private RuntimeAnimatorController carAnimController;

    [Header("Drone State")]
    [SerializeField] private Sprite droneSprite;
    [SerializeField] private RuntimeAnimatorController droneAnimController;


    // Getter로 안전하게 공개
    public Sprite BasicSprite => basicSprite;
    public RuntimeAnimatorController BasicAnimController => basicAnimController;

    public Sprite CarSprite => carSprite;
    public RuntimeAnimatorController CarAnimController => carAnimController;

    public Sprite DroneSprite => droneSprite;
    public RuntimeAnimatorController DroneAnimController => droneAnimController;


    // 변신 상태 객체
    private Player_Trans_Basic playerTransBasic;
    private Player_Trans_Car playerTransCar;
    private Player_Trans_Drone playerTransDrone;

    public IPlayerTrans curTrans { get; private set; }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        // 테스트용
        playerTransBasic = new Player_Trans_Basic(this);
        playerTransCar = new Player_Trans_Car(this);
        playerTransDrone = new Player_Trans_Drone(this);

        curTrans = playerTransBasic;
        curTrans.Enter();
    }

    private void Update()
    {
        // 테스트용
        float horizontal = Input.GetAxisRaw("Horizontal");

        // 테스트용 변신: 1 = 기본, 2 = 자동차, 3 = 드론
        if (Input.GetKeyDown(KeyCode.Alpha1)) TransChange(playerTransBasic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TransChange(playerTransCar);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TransChange(playerTransDrone);
    }

    private void TransChange(IPlayerTrans newTrans) 
    { 
        curTrans.Exit();
        curTrans = newTrans;
        curTrans.Enter();
    }



    #region 점프, 상승 하강 관련 내용
    // 점프 명령
    public void OrderJump()
    {
        if (!isJump)
        {
            Jump();
        }
    }

    private void Jump()
    {
        isJump = true;
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCheck();
        }
    }

    public void OrderFly(float vertical) {
        Fly(vertical);
    }

    private void Fly(float vertical)
    {
        transform.Translate(Vector3.up * vertical * flySpeed * Time.deltaTime);
    }



    private void GroundCheck()
    {
        Vector2 Playertrans = transform.position;

        Playertrans.y -= 1.0f; // 플레이어 중심으로 놓으니 옆면도 인식이 되는것 같아서 중심에서 플레이어 발 밑으로 임시로 옮김
        RaycastHit2D hit = Physics2D.Raycast(Playertrans, Vector2.down, 0.5f, checkGroundLayer); //선을 아래방향으로 0.3f만큼 쏘아 레이어를 구분
        if (hit.collider != null) // 레이어가 checkGroundLayer과 동일시
        {
            isJump = false;
        }
    }

    #endregion

    #region 이동 관련

    // 이동 명령
    public void OrderMove(float direction)
    {
        curDirect = direction;

        if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        Move(direction);
    }

    private void Move(float direction)
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
        animator.SetFloat("TryMove", Mathf.Abs(direction));
    }
    #endregion
}