using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Logic/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player Data")]
    public List<PlayerData> playerDataList = new List<PlayerData>(4);

    public List<PlayerData> GetConnectedPlayersData() => playerDataList.Where(pd => pd.IsPlayerConnected()).ToList();

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
