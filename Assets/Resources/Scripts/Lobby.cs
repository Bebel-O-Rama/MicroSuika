using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lobby : MonoBehaviour
{
    [SerializeField] public GameData gameData;

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
    }

    public void ResetPlayers()
    {
        gameData.ClearPlayers();
    }

    public void StartGame()
    {
        _playerInputManager.DisableJoining();
        gameData.ResetScores();
        
        // Do stuff to start the game 
    }
}
