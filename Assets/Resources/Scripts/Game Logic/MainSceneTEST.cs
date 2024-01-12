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
        Debug.Log("Start In");
        for (int i = 0; i < gameData.activePlayerNumber; ++i)
        {
            var playerObj = PlayerInput.Instantiate(playerPf, i, pairWithDevice: gameData.inputDevices[i]);
            var player = playerObj.GetComponentInParent<Player>();
            // var (playerIndex, player) = gameData.AddPlayer(playerObj.GetComponentInParent<Player>());
            var scores = gameData.GetPlayerScoreReferences(i+1);
            player.InitializePlayer(i+1, scores.mainScore, scores.miniGameScore, gameData.lobby);
        
            // Do custom stuff when a player joins in the lobby
            Color randColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.6f, 1f);
            // AddPlayerJoinPopup(playerIndex, player, randColor);
            player.UpdateMainCannonColor(randColor);
            // ConnectToLobbyScore(gameData.GetPlayerScoreReferences(playerIndex).mainScore, lobbyScore[playerIndex-1], randColor);
        }
        Debug.Log("Start Out");
    }

    private void Start()
    {
        // Debug.Log("In OnEnable");
        // for (int i = 0; i < players.Count - 1; ++i)        {
        //     players[i].playerInputHandler._playerInput.user = gameData.inputUsers[i];
        // }
        // Debug.Log("OnEnable Out");
    }

    // private void Update()
    // {
    //     throw new NotImplementedException();
    // }
}
