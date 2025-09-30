using System.Runtime.CompilerServices;
using UnityEngine;

public class GearWheel : MonoBehaviour
{
    private float rotateAmount = 70f;    // 회전 각도
    private float rotateSpeed = 400f;     // 회전 속도

    private bool isRotating = false;
    private float targetAngle;
    private float currentAngle = 0f;
    private float step;

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            float step = rotateSpeed * Time.deltaTime;
            currentAngle += step;

            if (currentAngle >= targetAngle)
            {
                step -= (currentAngle - targetAngle); // 초과분 제거
                isRotating = false;
                currentAngle = 0;
            }

            transform.Rotate(Vector3.forward, step);
        }
    }

    public void RotateGear()
    {
        if (!isRotating)
        {
            targetAngle = rotateAmount;
            currentAngle = 0f;
            isRotating = true;
        }
    }

    public void RotateValueSet(float speed, float angle)
    {
        step = rotateSpeed * Time.deltaTime;
        currentAngle += step;

        if (currentAngle >= targetAngle)
        {
            step -= (currentAngle - targetAngle);
            isRotating = false;
            currentAngle = 0;
        }
    }

}
