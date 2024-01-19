using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cannon/Cannon Spawn Position Data")]
public class CannonInitializationData : ScriptableObject
{
    [Header("Spawn Position Data")] 
    [Min(1)] public int cannonPerContainer = 1;
    public bool randomXSpawn = false;
    [Min(0f)] public float yDistanceFromContainer;
    
    [Header("Cannon Basic Parameters")]
    public FloatReference speed;
    public FloatReference reloadCooldown;
    public FloatReference shootingForce;
    
    [Header("Cannon Modifiers")]
    public BoolReference isUsingPeggleMode;
    // public Vector2 centerPosition;
    // [Min(0f)] public float maxHorizontalDelta;
    // [Min(0f)] public float xRandomSpawnRangeDelta;
}
