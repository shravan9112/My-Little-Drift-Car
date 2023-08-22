using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRoll : MonoBehaviour
{
    //[SerializeField] Wheel FrontLeftWheel;
    //[SerializeField] Wheel FrontRightWheel;
    [SerializeField] Wheel RearLeftWheel;
    [SerializeField] Wheel RearRightWheel;
    [SerializeField] float roll = 5000.0f;
    Rigidbody rB;

    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        WheelHit hit;
        var travelL = 1.0;
        var travelR = 1.0;

        //var groundedFL = FrontLeftWheel.WheelCollider.GetGroundHit(out hit);
        //if (groundedFL)
        //    travelL = (-FrontLeftWheel.WheelCollider.transform.InverseTransformPoint(hit.point).y - FrontLeftWheel.WheelCollider.radius) / FrontLeftWheel.WheelCollider.suspensionDistance;

        //var groundedFR = FrontRightWheel.WheelCollider.GetGroundHit(out hit);
        //if (groundedFR)
        //    travelR = (-FrontRightWheel.WheelCollider.transform.InverseTransformPoint(hit.point).y - FrontRightWheel.WheelCollider.radius) / FrontRightWheel.WheelCollider.suspensionDistance;

        var groundedRL = RearLeftWheel.WheelCollider.GetGroundHit(out hit);
        if (groundedRL)
            travelL = (-RearLeftWheel.WheelCollider.transform.InverseTransformPoint(hit.point).y - RearLeftWheel.WheelCollider.radius) / RearLeftWheel.WheelCollider.suspensionDistance;

        var groundedRR = RearRightWheel.WheelCollider.GetGroundHit(out hit);
        if (groundedRR)
            travelR = (-RearRightWheel.WheelCollider.transform.InverseTransformPoint(hit.point).y - RearRightWheel.WheelCollider.radius) / RearRightWheel.WheelCollider.suspensionDistance;


        var antiRollForce = (travelL - travelR) * roll;

        //if (groundedFL)
        //    rB.AddForceAtPosition(FrontLeftWheel.WheelCollider.transform.up * -roll,
        //           FrontLeftWheel.WheelCollider.transform.position);
        //if (groundedFR)
        //    rB.AddForceAtPosition(FrontRightWheel.WheelCollider.transform.up * roll,
        //           FrontRightWheel.WheelCollider.transform.position);
        if (groundedRL)
            rB.AddForceAtPosition(RearLeftWheel.WheelCollider.transform.up * -roll,
                   RearLeftWheel.WheelCollider.transform.position);
        if (groundedRR)
            rB.AddForceAtPosition(RearRightWheel.WheelCollider.transform.up * roll,
                   RearRightWheel.WheelCollider.transform.position);

    }
}
