using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class FMOD_Test_Manager : MonoBehaviour
{
    [Header("3D Spatial Reference Objects")]
    public GameObject SpatialObject;

    [Header("Scroll View")]
    public GameObject ScrollContent;
    public GameObject EventDisplayObject;

    private void Start() {

        foreach (var e in EventManager.Events) {                        
            
            // Instantiation & Reference
            GameObject listing = GameObject.Instantiate(EventDisplayObject, ScrollContent.transform.position, Quaternion.identity);
            FMOD_Test_Button_Handler handler = listing.GetComponent<FMOD_Test_Button_Handler>();

            // Positioning
            listing.transform.SetParent(ScrollContent.transform);                
            listing.transform.localScale = Vector3.one;

            // Initializing the Emitter
            handler.Instance = RuntimeManager.CreateInstance(e.Path);            
            RuntimeManager.AttachInstanceToGameObject(handler.Instance, SpatialObject.transform);
            handler.ButtonEvent.onClick.AddListener(delegate() { 
                
                handler.Instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

                if (state != FMOD.Studio.PLAYBACK_STATE.STOPPED) {
                    handler.Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                } else {
                    handler.Instance.start();
                }
            });

            // Getting Variables
            string eName = e.name.Substring(e.name.LastIndexOf('/') + 1);
           
            // Updating Visible Information
            handler.Identity.text = eName + " - " + e.Path;
            handler.Data.text = "0.00";             
        }
    }
}
