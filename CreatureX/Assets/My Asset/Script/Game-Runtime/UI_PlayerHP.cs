using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_PlayerHP : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    [SerializeField] private Text textCurHP;
    [SerializeField] private RectTransform HPUICanavas;  // 전체 체력 UI
    [SerializeField] private RectTransform barRect;      // 체력바 이미지 RectTransform

    private float maxHP;
    private float lastHP;               // 이전 체력 값
    private int lastDisplayedHP = -1;   // 마지막으로 화면에 표시된 체력
    private float maxBarWidth = 1150f;  // 체력바 최대 너비
    private Color oriColor;

    // DOTween 트래킹
    private Tween barTween;
    private Tween shakeTween;
    private Tween colorTween;

    private void Start()
    {
        if (player == null || barRect == null || textCurHP == null) return;

        maxHP = player.playerMaxHP;
        lastHP = player.playerCurHP;

        float percent = player.playerCurHP / maxHP;
        barRect.sizeDelta = new Vector2(maxBarWidth * percent, barRect.sizeDelta.y);
        textCurHP.text = player.playerCurHP.ToString();
        oriColor = textCurHP.color;
    }

    private void Update()
    {
        if (player == null || barRect == null || textCurHP == null) return;
        UpdatePlayerHP();
    }

    private void UpdatePlayerHP()
    {
        float curHP = player.playerCurHP;
        float percent = curHP / maxHP;
        float newBarWidth = maxBarWidth * percent;

        // 이전 Tween 중단 후 새로 시작 (메모리 누수 방지)
        if (barTween != null && barTween.IsActive()) barTween.Kill();
        barTween = barRect.DOSizeDelta(
            new Vector2(newBarWidth, barRect.sizeDelta.y),
            0.3f
        ).SetEase(Ease.OutQuad);

        int roundedHP = Mathf.RoundToInt(curHP);
        if (roundedHP != lastDisplayedHP)
        {
            lastDisplayedHP = roundedHP;
            textCurHP.text = roundedHP.ToString();

            if (curHP < lastHP)
            {
                // 체력 감소 효과
                if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();
                shakeTween = HPUICanavas.DOShakeAnchorPos(
                    duration: 0.45f,
                    strength: new Vector2(25f, 0),
                    vibrato: 60,
                    randomness: 90f
                );
            }
            else if (curHP > lastHP)
            {
                // 체력 회복 효과
                if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
                colorTween = textCurHP.DOColor(Color.green, 0.1f)
                    .OnComplete(() =>
                    {
                        textCurHP.DOColor(oriColor, 0.2f);
                    });

                textCurHP.rectTransform.DOKill();
                textCurHP.rectTransform.localScale = Vector3.one;
            }

            lastHP = curHP;
        }
    }

    private void OnDestroy()
    {
        // UI가 사라질 때 진행 중인 Tween 전부 종료
        if (barTween != null) barTween.Kill();
        if (shakeTween != null) shakeTween.Kill();
        if (colorTween != null) colorTween.Kill();

        DOTween.Kill(barRect);
        DOTween.Kill(textCurHP);
        DOTween.Kill(HPUICanavas);
    }

}