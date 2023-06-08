using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CarController))]

public class UserInput : MonoBehaviour
{
	CarController ControlledCar;
	PhotonView view;
	public GameObject manager;
	public float Horizontal { get; private set; }
	public float Vertical { get; private set; }
	public bool Brake { get; private set; }

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
		if(manager.GetComponent<Gamemanager>().photon == true)
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
