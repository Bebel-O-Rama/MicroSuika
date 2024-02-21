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
        [Header("NEED TO REFACTOR THE SCRIPT, IT'S JUST FOR PROTOTYPING")] 
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

        private void SetupRacingDataUI()
        {
            foreach (var container in _containers)
            {
                container.GetComponent<RacingDebugInfo>().ballAreaRef = _ballTracker.GetBallAreaForContainer(container);
            }
        }
        
        
    }
}
