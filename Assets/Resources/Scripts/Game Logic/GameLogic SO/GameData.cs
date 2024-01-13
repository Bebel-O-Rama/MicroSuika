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
    public IntReference numberPlayerActive;

    [Header("Game Mode Data")]
    public GameModeData lobby;
    public GameModeData mainGame;
    public GameModeData miniGame;
    public GameModeData scoreBoard;
    
    public (int, Player, PlayerData) RegisterPlayer(PlayerInput playerInput)
    {
        numberPlayerActive.Variable.ApplyChange(1);
        if (numberPlayerActive.Value >= 4)
        {
            Debug.LogError("Something went awfully wrong, you're trying to register a fifth+ player");
            return (-1, null, null);
        }

        var playerRegistered = playerInput.GetComponentInParent<Player>(); 
        playerDataList[numberPlayerActive.Value].SetInputParameters(playerInput.devices[0], playerInput.user);
        players.Add(playerRegistered);
        return (numberPlayerActive.Value, playerRegistered, playerDataList[numberPlayerActive.Value]);
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
        numberPlayerActive.Variable.SetValue(0);

        foreach (var playerData in playerDataList)
        {
            playerData.ResetInputParameters();
            playerData.ResetMainScore();
            playerData.ResetMiniGameScore();
        }
    }
}
