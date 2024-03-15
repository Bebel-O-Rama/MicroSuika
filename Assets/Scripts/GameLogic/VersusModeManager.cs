using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using MultiSuika.Player;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class VersusModeManager : MonoBehaviour, IGameModeManager
    {
        [SerializeField] public GameModeData gameModeData;
        
        private GameObject _versusGameInstance;
        private List<ContainerInstance> _containers;
        private List<CannonInstance> _cannons;
        private BallTracker _ballTracker = new BallTracker();

        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)][SerializeField] public int debugFakeNumberCount = 2;
    
        private void Awake()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();
        
            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            numberOfActivePlayer = useDebugSpawnContainer ? debugFakeNumberCount : numberOfActivePlayer;
            
            //// Init and set containers
            _containers = Initializer.InstantiateContainers(numberOfActivePlayer, gameModeData);
            Initializer.SetContainersParameters(_containers, gameModeData);
        
            //// Init and set cannons
            _cannons = Initializer.InstantiateCannons(numberOfActivePlayer, gameModeData,
                _containers);
            Initializer.SetCannonsParameters(_cannons, _containers, _ballTracker, gameModeData, ScoreManager.Instance.GetPlayerScoreReferences(), this);
        
            //// Link conditions to the VersusMode instance
            var versusFailConditions = _versusGameInstance.GetComponentsInChildren<VersusFailCondition>().ToList();
            foreach (var failCond in versusFailConditions)
            {
                failCond.SetCondition(this);
            }

            for (int i = 0; i < _cannons.Count; ++i)
            {
                _cannons[i].SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                _cannons[i].SetCannonInputEnabled(true);
            }
        }

        public void PlayerFailure(ContainerInstance containerInstance)
        {
            var balls = _ballTracker.GetBallsForContainer(containerInstance);
            
            foreach (var b in balls)
            {
                b.SetSimulatedParameters(false);
            }

            var cannonsToRemove = _cannons.Where(cannon => cannon.containerInstance == containerInstance).ToList();

            for (int i = 0; i < _cannons.Count; i++)
            {
                if (_cannons[i].containerInstance != containerInstance)
                    break;
                _cannons[i].DisconnectCannonToPlayer();
                cannonsToRemove.Add(_cannons[i]);
            }

            foreach (var cannon in cannonsToRemove)
            {
                cannon.DisconnectCannonToPlayer();
                _cannons.Remove(cannon);
                Destroy(cannon);
            }

            containerInstance.ContainerFailure();
            LookForPlayerSuccess();
        }

        private void LookForPlayerSuccess()
        {
            if (_cannons.Count != 1)
                return;
            
            _cannons[0].DisconnectCannonToPlayer();
            _cannons[0].containerInstance.ContainerSuccess();
        }

        public void OnBallFusion(BallInstance ballInstance)
        {
        }
    }
}
