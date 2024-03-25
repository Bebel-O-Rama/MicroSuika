using UnityEngine;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Player Variation Skin Data")]
    public class PlayerVariationSkinData : ScriptableObject
    {
        // Container skin parameters
        [Header("----- CONTAINER -----")] 
        [SerializeField] private Sprite _containerBackground;
        [SerializeField] private Sprite _containerSide;
        [SerializeField] private Sprite _containerFailure;
        [SerializeField] private Sprite _containerSuccess;

        // Cannon skin parameters
        [Header("----- CANNON -----")]
        [SerializeField] private Sprite _cannonSprite;

        // Ball skin parameters
        [Header("----- BALL -----")] 
        [SerializeField] private BallSkinData _ballTheme;
    
        // Ball skin parameters
        [Header("----- OTHER -----")] 
        [SerializeField] private Color _baseColor;
        
        
        public Sprite ContainerBackground { get => _containerBackground; }
        public Sprite ContainerSide { get => _containerSide; }
        public Sprite ContainerFailure { get => _containerFailure; }
        public Sprite ContainerSuccess { get => _containerSuccess; }
        public Sprite CannonSprite { get => _cannonSprite; }
        public BallSkinData BallTheme { get => _ballTheme; }
        public Color BaseColor { get => _baseColor; }
    }
}
