using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float curDirect { get; private set; }    // 현재 이동방향

    [Header("Player State")]
    [SerializeField] private float moveSpeed = 8;
    [SerializeField] private float flySpeed = 5;
    [SerializeField] private float jumpForce;
    [NonSerialized] public bool isJump;
    private bool isJumpAttack;
    public bool isAttack = false;
    private bool isDashing = false;
    // private bool isMoving = false;

    [SerializeField] private LayerMask checkGroundLayer; // 레이어
    [NonSerialized] public Rigidbody2D rigid;
    [NonSerialized] public CapsuleCollider2D collider;


    // 현재 SpriteRenderer, Animator, 콜라이더
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer CurSpriteRenderer => spriteRenderer;

    [SerializeField] private Animator animator;
    public Animator CurAnimator => animator;



    public enum AssimilateForm  // 얜 흡수상태 폼인데?
    {
        None,
        Car,
        Drone
    }
    public AssimilateForm curForm = AssimilateForm.None;

    //public enum TransForm
    //{ 
    //    Basic,
    //    Car,
    //    Drone
    //}
    //public TransForm curTrans = TransForm.Basic;

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


    // 변신 상태 객체 + 테스트용
    private Player_Trans_Basic playerTransBasic;
    private Player_Trans_Car playerTransCar;
    private Player_Trans_Drone playerTransDrone;
    public bool isTransDrone => curTrans == playerTransDrone;

    [Header("Test Data")]
    public IPlayerTrans curTrans { get; private set; }
    public IPlayerRelease curRelease { get; private set; }

    [SerializeField] private GameObject blackOrbPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject TestHitboxPrefab;
    private GameObject dashHitbox;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();

        // 테스트용
        playerTransBasic = new Player_Trans_Basic(this);
        playerTransCar = new Player_Trans_Car(this);
        playerTransDrone = new Player_Trans_Drone(this);

        curTrans = playerTransBasic;
        curTrans.Enter();
    }


    private void Update()
    {
        // GroundAutoCheck();

        // 테스트용
        float horizontal = Input.GetAxisRaw("Horizontal");

        // 테스트용 흡수: 1 = 기본, 2 = 자동차, 3 = 드론, T:변신하기
        if (Input.GetKeyDown(KeyCode.Alpha1)) TransChange(playerTransBasic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OrderAssmiliate(AssimilateForm.Car);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OrderAssmiliate(AssimilateForm.Drone);
        if (Input.GetKeyDown(KeyCode.T)) TryTransform();

        if (curTrans == playerTransDrone)
            IdleDrone();

        UpdateHitboxOffset();
    }

    public void TransChange(IPlayerTrans newTrans) 
    {
        curTrans.Exit();
        curTrans = newTrans;
        curTrans.Enter();

        if (newTrans != playerTransBasic && newTrans is IPlayerRelease releaseable)
        {
            curRelease = releaseable;
        }
        else
        {
            curRelease = null;
        }
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
        if (rigid != null)
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
        Vector2 playerPos = transform.position;
        playerPos.y -= 1.0f; 
        RaycastHit2D hit = Physics2D.Raycast(playerPos, Vector2.down, 0.5f, checkGroundLayer); 
        if (hit.collider != null)
        {
            isJump = false;
            isJumpAttack = false;
        }
    }

    private void IdleDrone() {
        Debug.LogWarning("현재 드론상태입니당");
    }

    #endregion

    #region 이동 관련 내용

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
        if (curTrans == playerTransBasic) 
            animator.SetFloat("TryMove", Mathf.Abs(direction));
        else if (curTrans == playerTransCar)
        {
            animator.SetFloat("TryMove", Mathf.Abs(direction));
        }
        else if (curTrans == playerTransDrone)
        {
            animator.SetFloat("TryMove", Mathf.Abs(direction));
        }
    }

    #endregion 내용

    #region 흡수, 변신 관련 내용
    // 흡수 명령
    public void OrderAssmiliate(AssimilateForm form)
    {
        curForm = form;
        Debug.LogWarning($"[{Time.time}] {form} 를 흡수함.");
    }

    // 흡수 -> 변신
    public void TryTransform()
    {
        switch (curForm)
        {
            case AssimilateForm.Drone:
                TransChange(playerTransDrone);
                break;
            case AssimilateForm.Car:
                TransChange(playerTransCar);
                break;
            default:
                Debug.LogWarning($"{Time.deltaTime}: 변신 가능한 대상이 없습니다.");
                break;
        }
    }

    // 변신
    public void TransApply(float speed, float jump, Sprite sprite, RuntimeAnimatorController animController)
    {
        moveSpeed = speed;
        jumpForce = jump;
        CurSpriteRenderer.sprite = sprite;
        CurAnimator.runtimeAnimatorController = animController;
    }

    // 방출 명령
    public void OrderRealese()
    {
        Realese();
    }

    private void Realese() 
    {
        TransChange(playerTransBasic);  // 일단 기본상태로 되돌아가고
        curForm = AssimilateForm.None;
        Test_FireBlackObj();    // 테스트용
    }

    public void Test_FireBlackObj()
    {
        if (blackOrbPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BlackOrbPrefab 또는 FirePoint가 설정 x");
            return;
        }

        GameObject orb = Instantiate(blackOrbPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f; // 바라보는 방향
            orbRb.linearVelocity = new Vector2(direction * 35f, 0);  // 속도 조절
        }
    }

    #endregion

    #region 공격+몬스터와 상호작용
    public void OrderAttack() => curTrans.Attack();

    public void BasicAttack()
    {
        Vector3 spawnPos = firePoint.position;
        float direction = transform.localScale.x > 0 ? 1 : -1;
        CreateHitBox(direction);


        // 오른쪽 또는 왼쪽으로 히트박스 생성
        //spawnPos += new Vector3(direction * 1.0f, 0f, 0f);
        //GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        //atk.transform.localScale = new Vector3(direction, 1, 1); // 뒤집히는 방향 반영
    }

    public void JumpAttack() 
    {
        if(isJumpAttack) return;
        isJumpAttack = true;

        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 0f; 

        Invoke(nameof(DoSlamAttack), 0.2f);
    }

    private void DoSlamAttack()
    {
        rigid.gravityScale = 2.5f;

        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(Vector2.down * 30f, ForceMode2D.Impulse);

        Vector3 spawnPos = firePoint.position + new Vector3(0f, -0.7f, 0f);
        GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        atk.transform.localScale = Vector3.one;

        Debug.Log($"{Time.deltaTime} 낙하 공격!");
    }

    private void JumpAttack2()
    {
        if (isJumpAttack) return;
        isJumpAttack = true;

        // 아래 대각선 방향으로
        float direction = transform.localScale.x > 0 ? 1 : -1;
        Vector2 slamDir = new Vector2(direction, -1).normalized;

        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(slamDir * 20f, ForceMode2D.Impulse);

        // 히트박스 생성
        Vector3 spawnPos = firePoint.position + new Vector3(direction * 0.5f, -0.5f, 0f);
        GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        atk.transform.localScale = new Vector3(direction, 1, 1);
    }

    private void JumpAttack3() {
        if (isJumpAttack) return;
        isJumpAttack = true;

        // rigid.velocity = Vector2.zero;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(Vector2.down * 25f, ForceMode2D.Impulse);

        // 히트박스 생성
        Vector3 spawnPos = firePoint.position + new Vector3(0f, -0.7f, 0f);
        GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        atk.transform.localScale = new Vector3(1, 1, 1);
    }

    //public void CarDashAttack(float dashPower, float dashDuration)
    //{
    //    if (isDashing) return;

    //    isDashing = true;
    //    float direction = transform.localScale.x > 0 ? 1f : -1f;

    //    rigid.linearVelocity = Vector2.zero;
    //    rigid.gravityScale = 0; // 수평 유지
    //    rigid.AddForce(Vector2.right * direction * dashPower, ForceMode2D.Impulse);

    //    // 돌진 히트박스 생성
    //    CreateHitBox(direction);
    //    Invoke(nameof(EndDash), dashDuration);
    //}

    //private void CreateHitBox(float direction)
    //{
    //    Vector3 spawnPos = firePoint.position + new Vector3(direction * 1f, 0f, 0f);
    //    GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity, transform); // 플레이어에 붙여서 같이 이동
    //    dashHitbox.transform.localScale = new Vector3(direction, 1, 1);
    //}

    //private void EndDash()
    //{
    //    isDashing = false;
    //    rigid.linearVelocity = Vector2.zero;
    //    rigid.gravityScale = 2.5f; // 원래대로
    //}


    public void CarDashAttack(float dashPower, float dashDuration)
    {
        if (isDashing) return;

        isDashing = true;
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 0;
        rigid.AddForce(Vector2.right * direction * dashPower, ForceMode2D.Impulse);

        CreateHitBox(direction);
        Invoke(nameof(EndDash), dashDuration);
    }

    private void CreateHitBox(float direction)
    {
        Vector3 offset = new Vector3(direction * 1f, 0f, 0f);
        dashHitbox = Instantiate(TestHitboxPrefab, transform.position + offset, Quaternion.identity);
        // dashHitbox.transform.localScale = new Vector3(direction, 1, 1); <- 크기 조절

        if (direction < 0)
            dashHitbox.transform.rotation = Quaternion.Euler(0f, 180f, 0f); // 좌우 반전
    }

    private void UpdateHitboxOffset() 
    {
        if (isDashing && dashHitbox != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            Vector3 offset = new Vector3(direction * 1f, 0f, 0f);  // 앞부분 오프셋
            dashHitbox.transform.position = transform.position + offset;
        }
    }

    private void EndDash()
    {
        isDashing = false;
        rigid.gravityScale = 8f;
        rigid.linearVelocity = Vector2.zero;       
    }
    

    #endregion
}