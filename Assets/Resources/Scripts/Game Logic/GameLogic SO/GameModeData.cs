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

    [Header("Position Data")] 
    public bool isCannonSpawnXPosRandom = false;
    [Min(0f)] public float cannonVerticalDistanceFromCenter;
    
    [Header("Cannon Basic Parameters")]
    public float cannonSpeed;
    public float cannonReloadCooldown;
    public float cannonShootingForce;
    [Min(0f)] public float emptyDistanceBetweenBallAndCannon;

    
    [Header("Cannon Modifiers")]
    public bool isCannonUsingPeggleMode;

    // Ball parameters
    [Header("----- BALL -----")]
    public GameObject ballPrefab;
    public BallSetData ballSetData;
    
    // Player parameters
    [Header("----- PLAYER -----")]
    public GameObject playerPrefab;
}
