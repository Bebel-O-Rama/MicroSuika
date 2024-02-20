using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Player;
using MultiSuika.Skin;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.GameLogic
{
    [CreateAssetMenu(menuName = "Game Logic/Game Mode Data")]
    public class GameModeData : ScriptableObject
    {
        // Skin parameters
        [Header("---- SKIN ----")] 
        public SkinData skinData;
    
        // Container parameters
        [Header("----- CONTAINER -----")]
        public Container.Container containerPrefab;
        public string containerParentName;

        [Header("Container Scaling and Position Parameters")]
        [Tooltip("This distances goes from one container center point to the other")]
        public List<Vector2> leftmostContainerPositions;
        public List<float> containerGeneralScaling;

        [Header("Other Parameters")]
        [Min(1)] public int playerPerContainer = 1;
    
        // Cannon parameters
        [Header("----- CANNON -----")]
        public Cannon.Cannon cannonPrefab;

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
        public BallSetData ballSetData;
    
        // Player parameters
        [FormerlySerializedAs("playerPrefab")] [Header("----- PLAYER -----")]
        public GameObject playerInputPrefab;
    }
}
