using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[CreateAssetMenu(menuName = "Game Logic/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player Data")]
    public List<PlayerData> playerDataList = new List<PlayerData>(4);
    public GameObject playerPf;

    // [Header("Game Mode Data")]
    // public GameModeData lobby;
    // public GameModeData mainGame;
    // public GameModeData miniGame;
    // public GameModeData scoreBoard;

    // public void InstantiatePlayers(GameModeData gameModeData)
    // {
    //     foreach (var playerData in playerDataList)
    //     {
    //         if (!playerData.IsPlayerConnected())
    //             break;
    //         var playerObj = PlayerInput.Instantiate(playerPf, playerData.playerIndexNumber, pairWithDevice: playerData.inputDevice);
    //         var player = playerObj.GetComponentInParent<Player>();
    //         
    //         player.InitializePlayer(playerData, gameModeData);
    //     }
    // }

    public List<PlayerData> GetConnectedPlayerData() => playerDataList.Where(pd => pd.IsPlayerConnected()).ToList();

    public int GetConnectedPlayerQuantity()
    {
        var playerNumber = 0;
        foreach (var playerData in playerDataList)
        {
            playerNumber += playerData.IsPlayerConnected() ? 1 : 0;
        }
        return playerNumber;
    }
}
