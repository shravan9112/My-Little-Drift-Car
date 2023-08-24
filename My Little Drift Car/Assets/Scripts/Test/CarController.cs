using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]

public class CarController : MonoBehaviour
{
	[SerializeField] Wheel FrontLeftWheel;
	[SerializeField] Wheel FrontRightWheel;
	[SerializeField] Wheel RearLeftWheel;
	[SerializeField] Wheel RearRightWheel;
	[SerializeField] Transform COM;

	public TextMeshProUGUI gearText;
	private Transform needleTransform;
	private Transform rpmTransform;
	public TextMeshProUGUI speedText;
	public TextMeshProUGUI rpmText;
	public Slider slider;
	float prevRatio = 0;
	float newRatio = 0;
	private float maxSpeedAngle = -20;
	private float zeroSpeedAngle = 210;
	private float speedMax,rpmMax;

	public float speed;
	public bool AWD;

	[SerializeField] CarConfig CarConfig;

	#region Properties of car parameters

	float MaxMotorTorque;
	float MaxSteerAngle { get { return CarConfig.MaxSteerAngle; } }

	private bool _automaticGearBox;

	bool AutomaticGearBox
	{
		get { return _automaticGearBox; }
		set { _automaticGearBox = value; }
	}
	AnimationCurve MotorTorqueFromRpmCurve { get { return CarConfig.MotorTorqueFromRpmCurve; } }
	float MaxRPM { get { return CarConfig.MaxRPM; } }
	float MinRPM { get { return CarConfig.MinRPM; } }
	float CutOffRPM { get { return CarConfig.CutOffRPM; } }
	float CutOffOffsetRPM { get { return CarConfig.CutOffOffsetRPM; } }
	float RpmToNextGear { get { return CarConfig.RpmToNextGear; } }
	float RpmToPrevGear { get { return CarConfig.RpmToPrevGear; } }
	float MaxForwardSlipToBlockChangeGear { get { return CarConfig.MaxForwardSlipToBlockChangeGear; } }
	float RpmEngineToRpmWheelsLerpSpeed { get { return CarConfig.RpmEngineToRpmWheelsLerpSpeed; } }
	float[] GearsRatio { get { return CarConfig.GearsRatio; } }
	float MainRatio { get { return CarConfig.MainRatio; } }
	float ReversGearRatio { get { return CarConfig.ReversGearRatio; } }
	float MaxBrakeTorque { get { return CarConfig.MaxBrakeTorque; } }
	#endregion

	#region Properties of drif Settings

	bool EnableSteerAngleMultiplier { get { return CarConfig.EnableSteerAngleMultiplier; } }
	float MinSteerAngleMultiplier { get { return CarConfig.MinSteerAngleMultiplier; } }
	float MaxSteerAngleMultiplier { get { return CarConfig.MaxSteerAngleMultiplier; } }
	float MaxSpeedForMinAngleMultiplier { get { return CarConfig.MaxSpeedForMinAngleMultiplier; } }
	float SteerAngleChangeSpeed { get { return CarConfig.SteerAngleChangeSpeed; } }
	float MinSpeedForSteerHelp { get { return CarConfig.MinSpeedForSteerHelp; } }
	float HelpSteerPower { get { return CarConfig.HelpSteerPower; } }
	float OppositeAngularVelocityHelpPower { get { return CarConfig.OppositeAngularVelocityHelpPower; } }
	float PositiveAngularVelocityHelpPower { get { return CarConfig.PositiveAngularVelocityHelpPower; } }
	float MaxAngularVelocityHelpAngle { get { return CarConfig.MaxAngularVelocityHelpAngle; } }
	float AngularVelucityInMaxAngle { get { return CarConfig.AngularVelucityInMaxAngle; } }
	float AngularVelucityInMinAngle { get { return CarConfig.AngularVelucityInMinAngle; } }

	#endregion

	public CarConfig GetCarConfig { get { return CarConfig; } }
	public Wheel[] Wheels { get; private set; }

	float[] AllGearsRatio;                                                           

	Rigidbody rB;
	public Rigidbody RB
	{
		get
		{
			if (!rB)
			{
				rB = GetComponent<Rigidbody>();
			}
			return rB;
		}
	}

	public float CurrentMaxSlip { get; private set; }
	public int CurrentMaxSlipWheelIndex { get; private set; }
	public float CurrentSpeed { get; private set; }                      
	public float SpeedInHour { get { return CurrentSpeed * 3.6f; } }
	public int CarDirection { get { return CurrentSpeed < 1 ? 0 : (VelocityAngle < 90 && VelocityAngle > -90 ? 1 : -1); } }

