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

    public int score;

    private void OnEnable()
    {
        score = 0;
    }
}
