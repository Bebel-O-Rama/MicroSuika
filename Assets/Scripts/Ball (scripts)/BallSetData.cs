using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Ball Set Data")]
public class BallSetData : ScriptableObject
{
    public string ballSetName;
    [Header("The smaller the index position of the ball, the smaller its radius should be")]
    public List<BallData> ballSetData;

    public int GetTierOfBall(BallData tier)
    {
        return ballSetData.IndexOf(tier);
    }

    public BallData GetBallUpgrade(int currentIndex)
    {
        if (ballSetData.Count - 1 >= currentIndex) return null;
        return ballSetData[currentIndex + 1];
    }

    public BallData GetBallData(int index) => ballSetData[index];

    private void OnValidate()
    {
        ballSetData = ballSetData.OrderBy(ball => ball.index).ToList();
    }

    private void OnEnable()
    {
        var tempSet = new List<BallData>(ballSetData.Count);
        foreach (var ballData in ballSetData)
        {
            if (ballData != null && ballData.index-1 <= tempSet.Count && !tempSet.Contains(ballData))
                tempSet.Insert(ballData.index-1, ballData);
        }
        if (tempSet != ballSetData)
            Debug.LogWarning("Duplicate or null BallData have been cleared from the BallSetData.");
        ballSetData = tempSet;
    }
}