using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[CreateAssetMenu(menuName = "Game Logic/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player Data")]
    // TODO : Put that back in private!!!
    public List<PlayerData> playerDataList = new List<PlayerData>(4);
    public List<Player> players = new List<Player>();

    [Header("Game Mode Data")]
    public GameModeData lobby;
    public GameModeData mainGame;
    public GameModeData miniGame;
    public GameModeData scoreBoard;
    
    public (Player, PlayerData) RegisterPlayer(PlayerInput playerInput)
    {
        var playerIndex = GetCurrentPlayerQuantity();
        if (GetCurrentPlayerQuantity() >= 4)
        {
            Debug.LogError("Something went awfully wrong, you're trying to register a fifth+ player");
            return (null, null);
        }

        var playerRegistered = playerInput.GetComponentInParent<Player>();
        var newPlayerData = playerDataList[playerIndex];
        
        if (newPlayerData.playerIndexNumber != playerIndex)
            Debug.LogError($"Wrong player index when registering a new player, playerData : {{newPlayerData.playerIndexNumber}} and playerIndex : {playerIndex}");
        
        newPlayerData.SetInputParameters(playerInput.devices[0], playerInput.user);
        players.Add(playerRegistered);
        return (playerRegistered, newPlayerData);
    }

    public void DisconnectPlayers()
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                player.DestroyPlayerCurrentBall();
                Destroy(player.gameObject);
            }
        }
        players.Clear();

        foreach (var playerData in playerDataList)
        {
            playerData.ResetInputParameters();
            playerData.ResetMainScore();
            playerData.ResetMiniGameScore();
        }
    }

    public int GetCurrentPlayerQuantity()
    {
        var playerNumber = 0;
        foreach (var playerData in playerDataList)
        {
            playerNumber += playerData.IsPlayerConnected() ? 1 : 0;
        }
        return playerNumber;
    }
}
