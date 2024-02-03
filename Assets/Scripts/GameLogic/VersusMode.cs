using System.Collections.Generic;
using System.Linq;
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
        
        private int _numberPlayerConnected;
        private List<PlayerInputHandler> _playerInputHandlers;
        private GameObject _versusGameInstance;
        private List<Container.Container> _containers;
        private List<Cannon.Cannon> _cannons;
        private BallTracker _ballTracker = new BallTracker();

        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)][SerializeField] public int debugFakeNumberCount = 2;
        // [SerializeField] public bool isSimulatedToggleA = true;
        // [SerializeField] public bool isSimulatedToggleB = true;
        // private bool _simulatedToggleLastFrameA = true;
        // private bool _simulatedToggleLastFrameB = true;

    
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
        
            //// Link conditions to the VersusMode instance
            var versusFailConditions = _versusGameInstance.GetComponentsInChildren<VersusFailCondition>().ToList();
            foreach (var failCond in versusFailConditions)
            {
                failCond.SetCondition(this);
            }
            
            //// Init and set playerInputHandlers
            _playerInputHandlers = Initializer.InstantiatePlayerInputHandlers(gameData.GetConnectedPlayersData(), gameModeData);
            Initializer.ConnectCannonsToPlayerInputs(_cannons, _playerInputHandlers);
        }

        public void TriggerPlayerFail(Container.Container container)
        {
            // 1. Freeze all the balls in the container
            var balls = _ballTracker.GetBallsForContainer(container);
            foreach (var b in balls)
            {
                b.SetBallFreeze(true);
            }
            // 2. Disconnect the cannon(s) in the container from any PlayerInputHandler
            var cannonsToRemove = new List<Cannon.Cannon>();
            for (int i = 0; i < _cannons.Count; i++)
            {
                if (_cannons[i].container != container)
                    break;
                Initializer.DisconnectCannonFromPlayer(_cannons[i], _playerInputHandlers[i]);
                cannonsToRemove.Add(_cannons[i]);
            }

            foreach (var cannon in cannonsToRemove)
            {
                _cannons.Remove(cannon);
                Destroy(cannon);
            }
            
            // 2.2. Make sure the cannon can't load a ball after the player lost
            
            // 3. Pop the death img on the container
            container.TriggerContainerFailure();
        }
    }
}
