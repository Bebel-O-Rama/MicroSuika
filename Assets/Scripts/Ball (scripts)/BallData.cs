using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ball Data")]
public class BallData : ScriptableObject
{
    [Tooltip("The smaller the index, the smaller the ball should be. The index of the smallest ball should be 1")]
    [Min(1)] public int index;
    public float scale;
    public float mass;
    public Sprite sprite;

    public int GetPointValue()
    {
        // Good old triangular number sequence. It's also used in the original Suika Game
        return index * (index + 1) / 2;
    }
    
}
