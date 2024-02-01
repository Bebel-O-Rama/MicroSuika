using UnityEngine;

namespace MultiSuika.Container
{
    public class Container : MonoBehaviour
    {
        [SerializeField] [Min(0f)] public float horizontalMvtHalfLength;
        [SerializeField] public SpriteRenderer sideSpriteRenderer;
        [SerializeField] public SpriteRenderer backgroundSpriteRenderer;

        private GameObject _containerParent;
    
        public GameObject ContainerParent
        {
            get => _containerParent;
            set
            {
                _containerParent = value;
                transform.SetParent(_containerParent.transform);
            }
        }
    
        public float GetContainerHorizontalHalfLength() => horizontalMvtHalfLength;
    }
}
