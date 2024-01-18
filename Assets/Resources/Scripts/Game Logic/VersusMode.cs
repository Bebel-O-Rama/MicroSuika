using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class VersusMode : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public Initializer initializer;
    [SerializeField] public GameModeData versusData;

    private List<Player> _players;
    private int _numberPlayerConnected;
    private List<Container> _containers;
    private List<GameObject> _containerParents;
    

    [Header("DEBUG DEBUG DEBUG")]
    [Range(0, 4)][SerializeField] public int debugSpawnContainerNumber = 0;
    
    private void Awake()
    {
        // 1. Fetch the initial game parameters
        // 1.1 Get the number of active player
        _numberPlayerConnected = gameData.GetConnectedPlayerQuantity();
        // 1.2 Get the theme used for the game
        // TODO
        
        // 1.3 Set the theme for the background
        // TODO
        
        // 2. Spawn the Container(s)
        // 2.1 Use the correct placement for each container based on the player number (don't scale them!)
        // 2.2 Move the camera accordingly
        // 2.3 Set the theme for each container (don't forget to also edit them based on the playerNumber)
        // 2.4 Keep a reference for each container
        (_containers, _containerParents) = initializer.InstantiateContainers(debugSpawnContainerNumber, versusData.containerInitializationData);
        
        
        
        // 3. Spawn the Cannons
        // 3.1 Use the information of each container (position, horizontal length, etc.) to spawn the cannon at the correct position
        // 3.2 Assign the data for the balls to each cannon
        // 3.3 Set the theme for each cannon (don't forget to also edit them based on the playerNumber)
        // 3.4 Keep a reference for each cannon

        // 4. Spawn the Players (just the player with it's player input, no visual or cannon)
        // 4.1 Assign the correct device to each player
        _players = initializer.InstantiatePlayers(gameData.GetConnectedPlayerData());
        // 4.2 Disable the PlayerInputs for now (we'll enable it when the loading is done)

    }
}
