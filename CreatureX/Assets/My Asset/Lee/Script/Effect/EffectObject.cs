using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EffectObject : MonoBehaviour, IRecyclableGameObject
{
    public bool isActive => gameObject.activeSelf;

    [SerializeField]
    private float EffectMove = 3.0f;

    private Vector3 MovePos;

    private float direction = 0;

    public void PlayEffect(Vector3 position, bool faceLeft)
    {
        transform.position = position;
        transform.position = Vector2.Lerp(transform.position, position, EffectMove*Time.deltaTime);
        MovePos = position;

        direction = faceLeft ? -1 : 1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction);
        transform.localScale = scale;

        gameObject.SetActive(true);

        //StartCoroutine(AutoDisable());
    }

    //private void Update()
    //{
    //    transform.position = Vector2.Lerp(transform.position,)
    //}

    public void UpdateEffect(Vector3 position, bool faceLeft)
    {
        this.transform.position = position;
        MovePos = position;
    }

    public void EffectEnd()
    {
        gameObject.SetActive(false);
    }

}
