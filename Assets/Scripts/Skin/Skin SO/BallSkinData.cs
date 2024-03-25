using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Ball Skin Data")]
    public class BallSkinData : ScriptableObject
    {
        [SerializeField] private List<Sprite> _ballSprites;

        public Sprite GetBallSprite(int ballTier) =>
            (ballTier <= _ballSprites.Count - 1) ? _ballSprites[ballTier] : _ballSprites[0];
    }
}