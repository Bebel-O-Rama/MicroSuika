using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public BallSetData ballSetData;
    public IntReference score;

    // public void IncrementScore(int incrementValue) => score += incrementValue;
    public int GetScore() => score;

    private void OnEnable()
    {
        // score.Value = 0;
    }
}
