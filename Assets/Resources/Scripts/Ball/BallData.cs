using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ball Data")]
public class BallData : ScriptableObject
{
    [Tooltip("The smaller the index, the smaller the ball should be. The index of the smallest ball should be 0")]
    [Min(0)] public int index;
    public float scale;
    public float mass;
    public Sprite sprite;
    [Tooltip("1 is the baseline here")] [Range(0f, 4f)]
    public float spawnChance = 1f;

    // Good old triangular number sequence. It's also used in the original Suika Game
    public int GetScoreValue() => (index+1) * (index + 2) / 2;
}
