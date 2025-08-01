using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectObject : MonoBehaviour, IRecyclableGameObject
{
    public bool isActive => gameObject.activeSelf;

    [SerializeField]
    private float EffectMove = 3.0f;

    private Transform followTarget = null;

    private Vector3 followOffset;

    private float direction = 0;

    public void PlayEffect(Vector3 position, bool faceLeft, Transform Effectfollow = null, Vector3 offset = default)
    {
        transform.position = position;
        transform.position = Vector2.Lerp(transform.position, position, EffectMove*Time.deltaTime);
        followTarget = Effectfollow;

        followOffset = offset;

        direction = faceLeft ? -1 : 1;
        if (direction == -1)
            followOffset.x *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction);
        transform.localScale = scale;

        gameObject.SetActive(true);

    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            UpdateEffect(followTarget.position);
        }
    }

    public void UpdateEffect(Vector3 position)
    {
        
        this.transform.position = position+followOffset;
        transform.localScale = new Vector3(direction, 1, 1);
    }

    public void EffectEnd()
    {
        gameObject.SetActive(false);
    }

}
