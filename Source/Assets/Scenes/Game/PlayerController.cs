using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    // Game Manager Reference
    GameManager _gm; 
    public Checkpoint CurrentCheckpoint;
    public bool IsDummyPuck = false;
    

    [Header("Components")]
    public Camera carCam;
    Rigidbody rb = null;   

    [Header("Vehicle Variables")]
    public float maxSpeed = 80.0f;
    public float MotorForce, SteerForce;

    public Vector3 CameraOffset = Vector3.zero;

    [Header("Player Variables")]
    public int currentLap;
    public int checkpointCounter;
    public List<bool> checkPointsCompleted;

    [Header("Private Variables")]
    [SerializeField] private float rbVelocity = 0;    

    // Temporary Ability Holder
    private List<Ability> Abilities = new List<Ability>();

    

    private void Awake() 
    {   
        // Find the game manager within the scene.  Should consider using singleton format instead.
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Find the finish line and grab the checkpoint class from it
        CurrentCheckpoint = GameObject.Find("FinishLine").GetComponent<Checkpoint>();
    }

    private void Start() 
    {   
        // Grab the rigidbody component from the object
        rb = GetComponent<Rigidbody>();

        // **TEMP** Load in abilities
        Abilities.Add(ScriptableObject.CreateInstance<Fishing_Rod>());
        Abilities.Add(ScriptableObject.CreateInstance<Harpoon>());
        Abilities.Add(ScriptableObject.CreateInstance<Freeze_Ray>());
        Abilities.Add(ScriptableObject.CreateInstance<Motor_Boost>());
        Abilities.Add(ScriptableObject.CreateInstance<Air_Blast>());
    }

    private void Update()
    {       
        // Too lazy to create a separate class to handle player data.  So we'll just use this class and a temp bool check to disable controls
        if (IsDummyPuck) return;

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

        // ** TEMP ** Ability Activation
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { ActivateAbility(0); }

            if (Input.GetKeyDown(KeyCode.Alpha2)) { ActivateAbility(1); }

            if (Input.GetKeyDown(KeyCode.Alpha3)) { ActivateAbility(2); }

            if (Input.GetKeyDown(KeyCode.Alpha4)) { ActivateAbility(3); }

            if (Input.GetKeyDown(KeyCode.Alpha5)) { ActivateAbility(4); }
        }
    }

    private void ActivateAbility(int index) {
        AbilitySettings settings = null;  
        Vector3 direction = transform.forward;
        Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        switch (Abilities[index].GetTargetingType()) {                   
            case AbilityTargetingType.Directional:                    
                if (Physics.Raycast(screenRay, out RaycastHit dir_hit, 75)) {			
                    settings = AbilitySettings.InitializeDirectionalCast(this, (dir_hit.point - transform.position).normalized, Abilities[index].GetRange());    
                    direction = (dir_hit.point - transform.position).normalized;
                }                        
                break;
            case AbilityTargetingType.Positional:
                if (Physics.Raycast(screenRay, out RaycastHit pos_hit, 75)) {			
                    settings = AbilitySettings.InitializePositionalCast(this, pos_hit.point);   
                    direction = (pos_hit.point - transform.position).normalized;;
                }                    
                break;
            default:
            case AbilityTargetingType.Self:
                settings = AbilitySettings.InitializeSelfCast(this);
                break;
        }
        
        Abilities[index].Cast(settings);
    }

    private void OnTriggerEnter(Collider other) 
    {   
        if (other.gameObject.tag == "Checkpoint") {
            // Get a reference to the checkpoint script
            Checkpoint cp = other.gameObject.GetComponent<Checkpoint>();
            
            // If a checkpoint hasn't been initialized, set the checkpoint.
            if (CurrentCheckpoint == null) {
                CurrentCheckpoint = cp;                
            } 
            
            // If the checkpoint is the current checkpoint's next checkpoint, set the checkpoint.
            // This prevents going in reverse or backtracking for points.
            else if (CurrentCheckpoint.NextCheckpoint == cp) {
                CurrentCheckpoint = cp;
                checkpointCounter++;
            }

            // Activate the specified checkpoint
            checkPointsCompleted[cp.index] = true;      
            _gm.OnLapCompleted(this);       
            
            // Check if lap is completed
            // Check if all checkpoints have been activated
            bool lapComplete = true;  
            for (int i = 0; i < checkPointsCompleted.Count; i++) {
                if (!checkPointsCompleted[i]) {
                    lapComplete = false;
                }
            }

            // If not all checkpoints were activated, then lap is not awarded.
            if (!lapComplete) return;

            // Increment Lap Counter
            currentLap++;

            // Reset all Checkpoints
            for (int i = 0; i < checkPointsCompleted.Count; i++) {
                checkPointsCompleted[i] = false;
            }            
        }    
    }
}
