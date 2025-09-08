using DG.Tweening;
using UnityEngine;

public class UI_Player_Attack : MonoBehaviour
{
    public static UI_Player_Attack Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    [SerializeField] private TestPlayer player;

    [Header("Basic Attack")]
    [SerializeField] private RectTransform attack_Basic;
    [SerializeField] private RectTransform attack_Car;
    [SerializeField] private RectTransform attack_Drone;
    [SerializeField] private RectTransform attack_Robot;
    [SerializeField] private RectTransform attack_Res;

    [Header("Skill")]
    [SerializeField] private RectTransform skill_Basic;
    [SerializeField] private RectTransform skill_Drone;
    [SerializeField] private RectTransform skill_Car;
    [SerializeField] private RectTransform skill_Robot;
    [SerializeField] private RectTransform skill_Res;

    [Header("Release")]
    [SerializeField] private CanvasGroup release;

    [Header("Absorb")]
    [SerializeField] private CanvasGroup absorb;


    void Start()
    {
        if (player != null)
        {
            player.OnAbleAssChange += HandleAbleAssChange;
            HandleAbleAssChange(player.ableAss);
        }
    }

    void Update()
    {
        Check_PlayerTrans();
    }

    private void Check_PlayerTrans()
    {
        if (player.curTrans == player.playerTransDrone)
        {
            DeactivateUI_Ab();
            ShowUI_BasicAttack(attack_Drone);
            ShowUI_SkillUI(skill_Drone);
            if (player.isGround)
                ActivateUI_Rel();
            else
                DeactivateUI_Rel();
        }
        else if (player.curTrans == player.playerTransCar)
        {
            DeactivateUI_Ab();
            ShowUI_BasicAttack(attack_Car);
            ShowUI_SkillUI(skill_Car);
            if (player.isGround)
                ActivateUI_Rel();
            else
                DeactivateUI_Rel();
        }
        else if (player.curTrans == player.playerTransRobot)
        {
            DeactivateUI_Ab();
            ShowUI_BasicAttack(attack_Robot);
            ShowUI_SkillUI(skill_Robot);
            if (player.isGround)
                ActivateUI_Rel();
            else
                DeactivateUI_Rel();
        }
        else if (player.curTrans == player.playerTransRes)
        {
            DeactivateUI_Ab();
            ActivateUI_Rel();
            ShowUI_BasicAttack(attack_Res);
            ShowUI_SkillUI(skill_Res);
            if (player.isGround)
                ActivateUI_Rel();
            else
                DeactivateUI_Rel();
        }
        else // 플레이어 기본 상태
        {
            ShowUI_BasicAttack(attack_Basic);
            ShowUI_SkillUI(skill_Basic);
            if (player.unableRel && player.isGround)
                ActivateUI_Rel();
            else
                DeactivateUI_Rel();
        }
    }

    public void HandleAbleAssChange(bool able)
    {
        if (player.curTrans == player.playerTransBasic)
        {
            if (able)
            {
                ActivateUI_Ab();
            }
            else DeactivateUI_Ab();
        }
    }

    private void ShowUI_BasicAttack(RectTransform rect)
    {      
        HideUI_BasicAttack();
        if (rect == null)
        {
            Debug.LogError("공격 UI RectTransform이 연결되지 않았습니다!");
            return;
        }
        // Debug.Log("공격 UI 활성화: " + rect.name);
        rect.gameObject.SetActive(true);
    }

    private void HideUI_BasicAttack()
    {
        attack_Drone.gameObject.SetActive(false);
        attack_Car.gameObject.SetActive(false);
        attack_Robot.gameObject.SetActive(false);
        attack_Res.gameObject.SetActive(false);
        attack_Basic.gameObject.SetActive(false);
    }

    private void ShowUI_SkillUI(RectTransform rect)
    {
        HideUI_SkillUI();
        rect.gameObject.SetActive(true);
    }

    private void HideUI_SkillUI()
    {
        skill_Drone.gameObject.SetActive(false);
        skill_Car.gameObject.SetActive(false);
        skill_Robot.gameObject.SetActive(false);
        skill_Res.gameObject.SetActive(false);
        skill_Basic.gameObject.SetActive(false);
    }


    private void ActivateUI_Ab()
    {
        absorb.alpha = 1f;
    }

    private void DeactivateUI_Ab()
    {
        absorb.alpha = 0.3f;   
    }

    private void ActivateUI_Rel()
    {
        release.alpha = 1f;
    }

    private void DeactivateUI_Rel()
    {
        release.alpha = 0.3f;
    }

}
