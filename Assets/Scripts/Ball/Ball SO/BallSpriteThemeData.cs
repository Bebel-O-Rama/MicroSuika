using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Ball
{
    [CreateAssetMenu(menuName = "Ball/Ball Sprite Theme Data")]
    public class BallSpriteThemeData : ScriptableObject
    {
        [SerializeField] private List<Sprite> _ballSprites;

        public Sprite GetBallSprite(int ballTier) =>
            (ballTier <= _ballSprites.Count - 1) ? _ballSprites[ballTier] : _ballSprites[0];
    }
}