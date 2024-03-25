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
        public Sprite containerSuccess;

        // Cannon skin parameters
        [Header("----- CANNON -----")]
        public Sprite cannonSprite;

        // Ball skin parameters
        [Header("----- BALL -----")] 
        public BallSkinData ballTheme;
    
        // Ball skin parameters
        [Header("----- OTHER -----")] 
        public Color baseColor;
    }
}
