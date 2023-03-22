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

    public TextMeshProUGUI textbox;

    [HideInInspector] private ParticleSystem rearRightWheel;
    [HideInInspector] private ParticleSystem rearLeftWheel;
    [HideInInspector] public GameObject smoke;

    public float power = 500f;
    public float brakingForce = 300f;
    public float slipAllowance = 0.1f;

    public float gasInput;
    public float brakeInput;

    private Rigidbody carRB;
    private bool handbrakePressed;
    public float speed;

    public float currentpower = 0f;
    public float slipAngle;
    public float currentTurningAngle = 0f;

    public AnimationCurve steering;

    private void Start()
    {
        carRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
    }

    void InstantiateSmoke()
    {
        rearRightWheel = Instantiate(smoke, rearRight.transform.position, Quaternion.Euler(-0f, -180f, 0f), rearLeft.transform).GetComponent<ParticleSystem>();
        rearLeftWheel = Instantiate(smoke, rearLeft.transform.position, Quaternion.Euler(-0f,-180f,0f),rearRight.transform).GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        input();
        accelerate();
        applySteering();
        updateWheels();
        checkparticles();
        textbox.text = speed.ToString();
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
        if (Input.GetKey(KeyCode.Space))
        {
            handbrakePressed = true;
            brakeInput = 1;   
            rearLeft.brakeTorque = 10000f;
            rearRight.brakeTorque = 10000f;
        }
        else
        {
            brakeInput = 0;
            handbrakePressed = false;
        }
           
            
        gasInput = Input.GetAxis("Vertical");
        currentpower = power * gasInput;
        currentTurningAngle = Input.GetAxis("Horizontal");
        speed = carRB.velocity.magnitude;
        if(speed < 0.01f)
        {
            carRB.velocity = Vector3.zero;
        }

        slipAngle = Vector3.Angle(transform.forward, carRB.velocity - transform.forward);

        if(slipAngle < 120f)
        {
            if(gasInput < 0)
            {
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0f;
            }
        }
        else
        {
            if(!handbrakePressed)
            {
                brakeInput = 0;
            }
        }
    }

    public void accelerate()
    {
        rearRight.motorTorque = currentpower;
        rearLeft.motorTorque = currentpower;

        frontRight.brakeTorque = brakeInput * brakingForce * 1f;
        frontLeft.brakeTorque = brakeInput * brakingForce * 1f;
        rearRight.brakeTorque = brakeInput * brakingForce * 0.7f;
        rearLeft.brakeTorque = brakeInput * brakingForce  *0.7f;
    }

    public void applySteering()
    {
        float steeringAngle = currentTurningAngle * steering.Evaluate(speed);
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
