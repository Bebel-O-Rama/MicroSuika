using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public GameObject onJoinPopup;
    [SerializeField] public List<Scoreboard> lobbyScore;
    private List<LobbyContainerTrigger> _lobbyContainerTriggers;
    private GameModeData _lobbyData;
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager.playerJoinedEvent.AddListener(NewPlayerDetected);
        _lobbyData = gameData.lobby;
        _lobbyContainerTriggers = FindObjectsOfType<LobbyContainerTrigger>().ToList();
        gameData.DisconnectPlayers();
        _playerInputManager.EnableJoining();
    }

    public void NewPlayerDetected(PlayerInput playerInput)
    {
        var (player, playerData) = gameData.RegisterPlayer(playerInput);
        
        // Do custom stuff when a player joins in the lobby
        Color randColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.6f, 1f);
        AddPlayerJoinPopup(playerData.playerIndexNumber, player, randColor);
        player.UpdateMainCannonColor(randColor);
        ConnectToLobbyScore(playerData.mainScore, lobbyScore[playerData.playerIndexNumber], randColor);
        UpdateLobbyTriggers(gameData.GetCurrentPlayerQuantity());
    }

    public void ResetPlayers()
    {
        gameData.DisconnectPlayers();
        foreach (var ls in lobbyScore)
            ls.playerScore = null;

        UpdateLobbyTriggers(0);
    }

    public void StartGame()
    {
        _playerInputManager.DisableJoining();
        gameData.ClearPlayerList();
        SceneManager.LoadScene("MainScene");
    }

    private void ConnectToLobbyScore(IntReference scoreRef, Scoreboard scoreboard, Color color)
    {
        scoreboard.playerScore = scoreRef.Variable;
        scoreboard.connectedColor = color;
    }

    private void AddPlayerJoinPopup(int playerIndex, Player player, Color randColor)
    {
        var popup = Instantiate(onJoinPopup, player.GetCannon(true).transform);
        var tmp = popup.GetComponent<TextMeshPro>();
        tmp.color = randColor;
        tmp.text = $"P{playerIndex}";
    }

    private void UpdateLobbyTriggers(int newPlayerNumber)
    {
        foreach (var containerTrigger in _lobbyContainerTriggers)
        {
            containerTrigger.UpdateContainerBehavior(newPlayerNumber);
        }
    }
}
