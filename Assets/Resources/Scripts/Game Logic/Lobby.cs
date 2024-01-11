using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lobby : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public GameObject onJoinPopup;
    [SerializeField] public List<Scoreboard> lobbyScore;
    
    private GameModeData _lobbyData;
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager.playerJoinedEvent.AddListener(NewPlayerDetected);
        _lobbyData = gameData.lobby;
        gameData.ResetScores();
        ResetPlayers();
        _playerInputManager.EnableJoining();
    }

    public void NewPlayerDetected(PlayerInput playerInput)
    {
        var (playerIndex, player) = gameData.AddPlayer(playerInput.GetComponentInParent<Player>());
        var scores = gameData.GetPlayerScoreReferences(playerIndex);
        player.InitializePlayer(playerIndex, scores.mainScore, scores.miniGameScore, _lobbyData);
        
        // Do custom stuff when a player joins in the lobby
        Color randColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.6f, 1f);
        AddPlayerJoinPopup(playerIndex, player, randColor);
        player.UpdateMainCannonColor(randColor);
        ConnectToLobbyScore(gameData.GetPlayerScoreReferences(playerIndex).mainScore, lobbyScore[playerIndex-1], randColor);
    }

    public void ResetPlayers()
    {
        gameData.ClearPlayers();
        foreach (var ls in lobbyScore)
            ls.playerScore = null;
    }

    public void StartGame()
    {
        _playerInputManager.DisableJoining();
        gameData.ResetScores();
        
        // Do stuff to start the game 
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
}
