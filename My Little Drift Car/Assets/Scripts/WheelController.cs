using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider rearRight;
    public WheelCollider rearLeft;

    public Transform frontRightTransfrom;
    public Transform frontLeftTransfrom;
    public Transform rearRightTransfrom;
    public Transform rearLeftTransfrom;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 20f;

    public float currentAcceleration = 0f;
    public float currentBreakingForce = 0f;
    public float currentTurningAngle = 0f;

    private void FixedUpdate()
    {
        input();
        accelerate();
        steering();
        updateWheels();
    }

    public void input()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakingForce = breakingForce;
        }
        else
            currentBreakingForce = 0f;

        currentAcceleration = acceleration * Input.GetAxis("Vertical");
    }

    public void accelerate()
    {
        rearRight.motorTorque = currentAcceleration;
        rearLeft.motorTorque = currentAcceleration;

        frontRight.brakeTorque = currentBreakingForce;
        frontLeft.brakeTorque = currentBreakingForce;
        rearRight.brakeTorque = currentBreakingForce;
        rearLeft.brakeTorque = currentBreakingForce;
    }

    public void steering()
    {
        currentTurningAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurningAngle;
        frontRight.steerAngle = currentTurningAngle;
    }

    public void updateWheels()
    {
        updatewheel(frontLeft, frontLeftTransfrom);
        updatewheel(frontRight, frontRightTransfrom);
        updatewheel(rearLeft, rearLeftTransfrom);
        updatewheel(rearRight, rearRightTransfrom);
    }

    public void updatewheel(WheelCollider col,Transform trans)
    {
        Vector3 postion;
        Quaternion rotation;
        col.GetWorldPose(out postion, out rotation);
        trans.position = postion;
        trans.rotation = rotation;
    }
}
