using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class Initializer : ScriptableObject
{
    [Header("Object Prefabs")] public GameObject playerPf;
    public GameObject cannonPf;

    [Header("Other Parameters")] public string containerParentName;

    /// <summary>
    /// Instantiate the player(s).
    /// Connect each playerInput with the correct input device. 
    /// </summary>
    /// <param name="connectedPlayerData"></param>
    /// <returns></returns>
    public List<Player> InstantiatePlayers(List<PlayerData> connectedPlayerData)
    {
        List<Player> instantiatedPlayers = new List<Player>();
        foreach (var playerData in connectedPlayerData)
        {
            var playerObj = PlayerInput.Instantiate(playerPf, playerData.playerIndexNumber,
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
    /// <param name="containerInitializationData"></param>
    /// <returns></returns>
    public (List<Container>, List<GameObject>) InstantiateContainers(int playerCount,
        ContainerInitializationData containerInitializationData)
    {
        int containerToSpawn = containerInitializationData.usingSingleContainer ? 1 : playerCount;
        if (containerToSpawn <= 0 && !containerInitializationData.usingSingleContainer)
            return (null, null);

        List<Container> instantiatedContainers = new List<Container>();
        List<GameObject> containerParents = new List<GameObject>();

        Vector2 distanceBetweenContainers = Vector2.zero;

        distanceBetweenContainers.x = (containerToSpawn > 1)
            ? Mathf.Abs(containerInitializationData.leftmostContainerPositions[containerToSpawn - 1].x) * 2f /
              (containerToSpawn - 1)
            : 0f;

        for (int i = 0; i < containerToSpawn; i++)
        {
            GameObject containerParent = new GameObject($"{containerParentName}_{(i + 1)}");
            containerParents.Add(containerParent);
            GameObject containerObj =
                Instantiate(containerInitializationData.containerPrefab, containerParent.transform);
            instantiatedContainers.Add(containerObj.GetComponent<Container>());

            containerParent.transform.position =
                containerInitializationData.leftmostContainerPositions[playerCount - 1] +
                (i * distanceBetweenContainers);
            containerParent.transform.localScale =
                Vector2.one * containerInitializationData.containerGeneralScaling[playerCount - 1];
        }

        return (instantiatedContainers, containerParents);
    }

    /// <summary>
    /// Instantiate the cannon(s).
    /// Move them under the appropriate containerParent and set the appropriate cannon attributes.
    /// </summary>
    /// <param name="cannonInitData"></param>
    /// <param name="containerParent"></param>
    /// <returns></returns>
    public List<Cannon> InstantiateCannons(int playerCount, CannonInitializationData cannonInitData,
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