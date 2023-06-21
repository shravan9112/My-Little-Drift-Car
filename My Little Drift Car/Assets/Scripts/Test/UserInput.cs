using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CarController))]

public class UserInput : MonoBehaviour
{
	CarController ControlledCar;
	PhotonView view;
	public bool test;
	public GameObject manager;
	public float Horizontal;
	public float Vertical;
	public bool Brake;

	private void Awake()
	{
		ControlledCar = GetComponent<CarController>();
		manager = GameObject.Find("GameManager");
	}

    private void Start()
    {
		view = GetComponent<PhotonView>();
    }

    void Update()
	{
		if(test)
        {
			Horizontal = Input.GetAxis("Horizontal");
			Vertical = Input.GetAxis("Vertical");
			Brake = Input.GetButton("Jump");

			ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
		}
		else
        {
			if (manager.GetComponent<Gamemanager>().photon == true)
			{
				if (view.IsMine)
				{
					Horizontal = Input.GetAxis("Horizontal");
					Vertical = Input.GetAxis("Vertical");
					Brake = Input.GetButton("Jump");

					ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
				}
			}
			else
			{
				Horizontal = Input.GetAxis("Horizontal");
				Vertical = Input.GetAxis("Vertical");
				Brake = Input.GetButton("Jump");

				ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
			}
		}

	}
}
