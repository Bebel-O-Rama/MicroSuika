using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using MultiSuika.Player;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class RacingMode : MonoBehaviour
    {
        [Header("Debug Parameters")] 
        [SerializeField] public bool canShareRanking;

        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;
        
        private int _numberPlayerConnected;
        private List<PlayerInputHandler> _playerInputHandlers;
        private GameObject _versusGameInstance;
        private List<Container.Container> _containers;
        private List<Cannon.Cannon> _cannons;
        private BallTracker _ballTracker = new BallTracker();

        private Dictionary<Container.Container, FloatReference> _playerCurrentSpeedReferences;
        private Dictionary<Container.Container, IntReference> _playerRankingReferences;
        private FloatReference _averageSpeed;

        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)][SerializeField] public int debugFakeNumberCount = 2;
    
        private void Awake()
        {
            _numberPlayerConnected = gameData.GetConnectedPlayerQuantity();
        
            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            _numberPlayerConnected = useDebugSpawnContainer ? debugFakeNumberCount : _numberPlayerConnected;
            
            //// Init and set containers
            _versusGameInstance = new GameObject("Versus Game Instance");

            _containers = Initializer.InstantiateContainers(_numberPlayerConnected, gameModeData, _versusGameInstance.transform);
            Initializer.SetContainersParameters(_containers, gameModeData);
        
            //// Init and set cannons
            _cannons = Initializer.InstantiateCannons(_numberPlayerConnected, gameModeData,
                _containers);
            Initializer.SetCannonsParameters(_cannons, _containers, _ballTracker, gameModeData, gameData.playerDataList);

            //// Init and set playerInputHandlers
            _playerInputHandlers = Initializer.InstantiatePlayerInputHandlers(gameData.GetConnectedPlayersData(), gameModeData);
            Initializer.ConnectCannonsToPlayerInputs(_cannons, _playerInputHandlers);
            
            
            
            //// Racing Stuff!!!
            SetupRacingDataUI();
        }

        private void Update()
        {
            UpdateAverageSpeed();
            UpdateRanking();
        }
        
        private void SetupRacingDataUI()
        {
            _averageSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _playerCurrentSpeedReferences = new Dictionary<Container.Container, FloatReference>();
            _playerRankingReferences = new Dictionary<Container.Container, IntReference>();
            
            foreach (var container in _containers)
            {
                FloatReference newCurrentSpeedVar = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
                IntReference newPlayerRankingRef = new IntReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
                
                _playerCurrentSpeedReferences[container] = newCurrentSpeedVar;
                _playerRankingReferences[container] = newPlayerRankingRef;

                var newRacingDebugInfo = container.GetComponent<RacingDebugInfo>();
                newRacingDebugInfo.ballAreaRef = _ballTracker.GetBallAreaForContainer(container);
                newRacingDebugInfo.currentSpeed = newCurrentSpeedVar;
                newRacingDebugInfo.averageSpeed = _averageSpeed;
                newRacingDebugInfo.currentRanking = newPlayerRankingRef;
            }
        }

        private void UpdateAverageSpeed()
        {
            _averageSpeed.Variable.SetValue(_playerCurrentSpeedReferences.Sum(x => x.Value) / _playerCurrentSpeedReferences.Count);
        }

        private void UpdateRanking()
        {
            var playerOrder = (from container in _playerCurrentSpeedReferences
                orderby container.Value.Value descending
                select container).ToList();
            
            int rankingIndex = 1;
            for (int i = 0; i < playerOrder.Count(); i++)
            {
                if (i == 0)
                {
                    _playerRankingReferences[playerOrder[i].Key].Variable.SetValue(rankingIndex);
                    continue;
                }

                if (!canShareRanking || Mathf.Abs(playerOrder[i].Value.Value - playerOrder[i - 1].Value.Value) > Mathf.Epsilon)
                    rankingIndex += 1;
                
                _playerRankingReferences[playerOrder[i].Key].Variable.SetValue(rankingIndex);
            }

        }
    }
}
