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
    [Header("ALSO, you can't have null here, you need to drag and drop the BallData SO on the list")]
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

    public BallData GetBallData(int index) => ballSetData.Count > index ? ballSetData[index] : null;

    private void OnValidate()
    {
        ballSetData.RemoveAll(item => item == null);
        ballSetData = ballSetData.OrderBy(ball => ball.index).ToList();
    }

    private void TestingAndCleaningSet()
    {
        // It's not flawless, but at least it takes care of null elements and duplicates. The OnValidate should already take care of the order of the BallData
        var tempSet = new List<BallData>();
        foreach (var ballData in ballSetData)
        {
            if (ballData != null && !tempSet.Contains(ballData))
                tempSet.Add(ballData);
        }
        // if (tempSet != ballSetData)
        //     Debug.LogWarning("Modifications have been made to the BallSetData " + ballSetName);
        ballSetData = tempSet;
    }
    
    private void OnEnable()
    {
        TestingAndCleaningSet();
    }
}