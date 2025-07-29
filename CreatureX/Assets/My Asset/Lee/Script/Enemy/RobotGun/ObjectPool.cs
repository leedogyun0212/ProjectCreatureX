using System.Collections.Generic;
using UnityEngine;

//  > null 이 전달되었을 때 발생시킬 예외 클래스를 정의합니다.
public class ParamIsNullException : System.Exception
{
    private string _ParamName;

    public ParamIsNullException(string paramName)
    { _ParamName = paramName; }

    public override string Message
    { get { return "전달된 " + _ParamName + "의 값이 null 입니다."; } }
}


public class ObjectPool<T> where T : IRecyclableGameObject
{
    //  > 재활용 가능한 오브젝트를 저장할 리스트입니다.
    private List<T> _ObjectPool = null;

    public ObjectPool()
    {
        _ObjectPool = new List<T>();
    }

    //  > 재활용 가능한 오브젝트를 찾습니다.
    public T GetRecyclableObject()
    {
        //  > 재활용 가능한 오브젝트 중 [isActive] 속성이 false 인 오브젝트를 찾습니다.
        return _ObjectPool.Find(
            (T recyclableObj) => !recyclableObj.isActive);
    }

    //  > 재활용 가능한 오브젝트를 등록합니다
    /// - 리턴 값 : 등록시킨 오브젝트를 리턴시킵니다.
    public T RegisterRecyclableObject(T recyclableObj)
    {
        //  > recyclableObj 가 null 이라면 예외를 던집니다.
        if (recyclableObj == null)
            throw new ParamIsNullException("recyclableObj");

        _ObjectPool.Add(recyclableObj);
        return recyclableObj;
    }

}