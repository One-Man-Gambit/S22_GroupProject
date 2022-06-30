using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTargetingType
{

    Directional,
    Positional,
    Self
}

public class AbilitySettings {

    public PlayerController m_Owner;
    public AbilityTargetingType m_TargetingType;    

    // ==================================
    // Universal Settings
    // ==================================

    public float m_MaximumRange;
    public PlayerController m_Target;

    // ===============================
    // Directional System
    // ===============================

    public Vector3 m_TargetDirection;

    public static AbilitySettings InitializeDirectionalCast(PlayerController owner, Vector3 targetDirection, float range) {
        AbilitySettings result = new AbilitySettings();
        result.m_Owner = owner;
        result.m_TargetingType = AbilityTargetingType.Directional;
        result.m_TargetDirection = targetDirection;
        result.m_MaximumRange = range;
        return result;
    }

    // ===============================
    // Positional System
    // ===============================

    public Vector3 m_TargetPosition;
    
    public static AbilitySettings InitializePositionalCast(PlayerController owner, Vector3 targetPosition) {
        AbilitySettings result = new AbilitySettings();
        result.m_Owner = owner;
        result.m_TargetingType = AbilityTargetingType.Positional;
        result.m_TargetPosition = targetPosition;
        return result;
    }

    // ===============================
    // Self System
    // ===============================

    public static AbilitySettings InitializeSelfCast(PlayerController owner) {
        AbilitySettings result = new AbilitySettings();
        result.m_Owner = owner;
        result.m_TargetingType = AbilityTargetingType.Self;
        return result;
    }
}
