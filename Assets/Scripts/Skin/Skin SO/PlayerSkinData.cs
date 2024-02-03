using MultiSuika.Ball;
using UnityEngine;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Player Skin Data")]
    public class PlayerSkinData : ScriptableObject
    {
        // Container skin parameters
        [Header("----- CONTAINER -----")] 
        public Sprite containerBackground;
        public Sprite containerSide;
        public Sprite containerFailure;

        // Cannon skin parameters
        [Header("----- CANNON -----")]
        public Sprite cannonSprite;

        // Ball skin parameters
        [Header("----- BALL -----")] 
        public BallSpriteThemeData ballTheme;
    
        // Ball skin parameters
        [Header("----- OTHER -----")] 
        public Color baseColor;
    }
}
