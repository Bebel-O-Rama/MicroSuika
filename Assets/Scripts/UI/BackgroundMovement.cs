using UnityEngine;

namespace MultiSuika.UI
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] [Range(0f, 2f * Mathf.PI)] private float _movementDirectionAngle;
        [SerializeField] [Min(0f)] private float movementSpeed;
        
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private Renderer _renderer;
        private Vector2 _directionalMovement;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _directionalMovement = new Vector2(Mathf.Cos(_movementDirectionAngle), Mathf.Sin(_movementDirectionAngle));
        }

        private void LateUpdate()
        {
            _directionalMovement = new Vector2(Mathf.Cos(_movementDirectionAngle), Mathf.Sin(_movementDirectionAngle));

            var currentOffset = _renderer.material.GetTextureOffset(MainTex);
            var newOffset = currentOffset + (movementSpeed / 15f) * Time.deltaTime * _directionalMovement;
            _renderer.material.SetTextureOffset(MainTex, newOffset);
        }
    }
}