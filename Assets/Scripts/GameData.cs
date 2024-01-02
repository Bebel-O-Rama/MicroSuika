using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data")]
public class GameData : ScriptableObject
{
    [Header("The size of the ball should grow as the index of the positions goes up (small index = small ball)")]
    public List<BallData> ballData;

    public int GetTierOfBall(BallData tier)
    {
        return ballData.IndexOf(tier);
    }

    public BallData GetBallUpgrade(int currentIndex)
    {
        if (ballData.Count - 1 >= currentIndex) return null;
        return ballData[currentIndex + 1];
    }

    private void OnValidate()
    {
        ballData = new List<BallData>(ballData.OrderBy(ball => ball.scale));
    }
}