	float CurrentSteerAngle;
	float horizontalinput;
	float CurrentAcceleration;
	float CurrentBrake;
	bool InHandBrake;

	public GameObject Manager;
	public Gamemanager gamemanager1;
	public bool exit;

	int FirstDriveWheel;
	int LastDriveWheel;

	public float VelocityAngle { get; private set; }

	private void Awake()
	{
		RB.centerOfMass = COM.localPosition;

		needleTransform = GameObject.Find("Needle").transform;
		rpmTransform = GameObject.Find("RpmNeedle").transform;
		speedMax = 100f;
		rpmMax = 7000f;

		Wheels = new Wheel[4] {
			FrontLeftWheel,
			FrontRightWheel,
			RearLeftWheel,
			RearRightWheel
		};
		
		if(AWD)
        {
			FirstDriveWheel = 0;
			LastDriveWheel = 3;
		}
		else
        {
			FirstDriveWheel = 2;
			LastDriveWheel = 3;
		}

		MaxMotorTorque = CarConfig.MaxMotorTorque / (LastDriveWheel - FirstDriveWheel + 1);

		AllGearsRatio = new float[GearsRatio.Length + 2];
		AllGearsRatio[0] = ReversGearRatio * MainRatio;
		AllGearsRatio[1] = 0;
		for (int i = 0; i < GearsRatio.Length; i++)
		{
			AllGearsRatio[i + 2] = GearsRatio[i] * MainRatio;
		}
	}

    private void Start()
    {
		Manager = GameObject.Find("GameManager");
	}
    public void UpdateControls(float horizontal, float vertical, bool handBrake)
	{
		float targetSteerAngle = horizontal * MaxSteerAngle;
		horizontalinput = horizontal;

		if (EnableSteerAngleMultiplier)
		{
			targetSteerAngle *= Mathf.Clamp(1 - SpeedInHour / MaxSpeedForMinAngleMultiplier, MinSteerAngleMultiplier, MaxSteerAngleMultiplier);
		}

		CurrentSteerAngle = Mathf.MoveTowards(CurrentSteerAngle, targetSteerAngle, Time.deltaTime * SteerAngleChangeSpeed);

		CurrentAcceleration = vertical;
		InHandBrake = handBrake;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			exit = true;
			Manager.GetComponent<Gamemanager>().exit = exit;
			exit = false;
			Destroy(gameObject);
		}
			

		if (AWD)
		{
			FirstDriveWheel = 0;
			LastDriveWheel = 3;
		}
		else
		{
			FirstDriveWheel = 2;
			LastDriveWheel = 3;
		}

		for (int i = 0; i < Wheels.Length; i++)
		{
			Wheels[i].UpdateTransform();
		}
		gearText.text = CurrentGear.ToString();
		needleTransform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
		rpmTransform.eulerAngles = new Vector3(0, 0, GetRpmRotation());
		slider.value = horizontalinput;
		if (CurrentSpeed > 0.1)
		{
			speed = CurrentSpeed;
		}
		else
			speed = 0;
		speedText.text = speed.ToString("F2");
		rpmText.text = EngineRPM.ToString("F2");
		if (Manager.GetComponent<Gamemanager>().maxspeed < speed)
			Manager.GetComponent<Gamemanager>().maxspeed = speed;

