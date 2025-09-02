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
    [NonSerialized] public bool isAttack = false;
    private bool unableMove = false;
    private bool unableAttack = false;
    private bool isAssimilating = false;
    private bool isReleasing = false;
    private bool isHiting = false;
    private bool isDie = false;
    private bool isMove = false;

    // 흡수 가능 여부 변경 이벤트
    public event Action<bool> OnAbleAssChange;
    private bool _ableAss;
    public bool ableAss
    {
        get => _ableAss;
        set
        {
            if (_ableAss != value)
            {
                _ableAss = value;
                OnAbleAssChange?.Invoke(_ableAss);
                UI_Player_Attack.Instance.HandleAbleAssChange(_ableAss);
            }
        }
    }

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
    [NonSerialized] public Player_Trans_Basic playerTransBasic;
    [NonSerialized] public Player_Trans_Car playerTransCar;
    [NonSerialized] public Player_Trans_Drone playerTransDrone;
    [NonSerialized] public Player_Trans_Robot playerTransRobot;
    [NonSerialized] public Player_Trans_Res playerTransRes;

    public bool isTransDrone => curTrans == playerTransDrone;
    public bool isTransCar => curTrans == playerTransCar;


    public IPlayerTrans curTrans { get; private set; }
    public bool unableRel
    {
        get
        {
            if (curForm != AssimilateForm.None) return true;
            return curTrans == playerTransRes
                || curTrans == playerTransRobot
                || curTrans == playerTransDrone
                || curTrans == playerTransCar;
        }
    }

    [Header("Test Data")]
    [SerializeField] private GameObject blackOrbPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject TestHitboxPrefab;
    private GameObject dashHitbox;
    [SerializeField] private GameObject testAssboxPre;   // 흡수 프리팹
    private TestAssBox assBoxRange;  // 흡수 범위
    private BaseEnemy targetAssEnemy;

    private float nextSpeed;
    private float nextJump;
    private RuntimeAnimatorController nextAnim;

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

        // Create the TestAssBox prefab
        var obj = Instantiate(testAssboxPre, transform.position, Quaternion.identity, transform);
        TestAssBox assBox = obj.GetComponent<TestAssBox>();

        // Define the desired assimilation range
        float desAssRange = 1.75f;
        Vector3 offset = new Vector3(desAssRange, 0, 0);

        if (assBox != null)
        {
            Debug.Log("흡수 범위 등록 완료!");
            assBox.SetRange(desAssRange);
            assBoxRange = assBox;  // ★ 여기를 이렇게 수정
            assBoxRange.transform.localPosition = offset;
        }
    }

    private void Update()
    {

        // 테스트용 흡수: 1 = 기본, 2 = 자동차, 3 = 드론, T:변신하기
        if (Input.GetKeyDown(KeyCode.Alpha1)) TransChange(playerTransBasic);
        if (curTrans == playerTransBasic)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2)) OrderAssmiliate(AssimilateForm.Car);
            if (Input.GetKeyDown(KeyCode.Alpha3)) OrderAssmiliate(AssimilateForm.Drone);
            if (Input.GetKeyDown(KeyCode.Alpha4)) OrderAssmiliate(AssimilateForm.Robot);
            if (Input.GetKeyDown(KeyCode.Alpha5)) OrderAssmiliate(AssimilateForm.Res);
        }
        //if (curTrans == playerTransDrone)
        //    IdleDrone();
        UpdateHitboxOffset();
        Check_PlayerDie();

        playerCurHP = Mathf.Clamp(playerCurHP, 0, playerMaxHP);

        // 테스트용
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TakeHit(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (playerCurHP < playerMaxHP)
                TakeHeal(5);
        }

    }

    public void TransChange(IPlayerTrans newTrans)
    {
        curTrans.Exit();
        curTrans = newTrans;
        curForm = AssimilateForm.None;
        curTrans.Enter();
    }


    #region 점프, 상승 하강 관련 내용
    // 점프 명령
    public void OrderJump()
    {
        if (unableMove) return;
        if (!isJump)
        {
            Jump();
        }
    }

    private void Jump()
    {
        isJump = true;
        animator.SetBool("TryJump", true);
        if (rigid != null)
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
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

            animator.SetBool("TryJump", false);
        }
    }

    #endregion

    #region 이동 관련 내용

    // 이동 명령
    public void OrderMove(float direction)
    {
        if (unableMove)
        {
            return;
        }
        if (direction == 0)
        {
            isMove = false;
            animator.SetFloat("TryMove", 0f);
            return;
        }
        isMove = true;
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
        animator.SetFloat("TryMove", Mathf.Abs(direction));
    }

    #endregion 내용

    #region 흡수, 변신 관련 내용
    // 흡수 명령
    public void OrderAssmiliate(AssimilateForm form)
    {
        curForm = form;
        Debug.Log($"[{Time.time}] {form} 를 흡수함.");
    }

    // 흡수 -> 변신
    public void TryTransform()
    {
        if (isJump || isDie || isAttack || isHiting) return;

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
        if (isAssimilating || isAttack || isDie) return;

        if (assBoxRange == null)
        {
            Debug.LogWarning("흡수 범위 assBoxRange가 설정되지 않음");
            return;
        }
        BaseEnemy target = assBoxRange.GetFirstAssEnemy();
        if (target != null)
        {
            targetAssEnemy = target;
            animator.SetTrigger("TryAssimilate");
            isAssimilating = true;
            unableAttack = true;
            unableMove = true;
        }
        else
        {
            Debug.LogWarning("흡수할 적이 범위 안에 없음");
        }
    }

    public void OnAssAnimEnd()
    {
        if (targetAssEnemy != null)
        {
            AssimilateForm form = Convert_EnemyIDToForm(targetAssEnemy.enemyID);
            OrderAssmiliate(form);
            EnemyManager.Instance.UnregisterEnemy(targetAssEnemy);
            Destroy(targetAssEnemy.gameObject);
            targetAssEnemy = null;
        }
        isAssimilating = false;
        unableAttack = false;
        unableMove = false;
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
    public void TransApply(float speed, float jump, RuntimeAnimatorController animController)
    {
        if (isJump || isAttack) { return; }
        if (curTrans == playerTransBasic)
        {
            playerCurMoveSpeed = speed;
            jumpForce = jump;
            CurAnimator.runtimeAnimatorController = animController;
        }
        else
        {
            animator.SetTrigger("TryTrans");
            unableMove = true;
            nextSpeed = speed;
            nextJump = jump;
            nextAnim = animController;
        }
    }

    public void OnTransAnimEnd()
    {
        playerCurMoveSpeed = nextSpeed;
        jumpForce = nextJump;
        CurAnimator.runtimeAnimatorController = nextAnim;
        unableMove = false;

        curTrans.TransValueApply();
    }

    // 방출 명령
    public void OrderRealese()
    {
        if (isReleasing || isDie) return;

        Realese();
    }

    private void Realese()
    {
        TransChange(playerTransBasic);  // 일단 기본상태로 되돌아가고
        animator.SetTrigger("TryRelease");
        isReleasing = true;
        unableAttack = true;
        unableMove = true;
    }

    public void OnRealeseAnimEnd()
    {
        isReleasing = false;
        unableAttack = false;
        unableMove = false;
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
            orbRb.linearVelocity = new Vector2(direction * 45f, 0);  // 속도 조절
        }
    }

    #endregion

    #region 공격+몬스터 상호작용
    private void Check_PlayerDie()
    {
        if (playerCurHP <= 0)
        {
            isDie = true;
            PlayerPause();
            animator.SetTrigger("TryFailed");
        }
    }
    
    public void TakeHit(int damage)
    {
        if (isDie) return;
        if (!isAttack && !isHiting)
        {
            isHiting = true;
            PlayerPause();
            if (curTrans == playerTransDrone)
            {
                StartCoroutine(PlayerHitBack_Drone());
            }
            else
            {
                PlayerHitBack();
            }
            animator.SetTrigger("TryHit");
        }
        TakeDamage(damage);
    }

    public void OnHitEnd()
    {
        isHiting = false;
        PlayerContinue();
    }

    public void TakeHeal(int healindex)
    {
        TakeDamage(-healindex);
    }
    private void TakeDamage(int damage)
    {
        playerCurHP -= damage;
    }

    // 히트 백
    private void PlayerHitBack()
    {
        HitBack();
    }

    private IEnumerator PlayerHitBack_Drone()
    {
        float duration = 0.25f;       // 히트백 지속 시간
        HitBack();
        yield return new WaitForSeconds(duration);
        // 히트백 이후 속도 서서히 감속 (정지)
        rigid.linearVelocity = Vector2.zero;
    }

    private void HitBack()
    {
        float knockbackForce = 5f;   // 뒤로 밀리는 힘
        // float knockbackUp = 2f;      // 살짝 위로 뜨는 힘

        // 현재 바라보는 방향의 반대쪽으로
        float direction = transform.localScale.x > 0 ? -1f : 1f;

        // 기존 속도 초기화 후 넉백 적용
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(new Vector2(direction * knockbackForce, 0), ForceMode2D.Impulse);
    }


    // 멈춤
    public void PlayerPause()
    {
        unableMove = true;
        unableAttack = true;
    }
    // 재개
    public void PlayerContinue()
    {
        unableMove = false;
        unableAttack = false;
    }
    // 사망
    public void PlayerDie()
    {
        isPlayerLive = false;
        PlayerPause();
    }
    // 부활
    public void PlayerRevive()
    {
        PlayerContinue();
        playerCurHP = playerMaxHP;
        isPlayerLive = true;
        isDie = false;
        Debug.Log($"{Time.deltaTime}: 플레이어 부활! 체력: {playerCurHP}");
    }


    public void OrderAttack()
    {
        if (unableMove || unableAttack) return;
        curTrans.Attack();
    }
    public void OrderTransSkill() => curTrans.TransSkill();

    public void BasicAttack_anim()
    {
        StartAttack();
        Vector3 spawnPos = firePoint.position;
        // float direction = transform.localScale.x > 0 ? 1 : -1;
        // float dashDistance = 0.85f;
        // transform.Translate(Vector3.right * direction * dashDistance);
        // StartCoroutine(DashForward(direction * dashDistance));

        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 0) animator.SetTrigger("TryAttack1");
        else animator.SetTrigger("TryAttack2");
    }

    // 애니메이션 이벤트로 실행
    public void BasicAttack()
    {
        float direction = transform.localScale.x > 0 ? 1 : -1;
        CreateHitBox(direction, new Vector3(2f, 0f, 0f), 1.75f);
    }


    private IEnumerator DashForward(float direction)
    {
        float dashTime = 0.225f; // 이동 지속 시간
        float dashSpeed = 7.5f;   // 이동 속도
        float elapsed = 0.05f;

        while (elapsed < dashTime)
        {
            transform.Translate(Vector3.right * direction * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void JumpAttack()
    {
        if (isJumpAttack) return;
        isJumpAttack = true;
        StartAttack();

        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 3.5f;
        animator.SetBool("TryJumpAttack", true);
        // Invoke(nameof(JumpUpperAttack), 0.2f);
    }


    private void JumpUpperAttack()
    {
        float direction = transform.localScale.x > 0 ? 1 : -1;
        Vector2 forceDir = new Vector2(direction, 1).normalized;
        rigid.AddForce(forceDir * 15f, ForceMode2D.Impulse);

        CreateHitBox(direction, new Vector3(2f, 0f, 0f), 1.5f);
        Invoke(nameof(EndJumpAttack), 0.35f);
    }

    private void EndJumpAttack()
    {
        isJumpAttack = false;
        rigid.gravityScale = 8f;
        animator.SetBool("TryJumpAttack", false);
        EndAttack();
    }


    public void Trans_RobotAttack()
    {
        StartAttack();
        animator.SetTrigger("TryAttack");
        Test_FireBlackObj();
    }

    public void Trans_DashAttack(float dashPower, float dashDuration, Vector3 HitPos, float size)
    {
        if (isTransDashing) return;
        StartAttack();
        animator.SetTrigger("TryAttack");
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
        isAttack = true;
        PlayerPause();
    }

    private void EndAttack()
    {
        isAttack = false;
        PlayerContinue();
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