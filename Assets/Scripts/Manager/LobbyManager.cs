using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.UI;
using MultiSuika.Utilities;
using MultiSuika.zOther;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MultiSuika.Manager
{
    public class LobbyManager : MonoBehaviour
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
            // Register the player with its control device
            var mainContainer = ContainerTracker.Instance.GetItemByIndex(0);
            ContainerTracker.Instance.SetPlayerForItem(playerIndex, mainContainer);

            // Fetch the PlayerInputHandler
            var playerInputHandler = PlayerManager.Instance.GetPlayerInputHandler();

            // Instantiate and Set the CannonInstance
            var cannon = Instantiate(gameModeData.CannonInstancePrefab, mainContainer.ContainerParent.transform);
            cannon.SetCannonPosition(gameModeData.CannonVerticalDistanceFromCenter, gameModeData.IsCannonXSpawnPositionRandom,
                mainContainer.GetContainerHorizontalHalfLength());

            Initializer.SetCannonParameters(playerIndex, cannon, mainContainer, gameModeData,
                gameModeData.SkinData.playersSkinData[playerIndex]);
            cannon.SetInputParameters(playerInputHandler);
            cannon.SetCannonInputEnabled(true);

            CannonTracker.Instance.AddNewItem(cannon, playerIndex);

            // Other feedback after a player joined the lobby
            Color popupColor = gameModeData.SkinData.playersSkinData[playerIndex].baseColor;
            AddPlayerJoinPopup(playerIndex, cannon, popupColor);

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