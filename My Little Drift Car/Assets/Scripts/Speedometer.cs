using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Transform needleTransfrom;
    private float speedText;
    private float maxSpeedAngle = -20;
    private float zeroSpeedAngle = 210;
    private float speedMax;
    public float speed;

    private void Awake()
    {
        needleTransfrom = GameObject.Find("Needle").transform;
        speedText = this.GetComponent<CarController>().speed;
        speed = 0;
        speedMax = 200f;
    }

    private void Update()
    {
        Debug.Log(speedText);
        speed = speedText;
    }

    private float GetSpeedRotation()
    {
        float totalAngleSize = zeroSpeedAngle = maxSpeedAngle;
        float speedNormalized = speed / speedMax;
        return zeroSpeedAngle - speedNormalized * totalAngleSize;
    }
}
