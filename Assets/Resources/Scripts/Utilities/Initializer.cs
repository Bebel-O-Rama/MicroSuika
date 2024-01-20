using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public static class Initializer
{
    #region Player
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

    public static void SetPlayersParameters(List<PlayerData> playerDatas, List<Player> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            SetPlayerParameters(playerDatas[i], players[i]);
        }
    }

    public static void SetPlayerParameters(PlayerData playerData, Player player)
    {
        player.score = playerData.mainScore;
    }

    #endregion

    #region Container
    public static List<Container> InstantiateContainers(int playerCount,
        GameModeData gameModeData)
    {
        playerCount = playerCount <= 0 ? 1 : playerCount; // For cases like the lobby
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
            ResetLocalTransform(containerObj.transform);
            
            Container newContainer = containerObj.GetComponent<Container>();
            instantiatedContainers.Add(newContainer);

            GameObject containerParent = new GameObject($"{gameModeData.containerParentName}_{(i + 1)}");
            newContainer.containerParent = containerParent;

            containerParent.transform.position =
                gameModeData.leftmostContainerPositions[containerToSpawn - 1] +
                (i * distanceBetweenContainers);
            containerParent.transform.localScale =
                Vector3.one * gameModeData.containerGeneralScaling[containerToSpawn - 1];
        }

        return instantiatedContainers;
    }

    #endregion

    #region Cannon
    public static List<Cannon> InstantiateCannons(int playerCount, GameModeData gameModeData,
        List<Container> containers)
    {
        if (!containers.Any() || playerCount <= 0)
            return null;

        List<Cannon> instantiatedCannons = new List<Cannon>();

        for (int i = 0; i < playerCount; i++)
        {
            Container cannonContainer = containers[DivideIntRoundedUp(i + 1, gameModeData.playerPerContainer) - 1];
            instantiatedCannons.Add(InstantiateCannon(gameModeData, cannonContainer));
        }

        return instantiatedCannons;
    }

    public static Cannon InstantiateCannon(GameModeData gameModeData, Container container)
    {
        GameObject cannonObj = Object.Instantiate(gameModeData.cannonPrefab, container.containerParent.transform);
        ResetLocalTransform(cannonObj.transform);
        
        float xPos = gameModeData.isCannonSpawnXPosRandom ? Random.Range(-container.GetContainerHorizontalHalfLength(),
            container.GetContainerHorizontalHalfLength()) : 0f;
        cannonObj.transform.localPosition = new Vector2(xPos, gameModeData.cannonVerticalDistanceFromCenter);

        return cannonObj.GetComponent<Cannon>();
    }

    public static void SetCannonsParameters(List<Cannon> cannons, List<Container> containers, GameModeData gameModeData, List<PlayerData> playerData)
    {
        for (int i = 0; i < cannons.Count; ++i)
        {
            SetCannonParameters(cannons[i], containers[DivideIntRoundedUp(i + 1, gameModeData.playerPerContainer) - 1], gameModeData, playerData[i]);
        }
    }

    public static void SetCannonParameters(Cannon cannon, Container container, GameModeData gameModeData, PlayerData playerData)
    {
        cannon.speed = gameModeData.cannonSpeed;
        cannon.reloadCooldown = gameModeData.cannonReloadCooldown;
        cannon.shootingForce = gameModeData.cannonShootingForce;
        cannon.isUsingPeggleMode = gameModeData.isCannonUsingPeggleMode;
        cannon.horizontalMargin = container.GetContainerHorizontalHalfLength();
        
        cannon.ballSetData = gameModeData.ballSetData;
        cannon.scoreReference = playerData.mainScore;
        
        
        // TODO: Remove that when we'll add real skins
        cannon.spriteObj.SetActive(true);
    }

    public static void ConnectCannonsToPlayers(List<Cannon> cannons, List<Player> players, bool isActive)
    {
        for (int i = 0; i < cannons.Count; ++i)
        {
            ConnectCannonToPlayer(cannons[i], players[i], isActive);
        }
    }

    public static void ConnectCannonToPlayer(Cannon cannon, Player player, bool isActive)
    {
        cannon.SetCannonControlConnexion(player.playerInputHandler, isActive);
    }
    #endregion


    public static int DivideIntRoundedUp(int a, int b) => a / b + (a % b > 0 ? 1 : 0);

    public static void ResetLocalTransform(Transform child)
    {
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }
}