using DG.Tweening;
using UnityEngine;

public class UI_PlayerTrans : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    private int lastDisplayedHP = -1;
    private float lastHP;               // 이전 체력 값

    private Tween shakeTween;
    [SerializeField] private RectTransform TransUICanva; // 전체 캔버스

    [SerializeField] private RectTransform trans_Drone;
    [SerializeField] private RectTransform trans_Car;
    [SerializeField] private RectTransform trans_Robot;
    [SerializeField] private RectTransform trans_Res;
    [SerializeField] private RectTransform trans_Basic;

    [SerializeField] private RectTransform Ass_Drone;
    [SerializeField] private RectTransform Ass_Car;
    [SerializeField] private RectTransform Ass_Robot;
    [SerializeField] private RectTransform Ass_Res;
    // [SerializeField] private RectTransform Ass_Basic;

    private void Update()
    {
        Check_PlayerTrans();    
        Check_PlayerASS();
        UpdatePlayerHP();
    }

    private void OnDestroy()
    {
        if (shakeTween != null) shakeTween.Kill();
    }

    private void Check_PlayerTrans()
    {
        if (player.curTrans == player.playerTransDrone)
        {
            TransUI_Show(trans_Drone);
        } 
        else if (player.curTrans == player.playerTransCar) 
        {
            TransUI_Show(trans_Car);
        }
        else if (player.curTrans == player.playerTransRobot) 
        {
            TransUI_Show(trans_Robot);
        }
        else if (player.curTrans == player.playerTransRes) 
        {
            TransUI_Show(trans_Res);
        }
        else 
        {
            TransUI_Show(trans_Basic);
        }
    }

    private void Check_PlayerASS()
    {
        if (player.curForm == TestPlayer.AssimilateForm.Drone)
        {
            AssUI_Show(Ass_Drone);
        }
        else if (player.curForm == TestPlayer.AssimilateForm.Car)
        {
            AssUI_Show(Ass_Car);
        }
        else if (player.curForm == TestPlayer.AssimilateForm.Robot)
        {
            AssUI_Show(Ass_Robot);
        }
        else if (player.curForm == TestPlayer.AssimilateForm.Res)
        {
            AssUI_Show(Ass_Res);
        }
        else
        {
            AssUI_Hide();   
        }
    }


    private void UpdatePlayerHP()
    {
        float curHP = player.playerCurHP;
        int roundedHP = Mathf.RoundToInt(curHP);

        if (roundedHP != lastDisplayedHP)
        {
            // HP 감소 시
            if (curHP < lastHP)
            {
                if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();
                shakeTween = TransUICanva.DOShakeAnchorPos(
                    duration: 0.45f,
                    strength: new Vector2(25f, 0),
                    vibrato: 60,
                    randomness: 90f
                );
            }

            lastDisplayedHP = roundedHP;
            lastHP = curHP;
        }
    }

    //private void TransUI_Hide(RectTransform rect) 
    //{ 
    //    rect.gameObject.SetActive(false);
    //}

    private void TransUI_Hide()
    {
        trans_Drone.gameObject.SetActive(false);
        trans_Car.gameObject.SetActive(false);
        trans_Robot.gameObject.SetActive(false);
        trans_Res.gameObject.SetActive(false);
        trans_Basic.gameObject.SetActive(false);
    }

    private void TransUI_Show(RectTransform rect) 
    {
        TransUI_Hide();
        rect.gameObject.SetActive(true);
    }

    private void AssUI_Show(RectTransform rect) 
    {
        AssUI_Hide();
        rect.gameObject.SetActive(true);
    }

    private void AssUI_Hide() {
        Ass_Drone.gameObject.SetActive(false);
        Ass_Car.gameObject.SetActive(false);
        Ass_Robot.gameObject.SetActive(false);
        Ass_Res.gameObject.SetActive(false);
    }


}
