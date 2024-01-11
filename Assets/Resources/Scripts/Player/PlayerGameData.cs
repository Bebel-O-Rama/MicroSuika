using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Game Data")]
public class PlayerGameData : ScriptableObject
{
    public BallSetData ballSetData;
    public IntReference score;
    public Cannon currentCannon;

    public int GetScore() => score;

    public void AddToScore(int amountAdded) => score.Variable.ApplyChange(amountAdded);
    

    
}
