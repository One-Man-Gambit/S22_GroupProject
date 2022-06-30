using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileFunction
{
    Directional,    // Moves in a specified direction until hit or maximum distance
    Positional      // Moves towards a specified position, doing nothing until it reaches destination
}

public class ProjectileSettings 
{
    public PlayerController m_Owner;
    public ProjectileFunction m_Function;

    // =============================
    // Universal Settings
    // =============================
    public float m_ProjectileSpeed;
    public float m_ProjectileRadius;
    public float m_MaximumRange;
    public AbilitySettings m_AbilitySettings;

    // =============================
    // Directional System
    // =============================

    public Vector3 m_TargetDirection;

    public static ProjectileSettings InitializeDirectional(PlayerController owner, Vector3 targetDirection, float range)
    {
        ProjectileSettings result = new ProjectileSettings();
        result.m_Owner = owner;
        result.m_Function = ProjectileFunction.Directional;
        result.m_TargetDirection = targetDirection;
        result.m_MaximumRange = range;
        return result;
    }

    // =============================
    // Positional System
    // =============================

    public Vector3 m_TargetPosition;

    public static ProjectileSettings InitializePositional(PlayerController owner, Vector3 targetPosition) 
    {
        ProjectileSettings result = new ProjectileSettings();
        result.m_Owner = owner;
        result.m_Function = ProjectileFunction.Positional;
        result.m_TargetPosition = targetPosition;
        return result;
    }
}
