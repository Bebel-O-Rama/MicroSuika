using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInputManager))]
public class Lobby : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public GameModeData lobbyData;
    [SerializeField] public GameObject onJoinPopup;
    [SerializeField] public List<Scoreboard> lobbyScore;
    
    private List<LobbyContainerTrigger> _lobbyContainerTriggers;
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
        // Parameters getter and setter
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager.playerJoinedEvent.AddListener(NewPlayerDetected);
        _lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
        
        // Clear any connected players and enable joining with the PlayerInputManager
        DisconnectPlayers();
        _playerInputManager.EnableJoining();
    }

    public void NewPlayerDetected(PlayerInput playerInput)
    {
        var (player, playerData) = RegisterPlayer(playerInput);
        
        // Do custom stuff when a player joins in the lobby
        Color randColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.6f, 1f);
        AddPlayerJoinPopup(playerData.playerIndexNumber, player, randColor);
        player.UpdateMainCannonColor(randColor);
        ConnectToLobbyScore(playerData.mainScore, lobbyScore[playerData.playerIndexNumber], randColor);
        UpdateLobbyTriggers(gameData.GetConnectedPlayerQuantity());
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

    private void ConnectToLobbyScore(IntReference scoreRef, Scoreboard scoreboard, Color color)
    {
        scoreboard.playerScore = scoreRef.Variable;
        scoreboard.connectedColor = color;
    }

    private (Player, PlayerData) RegisterPlayer(PlayerInput playerInput)
    {
        var playerIndex = gameData.GetConnectedPlayerQuantity();
        if (playerIndex >= 4)
        {
            Debug.LogError("Something went awfully wrong, you're trying to register a fifth+ player");
            return (null, null);
        }

        var playerRegistered = playerInput.GetComponentInParent<Player>();
        var newPlayerData = gameData.playerDataList[playerIndex];
        
        if (newPlayerData.playerIndexNumber != playerIndex)
            Debug.LogError($"Wrong player index when registering a new player, playerData : {{newPlayerData.playerIndexNumber}} and playerIndex : {playerIndex}");
        
        newPlayerData.SetInputParameters(playerInput.devices[0]);
        playerRegistered.InitializePlayer(newPlayerData, lobbyData);

        return (playerRegistered, newPlayerData);
    }
    
    private void DisconnectPlayers()
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (player != null)
            {
                player.DestroyPlayerCurrentBall();
                Destroy(player.gameObject);
            }
        }

        foreach (var playerData in gameData.playerDataList)
        {
            playerData.ResetInputParameters();
            playerData.ResetMainScore();
            playerData.ResetMiniGameScore();
        }
    }
    
    private void AddPlayerJoinPopup(int playerIndex, Player player, Color randColor)
    {
        var popup = Instantiate(onJoinPopup, player.GetCannon(true).transform);
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
