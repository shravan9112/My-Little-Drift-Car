using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WheelController : MonoBehaviour
{

    [HideInInspector] public WheelCollider frontRight;
    [HideInInspector] public WheelCollider frontLeft;
    [HideInInspector] public WheelCollider rearRight;
    [HideInInspector] public WheelCollider rearLeft;

    [HideInInspector] public MeshRenderer frontRightTransfrom;
    [HideInInspector] public MeshRenderer frontLeftTransfrom;
    [HideInInspector] public MeshRenderer rearRightTransfrom;
    [HideInInspector] public MeshRenderer rearLeftTransfrom;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gearText;

    [HideInInspector] private ParticleSystem rearRightWheel;
    [HideInInspector] private ParticleSystem rearLeftWheel;
    [HideInInspector] public GameObject smoke;

    public float power = 500f;
    public float brakingForce = 300f;
    public float slipAllowance = 0.1f;
    public int currentGear;

    public float gasInput;
    public float brakeInput;

    private Rigidbody carRB;
    public bool handbrakePressed;
    public bool accelerating;
    public bool braking;
    public bool coasting;
    public bool reversing;
    public float speed;

    public float currentpower = 0f;
    public float slipAngle;
    public float turnInput = 0f;

    public AnimationCurve steering;

    private void Start()
    {
        carRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
        currentGear = 0;
    }

    private void FixedUpdate()
    {
        input();
        accelerate();
        applySteering();
        updateWheels();
        checkparticles();
        updateDisplay();
    }

    private void Update()
    {
        ShiftGears();
    }

    private void updateDisplay()
    {
        if (speed > 0.1)
            speedText.text = speed.ToString();
        else
            speedText.text = 0f.ToString();
        gearText.text = currentGear.ToString();
    }

    void ShiftGears()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(currentGear < 5)
            {
                currentGear = currentGear + 1;
                //ShiftUP();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(currentGear > -1)
            {
                currentGear = currentGear - 1;
                //ShiftDown();
            }
        }    
    }

    void InstantiateSmoke()
    {
        rearRightWheel = Instantiate(smoke, rearRight.transform.position, Quaternion.Euler(-0f, -180f, 0f), rearLeft.transform).GetComponent<ParticleSystem>();
        rearLeftWheel = Instantiate(smoke, rearLeft.transform.position, Quaternion.Euler(-0f, -180f, 0f), rearRight.transform).GetComponent<ParticleSystem>();
    }

    public void checkparticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        frontLeft.GetGroundHit(out wheelHits[0]);
        frontRight.GetGroundHit(out wheelHits[1]);
        rearLeft.GetGroundHit(out wheelHits[2]);
        rearRight.GetGroundHit(out wheelHits[3]);

        if (Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)
        {
            rearLeftWheel.Play();
        }
        else
        {
            rearLeftWheel.Stop();
        }
        if (Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)
        {
            rearRightWheel.Play();
        }
        else
        {
            rearRightWheel.Stop();
        }
    }

    public void input()
    {
        gasInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        speed = carRB.velocity.magnitude;

        slipAngle = Vector3.Angle(transform.forward, carRB.velocity - transform.forward);
        if (gasInput > 0)
        {
            if (currentGear > 0)
            {
                coasting = false;
                accelerating = true;
                braking = false;
            }
            else if (currentGear < 0)
            {
                coasting = false;
                reversing = true;
                braking = false;
            }
        }
        else if (gasInput < 0)
        {
            if (currentGear > 0)
            {
                coasting = false;
                braking = true;
                accelerating = false;
            }
            else if(currentGear < 0)
            {
                coasting = false;
                reversing = false;
                braking = true;
            }
        }
        else
        {
            braking = reversing = accelerating = false;
            coasting = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            handbrakePressed = true;
        }
        else
        {
            handbrakePressed = false;
        }
    }

    public void accelerate()
    {
        if (reversing)
        {
            rearRight.motorTorque = -gasInput * power;
            rearLeft.motorTorque = -gasInput * power;
            frontRight.brakeTorque = 0f;
            frontLeft.brakeTorque = 0f;
            rearRight.brakeTorque = 0f;
            rearLeft.brakeTorque = 0f;
        }
        if (accelerating)
        {
            rearRight.motorTorque = gasInput * power;
            rearLeft.motorTorque = gasInput * power;
            frontRight.brakeTorque = 0f;
            frontLeft.brakeTorque = 0f;
            rearRight.brakeTorque = 0f;
            rearLeft.brakeTorque = 0f;
        }
        if(braking)
        {
            rearRight.motorTorque = 0;
            rearLeft.motorTorque = 0;
            frontRight.brakeTorque = -gasInput * brakingForce * 1f;
            frontLeft.brakeTorque = -gasInput * brakingForce * 1f;
            rearRight.brakeTorque = -gasInput * brakingForce * 0.7f;
            rearLeft.brakeTorque = -gasInput * brakingForce * 0.7f;
        }
        if(coasting)
        {
            frontRight.brakeTorque = 0f;
            frontLeft.brakeTorque = 0f;
            rearRight.brakeTorque = 0f;
            rearLeft.brakeTorque = 0f;
            rearRight.motorTorque = 0;
            rearLeft.motorTorque = 0;
        }
        if(handbrakePressed)
        {
            rearRight.motorTorque = 0;
            rearLeft.motorTorque = 0;
            rearRight.brakeTorque = brakingForce;
            rearLeft.brakeTorque = brakingForce;
        }
    }

    public void applySteering()
    {
        float steeringAngle = turnInput * steering.Evaluate(speed);
        frontLeft.steerAngle = steeringAngle;
        frontRight.steerAngle = steeringAngle;
    }

    public void updateWheels()
    {
        updatewheel(frontLeft, frontLeftTransfrom);
        updatewheel(frontRight, frontRightTransfrom);
        updatewheel(rearLeft, rearLeftTransfrom);
        updatewheel(rearRight, rearRightTransfrom);
    }

    public void updatewheel(WheelCollider col,MeshRenderer trans)
    {
        Vector3 postion;
        Quaternion rotation;
        col.GetWorldPose(out postion, out rotation);
        trans.transform.position = postion;
       // trans.transform.rotation = Quaternion.Euler(rotation.x,rotation.y + 180f,rotation.z);
        trans.transform.rotation = rotation;
    }
}
