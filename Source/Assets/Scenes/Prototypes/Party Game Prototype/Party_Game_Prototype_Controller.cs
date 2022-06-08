using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party_Game_Prototype_Controller : MonoBehaviour
{
    
    
    [Header("Components")]
    public Camera carCam;
    Rigidbody rb = null;   

    [Header("Vehicle Variables")]
    public float maxSpeed = 80.0f;
    public float MotorForce, SteerForce;

    public Vector3 CameraOffset = Vector3.zero;

    [Header("Private Variables")]
    [SerializeField]
    private float rbVelocity = 0;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Camera
        {
            if (carCam != null) 
            {
                Vector3 camPos = transform.position + (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * CameraOffset);     

                Quaternion camRot = Quaternion.Euler(40, transform.eulerAngles.y, 0);
                carCam.transform.SetPositionAndRotation(camPos, camRot);
            }            
        }

        // Motion
        {
            // Moving
            float v = Input.GetAxis("Vertical") * MotorForce;
            float h = Input.GetAxis("Horizontal") * -SteerForce;

            rb.AddForce(transform.forward * v);   

            // Turning
            transform.RotateAround(transform.position, Vector3.up, -h * Time.deltaTime);

            // Braking
            float speed = Vector3.Magnitude(rb.velocity); 
            rbVelocity = speed;
            if (speed > maxSpeed)
            {
                float brakeSpeed = speed - maxSpeed;
                Vector3 normalizedVelocity = rb.velocity.normalized;
                Vector3 brakeVelocity = normalizedVelocity * brakeSpeed;
                rb.AddForce(-brakeVelocity);
            }

        }
    }
}
