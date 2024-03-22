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
    public class LobbyModeManager : MonoBehaviour
    {
        [SerializeField] public GameModeData gameModeData;

        [Header("Lobby Specific Parameters"), SerializeField] 
        public string nextSceneName;

        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScoreboard;

        private void Start()
        {
            var lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
            foreach (var trigger in lobbyContainerTriggers)
            {
                trigger.SetNumberOfActivePlayerParameters(PlayerManager.Instance.GetNumberOfActivePlayer());
            }
            ContainerTracker.Instance.AddNewItem(Initializer.InstantiateContainers(0, gameModeData)[0]);

            PlayerManager.Instance.SetJoiningEnabled(true);
            PlayerManager.Instance.SubscribePlayerPush(NewPlayerDetected);
        }

        public void ResetPlayers()
        {
            CannonTracker.Instance.ClearItems();
            foreach (var ls in lobbyScoreboard)
                ls.SetScoreboardActive(false);
            PlayerManager.Instance.ClearAllPlayers();
        }

        public void ExitLobby()
        {
            PlayerManager.Instance.SetJoiningEnabled(false);
            ContainerTracker.Instance.ClearItems();
            CannonTracker.Instance.ClearItems();
            
            SceneManager.LoadScene(nextSceneName);
        }

        private void NewPlayerDetected(int playerIndex, PlayerInput playerInput)
        {
            // Register the player for the container
            var mainContainer = ContainerTracker.Instance.GetItemByIndex(0);
            ContainerTracker.Instance.SetPlayerForItem(playerIndex, mainContainer);
            
            // Instantiate and Set CannonInstance
            var playerInputHandler = PlayerManager.Instance.GetPlayerInputHandler();

            CannonInstance cannonInstance = Initializer.InstantiateCannon(gameModeData, mainContainer);
            Initializer.SetCannonParameters(playerIndex, cannonInstance, mainContainer, gameModeData, gameModeData.skinData.playersSkinData[playerIndex]);
            cannonInstance.SetInputParameters(playerInputHandler);
            cannonInstance.SetCannonInputEnabled(true);
            
            CannonTracker.Instance.AddNewItem(cannonInstance, playerIndex);
            
            // Feedback
            Color popupColor = gameModeData.skinData.playersSkinData[playerIndex].baseColor;
            AddPlayerJoinPopup(playerIndex, cannonInstance, popupColor);

            ConnectScoreboardToPlayer(lobbyScoreboard[playerIndex], popupColor);
        }

        private void ConnectScoreboardToPlayer(Scoreboard scoreboard, Color color)
        {
            scoreboard.connectedColor = color;
            scoreboard.SetScoreboardActive(true);
        }
        
        private void AddPlayerJoinPopup(int playerIndex, CannonInstance cannonInstance, Color randColor)
        {
            var popup = Instantiate(onJoinPopup, cannonInstance.transform);
            var tmp = popup.GetComponent<TextMeshPro>();
            tmp.color = randColor;
            tmp.text = $"P{playerIndex + 1}";
        }
    }
}