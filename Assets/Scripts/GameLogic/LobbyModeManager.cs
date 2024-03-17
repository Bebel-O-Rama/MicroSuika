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
        [SerializeField] public GameModeData gameModeData;

        [Header("Lobby Specific Parameters")] [SerializeField]
        public string nextSceneName;

        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScore;

        private BallTracker _ballTracker = new BallTracker();

        private void Start()
        {
            _ballTracker = new BallTracker();

            var lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
            foreach (var trigger in lobbyContainerTriggers)
            {
                trigger.SetNumberOfActivePlayerParameters(PlayerManager.Instance.GetNumberOfActivePlayer());
            }

            var containers = Initializer.InstantiateContainers(0, gameModeData);

            GamePartsManager.Instance.ContainerTracker.AddNewContainer(containers[0]);
            
            PlayerManager.Instance.SetJoiningEnabled(true);
            PlayerManager.Instance.SubscribePlayerPush(NewPlayerDetected);
        }

        public void ResetPlayers()
        {
            CannonTracker.Instance.ClearItems();
            
            ScoreManager.Instance.ResetScoreInformation();
            foreach (var ls in lobbyScore)
                ls.playerScore = null;
            PlayerManager.Instance.ClearAllPlayers();
        }

        public void ExitLobby()
        {
            PlayerManager.Instance.SetJoiningEnabled(false);
            SceneManager.LoadScene(nextSceneName);
        }

        private void NewPlayerDetected(int playerIndex, PlayerInput playerInput)
        {
            // Register the player for the container
            var mainContainer = GamePartsManager.Instance.ContainerTracker.GetContainerByIndex(0);
            GamePartsManager.Instance.ContainerTracker.ConnectPlayerToContainer(mainContainer, playerIndex);
            
            // Instantiate and Set CannonInstance
            var playerInputHandler = PlayerManager.Instance.GetPlayerInputHandler();

            CannonInstance cannonInstance = Initializer.InstantiateCannon(gameModeData, mainContainer);
            Initializer.SetCannonParameters(cannonInstance, mainContainer, _ballTracker, gameModeData,
                ScoreManager.Instance.GetPlayerScoreReference(playerIndex), gameModeData.skinData.playersSkinData[playerIndex], this);
            cannonInstance.SetInputParameters(playerInputHandler);
            cannonInstance.SetCannonInputEnabled(true);
            
            // GamePartsManager.Instance.CannonTracker.AddNewCannon(cannonInstance, playerIndex);
            
            CannonTracker.Instance.AddNewItem(cannonInstance, playerIndex);
            
            // Feedback
            Color popupColor = gameModeData.skinData.playersSkinData[playerIndex].baseColor;
            AddPlayerJoinPopup(playerIndex, cannonInstance, popupColor);

            ConnectToLobbyScore(ScoreManager.Instance.GetPlayerScoreReference(playerIndex), lobbyScore[playerIndex], popupColor);
        }

        private void ConnectToLobbyScore(IntReference scoreRef, Scoreboard scoreboard, Color color)
        {
            scoreboard.playerScore = scoreRef.Variable;
            scoreboard.connectedColor = color;
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