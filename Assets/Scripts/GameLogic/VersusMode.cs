using System;
using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.GameLogic.GameLogic_SO;
using MultiSuika.Utilities;
using MultiSuika.Player;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class VersusMode : MonoBehaviour
    {
        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;

        private Transform containersParent;
        
        private int _numberPlayerConnected;
        private List<PlayerInputHandler> _playerInputHandlers;
        private List<Cannon.Cannon> _cannons;
        private List<Container.Container> _containers;
        private BallTracker _ballTracker = new BallTracker();
        
        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)][SerializeField] public int debugFakeNumberCount = 2;
        [SerializeField] public bool isSimulatedToggleA = true;
        [SerializeField] public bool isSimulatedToggleB = true;
        private bool _simulatedToggleLastFrameA = true;
        private bool _simulatedToggleLastFrameB = true;

    
        private void Awake()
        {
            _numberPlayerConnected = gameData.GetConnectedPlayerQuantity();
        
            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            _numberPlayerConnected = useDebugSpawnContainer ? debugFakeNumberCount : _numberPlayerConnected;
        
            
            
            //// Init and set containers
            _containers = Initializer.InstantiateContainers(_numberPlayerConnected, gameModeData);
            Initializer.SetContainersParameters(_containers, gameModeData);
        
            //// Init and set cannons
            _cannons = Initializer.InstantiateCannons(_numberPlayerConnected, gameModeData,
                _containers);
            Initializer.SetCannonsParameters(_cannons, _containers, _ballTracker, gameModeData, gameData.playerDataList);
        
            //// Init and set playerInputHandlers
            _playerInputHandlers = Initializer.InstantiatePlayerInputHandlers(gameData.GetConnectedPlayersData(), gameModeData);
            Initializer.ConnectCannonsToPlayerInputs(_cannons, _playerInputHandlers, true);

        }


        private void Update()
        {
            if (isSimulatedToggleA != _simulatedToggleLastFrameA)
            {
                var balls = _ballTracker.GetBallsForContainer(_containers[0]);
                foreach (var ball in balls)
                {
                    ball.SetBallFreeze(isSimulatedToggleA);
                }

                _simulatedToggleLastFrameA = isSimulatedToggleA;
            }
            if (isSimulatedToggleB != _simulatedToggleLastFrameB)
            {
                var balls = _ballTracker.GetBallsForContainer(_containers[1]);
                foreach (var ball in balls)
                {
                    ball.SetBallFreeze(isSimulatedToggleB);
                }

                _simulatedToggleLastFrameB = isSimulatedToggleB;
            }
        }
    }
}
