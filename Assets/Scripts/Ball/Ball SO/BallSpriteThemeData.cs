using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ball/Ball Sprite Theme Data")]
public class BallSpriteThemeData : ScriptableObject
{
    public List<Sprite> ballSprites;
    public Sprite GetBallSprite(int ballTier) => (ballTier <= ballSprites.Count - 1) ? ballSprites[ballTier] : ballSprites[0];
}