		if (Input.GetKeyDown(KeyCode.E))
			Shiftup();
		if (Input.GetKeyDown(KeyCode.Q))
			ShiftDown();
		if(Input.GetKeyDown(KeyCode.P))
			AutomaticGearBox = !AutomaticGearBox;
	}

	private void FixedUpdate()
	{
		CurrentMaxSlip = Wheels[0].CurrentMaxSlip;
		CurrentMaxSlipWheelIndex = 0;

		CurrentSpeed = RB.velocity.magnitude;

		UpdateSteerAngleLogic();
		if (AutomaticGearBox)
		{
			UpdateRpmAndTorqueLogic();
		}
		else
			ManualShift();

		if (InHandBrake)
		{
			RearLeftWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
			RearRightWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
			FrontLeftWheel.WheelCollider.brakeTorque = 0;
			FrontRightWheel.WheelCollider.brakeTorque = 0;
		}

		for (int i = 0; i < Wheels.Length; i++)
		{
			if (!InHandBrake)
			{
				Wheels[i].WheelCollider.brakeTorque = CurrentBrake;
			}

            Wheels[i].FixedUpdate();

            if (CurrentMaxSlip < Wheels[i].CurrentMaxSlip)
            {
                CurrentMaxSlip = Wheels[i].CurrentMaxSlip;
                CurrentMaxSlipWheelIndex = i;
            }
        }
	}

	public int CurrentGear { get; private set; }
	public int CurrentGearIndex { get { return CurrentGear + 1; } }
	public float EngineRPM { get; private set; }
	public float GetMaxRPM { get { return MaxRPM; } }
	public float GetMinRPM { get { return MinRPM; } }
	public float GetInCutOffRPM { get { return CutOffRPM - CutOffOffsetRPM; } }

	float CutOffTimer;
	public bool InCutOff;

    void UpdateRpmAndTorqueLogic()
    {
        //Automatic gearbox logic. 
        if (AutomaticGearBox)
        {

            bool forwardIsSlip = false;
            for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
            {
                if (Wheels[i].CurrentForwardSleep > MaxForwardSlipToBlockChangeGear)
                {
                    forwardIsSlip = true;
                    break;
                }
            }

            float prevRatio = 0;
            float newRatio = 0;

            if (!forwardIsSlip && EngineRPM > RpmToNextGear && CurrentGear >= 0 && CurrentGear < (AllGearsRatio.Length - 2))
            {
                prevRatio = AllGearsRatio[CurrentGearIndex];
                CurrentGear++;
                newRatio = AllGearsRatio[CurrentGearIndex];
            }
            else if (EngineRPM < RpmToPrevGear && CurrentGear > 0 && (EngineRPM <= MinRPM || CurrentGear != 1))
            {
                prevRatio = AllGearsRatio[CurrentGearIndex];
                CurrentGear--;
                newRatio = AllGearsRatio[CurrentGearIndex];
            }

            if (!Mathf.Approximately(prevRatio, 0) && !Mathf.Approximately(newRatio, 0))
            {
                EngineRPM = Mathf.Lerp(EngineRPM, EngineRPM * (newRatio / prevRatio), RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime); //EngineRPM * (prevRatio / newRatio);// 
            }

            if (CarDirection <= 0 && CurrentAcceleration < 0)
            {
                CurrentGear = -1;
            }
            else if (CurrentGear <= 0 && CarDirection >= 0 && CurrentAcceleration > 0)
            {
                CurrentGear = 1;
            }
            else if (CarDirection == 0 && CurrentAcceleration == 0)
            {
                CurrentGear = 0;
            }
        }

        if (InCutOff)
        {
            if (CutOffTimer > 0)
            {
                CutOffTimer -= Time.fixedDeltaTime;
                EngineRPM = Mathf.Lerp(EngineRPM, GetInCutOffRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
            }
            else
            {
                InCutOff = false;
            }
        }

        float rpm = CurrentAcceleration > 0 ? MaxRPM : MinRPM;
        float speed = CurrentAcceleration > 0 ? RpmEngineToRpmWheelsLerpSpeed : RpmEngineToRpmWheelsLerpSpeed * 0.2f;
        EngineRPM = Mathf.Lerp(EngineRPM, rpm, speed * Time.fixedDeltaTime);

        if (EngineRPM >= CutOffRPM)
        {
            InCutOff = true;
            CutOffTimer = CarConfig.CutOffTime;
        }

        float minRPM = 0;
        for (int i = FirstDriveWheel + 1; i <= LastDriveWheel; i++)
        {
            minRPM += Wheels[i].WheelCollider.rpm;
        }

        minRPM /= LastDriveWheel - FirstDriveWheel + 1;

        if (!InCutOff)
        {
            float targetRPM = Mathf.Abs((minRPM + 20) * AllGearsRatio[CurrentGearIndex]);              
            targetRPM = Mathf.Clamp(targetRPM, MinRPM, MaxRPM);
            EngineRPM = Mathf.Lerp(EngineRPM, targetRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
        }

        if (EngineRPM >= CutOffRPM)
        {
            InCutOff = true;
            CutOffTimer = CarConfig.CutOffTime;
            return;
        }

        if (!Mathf.Approximately(CurrentAcceleration, 0))
        {
            //If the direction of the car is the same as Current Acceleration.
            if (CarDirection * CurrentAcceleration >= 0)
            {
                CurrentBrake = 0;

                float motorTorqueFromRpm = MotorTorqueFromRpmCurve.Evaluate(EngineRPM * 0.001f);
                var motorTorque = CurrentAcceleration * (motorTorqueFromRpm * (MaxMotorTorque * AllGearsRatio[CurrentGearIndex]));
                if (Mathf.Abs(minRPM) * AllGearsRatio[CurrentGearIndex] > MaxRPM)
                {
                    motorTorque = 0;
                }

                //If the rpm of the wheel is less than the max rpm engine * current ratio, then apply the current torque for wheel, else not torque for wheel.
                float maxWheelRPM = AllGearsRatio[CurrentGearIndex] * EngineRPM;
                for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
                {
                    if (Wheels[i].WheelCollider.rpm <= maxWheelRPM)
                    {
                        Wheels[i].WheelCollider.motorTorque = motorTorque;
                    }
                    else
                    {
                        Wheels[i].WheelCollider.motorTorque = 0;
                    }
                }
            }
            else
            {
                CurrentBrake = MaxBrakeTorque;
				Wheels[0].WheelCollider.brakeTorque = CurrentBrake * 0.3f;
				Wheels[1].WheelCollider.brakeTorque = CurrentBrake * 0.3f;
				Wheels[1].WheelCollider.brakeTorque = CurrentBrake * 0.2f;
				Wheels[3].WheelCollider.brakeTorque = CurrentBrake * 0.2f;
			}
        }
        else
        {
            CurrentBrake = 0;

            for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
            {
                Wheels[i].WheelCollider.motorTorque = 0;
            }
        }
    }

    void ManualShift()
    {
		if (EngineRPM >= CutOffRPM)
		{
			InCutOff = true;
		}
		if(InCutOff)
        {
			if (EngineRPM <= CutOffRPM)
				InCutOff = false;
		}

        //float prevRatio = 0;
        //float newRatio = 0;
        //if (!Mathf.Approximately(prevRatio, 0) && !Mathf.Approximately(newRatio, 0))
        //{
        //    EngineRPM = Mathf.Lerp(EngineRPM, EngineRPM * (newRatio / prevRatio), RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime); //EngineRPM * (prevRatio / newRatio);// 
        //}

        float rpm = CurrentAcceleration > 0 ? MaxRPM : MinRPM;
		float speed = CurrentAcceleration > 0 ? RpmEngineToRpmWheelsLerpSpeed : RpmEngineToRpmWheelsLerpSpeed * 0.2f;
		EngineRPM = Mathf.Lerp(EngineRPM, rpm, speed * Time.fixedDeltaTime);
		float minRPM = 0;
		for (int i = FirstDriveWheel + 1; i <= LastDriveWheel; i++)
		{
			minRPM += Wheels[i].WheelCollider.rpm;
		}

		minRPM /= LastDriveWheel - FirstDriveWheel + 1;

        if (!InCutOff)
        {
            float targetRPM = Mathf.Abs((minRPM + 20) * AllGearsRatio[CurrentGearIndex]);              //+20 for normal work CutOffRPM
            targetRPM = Mathf.Clamp(targetRPM, MinRPM, MaxRPM);
            EngineRPM = Mathf.Lerp(EngineRPM, targetRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
        }

        if (!Mathf.Approximately(CurrentAcceleration, 0))
		{
			//If the direction of the car is the same as Current Acceleration.
			if (CarDirection * CurrentAcceleration >= 0)
			{
				CurrentBrake = 0;

				float motorTorqueFromRpm = MotorTorqueFromRpmCurve.Evaluate(EngineRPM * 0.001f);
				var motorTorque = CurrentAcceleration * (motorTorqueFromRpm * (MaxMotorTorque * AllGearsRatio[CurrentGearIndex]));
				if (Mathf.Abs(minRPM) * AllGearsRatio[CurrentGearIndex] > MaxRPM)
				{
					motorTorque = 0;
				}

				//If the rpm of the wheel is less than the max rpm engine * current ratio, then apply the current torque for wheel, else not torque for wheel.
				float maxWheelRPM = AllGearsRatio[CurrentGearIndex] * EngineRPM;
				for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
				{
					if (Wheels[i].WheelCollider.rpm <= maxWheelRPM)
					{
							Wheels[i].WheelCollider.motorTorque = motorTorque;
					}
					else
					{
						Wheels[i].WheelCollider.motorTorque = 0;
					}
				}
			}
			else
			{
				CurrentBrake = MaxBrakeTorque;
			}
		}
		else
		{
			CurrentBrake = 0;

			for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
			{
				Wheels[i].WheelCollider.motorTorque = 0;
			}
		}
	}

	void Shiftup()
    {
		if(!AutomaticGearBox)
        {
			if(CurrentGear < 5)
            {
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear++;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}
		}	
	}

	void ShiftDown()
    {
		if (!AutomaticGearBox)
        {
			if(CurrentGear > 0)
            {
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear--;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}
		}
	}

	void UpdateSteerAngleLogic()
	{
		var needHelp = SpeedInHour > MinSpeedForSteerHelp && CarDirection > 0;
		float targetAngle = 0;
		VelocityAngle = -Vector3.SignedAngle(RB.velocity, transform.TransformDirection(Vector3.forward), Vector3.up);

		if (needHelp)
		{
			targetAngle = Mathf.Clamp(VelocityAngle * HelpSteerPower, -MaxSteerAngle, MaxSteerAngle);
		}

		targetAngle = Mathf.Clamp(targetAngle + CurrentSteerAngle, -(MaxSteerAngle + 10), MaxSteerAngle + 10);

		Wheels[0].WheelCollider.steerAngle = targetAngle;
		Wheels[1].WheelCollider.steerAngle = targetAngle;

        if (needHelp)
        {
            var absAngle = Mathf.Abs(VelocityAngle);

            float currentAngularProcent = absAngle / MaxAngularVelocityHelpAngle;

            var currAngle = RB.angularVelocity;

            if (VelocityAngle * CurrentSteerAngle > 0)
            {
                var angularVelocityMagnitudeHelp = OppositeAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
                currAngle.y += angularVelocityMagnitudeHelp * currentAngularProcent;
            }
            else if (!Mathf.Approximately(CurrentSteerAngle, 0))
            {

                var angularVelocityMagnitudeHelp = PositiveAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
                currAngle.y += angularVelocityMagnitudeHelp * (1 - currentAngularProcent);
            }

            //Clamp and apply of angular velocity.
            var maxMagnitude = ((AngularVelucityInMaxAngle - AngularVelucityInMinAngle) * currentAngularProcent) + AngularVelucityInMinAngle;
            currAngle.y = Mathf.Clamp(currAngle.y, -maxMagnitude, maxMagnitude);
            RB.angularVelocity = currAngle;
        }
    }

	private float GetSpeedRotation()
	{

		//Function used to get the angle required for the needle of speedometer
		float totalAngleSize = zeroSpeedAngle - maxSpeedAngle;
		float speedNormalized = speed / speedMax;
		return zeroSpeedAngle - speedNormalized * totalAngleSize;
	}

	private float GetRpmRotation()
	{
		//Function used to get the angle required for the needle of tachometer
		float totalAngleSize = zeroSpeedAngle - maxSpeedAngle;
		float rpmNormalized = EngineRPM / rpmMax;
		return zeroSpeedAngle - rpmNormalized * totalAngleSize;
	}
}


[System.Serializable]
public class CarConfig
{
	[Header("Steer Settings")]
	public float MaxSteerAngle = 25;

	[Header("Engine and power settings")]             
	public bool AutomaticGearBox = true;
	public float MaxMotorTorque = 150;                      
	public AnimationCurve MotorTorqueFromRpmCurve;          
	public float MaxRPM = 7000;
	public float MinRPM = 700;
	public float CutOffRPM = 6800;                          
	public float CutOffOffsetRPM = 500;
	public float CutOffTime = 0.1f;
	public float RpmToNextGear = 6500;                      
	public float RpmToPrevGear = 4500;                      
	public float MaxForwardSlipToBlockChangeGear = 0.5f;    
	public float RpmEngineToRpmWheelsLerpSpeed = 15;        
	public float[] GearsRatio;                              
	public float MainRatio;
	public float ReversGearRatio;                           

	[Header("Braking settings")]
	public float MaxBrakeTorque = 1000;

	[Header("Helper settings")]                             

	public bool EnableSteerAngleMultiplier = true;
	public float MinSteerAngleMultiplier = 0.05f;           
	public float MaxSteerAngleMultiplier = 1f;          
	public float MaxSpeedForMinAngleMultiplier = 250;       
	[Space(10)]

	public float SteerAngleChangeSpeed;                     
	public float MinSpeedForSteerHelp;                      
	[Range(0f, 1f)] public float HelpSteerPower;           
	public float OppositeAngularVelocityHelpPower = 0.1f;   
	public float PositiveAngularVelocityHelpPower = 0.1f;   
	public float MaxAngularVelocityHelpAngle;               
	public float AngularVelucityInMaxAngle;                 
	public float AngularVelucityInMinAngle;                 
}
