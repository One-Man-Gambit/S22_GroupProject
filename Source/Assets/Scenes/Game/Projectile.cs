using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSettings m_Settings;
    private bool isInitialized;

    // Live Variables
    public delegate IEnumerator OnEffectTriggerDelegate(AbilitySettings a_settings);
    public OnEffectTriggerDelegate OnEffectTrigger;
    private List<PlayerController> targetsHit = new List<PlayerController>();
    private float accumulatedDistance = 0;
    private bool isExpiring = false;

    public void SetAbilitySettings(AbilitySettings a_settings) { m_Settings.m_AbilitySettings = a_settings; }
    public void SetSpeed(float newSpeed) { m_Settings.m_ProjectileSpeed = newSpeed; }
    public void SetRadius(float newRadius) { m_Settings.m_ProjectileRadius = newRadius; }

    public void Initialize(ProjectileSettings settings) 
    {
        m_Settings = settings;
        isInitialized = true;
    }

    private void Update() 
    {
        if (!isInitialized || isExpiring) return;

        switch (m_Settings.m_Function) {
            case ProjectileFunction.Directional:
                Vector3 dir_direction = (m_Settings.m_TargetDirection).normalized;
                transform.position += (dir_direction * m_Settings.m_ProjectileSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(dir_direction);
                if (accumulatedDistance >= m_Settings.m_MaximumRange) {
                    OnExpire();
                }
                break;
            case ProjectileFunction.Positional:
                Vector3 pos_direction = (m_Settings.m_TargetPosition - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(pos_direction);
                float pos_distanceToTravel = m_Settings.m_ProjectileSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, m_Settings.m_TargetPosition) > pos_distanceToTravel) {
                    transform.position += (pos_direction * pos_distanceToTravel);                    
                } else {
                    transform.position = m_Settings.m_TargetPosition;
                    OnExpire();
                }
                break;
        }
    }

    bool debugDetonationRadius = false;
    private IEnumerator Detonate() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_Settings.m_ProjectileRadius);
        foreach (var hit in hitColliders) {
            if (hit.gameObject.tag == "Player") {
                m_Settings.m_AbilitySettings.m_Target = hit.gameObject.GetComponent<PlayerController>();                
                CoroutineManager.Instance.StartCoroutine(OnEffectTrigger?.Invoke(m_Settings.m_AbilitySettings));
                targetsHit.Add(hit.gameObject.GetComponent<PlayerController>());
            }
        }

        debugDetonationRadius = true;
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    private void OnExpire()
    {
        isExpiring = true;

        // Positional Projectiles deal damage in an area around their impact point
        if (m_Settings.m_Function == ProjectileFunction.Positional) {
            // Deal damage in radius around impact
            StartCoroutine(Detonate());            
        } else {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (m_Settings.m_Function == ProjectileFunction.Directional) {
            if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>() != m_Settings.m_Owner) {
                m_Settings.m_AbilitySettings.m_Target = other.gameObject.GetComponent<PlayerController>();
                //OnEffectTrigger(m_Settings.m_AbilitySettings);
                
                CoroutineManager.Instance.StartCoroutine(OnEffectTrigger?.Invoke(m_Settings.m_AbilitySettings));
                targetsHit.Add(other.gameObject.GetComponent<PlayerController>());
                OnExpire();
            }
        }
            
    }

    private void OnDrawGizmos() 
    {
        if (debugDetonationRadius) {
            Gizmos.DrawSphere(transform.position, m_Settings.m_ProjectileRadius);
        }    
    }

}
