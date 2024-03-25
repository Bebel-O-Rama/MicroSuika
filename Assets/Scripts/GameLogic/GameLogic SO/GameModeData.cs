using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.Skin;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    [CreateAssetMenu(menuName = "Game Logic/Game Mode Data")]
    public class GameModeData : ScriptableObject
    {
        [Header("---- SKIN ----")] 
        [SerializeField] private SkinData _skinData;
    
        [Header("----- CONTAINER -----")]
        [SerializeField] private ContainerInstance _containerInstancePrefab;

        [Header("Container Scaling and Position Parameters")]
        [Tooltip("This distances goes from one container center point to the other")]
        [SerializeField] private List<Vector2> _leftmostContainerPositions;
        [SerializeField] private List<float> _containerScaling;

        [Header("----- CANNON -----")]
        [SerializeField] private CannonInstance _cannonInstancePrefab;

        [Header("Position Data")] 
        [SerializeField] private bool _isCannonXSpawnPositionRandom = false;
        [SerializeField] [Min(0f)] private float _cannonVerticalDistanceFromCenter;
    
        [Header("Cannon Basic Parameters")]
        [SerializeField] private float _cannonSpeed;
        [SerializeField] private float _cannonReloadCooldown;
        [SerializeField] private float _cannonShootingForce;
        [SerializeField] [Min(0f)] private float _distanceBetweenBallAndCannon;
        
        [Header("Cannon Modifiers")]
        [SerializeField] private bool _isCannonUsingPeggleMode;

        [Header("----- BALL -----")]
        [SerializeField] private BallSetData _ballSetData;
        
        
        public SkinData SkinData { get => _skinData; }
        public ContainerInstance ContainerInstancePrefab { get => _containerInstancePrefab; }
        public List<Vector2> LeftmostContainerPositions { get => _leftmostContainerPositions; }
        public List<float> ContainerScaling { get => _containerScaling; }
        public CannonInstance CannonInstancePrefab { get => _cannonInstancePrefab; }
        public bool IsCannonXSpawnPositionRandom {  get => _isCannonXSpawnPositionRandom; }
        public float CannonVerticalDistanceFromCenter { get => _cannonVerticalDistanceFromCenter; }
        public float CannonSpeed { get => _cannonSpeed; }
        public float CannonReloadCooldown { get => _cannonReloadCooldown; }
        public float CannonShootingForce { get => _cannonShootingForce; }
        public float DistanceBetweenBallAndCannon { get => _distanceBetweenBallAndCannon; }
        public bool IsCannonUsingPeggleMode { get => _isCannonUsingPeggleMode; }
        public BallSetData BallSetData { get => _ballSetData; }
    }
}
