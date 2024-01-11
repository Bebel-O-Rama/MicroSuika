using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player Data")]
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    public IntReference activePlayerNumber;
    public List<IntReference> playerMainScores;
    public List<IntReference> playerMiniGameScores;

    [Header("Game Mode Data")]
    public GameModeData lobby;
    public GameModeData mainGame;
    public GameModeData miniGame;
    public GameModeData scoreBoard;
    
    public (int, Player) AddPlayer(Player player)
    {
        players.Add(activePlayerNumber, player);
        activePlayerNumber.Variable.ApplyChange(1);
        return (activePlayerNumber, player);
    }

    public void ClearPlayers()
    {
        foreach (var player in players)
        {
            player.Value.DestroyPlayerCurrentBall();
            Destroy(player.Value.gameObject);
        }
        players.Clear();
        activePlayerNumber.Variable.SetValue(0);
        ResetScores();
    }

    public (IntReference mainScore, IntReference miniGameScore) GetPlayerScoreReferences(int index) => (playerMainScores[index-1], playerMiniGameScores[index-1]);

    public void ResetScores(bool resetEventOnly = false)
    {
        foreach (var score in playerMiniGameScores)
            score.Variable.SetValue(0);

        if (resetEventOnly)
            return;
        
        foreach (var score in playerMainScores)
            score.Variable.SetValue(0);
    }
}
