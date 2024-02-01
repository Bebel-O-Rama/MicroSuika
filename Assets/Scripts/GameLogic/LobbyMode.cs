using System.Collections.Generic;
using System.Linq;
using MultiSuika.GameLogic.GameLogic_SO;
using MultiSuika.Player;
using MultiSuika.UI;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MultiSuika.GameLogic
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class LobbyMode : MonoBehaviour
    {
        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;
        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScore;
    
        private List<LobbyContainerTrigger> _lobbyContainerTriggers;
        private PlayerInputManager _playerInputManager;
    
        private List<PlayerInputHandler> _playerInputHandlers = new List<PlayerInputHandler>();
        private List<Cannon.Cannon> _cannons = new List<Cannon.Cannon>();
        private List<Container.Container> _containers = new List<Container.Container>();
    
        private void Awake()
        {
            // Connect to the PlayerInputManager and Set the lobbyContainerTrigger
            _playerInputManager = FindObjectOfType<PlayerInputManager>();
            _playerInputManager.playerJoinedEvent.AddListener(NewPlayerDetected);
            _lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();

            _containers = Initializer.InstantiateContainers(0, gameModeData);
        
            // Clear any connected players and enable joining with the PlayerInputManager
            DisconnectPlayers();
            _playerInputManager.EnableJoining();
        }
    
        public void ResetPlayers()
        {
            DisconnectPlayers();
            foreach (var ls in lobbyScore)
                ls.playerScore = null;

            UpdateLobbyTriggers(0);
        }

        public void StartGame()
        {
            _playerInputManager.DisableJoining();
            SceneManager.LoadScene("Versus");
        }

        private void NewPlayerDetected(PlayerInput playerInput)
        {
            var (newPlayerInputHandler, playerIndex) = ConnectPlayerToInputDevice(playerInput);
            _playerInputHandlers.Add(newPlayerInputHandler);
            Cannon.Cannon newCannon = Initializer.InstantiateCannon(gameModeData, _containers[0]);
            _cannons.Add(newCannon);
            Initializer.SetCannonParameters(newCannon, _containers[0], gameModeData, gameData.playerDataList[playerIndex], gameModeData.skinData.playersSkinData[playerIndex]);
            Initializer.ConnectCannonToPlayer(newCannon, newPlayerInputHandler, true);

            // Do custom stuff when a player joins in the lobby
            Color popupColor = gameModeData.skinData.playersSkinData[playerIndex].baseColor;
            AddPlayerJoinPopup(playerIndex, newCannon, popupColor);
        
            ConnectToLobbyScore(gameData.playerDataList[playerIndex].mainScore, lobbyScore[playerIndex], popupColor);
            UpdateLobbyTriggers(gameData.GetConnectedPlayerQuantity());
        }
    
        private void ConnectToLobbyScore(IntReference scoreRef, Scoreboard scoreboard, Color color)
        {
            scoreboard.playerScore = scoreRef.Variable;
            scoreboard.connectedColor = color;
        }

        private (PlayerInputHandler, int) ConnectPlayerToInputDevice(PlayerInput playerInput)
        {
            var playerIndex = gameData.GetConnectedPlayerQuantity();
            if (playerIndex >= 4)
            {
                Debug.LogError("Something went awfully wrong, you're trying to register a fifth+ player");
                return (null, -1);
            }

            var playerInputRegistered = playerInput.GetComponentInParent<PlayerInputHandler>();
            var newPlayerData = gameData.playerDataList[playerIndex];
        
            if (newPlayerData.playerIndexNumber != playerIndex)
                Debug.LogError($"Wrong player index when registering a new player, playerData : {{newPlayerData.playerIndexNumber}} and playerIndex : {playerIndex}");
        
            newPlayerData.SetInputParameters(playerInput.devices[0]);

            return (playerInputRegistered, playerIndex);
        }
    
        private void DisconnectPlayers()
        {
            foreach (var cannon in _cannons)
            {
                cannon.DestroyCurrentBall();
                Destroy(cannon.gameObject);
            }

            foreach (var playerInputHandler in _playerInputHandlers)
            {
                Destroy(playerInputHandler.gameObject);
            }
            _cannons.Clear();
            _playerInputHandlers.Clear();

            foreach (var playerData in gameData.playerDataList)
            {
                playerData.ResetInputParameters();
                playerData.ResetMainScore();
            }
        }
    
        private void AddPlayerJoinPopup(int playerIndex, Cannon.Cannon cannon, Color randColor)
        {
            var popup = Instantiate(onJoinPopup, cannon.transform);
            var tmp = popup.GetComponent<TextMeshPro>();
            tmp.color = randColor;
            tmp.text = $"P{playerIndex + 1}";
        }

        private void UpdateLobbyTriggers(int newPlayerNumber)
        {
            if (_lobbyContainerTriggers == null)
                return;
            foreach (var containerTrigger in _lobbyContainerTriggers)
            {
                containerTrigger.UpdateContainerBehavior(newPlayerNumber);
            }
        }
    }
}
