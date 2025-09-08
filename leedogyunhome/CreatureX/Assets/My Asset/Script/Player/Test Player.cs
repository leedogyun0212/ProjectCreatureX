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
    public bool isJump;
    [NonSerialized] public bool isGround;
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


    public AudioSource audioSource;

    public AudioSource WalkaudioSource;

    public PlayerSound soundData;

    private AudioClip lastClip = null;

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

    [SerializeField] private GameObject relCar;
    [SerializeField] private GameObject relDrone;
    [SerializeField] private GameObject relRobot;
    [SerializeField] private GameObject relRes;
    private GameObject relObj;


    [SerializeField] private GameObject testCarSkill;
    [SerializeField] private GameObject testDroneSkill;
    [SerializeField] private GameObject testRobotSkill;


    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();

        // AudioSource 초기화
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

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
        var obj = Instantiate(testAssboxPre, transform.position, Quaternion.identity, transform);
        TestAssBox assBox = obj.GetComponent<TestAssBox>();

        float desAssPos = 1.25f;
        Vector3 offset = new Vector3(desAssPos, 0, 0);

        if (assBox != null)
        {
            assBoxRange = assBox;
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

        animator.SetBool("TryJump", isJump);
    }

    private float _pointTime = 1.0f; // 1초마다 실행
    private float _nextTime = 0.0f;


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
        if (isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        isJump = true;
        isGround = false; // 점프 시작 시 땅 상태 해제
        // animator.SetBool("TryJump", true);
        if (rigid != null)
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
        {
            isGround = true;
            isJump = false;
            // animator.SetBool("TryJump", isJump);
        }
        //else {
        //    isJump = true;
        //    isGround = false;
        //}

        //if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
        //&& rigid.linearVelocity.y <= 0.1f)
        //{
        //    GroundCheck();
        //}
    }

    // 땅에 닿는 순간
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // 충돌이 감지된 오브젝트가 땅 또는 박스 태그인지 확인
    //    if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
    //    {
    //        // 충돌이 발생한 지점의 법선 벡터를 확인
    //        // 점프하여 땅에 착지했을 경우, 충돌 법선은 위쪽(Vector2.up)을 향합니다.
    //        // Dot product는 두 벡터의 방향 유사성을 측정합니다.
    //        // 0.9f는 거의 완벽하게 위에서 충돌했음을 의미합니다.
    //        if (Vector2.Dot(collision.contacts[0].normal, Vector2.up) > 0.9f)
    //        {
    //            isGround = true;
    //            isJump = false;
    //            animator.SetBool("TryJump", false);
    //        }
    //    }
    //}

    // 땅에서 떨어지는 순간
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isGround && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box")))
        {
            // 플레이어 발 아래에 땅이 없음을 Raycast로 다시 확인
            if (!IsGroundedWithRaycast())
            {
                isJump = true;
                isGround = false;
            }
        }
    }

    // Raycast를 사용해 현재 발 아래에 땅이 있는지 확인
    private bool IsGroundedWithRaycast()
    {
        Vector2 playerPos = transform.position;
        Vector2 raycastStartPos = new Vector2(playerPos.x, playerPos.y - 1.0f);
        float rayDistance = 0.5f;
        Debug.DrawRay(raycastStartPos, Vector2.down * rayDistance, Color.green);

        RaycastHit2D hit = Physics2D.Raycast(raycastStartPos, Vector2.down, rayDistance, checkGroundLayer);

        if (hit.collider != null)
        {
            isJump = false;
            isJumpAttack = false;
            // animator.SetBool("TryJump", false);
        }

        return hit.collider != null;
    }


    //private void GroundCheck()
    //{
    //    //Vector2 playerPos = transform.position;
    //    //playerPos.y -= 1.0f;
    //    //RaycastHit2D hit = Physics2D.Raycast(playerPos, Vector2.down, 0.5f, checkGroundLayer);

    //    Vector2 playerPos = transform.position;
    //    Vector2 raycastStartPos = new Vector2(playerPos.x, playerPos.y - 1.0f);
    //    float rayDistance = 0.5f;


    //    Debug.DrawRay(raycastStartPos, Vector2.down * rayDistance, Color.red);

    //    RaycastHit2D hit = Physics2D.Raycast(raycastStartPos, Vector2.down, rayDistance, checkGroundLayer);

    //    if (hit.collider != null)
    //    {
    //        isJump = false;
    //        isJumpAttack = false;
    //        animator.SetBool("TryJump", false);
    //    }
    //}

    public void OrderFly(float vertical)
    {
        Fly(vertical);
    }

    private void Fly(float vertical)
    {
        transform.Translate(Vector3.up * vertical * flySpeed * Time.deltaTime);
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
        // Debug.Log($"[{Time.time}] {form} 를 흡수함.");
    }

    // 흡수 -> 변신
    public void TryTransform()
    {
        if (isJump || isDie || isAttack || isHiting) return;

        TransSound();

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
                // Debug.LogWarning($"{Time.deltaTime}: 변신 가능한 대상이 없습니다.");
                break;
        }
    }

    public void TryAssimilate()
    {
        if (isAssimilating || isAttack || isDie) return;

        if (assBoxRange == null)
        {
            return;
        }
        BaseEnemy target = assBoxRange.GetFirstAssEnemy();
        if (target != null)
        {
            targetAssEnemy = target;

            // 흡수 소리 재생
            AssimilateSound();

            animator.SetTrigger("TryAssimilate");
            isAssimilating = true;
            unableAttack = true;
            unableMove = true;
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
            playerCurHP = playerMaxHP;
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
        if (isJump) return;
        if (isReleasing || isDie || !unableRel) return;

        releaseSound();

        if (curTrans != playerTransBasic)
        {
            if (curTrans == playerTransCar)
                relObj = relCar;
            else if (curTrans == playerTransDrone)
                relObj = relDrone;
            else if (curTrans == playerTransRobot)
                relObj = relRobot;
            else if (curTrans == playerTransRes)
                relObj = relRes;
            TransChange(playerTransBasic);
            Release();
        }
        else
        {
            if (curForm == AssimilateForm.Car)
                relObj = relCar;
            else if (curForm == AssimilateForm.Drone)
                relObj = relDrone;
            else if (curForm == AssimilateForm.Robot)
                relObj = relRobot;
            else if (curForm == AssimilateForm.Res)
                relObj = relRes;
            curForm = AssimilateForm.None;
            Release();
        }
    }

    private void Release()
    {
        StartAttack();
        animator.SetTrigger("TryRelease");
        isReleasing = true;
        StartCoroutine(LateRealese());
    }

    private IEnumerator LateRealese()
    {
        yield return new WaitForSecondsRealtime(0.81f);

        EffectManager.Play("P_Release", this.gameObject, this.transform.localScale.x < 0, new Vector3(1.40f, 1.1f, 0f));
    }

    public void OnReleaseAnimEnd()
    {
        EndAttack();
        isReleasing = false;
    }
    
    public void RelObject()
    {
        GameObject orb = Instantiate(relObj, firePoint.position, Quaternion.identity);

        SpriteRenderer sr = orb.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.black;
        }

        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            orbRb.linearVelocity = new Vector2(direction * 45f, 0);
        }
    }

    public void Test_FireBlackObj()
    {
        GameObject orb = Instantiate(blackOrbPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            orbRb.linearVelocity = new Vector2(direction * 45f, 0);
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

        HitSound();

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
        float hitBack = 0.25f;
        HitBack();
        yield return new WaitForSeconds(hitBack);
        rigid.linearVelocity = Vector2.zero;
    }

    private void HitBack()
    {
        float knockbackForce = 5f;
        // float knockbackUp = 2f;  

        // 현재 바라보는 방향 반대 방향
        float direction = transform.localScale.x > 0 ? -1f : 1f;

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
        DeadSound();
        PlayerPause();
    }
    // 부활
    public void PlayerRevive()
    {
        PlayerContinue();
        playerCurHP = playerMaxHP;
        isPlayerLive = true;
        isDie = false;
    }


    public void OrderAttack()
    {
        if (unableMove || unableAttack) return;

        // 공격 소리 재생
        AttackSound();

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

    public void BasicAttack()
    {
        float direction = transform.localScale.x > 0 ? 1 : -1;
        CreateHitBox(direction, new Vector3(2f, 0f, 0f), 1.75f);
    }

    private IEnumerator DashForward(float direction)
    {
        float dashTime = 0.225f;  // 이동 지속 시간
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
    }

    private void JumpUpperAttack()
    {
        float direction = transform.localScale.x > 0 ? 1 : -1;
        Vector2 forceDir = new Vector2(direction, 1).normalized;
        rigid.AddForce(forceDir * 10f, ForceMode2D.Impulse);

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
            dashHitbox.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
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

        StartCoroutine(BasicReturnDelay(0.35f));
    }

    public void TransSkill_Car()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        GameObject orb = Instantiate(testCarSkill, firePoint.position, Quaternion.identity);
        orb.GetComponent<TestCarSkill>().InitializeDirection(direction);

        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb != null)
        {
            orbRb.linearVelocity = new Vector2(direction * 50f, 0);
        }
        StartCoroutine(BasicReturnDelay(0.35f));
    }

    public void TransSkill_Robot()
    {
        StartAttack();
        animator.SetTrigger("TryAttack");

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

        StartCoroutine(BasicReturnDelay(0.30f));
    }


    public void TransSkill_Res()
    {

    }

    #endregion

    #region 소리 관련 내용
    public void WalkSound()
    {
        AudioClip Playclip = null;
        if (curTrans == playerTransBasic || curTrans == playerTransRes)
            Playclip = soundData.WalkBasic;
        else if (curTrans == playerTransRobot)
            Playclip = soundData.WalkRobot;
        else if (curTrans == playerTransDrone)
            Playclip = soundData.WalkDrone;
        else if (curTrans == playerTransCar)
            Playclip = soundData.WalkCar;


        if (Playclip != lastClip && Playclip != null)
        {
            lastClip = Playclip;
            WalkaudioSource.clip = Playclip;
            WalkaudioSource.Play();
        }
        else if (Playclip != null && !WalkaudioSource.isPlaying)
            WalkaudioSource.UnPause();
    }

    public void StopWalkSound()
    {
        if (WalkaudioSource.isPlaying)
        {
            WalkaudioSource.Pause();
        }
    }

    public void DroneFlySound()
    {
        WalkaudioSource.PlayOneShot(soundData.WalkDrone);
    }

    public void JumpSound()
    {
        if (isGround)
            audioSource.PlayOneShot(soundData.Jump);
    }

    public void AttackSound()
    {
        // AudioSource가 null이거나 비활성화된 경우 리턴
        if (audioSource == null || !audioSource.enabled)
        {
            Debug.LogWarning("AudioSource가 null이거나 비활성화되어 있습니다.");
            return;
        }

        AudioClip PlayAttackclip = null;
        if (curTrans == playerTransBasic)
            PlayAttackclip = soundData.AttackBasic;
        else if (curTrans == playerTransRobot)
            PlayAttackclip = soundData.AttackRobot;
        else if (curTrans == playerTransDrone)
            PlayAttackclip = soundData.AttackDrone;
        else if (curTrans == playerTransCar)
            PlayAttackclip = soundData.AttackCar;

        if (PlayAttackclip != null)
        {
            audioSource.PlayOneShot(PlayAttackclip);
        }
        else
        {
            Debug.LogWarning($"현재 변신 상태({curTrans?.Name})에 해당하는 공격 사운드가 없습니다.");
        }
    }

    public void AssimilateSound()
    {
        // AudioSource가 null이거나 비활성화된 경우 리턴
        if (audioSource == null || !audioSource.enabled)
            return;

        if (soundData.assimilate != null)
            audioSource.PlayOneShot(soundData.assimilate);
    }

    public void releaseSound()
    {
        if (audioSource == null || !audioSource.enabled)
            return;
        if (soundData.assimilate != null)
            audioSource.PlayOneShot(soundData.release);
    }

    public void TransSound()
    {
        if (audioSource == null || !audioSource.enabled)
            return;
        if (soundData.assimilate != null)
            audioSource.PlayOneShot(soundData.trans);
    }

    public void HitSound()
    {
        AudioClip PlayHitclip = null;
        if (curTrans == playerTransBasic || curTrans == playerTransRes)
            PlayHitclip = soundData.HitBasic;
        else if (curTrans == playerTransRobot || curTrans == playerTransCar)
            PlayHitclip = soundData.HitRobot;
        else if (curTrans == playerTransDrone)
            PlayHitclip = soundData.HitDrone;

        if (PlayHitclip != null)
        {
            audioSource.PlayOneShot(PlayHitclip);
        }
    }

    public void DeadSound()
    {
        if (audioSource == null || !audioSource.enabled)
            return;

        if (soundData.Die != null)
        {
            audioSource.PlayOneShot(soundData.Die);
            Debug.Log("재생???");
        }

        Invoke(nameof(StopAllSound), soundData.Die.length);
    }

    private void StopAllSound()
    {
        audioSource.Stop();
    }

    #endregion
}