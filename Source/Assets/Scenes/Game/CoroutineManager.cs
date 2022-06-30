using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    private void Start() {
        if (CoroutineManager.Instance != null) {
            Debug.Log("Instance already exists.");
            Destroy(this.gameObject);
        }
        
        CoroutineManager.Instance = this;
    }
}
