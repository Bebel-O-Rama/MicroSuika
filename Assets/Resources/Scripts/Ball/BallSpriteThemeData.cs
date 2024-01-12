using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BallSpriteThemeData : ScriptableObject
{
    public List<Sprite> ballSprites;
    public Sprite GetBallSprite(int ballTier) => (ballTier <= ballSprites.Count - 1) ? ballSprites[ballTier] : ballSprites[0];
}
