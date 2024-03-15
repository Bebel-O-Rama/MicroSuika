using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.UI;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MultiSuika.GameLogic
{
    public class LobbyModeManager : MonoBehaviour, IGameModeManager
    {
        [Header("Manager Base Parameters")] 
        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;

        [Header("Lobby Specific Parameters")] [SerializeField]
        public string nextSceneName;

        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScore;

        private List<CannonInstance> _cannons = new List<CannonInstance>();
        private List<ContainerInstance> _containers = new List<ContainerInstance>();
        private BallTracker _ballTracker = new BallTracker();

        private void Start()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();

            _cannons = new List<CannonInstance>();
            _containers = new List<ContainerInstance>();
            _ballTracker = new BallTracker();

            var lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
            foreach (var trigger in lobbyContainerTriggers)
            {
                trigger.SetNumberOfActivePlayerParameters(PlayerManager.Instance.GetNumberOfActivePlayer());
            }

            _containers = Initializer.InstantiateContainers(0, gameModeData);

            PlayerManager.Instance.SetJoiningEnabled(true);
            PlayerManager.Instance.SubscribePlayerPush(NewPlayerDetected);
        }

        public void ResetPlayers()
        {
            DisconnectPlayers();
            foreach (var ls in lobbyScore)
                ls.playerScore = null;
            PlayerManager.Instance.ClearAllPlayers();
        }

        public void ExitLobby()
        {
            PlayerManager.Instance.SetJoiningEnabled(false);
            SceneManager.LoadScene(nextSceneName);
        }

        private void NewPlayerDetected(int index, PlayerInput playerInput)
        {
            // Instantiate and Set CannonInstance
            var newPlayerInputHandler = PlayerManager.Instance.GetPlayerInputHandler();

            CannonInstance newCannonInstance = Initializer.InstantiateCannon(gameModeData, _containers[0]);
            Initializer.SetCannonParameters(newCannonInstance, _containers[0], _ballTracker, gameModeData,
                gameData.GetPlayerScoreReference(index), gameModeData.skinData.playersSkinData[index], this);
            newCannonInstance.SetInputParameters(newPlayerInputHandler);
            newCannonInstance.SetCannonInputEnabled(true);
            _cannons.Add(newCannonInstance);


            // Feedback
            Color popupColor = gameModeData.skinData.playersSkinData[index].baseColor;
            AddPlayerJoinPopup(index, newCannonInstance, popupColor);

            ConnectToLobbyScore(gameData.GetPlayerScoreReference(index), lobbyScore[index], popupColor);
        }

        private void ConnectToLobbyScore(IntReference scoreRef, Scoreboard scoreboard, Color color)
        {
            scoreboard.playerScore = scoreRef.Variable;
            scoreboard.connectedColor = color;
        }

        private void DisconnectPlayers()
        {
            foreach (var cannon in _cannons)
            {
                cannon.DestroyCurrentBall();
                Destroy(cannon.gameObject);
            }

            _cannons.Clear();
            
            gameData.ResetPlayerScores();
        }

        private void AddPlayerJoinPopup(int playerIndex, CannonInstance cannonInstance, Color randColor)
        {
            var popup = Instantiate(onJoinPopup, cannonInstance.transform);
            var tmp = popup.GetComponent<TextMeshPro>();
            tmp.color = randColor;
            tmp.text = $"P{playerIndex + 1}";
        }

        public void OnBallFusion(BallInstance ballInstance)
        {
        }
    }
}