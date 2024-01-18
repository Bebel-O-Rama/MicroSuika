using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class Initializer : ScriptableObject
{
    [Header("Object Prefabs")] 
    public GameObject playerPf;

    [Header("Other Parameters")] 
    public string containerParentName;

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

    public (List<Container>, List<GameObject>) InstantiateContainers(int containerNeeded,
        ContainerInitializationData containerInitializationData)
    {
        if (containerNeeded <= 0)
            return (null, null);

        List<Container> instantiatedContainers = new List<Container>();
        List<GameObject> containerParents = new List<GameObject>();
        
        Vector2 distanceBetweenContainers = Vector2.zero;

        distanceBetweenContainers.x = (containerNeeded > 1)
            ? Mathf.Abs(containerInitializationData.leftmostContainerPositions[containerNeeded - 1].x) * 2f /
              (containerNeeded - 1)
            : 0f;

        for (int i = 0; i < containerNeeded; i++)
        {
            GameObject containerParent = new GameObject($"{containerParentName}_{(i + 1)}");
            containerParents.Add(containerParent);
            GameObject containerObj = Instantiate(containerInitializationData.containerPrefab, containerParent.transform);
            instantiatedContainers.Add(containerObj.GetComponent<Container>());

            containerParent.transform.position =
                containerInitializationData.leftmostContainerPositions[containerNeeded - 1] +
                (i * distanceBetweenContainers);
            containerParent.transform.localScale =
                Vector2.one * containerInitializationData.containerGeneralScaling[containerNeeded - 1];
        }

        return (instantiatedContainers, containerParents);
    }
}