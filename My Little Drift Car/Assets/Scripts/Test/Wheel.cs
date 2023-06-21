using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics.Wheel;

[System.Serializable]
public struct Wheel
{
	public WheelCollider WheelCollider;
	public Transform WheelView;

	public void UpdateTransform()
	{
		Vector3 pos;
		Quaternion quat;
		WheelCollider.GetWorldPose(out pos, out quat);
		WheelView.position = pos;
		WheelView.rotation = quat;
	}
}