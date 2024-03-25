using System.Collections.Generic;
using System.Linq;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MultiSuika.Manager
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] public GameModeData gameModeData;
        [SerializeField] public string nextSceneName;

        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScoreboard;

        private void Start()
        {
            SpawnContainerLobby();

            PlayerManager.Instance.SetJoiningEnabled(true);
            PlayerManager.Instance.SubscribePlayerPush(NewPlayerDetected);
        }

        public void ResetPlayers()
        {
            ContainerTracker.Instance.ClearPlayersForItem(ContainerTracker.Instance.GetItemByIndex(0));
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
            // Register the player to its control device
            var mainContainer = ContainerTracker.Instance.GetItemByIndex(0);
            ContainerTracker.Instance.SetPlayerForItem(playerIndex, mainContainer);

            SpawnPlayerCannonLobby(playerIndex);

            // Feedback after a player joined the lobby
            Color popupColor = gameModeData.SkinData.GetPlayerSkinData(playerIndex).BaseColor;
            AddPlayerJoinPopup(playerIndex, CannonTracker.Instance.GetItemsByPlayer(playerIndex).First(), popupColor);

            ConnectScoreboardToPlayer(lobbyScoreboard[playerIndex], popupColor);
        }

        private void SpawnContainerLobby()
        {
            var objHolder = GameObject.Find("Objects");
            if (!objHolder)
            {
                objHolder = new GameObject("Objects");
            }
            
            GameObject containerParent = new GameObject($"Container Lobby");
            containerParent.transform.SetParent(objHolder.transform, false);
            ContainerInstance container = Instantiate(gameModeData.ContainerInstancePrefab, containerParent.transform);
            
            ContainerTracker.Instance.AddNewItem(container);
            container.SetContainerParameters(gameModeData);
        }

        private void SpawnPlayerCannonLobby(int playerIndex)
        {
            var mainContainer = ContainerTracker.Instance.GetItemByIndex(0);
            var playerInputHandler = PlayerManager.Instance.GetPlayerInputHandler();

            var cannon = Instantiate(gameModeData.CannonInstancePrefab, mainContainer.ContainerParent.transform);
            CannonTracker.Instance.AddNewItem(cannon, playerIndex);
            
            cannon.SetCannonParameters(playerIndex, gameModeData);
            cannon.SetInputParameters(playerInputHandler);
            cannon.SetCannonInputEnabled(true);
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