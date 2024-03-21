using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.ScoreSystemTransition;
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
        // #region Singleton
        //
        // [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        // public static LobbyModeManager Instance => _instance ??= new LobbyModeManager();
        //
        // private static LobbyModeManager _instance;
        //
        // private LobbyModeManager()
        // {
        // }
        //
        // #endregion
        //
        // [Header("Score Parameters")]
        [SerializeField] private ScoreHandlerData _scoreHandlerData;
        // public ScoreManager ScoreManager { get; private set; }
        
        [SerializeField] public GameModeData gameModeData;

        [Header("Lobby Specific Parameters"), SerializeField] 
        public string nextSceneName;

        [SerializeField] public GameObject onJoinPopup;
        [SerializeField] public List<Scoreboard> lobbyScore;

        // private BallTracker _ballTracker = new BallTracker();

        private void Awake()
        {
            ScoreHandler.Instance.Initialize(_scoreHandlerData);
        }

        private void Start()
        {
            ScoreHandler.Instance.SetActive(true);
            // _ballTracker = new BallTracker();

            var lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
            foreach (var trigger in lobbyContainerTriggers)
            {
                trigger.SetNumberOfActivePlayerParameters(PlayerManager.Instance.GetNumberOfActivePlayer());
            }
            ContainerTracker.Instance.AddNewItem(Initializer.InstantiateContainers(0, gameModeData)[0]);

            PlayerManager.Instance.SetJoiningEnabled(true);
            PlayerManager.Instance.SubscribePlayerPush(NewPlayerDetected);
        }

        private void Update()
        {
            _scoreHandlerData.UpdateScore();
        }

        public void ResetPlayers()
        {
            CannonTracker.Instance.ClearItems();
            
            ScoreHandler.Instance.ResetScoreInformation();
            foreach (var ls in lobbyScore)
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
            Initializer.SetCannonParameters(playerIndex, cannonInstance, mainContainer, gameModeData,
                ScoreHandler.Instance.GetPlayerScoreReference(playerIndex), gameModeData.skinData.playersSkinData[playerIndex], this);
            cannonInstance.SetInputParameters(playerInputHandler);
            cannonInstance.SetCannonInputEnabled(true);
            
            CannonTracker.Instance.AddNewItem(cannonInstance, playerIndex);
            
            // Feedback
            Color popupColor = gameModeData.skinData.playersSkinData[playerIndex].baseColor;
            AddPlayerJoinPopup(playerIndex, cannonInstance, popupColor);

            ConnectToLobbyScore(lobbyScore[playerIndex], popupColor);
        }

        private void ConnectToLobbyScore(Scoreboard scoreboard, Color color)
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

        public void OnBallFusion(BallInstance ballInstance)
        {
        }
    }
}