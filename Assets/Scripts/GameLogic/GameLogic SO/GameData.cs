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
        // [Header("Player Data")] public List<ScoreManager> playerDataList = new List<ScoreManager>(4);

        // public List<IntReference> GetPlayerScoreReferences() => playerDataList.Select(p => p.mainScore).ToList();
        //
        // public IntReference GetPlayerScoreReference(int playerIndex) =>
        //     playerDataList.First(p => p.playerIndexNumber == playerIndex).mainScore;
        //
        // public void ResetPlayerScores()
        // {
        //     for (int i = 0; i <= playerDataList.Count; i++)
        //         ResetPlayerScore(i);
        // }
        //
        // private void ResetPlayerScore(int playerIndex)
        // {
        //     if (playerIndex >= playerDataList.Count)
        //         return;
        //     playerDataList[playerIndex].ResetMainScore();
        // }
    }
}