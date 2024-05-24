using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Player Variation Skin Data")]
    public class PlayerVariationSkinData : ScriptableObject
    {
        // Container skin parameters
        [Header("----- CONTAINER -----")] 
        [SerializeField] private Sprite _containerBackground;
        [SerializeField] private Sprite _containerSide;
        [SerializeField] private Sprite _containerNextBallHolder;
        [SerializeField] private Sprite _containerOutside;
        [SerializeField] private Material _containerGlow;
        

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
        public Sprite ContainerNextBallHolder { get => _containerNextBallHolder; }
        public Sprite ContainerOutside { get => _containerOutside; }
        public Material ContainerGlow { get => _containerGlow; }
        public Sprite CannonSprite { get => _cannonSprite; }
        public BallSkinData BallTheme { get => _ballTheme; }
        public Color BaseColor { get => _baseColor; }
    }
}
