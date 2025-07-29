using System.Collections;
using UnityEngine;

public class EffectObject : MonoBehaviour, IRecyclableGameObject
{
    public bool isActive => gameObject.activeSelf;

    public void PlayEffect(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        //StartCoroutine(AutoDisable());
    }

    //private IEnumerator AutoDisable()
    //{
    //    yield return new WaitForSeconds(1f); // 이펙트 지속 시간
    //    Debug.Log(isActive);
    //    gameObject.SetActive(false);
    //}

    public void EffectEnd()
    {
        gameObject.SetActive(false);
    }

}
