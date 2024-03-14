using System.Collections.Generic;
using System.Linq;
using MultiSuika.Player;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    [CreateAssetMenu(menuName = "Game Logic/Game Data")]
    public class GameData : ScriptableObject
    {
        [Header("Player Data")] 
        public List<PlayerDataOLD> playerDataList = new List<PlayerDataOLD>(4);

        public List<PlayerDataOLD> GetConnectedPlayersData() =>
            playerDataList.Where(pd => pd.IsPlayerConnected()).ToList();

        public int GetConnectedPlayerQuantity()
        {
            var playerNumber = 0;
            foreach (var playerData in playerDataList)
            {
                playerNumber += playerData.IsPlayerConnected() ? 1 : 0;
            }

            return playerNumber;
        }

        public List<IntReference> GetPlayerScoreReferences() => playerDataList.Select(p => p.mainScore).ToList();
    }
}