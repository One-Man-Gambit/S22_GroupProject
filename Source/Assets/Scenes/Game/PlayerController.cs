using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    // Game Manager Reference
    GameManager _gm; 

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

    private void Awake() 
    {   
        // Find the game manager within the scene.  Should consider using singleton format instead.
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start() 
    {   
        // Grab the rigidbody component from the object
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {        
        // Camera
        {
            if (carCam != null) 
            {
                // Adjust the camera position based on offset.
                Vector3 camPos = transform.position + (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * CameraOffset);     

                // Adjust the camera angle based on forward direction of controller.
                Quaternion camRot = Quaternion.Euler(40, transform.eulerAngles.y, 0);
                carCam.transform.SetPositionAndRotation(camPos, camRot);
            }            
        }

        // Player cannot move until race begins
        if (!_gm.RaceHasStarted) return;

        // Motion
        {
            // Grab input values and store in floats
            float v = Input.GetAxis("Vertical") * MotorForce;
            float h = Input.GetAxis("Horizontal") * -SteerForce;

            // Apply force to the rigidbody based on vertical input.
            rb.AddForce(transform.forward * v);   

            // Turn the controller based on horizontal input.
            transform.RotateAround(transform.position, Vector3.up, -h * Time.deltaTime);

            // If the speed of the controller exceeds maximum speed, slow it down to maximum speed
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
