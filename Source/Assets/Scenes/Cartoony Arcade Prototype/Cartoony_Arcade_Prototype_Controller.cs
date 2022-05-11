using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartoony_Arcade_Prototype_Controller : MonoBehaviour
{
    [Header("Vehicle Variables")]
    public float maxSpeed = 80.0f;
    public float MotorForce, SteerForce, BrakeForce;

    public Vector3 CameraOffset = Vector3.zero;
    
    [Header("Components")]
    public Camera carCam;
    public WheelCollider Wheel_FR, Wheel_FL, Wheel_RR, Wheel_RL;
    Rigidbody rb = null;   

    [Header("Private Variables")]
    [SerializeField] private float rbSpeed = 0;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        // Camera
        {
            Vector3 camPos = transform.position + (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * CameraOffset);     

            Quaternion camRot = Quaternion.Euler(75, -90, 0);
            carCam.transform.SetPositionAndRotation(camPos, camRot);
        }

        // Motion
        {
            rbSpeed = rb.velocity.sqrMagnitude;

            float v = Input.GetAxis("Vertical") * MotorForce;
            float h = Input.GetAxis("Horizontal") * -SteerForce;

            if (rbSpeed >= maxSpeed)
            {
                v = 0;
                rbSpeed = maxSpeed;
            }

            Wheel_RR.motorTorque = v * 100;
            Wheel_RL.motorTorque = v * 100;
            Wheel_FR.steerAngle = h;
            Wheel_FL.steerAngle = h;

            
            // Drifting
            if (Input.GetKey(KeyCode.Space))
            {
                Wheel_FL.steerAngle = h * 2.5f;
                Wheel_FR.steerAngle = h * 2.5f;
            }

            // Braking
            if (Input.GetAxis("Vertical") == 0)
            {
                Wheel_RL.brakeTorque = BrakeForce; // Replace with Deacceleration rate
                Wheel_RR.brakeTorque = BrakeForce; // Replace with Deacceleration rate
            }

            else {
                Wheel_RL.brakeTorque = 0;
                Wheel_RR.brakeTorque = 0;
            }
        }
    }
}
