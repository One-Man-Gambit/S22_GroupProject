using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


public class CheckpointManager : MonoBehaviour
{
    public UnityEvent OnTriggerEvent;

    // When the checkpoint is struck by the player, trigger all events specified in the inspector;
    private void OnTriggerEnter(Collider other) 
    {        
        OnTriggerEvent.Invoke();    
    }
}
