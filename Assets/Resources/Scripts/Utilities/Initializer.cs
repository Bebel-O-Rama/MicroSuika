using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;
using Random = System.Random;

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
            var playerInputObj = PlayerInput.Instantiate(gameModeData.playerPrefab, playerData.playerIndexNumber,
                pairWithDevice: playerData.inputDevice);
            instantiatedPlayers.Add(playerInputObj.GetComponentInParent<Player>());
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
    public static List<Container> InstantiateContainers(int playerCount,
        GameModeData gameModeData)
    {
        int containerToSpawn = DivideIntRoundedUp(playerCount, gameModeData.playerPerContainer);
        if (containerToSpawn <= 0)
            return null;

        List<Container> instantiatedContainers = new List<Container>();
        Vector2 distanceBetweenContainers = Vector2.zero;

        distanceBetweenContainers.x = (containerToSpawn > 1)
            ? Mathf.Abs(gameModeData.leftmostContainerPositions[containerToSpawn - 1].x) * 2f /
              (containerToSpawn - 1)
            : 0f;

        for (int i = 0; i < containerToSpawn; i++)
        {
            GameObject containerObj = Object.Instantiate(gameModeData.containerPrefab);
            Container newContainer = containerObj.GetComponent<Container>();
            instantiatedContainers.Add(newContainer);

            GameObject containerParent = new GameObject($"{gameModeData.containerParentName}_{(i + 1)}");
            newContainer.containerParent = containerParent;

            containerParent.transform.position =
                gameModeData.leftmostContainerPositions[containerToSpawn - 1] +
                (i * distanceBetweenContainers);
            containerParent.transform.localScale =
                Vector2.one * gameModeData.containerGeneralScaling[containerToSpawn - 1];
        }

        return instantiatedContainers;
    }

    /// <summary>
    /// Instantiate the cannon(s).
    /// Move them under the appropriate containerParent, move them at the correct starting position and set the
    /// appropriate cannon attributes.
    /// </summary>
    /// <param name="gameModeData"></param>
    /// <param name="containerParent"></param>
    /// <returns></returns>
    public static List<Cannon> InstantiateCannons(int playerCount, GameModeData gameModeData,
        List<Container> containers)
    {
        // int cannonToSpawn = DivideIntRoundedUp(playerCount, gameModeData.playerPerContainer);
        if (!containers.Any() || playerCount <= 0)
            return null;

        List<Cannon> instantiatedCannons = new List<Cannon>();

        for (int i = 0; i < playerCount; i++)
        {
            Container cannonContainer = containers[DivideIntRoundedUp(i + 1, gameModeData.playerPerContainer) - 1];
            GameObject cannonObj =
                Object.Instantiate(gameModeData.cannonPrefab, cannonContainer.containerParent.transform);
            ResetLocalTransform(cannonObj.transform);

            float xPos = UnityEngine.Random.Range(-cannonContainer.GetContainerHorizontalLength() * gameModeData.randomXRange / 2f,
                cannonContainer.GetContainerHorizontalLength() * gameModeData.randomXRange / 2f);

            cannonObj.transform.localPosition = new Vector2(xPos, gameModeData.cannonVerticalDistanceFromCenter);

            instantiatedCannons.Add(cannonObj.GetComponent<Cannon>());
        }
        return instantiatedCannons;
    }

    public static int DivideIntRoundedUp(int a, int b) => a / b + (a % b > 0 ? 1 : 0);

    public static void ResetLocalTransform(Transform child)
    {
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }
}