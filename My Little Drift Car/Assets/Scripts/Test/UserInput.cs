using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]

public class UserInput : MonoBehaviour
{
	CarController ControlledCar;

	public float Horizontal { get; private set; }
	public float Vertical { get; private set; }
	public bool Brake { get; private set; }

	private void Awake()
	{
		ControlledCar = GetComponent<CarController>();
	}

	void Update()
	{
		Horizontal = Input.GetAxis("Horizontal");
		Vertical = Input.GetAxis("Vertical");
		Brake = Input.GetButton("Jump");

		ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
	}
}
