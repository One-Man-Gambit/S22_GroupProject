using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour
{
    public GameSettings Settings;
    public List<GameObject> CheckPoints = new List<GameObject>();
    public TMP_Text StartCountdownDisplay;   
    public TMP_Text GameTimeDisplay;
    public TMP_Text GameLapDisplay;

    public bool RaceHasStarted;
    public bool RaceHasFinished;

    [Header("Private Variables")]
    [SerializeField] private float time;
    [SerializeField] private int currentLap;
    [SerializeField] private float countdownTimer = 5;    
    [SerializeField] private  List<bool> checkPointsCompleted;

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
        checkPointsCompleted = new List<bool>();
        for (int i = 0; i < CheckPoints.Count; i++) {
            checkPointsCompleted.Add(false);
        }

        StartCoroutine("StartRace");
    }

    private void Update() 
    {   
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
            currentLap = 1;

            // Update the UI for Lap Display
            GameLapDisplay.text = "Lap: " + currentLap.ToString();
        } 
        
        // If there is no lap limit, just turn off the UI display.
        else  {
            GameLapDisplay.enabled = false;
        }

        yield return new WaitForSeconds(countdownTimer);        

        RaceHasStarted = true;
    }

    public void OnLapCompleted() 
    {   
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
        GameLapDisplay.text = "Lap: " + currentLap.ToString();

        // Reset all Checkpoints
        for (int i = 0; i < checkPointsCompleted.Count; i++) {
            checkPointsCompleted[i] = false;
        }

        // If the last lap is complete, end the game.
        if (currentLap > Settings.LapLimit) {
            // Game End
            GameLapDisplay.text = "Finished!";
            RaceHasFinished = true;
        }
    }

    public void OnCheckpointCompleted(int index) 
    {
        // Activate the specified checkpoint
        checkPointsCompleted[index] = true;
    }
}
