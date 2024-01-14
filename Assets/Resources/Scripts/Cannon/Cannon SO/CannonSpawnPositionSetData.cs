using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cannon/Cannon Spawn Position Set Data")]
public class CannonSpawnPositionSetData : ScriptableObject
{
    public List<CannonSpawnPositionData> twoPlayers;
    public List<CannonSpawnPositionData> threePlayers;
    public List<CannonSpawnPositionData> fourPlayers;
    
    public List<CannonSpawnPositionData> defaultSpawnPositionData;

    public List<CannonSpawnPositionData> GetCannonSpawnPositionData(int connectedPlayerNumber)
    {
        switch (connectedPlayerNumber)
        {
            case 2:
                return twoPlayers.Count == 2 ? twoPlayers : defaultSpawnPositionData;
            case 3:
                return threePlayers.Count == 3 ? threePlayers : defaultSpawnPositionData;
            case 4:
                return fourPlayers.Count == 4 ? fourPlayers : defaultSpawnPositionData;
            default:
                return defaultSpawnPositionData;
        }
    }
}
