using System;
using System.Collections;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float curDirect { get; private set; }    // 현재 이동방향

    [Header("Player State")]
    public string playerID = "player001";
    public float playerMaxHP = 100f;
    [NonSerialized] public float playerCurHP;
    [NonSerialized] public bool isPlayerLive = true;

    public float playerMoveSpeed = 8;
    private float playerCurMoveSpeed;

    [SerializeField] private float flySpeed = 5;
    [SerializeField] private float jumpForce;
    [NonSerialized] public bool isJump;
    private bool isJumpAttack = false;
    [NonSerialized] public bool isTransDashing = false;
    private bool unableMove = false;
    [NonSerialized] public bool isAttack = false;


    [SerializeField] private LayerMask checkGroundLayer; // 레이어
    [NonSerialized] public Rigidbody2D rigid;
    [NonSerialized] public CapsuleCollider2D collider;


    // 현재 SpriteRenderer, Animator, 콜라이더
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer CurSpriteRenderer => spriteRenderer;

    [SerializeField] private Animator animator;
    public Animator CurAnimator => animator;
    public enum AssimilateForm  // 흡수상태 폼
    {
        None,
        Car,
        Drone,
        Robot,
        Res
    }
    public AssimilateForm curForm = AssimilateForm.None;


    // 상태별 스프라이트 + 애니메이션 컨트롤러
    [Header("Basic State")]
    [SerializeField] private RuntimeAnimatorController basicAnimController;
    public RuntimeAnimatorController BasicAnimController => basicAnimController;

    [Header("Car State")]
    [SerializeField] private RuntimeAnimatorController carAnimController;
    public RuntimeAnimatorController CarAnimController => carAnimController;


    [Header("Drone State")]
    [SerializeField] private RuntimeAnimatorController droneAnimController;
    public RuntimeAnimatorController DroneAnimController => droneAnimController;

    [Header("Robot State")]
    [SerializeField] private RuntimeAnimatorController robotAnimController;
    public RuntimeAnimatorController RobotAnimController => robotAnimController;

    [Header("Res State")]
    [SerializeField] private RuntimeAnimatorController resAnimController;
    public RuntimeAnimatorController ResAnimController => resAnimController;


    // 변신 상태 객체 + 테스트용
    private Player_Trans_Basic playerTransBasic;
    private Player_Trans_Car playerTransCar;
    private Player_Trans_Drone playerTransDrone;
    private Player_Trans_Robot playerTransRobot;
    private Player_Trans_Res playerTransRes;

    public bool isTransDrone => curTrans == playerTransDrone;
    public bool isTransCar => curTrans == playerTransCar;   


    [Header("Test Data")]
    public IPlayerTrans curTrans { get; private set; }
    public IPlayerRelease curRelease { get; private set; }

    [SerializeField] private GameObject blackOrbPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject TestHitboxPrefab;
    private GameObject dashHitbox;
    [SerializeField] private GameObject testAssboxPre;   // 흡수 범위
    private float assRange;  // 흡수 범위

    [SerializeField] private GameObject testCarSkill;
    [SerializeField] private GameObject testDroneSkill;
    [SerializeField] private GameObject testRobotSkill;


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
        playerTransRobot = new Player_Trans_Robot(this);
        playerTransRes = new Player_Trans_Res(this);

        curTrans = playerTransBasic;
        curTrans.Enter();
    }

    private void Start()
    {
        InitPlayerState();
    }

    private void InitPlayerState()
    {
        RuntimeGameManager.Instance.Regist_Player(this);

        playerCurHP = playerMaxHP;
        playerCurMoveSpeed = playerMoveSpeed;

        Vector3 offset = transform.right * 1.5f;
        var obj = Instantiate(testAssboxPre, transform.position + offset, Quaternion.identity, transform);
        assRange = obj.transform.localScale.x * 1.2f;
    }


    private void Update()
    {
        // 테스트용
        float horizontal = Input.GetAxisRaw("Horizontal");

        // 테스트용 흡수: 1 = 기본, 2 = 자동차, 3 = 드론, T:변신하기
        if (Input.GetKeyDown(KeyCode.Alpha1)) TransChange(playerTransBasic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OrderAssmiliate(AssimilateForm.Car);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OrderAssmiliate(AssimilateForm.Drone);
        if (Input.GetKeyDown(KeyCode.Alpha4)) OrderAssmiliate(AssimilateForm.Robot);
        if (Input.GetKeyDown(KeyCode.Alpha5)) OrderAssmiliate(AssimilateForm.Res);
        // if (Input.GetKeyDown(KeyCode.T)) TryTransform();


        if (Input.GetKeyDown(KeyCode.Alpha8)) TakeDamage(500);

        //if (curTrans == playerTransDrone)
        //    IdleDrone();
        UpdateHitboxOffset();
        Check_PlayerDie();
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")|| collision.gameObject.CompareTag("Box"))
        {
            GroundCheck();
        }
    }

    public void OrderFly(float vertical)
    {
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

    #endregion

    #region 이동 관련 내용

    // 이동 명령
    public void OrderMove(float direction)
    {
        if (unableMove)
        {
            // Debug.LogWarning($"{Time.deltaTime} 현재 플레이어는 움직이지 못함!");
            return;
        }
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
        transform.Translate(Vector3.right * direction * playerCurMoveSpeed * Time.deltaTime);
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
        else if (curTrans == playerTransRobot)
        {
            animator.SetFloat("TryMove", Mathf.Abs(direction));
        }
        else if (curTrans == playerTransRes)
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
            case AssimilateForm.Robot:
                TransChange(playerTransRobot);
                break;
            case AssimilateForm.Res:
                TransChange(playerTransRes);
                break;
            default:
                Debug.LogWarning($"{Time.deltaTime}: 변신 가능한 대상이 없습니다.");
                break;
        }
    }

    public void TryAssimilate()
    {
        BaseEnemy target = EnemyManager.Instance.GetAssimilateEnemy(transform.position, assRange);
        if (target != null)
        {
            AssimilateForm form = Convert_EnemyIDToForm(target.enemyID);
            OrderAssmiliate(form);
            Debug.Log($"{target.name} 흡수!");

            Destroy(target.gameObject); // 흡수 후 제거
            EnemyManager.Instance.UnregisterEnemy(target); // 리스트에서 제거

            
        }
        else
        {
            Debug.LogWarning("흡수할 적이 범위 안에 없음");
        }
    }

    public AssimilateForm Convert_EnemyIDToForm(int enemyID)
    {
        switch (enemyID)
        {
            case 001: return AssimilateForm.Car;
            case 002: return AssimilateForm.Drone;
            case 003: return AssimilateForm.Robot;
            case 004: return AssimilateForm.Res;
            default: return AssimilateForm.None;
        }
    }



    // 변신
    public void TransApply(float speed, float jump, Sprite sprite, RuntimeAnimatorController animController)
    {
        playerCurMoveSpeed = speed;
        jumpForce = jump;
        CurSpriteRenderer.sprite = sprite;
        CurAnimator.runtimeAnimatorController = animController;
    }
    public void TransApply(float speed, float jump, RuntimeAnimatorController animController)
    {
        playerCurMoveSpeed = speed;
        jumpForce = jump;
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
            Debug.LogError("BlackOrbPrefab 또는 FirePoint가 설정 x");
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

    #region 공격+몬스터 상호작용
    private void Check_PlayerDie()
    {
        if (playerCurHP <= 0)
        {
            isPlayerLive = false;
        }
    }
    public void TakeDamage(int damage)
    {
        playerCurHP -= damage;
        Debug.Log($"{Time.deltaTime}: {damage}피해를 입음! 현재 체력: {playerCurHP}");
    }
    // 멈춤
    public void PlayerPause()
    {
        unableMove = true;
    }
    // 재개
    public void PlayerContinue()
    {
        unableMove = false;
    }
    // 사망
    public void PlayerDie()
    {
        PlayerPause();
        // animator.SetTrigger("TryDie");
    }
    // 부활
    public void PlayerRevive()
    {
        PlayerContinue();
        playerCurHP = playerMaxHP;
        isPlayerLive = true;
        Debug.Log($"{Time.deltaTime}: 플레이어 부활! 체력: {playerCurHP}");
    }


    public void OrderAttack() => curTrans.Attack();
    public void OrderTransSkill() => curTrans.TransSkill();

    public void BasicAttack()
    {
        //StartAttack();
        Vector3 spawnPos = firePoint.position;
        float direction = transform.localScale.x > 0 ? 1 : -1;
        CreateHitBox(direction, new Vector3(2f, 0f, 0f), 1f);
        //EndAttack();

        // 오른쪽 또는 왼쪽으로 히트박스 생성
        //spawnPos += new Vector3(direction * 1.0f, 0f, 0f);
        //GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        //atk.transform.localScale = new Vector3(direction, 1, 1); // 뒤집히는 방향 반영
    }

    public void JumpAttack()
    {
        //StartAttack();
        if (isJumpAttack) return;
        isJumpAttack = true;

        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 0f;

        Invoke(nameof(DoSlamAttack), 0.2f);
    }

    private void DoSlamAttack()
    {
        rigid.gravityScale = 8f;

        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(Vector2.down * 30f, ForceMode2D.Impulse);

        Vector3 spawnPos = firePoint.position + new Vector3(0f, -0.7f, 0f);
        GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
        atk.transform.localScale = Vector3.one;
        //EndAttack();
    }

    //private void JumpAttack2()
    //{
    //    if (isJumpAttack) return;
    //    isJumpAttack = true;

    //    // 아래 대각선 방향으로
    //    float direction = transform.localScale.x > 0 ? 1 : -1;
    //    Vector2 slamDir = new Vector2(direction, -1).normalized;

    //    rigid.linearVelocity = Vector2.zero;
    //    rigid.AddForce(slamDir * 20f, ForceMode2D.Impulse);

    //    // 히트박스 생성
    //    Vector3 spawnPos = firePoint.position + new Vector3(direction * 0.5f, -0.5f, 0f);
    //    GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
    //    atk.transform.localScale = new Vector3(direction, 1, 1);
    //}

    //private void JumpAttack3()
    //{
    //    if (isJumpAttack) return;
    //    isJumpAttack = true;

    //    // rigid.velocity = Vector2.zero;
    //    rigid.linearVelocity = Vector2.zero;
    //    rigid.AddForce(Vector2.down * 25f, ForceMode2D.Impulse);

    //    // 히트박스 생성
    //    Vector3 spawnPos = firePoint.position + new Vector3(0f, -0.7f, 0f);
    //    GameObject atk = Instantiate(TestHitboxPrefab, spawnPos, Quaternion.identity);
    //    atk.transform.localScale = new Vector3(1, 1, 1);
    //}


    public void Trans_RobotAttack()
    {
        StartAttack();
        animator.SetTrigger("TryAttack");
        Test_FireBlackObj();
    }

    public void Trans_DashAttack(float dashPower, float dashDuration, Vector3 HitPos, float size)
    {
        if (isTransDashing) return;
        isTransDashing = true;
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        rigid.linearVelocity = Vector2.zero;
        // rigid.gravityScale = 0;
        rigid.AddForce(Vector2.right * direction * dashPower, ForceMode2D.Impulse);
        CreateHitBox(direction, HitPos, size);
        Invoke(nameof(EndDash), dashDuration);
    }

    private void CreateHitBox(float direction, Vector3 customOffset, float size)
    {
        Vector3 offset = new Vector3(customOffset.x * direction, customOffset.y, 0f);
        dashHitbox = Instantiate(TestHitboxPrefab, transform.position + offset, Quaternion.identity);
        dashHitbox.transform.localScale *= size;

        if (direction < 0)
            dashHitbox.transform.rotation = Quaternion.Euler(0f, 180f, 0f); // 좌우 반전
    }

    private void UpdateHitboxOffset()
    {
        if (isTransDashing && dashHitbox != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            Vector3 offset = new Vector3(direction * 1f, 0f, 0f);  // 앞부분 오프셋
            dashHitbox.transform.position = transform.position + offset;
        }
    }

    private void EndDash()
    {
        isTransDashing = false;
        if (curTrans != playerTransDrone)
            rigid.gravityScale = 8f;
        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Dynamic;
    }

    private void StartAttack()
    {
        unableMove = true;
    }

    // 이건 anim 이벤트로
    private void EndAttack()
    {
        unableMove = false;
    }


    private IEnumerator BasicReturnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TransChange(playerTransBasic);
        curForm = AssimilateForm.None;
    }


    public void TransSkill_Drone()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        GameObject orb = Instantiate(testDroneSkill, firePoint.position, Quaternion.identity);

        orb.GetComponent<TestCarSkill>().InitializeDirection(direction);
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            orbRb.linearVelocity = new Vector2(direction * 50f, 0);
        }

        // 끝난뒤에? (추후 애니메이션 연출 추가 = 원래대로 돌아가는 모션)
        StartCoroutine(BasicReturnDelay(0.35f));
    }

    public void TransSkill_Car()
    {
        // 방출 이펙트 + 사운드
        //if (carTransSkill_Effect != null)
        //    Instantiate(carTransSkill_Effect, transform.position, Quaternion.identity);
        //if (carTransSkill_Sound != null)
        //    AudioSource.PlayClipAtPoint(carTransSkill_Sound, transform.position);
        // 스킬 오브젝트 생성
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        GameObject orb = Instantiate(testCarSkill, firePoint.position, Quaternion.identity);
        orb.GetComponent<TestCarSkill>().InitializeDirection(direction);
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            orbRb.linearVelocity = new Vector2(direction * 50f, 0);
        }

        // 끝난뒤에? (추후 애니메이션 연출 추가 = 원래대로 돌아가는 모션)
        StartCoroutine(BasicReturnDelay(0.35f));
    }

    public void TransSkill_Robot()
    {
        StartAttack();
        animator.SetTrigger("TryAttack");
        // 방출 이펙트 + 사운드
        //if (carTransSkill_Effect != null)
        //    Instantiate(carTransSkill_Effect, transform.position, Quaternion.identity);
        //if (carTransSkill_Sound != null)
        //    AudioSource.PlayClipAtPoint(carTransSkill_Sound, transform.position);

        float direction = transform.localScale.x > 0 ? 1f : -1f;

        GameObject grenade = Instantiate(testRobotSkill, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 10f;
            float angle = 45f * Mathf.Deg2Rad;
            float speed = 25f;
            Vector2 launchVelocity = new Vector2(Mathf.Cos(angle) * speed * direction,
                                                 Mathf.Sin(angle) * speed);
            rb.linearVelocity = launchVelocity;
        }

        // 이후 폭발 처리 (grenade 프리팹 스크립트에서 OnCollisionEnter2D로)
        StartCoroutine(BasicReturnDelay(0.30f));
    }


    public void TransSkill_Res()
    { 
    
    }

    #endregion
}