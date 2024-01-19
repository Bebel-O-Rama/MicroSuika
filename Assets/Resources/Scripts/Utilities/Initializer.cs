using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public static class Initializer
{
    /// <summary>
    /// Instantiate the player(s).
    /// Connect each playerInput with the correct input device. 
    /// </summary>
    /// <param name="connectedPlayerData"></param>
    /// <returns></returns>
    public static List<Player> InstantiatePlayers(List<PlayerData> connectedPlayerData, GameModeData gameModeData)
    {
        List<Player> instantiatedPlayers = new List<Player>();
        foreach (var playerData in connectedPlayerData)
        {
            var playerObj = PlayerInput.Instantiate(gameModeData.playerPrefab, playerData.playerIndexNumber,
                pairWithDevice: playerData.inputDevice);
            instantiatedPlayers.Add(playerObj.GetComponentInParent<Player>());
        }

        return instantiatedPlayers;
    }

    /// <summary>
    /// Instantiate the container(s).
    /// Set them under a unique parent and move/scale them at the right place.
    /// </summary>
    /// <param name="playerCount"></param>
    /// <param name="gameModeData"></param>
    /// <returns></returns>
    public static (List<Container>, List<GameObject>) InstantiateContainers(int playerCount,
        GameModeData gameModeData)
    {
        int containerToSpawn = playerCount / gameModeData.playerPerContainer + (playerCount % gameModeData.playerPerContainer > 0 ? 1 : 0);
        if (containerToSpawn <= 0)
            return (null, null);

        List<Container> instantiatedContainers = new List<Container>();
        List<GameObject> containerParents = new List<GameObject>();
        Vector2 distanceBetweenContainers = Vector2.zero;

        distanceBetweenContainers.x = (containerToSpawn > 1)
            ? Mathf.Abs(gameModeData.leftmostContainerPositions[containerToSpawn - 1].x) * 2f /
              (containerToSpawn - 1)
            : 0f;

        for (int i = 0; i < containerToSpawn; i++)
        {
            GameObject containerParent = new GameObject($"{gameModeData.containerParentName}_{(i + 1)}");
            containerParents.Add(containerParent);
            GameObject containerObj =
                Object.Instantiate(gameModeData.containerPrefab, containerParent.transform);
            instantiatedContainers.Add(containerObj.GetComponent<Container>());

            containerParent.transform.position =
                gameModeData.leftmostContainerPositions[containerToSpawn - 1] +
                (i * distanceBetweenContainers);
            containerParent.transform.localScale =
                Vector2.one * gameModeData.containerGeneralScaling[containerToSpawn - 1];
        }

        return (instantiatedContainers, containerParents);
    }

    /// <summary>
    /// Instantiate the cannon(s).
    /// Move them under the appropriate containerParent and set the appropriate cannon attributes.
    /// </summary>
    /// <param name="gameModeData"></param>
    /// <param name="containerParent"></param>
    /// <returns></returns>
    public static List<Cannon> InstantiateCannons(int playerCount, GameModeData gameModeData,
        List<GameObject> containerParent)
    {
        if (!containerParent.Any())
            return null;
        
        List<Cannon> instantiatedCannons = new List<Cannon>();
        
        for (int i = 0; i < playerCount; i++)
        {
            
        }
        
        
        
        
        return instantiatedCannons;
    }
}