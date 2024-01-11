using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CannonData : ScriptableObject
{
    [Header("Cannon Basic Parameters")]
    [Min(0f)] public FloatReference speed;
    [Min(0f)] public FloatReference reloadCooldown;
    [Min(0f)] public FloatReference shootingForce;
    
    [Header("Cannon Modifiers")]
    public BoolReference isUsingPeggleMode;
}
