using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class FMOD_Test_Manager : MonoBehaviour
{
    bool isMusicPlaying = false;
    public StudioEventEmitter musicEmitter;
    public TMPro.TMP_Text musicToggleText;

    private void Awake() {

        // Begin the scene with music playing.
        if (musicEmitter != null && musicToggleText != null) {
            ToggleMusic();
        }
    }

    public void PlayBoop() {
        RuntimeManager.PlayOneShot("event:/SFX/Boop");
    }

    public void PlayBoof() {
        RuntimeManager.PlayOneShot("event:/SFX/Boof");
    }

    public void ToggleMusic() {
        isMusicPlaying = !isMusicPlaying;

        if (isMusicPlaying) {
            musicEmitter.Play(); 
            musicToggleText.text = "Stop Music";
        } else {
            musicEmitter.Stop();
            musicToggleText.text = "Play Music";
        }
    }

}
