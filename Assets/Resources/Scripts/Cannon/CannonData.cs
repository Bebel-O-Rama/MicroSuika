using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CannonData : ScriptableObject
{
    [Header("Cannon Basic Parameters")]
    public FloatReference speed;
    public FloatReference reloadCooldown;
    public FloatReference shootingForce;
    
    [Header("Cannon Modifiers")]
    public BoolReference isUsingPeggleMode;
}
