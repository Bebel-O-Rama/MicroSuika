using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game Logic/Game Mode Data")]
public class GameModeData : ScriptableObject
{
    // Container parameters
    [Header("----- CONTAINER -----")]
    public GameObject containerPrefab;
    public string containerParentName;

    [Header("Container Scaling and Position Parameters")]
    [Tooltip("This distances goes from one container center point to the other")]
    public List<Vector2> leftmostContainerPositions;
    public List<float> containerGeneralScaling;
    
    [Header("Other Parameters")]
    [Min(1)] public int playerPerContainer = 1;
    
    // Cannon parameters
    [Header("---- CANNON -----")]
    public GameObject cannonPrefab;
    
    [Header("Spawn Position Data")] 
    [Range(0f, 0.85f)] public float randomXRange = 0f;
    [Min(0f)] public float cannonVerticalDistanceFromCenter;
    
    [Header("Cannon Basic Parameters")]
    public float speed;
    public float reloadCooldown;
    public float shootingForce;
    
    [Header("Cannon Modifiers")]
    public bool isUsingPeggleMode;

    // Ball parameters
    [Header("----- BALL -----")]
    public BallSetData ballSetData;
    
    // Player parameters
    [Header("----- PLAYER -----")]
    public GameObject playerPrefab;
}
