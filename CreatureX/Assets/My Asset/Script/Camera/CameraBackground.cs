using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    private Camera Camera;

    public Sprite BackGround1;
    public Sprite BackGround2;
    public Sprite BackGround3;
    public Sprite BackGround4;

    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    private int lastBackNum = int.MinValue;

    private void Awake()
    {
        Camera = GetComponentInParent<Camera>();
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (Camera != null)
            ChangeBackGround(Camera.BackNum);
    }

    private void Update()
    {
        if (Camera == null || backgroundRenderer == null)
            return;

        if (lastBackNum != Camera.BackNum)
        {
            ChangeBackGround(Camera.BackNum);
        }
    }

    private void ChangeBackGround(int backNum)
    {
        if (backgroundRenderer == null)
            return;

        Sprite target = null;
        switch (backNum)
        {
            case 1:
                target = BackGround1;
                break;
            case 2:
                target = BackGround2;
                break;
            case 3:
                target = BackGround3;
                break;
            case 4:
                target = BackGround4;
                break;
            default:
                if (backNum < 1) target = BackGround1;
                else target = BackGround4;
                break;
        }

        if (target != BackGround4)
            transform.localScale = new Vector3(1.42f, 0.88f, 1f);
        else
            transform.localScale = new Vector3(1.42f, 1.5f, 1f);

        if (target != null && backgroundRenderer.sprite != target)
        {
            backgroundRenderer.sprite = target;
        }

        lastBackNum = backNum;
    }
}
