using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics.Wheel;

[System.Serializable]
public struct Wheel
{
	public WheelCollider WheelCollider;
	public Transform WheelView;
	public float SlipToGenerateParticle;

	public float CurrentMaxSlip { get { return Mathf.Max(CurrentForwardSleep, CurrentSidewaysSleep); } }
	public float CurrentForwardSleep { get; private set; }
	public float CurrentSidewaysSleep { get; private set; }
	public WheelHit GetHit { get { return Hit; } }

	WheelHit Hit;

	EffectsController EffectsController { get { return EffectsController.Instance; } }

	Vector3 HitPoint;

	void Awake()
    {
		SlipToGenerateParticle = 0.7f;

	}

	public void FixedUpdate()
	{

		if (WheelCollider.GetGroundHit(out Hit))   //Calculating tire slipping the most and it's slip
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

	public void UpdateTransform()  //Sync wheel's transform to that of the wheel collider
	{
		Vector3 pos;
		Quaternion quat;
		WheelCollider.GetWorldPose(out pos, out quat);
		WheelView.position = pos;
		WheelView.rotation = quat;

		if (WheelCollider.isGrounded && CurrentMaxSlip > SlipToGenerateParticle)
        {
			var particles = EffectsController.GetParticles;
			var point = WheelCollider.transform.position;
			point.y = Hit.point.y;
			particles.transform.position = point;
			particles.Emit(1);
		}
	}
}