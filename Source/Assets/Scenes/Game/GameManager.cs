using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController playerRef;

    public GameSettings Settings;
    public List<GameObject> CheckPoints = new List<GameObject>();
    public TMP_Text StartCountdownDisplay;   
    public TMP_Text GameTimeDisplay;
    public TMP_Text GameLapDisplay;

    public bool RaceHasStarted;
    public bool RaceHasFinished;

    public List<PlayerController> Players = new List<PlayerController>();

    [Header("Private Variables")]
    [SerializeField] private float time;
    //[SerializeField] private int currentLap; // moved to playercontroller
    [SerializeField] private float countdownTimer = 5;    
    
    private void Awake() 
    {   
        // To be decided upon by the menu scene.  This is only temporary
        Settings = new GameSettings(Race_GameMode.Racing) {
            TimeLimit = 600,
            LapLimit = 3
        };
    }

    private void Start() 
    {        
        
        for (int i = 0; i < Players.Count; i++) {
            Players[i].currentLap = 1;
            Players[i].checkpointCounter = 0;

            Players[i].checkPointsCompleted = new List<bool>();
            for (int j = 0; j < CheckPoints.Count; j++) {
                Players[i].checkPointsCompleted.Add(false);
            }
        }

        StartCoroutine("StartRace");
    }

    private void Update() 
    {   
        // Sort Leaderboard
        SortLeaderboard();

        // If race has not started yet, perform the countdown.
        if (!RaceHasStarted) {
            countdownTimer -= Time.deltaTime;

            // Once countdown is at 3 seconds, begin the visual 3.2.1.GO! sequence
            if (countdownTimer < 3) {
                if (!StartCountdownDisplay.enabled) {
                    StartCountdownDisplay.enabled = true;
                }

                StartCountdownDisplay.text = Mathf.Ceil(countdownTimer).ToString();

                if (countdownTimer < 0 && StartCountdownDisplay.enabled) {
                    countdownTimer = 0;
                    StartCountdownDisplay.enabled = false;

                }
            }

            return;
        }

        // Once the race is over, stop counting timer.
        if (RaceHasFinished) return;

        if (Settings.TimeLimit > 0) {
            
            // Update the UI for Time Display
            int minutes = (int)Mathf.Floor(time/60);
            int seconds = (int)(time - (minutes*60));

            if (seconds < 10) {
                GameTimeDisplay.text =  minutes.ToString() + ":0" + seconds.ToString();
            } else {
                GameTimeDisplay.text =  minutes.ToString() + ":" + seconds.ToString();
            }

            // Update Time
            if (time > 0) {                
                time -= Time.deltaTime;
                
            } else if (time < 0) {
                time = 0;
                // Game End
            }            
        }
    }

    private IEnumerator StartRace() 
    {
        Debug.Log("Start Race Called!");

        // Initialize the time limit based on GameSettings
        if (Settings.TimeLimit > 0) {            
            time = Settings.TimeLimit;

            // Update the UI for Time Display
            int minutes = (int)Mathf.Floor(Settings.TimeLimit/60);
            int seconds = (int)(Settings.TimeLimit/60) - minutes;

            if (seconds < 10) {
                GameTimeDisplay.text =  minutes.ToString() + ":0" + seconds.ToString();
            } else {
                GameTimeDisplay.text =  minutes.ToString() + ":" + seconds.ToString();
            }
        } 
        
        // If there is no time limit, just turn off the UI display.
        else {
            GameTimeDisplay.enabled = false;
        }

        // Initialize the lap requirement based on GameSettings
        if (Settings.LapLimit > 0) {
            playerRef.currentLap = 1;

            // Update the UI for Lap Display
            GameLapDisplay.text = "Lap: " + playerRef.currentLap.ToString();
        } 
        
        // If there is no lap limit, just turn off the UI display.
        else  {
            GameLapDisplay.enabled = false;
        }

        yield return new WaitForSeconds(countdownTimer);        

        RaceHasStarted = true;
    }

    public void SortLeaderboard()
    {
        // Player with highest lap comes first, followed by highest checkpoint, followed by closest distance to next checkpoint.        
        List<PlayerController> pc = new List<PlayerController>();
 
        //Debug.Log("Distance Player: " + Vector3.Distance(playerRef.gameObject.transform.position, playerRef.CurrentCheckpoint.NextCheckpoint.gameObject.transform.position));
        //Debug.Log("Distance First: " + Vector3.Distance(Players[0].gameObject.transform.position, Players[0].CurrentCheckpoint.NextCheckpoint.gameObject.transform.position));

        for (int i = 0; i < Players.Count; i++) {
                        
            PlayerController best = null;            

            for (int j = 0; j < Players.Count; j++) {                
                
                PlayerController current = Players[j];

                if (pc.Contains(current)) continue;

                // If there are no best in slot decided yet, then set current as best.
                if (best == null) { 
                    best = current;
                }
 
                // Store distance to next checkpoint for later
                float currentDist = Vector3.Distance(current.gameObject.transform.position, current.CurrentCheckpoint.NextCheckpoint.gameObject.transform.position);
                float bestDist = Vector3.Distance(best.gameObject.transform.position, best.CurrentCheckpoint.NextCheckpoint.gameObject.transform.position);                 

                // Check if lap is greater than best.
                if (current.currentLap > best.currentLap) {
                    Debug.Log("Lap is greater");
                    best = current;
                }

                // Check if current's checkpoint is greater than best's checkpoint
                else if (current.currentLap == best.currentLap && current.checkpointCounter > best.checkpointCounter) {
                    Debug.Log("Checkpoint is greater");
                    best = current;
                }

                // Check if current's distance to next checkpoint is closer than best's distance to next checkpoint
                else if (current.currentLap == best.currentLap && current.CurrentCheckpoint == best.CurrentCheckpoint && currentDist < bestDist) {
                    Debug.Log("Distance is closer");
                    best = current;
                } 
            }

            pc.Add(best);
        }

        Players = pc;
    }

    public void OnLapCompleted(PlayerController pc) 
    {   
        GameLapDisplay.text = "Lap: " + pc.currentLap.ToString();

        // If the last lap is complete, end the game.
        if (pc.currentLap > Settings.LapLimit) {
            // Game End
            GameLapDisplay.text = "Finished!";
            RaceHasFinished = true;
        }
    }

    public void OnCheckpointCompleted(int index) 
    {
              
    }
}
