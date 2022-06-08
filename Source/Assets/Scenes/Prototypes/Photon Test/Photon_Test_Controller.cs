using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Photon_Test_Controller : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
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

        if (photonView.IsMine)
        {
            Photon_Test_Gameplay_Manager.LocalPlayerInstance = this.gameObject;
            carCam = Camera.main;
        }
    }

    private void Update()
    {  
        // Disable all controls if this controller is not the local player.
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        // Camera
        {
            if (carCam != null) 
            {
                Vector3 camPos = transform.position + (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * CameraOffset);     

                Quaternion camRot = Quaternion.Euler(60, transform.eulerAngles.y, 0);
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

    public void OnPhotonInstantiate(PhotonMessageInfo info) 
    {
        Debug.Log("Photon Instantiation Made!");

        // Change Color and Model based on Player Loadout
        int floatyID = (int)info.Sender.CustomProperties["Floaty"];
        int colorID = (int)info.Sender.CustomProperties["Color"];
       
        info.photonView.GetComponent<MeshRenderer>().material = GetMaterialByID(colorID);        
        info.photonView.GetComponentsInChildren<MeshRenderer>()[1].material = GetIconByID(floatyID);
    }
    
    private Material GetMaterialByID(int id) 
    {
        Material mat = null;

        switch (id) {
            case 0: // Red
                mat = Resources.Load<Material>("Materials/Red");
                break;
            case 1: // Green
                mat = Resources.Load<Material>("Materials/Green");
                break;
            case 2: // Blue
                mat = Resources.Load<Material>("Materials/Blue");
                break;
            case 3: // Yellow
                mat = Resources.Load<Material>("Materials/Yellow");
                break;
            case 4: // Orange
                mat = Resources.Load<Material>("Materials/Orange");
                break;
            case 5: // Purple
                mat = Resources.Load<Material>("Materials/Purple");
                break;
            case 6: // Pink
                mat = Resources.Load<Material>("Materials/Pink");
                break;
            case 7: // Cyan
                mat = Resources.Load<Material>("Materials/Cyan");
                break;
            case 8: // White
                mat = Resources.Load<Material>("Materials/White");
                break;
            case 9: // Black
                mat = Resources.Load<Material>("Materials/Black");
                break;

        }

        return mat;
    }

    private Material GetIconByID(int id) 
    {
        Material mat = null;

        switch (id) {
            case 0: // Shibo
                mat = Resources.Load<Material>("Materials/Shiba");
                break;
            case 1: // Flamingo
                mat = Resources.Load<Material>("Materials/Flamingo");
                break;
            case 2: // Pool Floaty
                mat = Resources.Load<Material>("Materials/Pool_Floaty");
                break;
            case 3: // Ducky
                mat = Resources.Load<Material>("Materials/Ducky");
                break;
        }

        return mat;
    }
}
