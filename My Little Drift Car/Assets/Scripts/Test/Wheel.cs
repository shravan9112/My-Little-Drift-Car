using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics.Wheel;

[System.Serializable]
public struct Wheel
{
    public WheelCollider WheelCollider;
    public Transform WheelView;
    public float SlipForGenerateParticle;

    public float CurrentMaxSlip { get { return Mathf.Max(CurrentForwardSlip, CurrentSidewaysSlip); } }
    public float CurrentForwardSlip { get; private set; }
    public float CurrentSidewaysSlip { get; private set; }
    public WheelHit GetHit { get { return Hit; } }

    WheelHit Hit;

    CarWheelCollider WC;

    public CarWheelCollider CarWheelCollider
    {
        get
        {
            if (WC == null)
            {
                WC = WheelCollider.GetComponent<CarWheelCollider>();
            }
            if (WC == null)
            {
                WC = WheelCollider.gameObject.AddComponent<CarWheelCollider>();
                WC.CheckFirstEnable();
            }
            return WC;
        }
    }

    Vector3 HitPoint;

    const int SmoothValuesCount = 3;

    public void FixedUpdate()
    {

        if (WheelCollider.GetGroundHit(out Hit))
        {
            var prevForwar = CurrentForwardSleep;
            var prevSide = CurrentSidewaysSleep;

            CurrentForwardSleep = (prevForwar + Mathf.Abs(Hit.forwardSlip)) / 2;
            CurrentSidewaysSleep = (prevSide + Mathf.Abs(Hit.sidewaysSlip)) / 2;
        }
        else
        {
            CurrentForwardSleep = 0;
            CurrentSidewaysSleep = 0;
        }
    }

    public void UpdateTransform()
    {
        Vector3 pos;
        Quaternion quat;
        WheelCollider.GetWorldPose(out pos, out quat);
        WheelView.position = pos;
        WheelView.rotation = quat;
    }

    public void UpdateFrictionConfig(WheelColliderConfig config)
    {
        CarWheelCollider.UpdateConfig(config);
    }
}
