using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


public class Checkpoint : MonoBehaviour
{
    public Checkpoint PreviousCheckpoint, NextCheckpoint;
    public UnityEvent OnTriggerEvent;

    public int index;
}
