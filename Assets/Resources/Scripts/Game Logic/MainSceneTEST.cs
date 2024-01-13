using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MainSceneTEST : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public GameObject playerPf;

    public List<Player> players;
    
    private void OnEnable()
    {
        foreach (var playerData in gameData.playerDataList)
        {
            if (!playerData.IsPlayerConnected())
                break;
            var playerObj = PlayerInput.Instantiate(playerPf, playerData.playerIndexNumber, pairWithDevice: playerData.inputDevice);
            var player = playerObj.GetComponentInParent<Player>();

            player.InitializePlayer(playerData, gameData.lobby);
        }
    }
}
