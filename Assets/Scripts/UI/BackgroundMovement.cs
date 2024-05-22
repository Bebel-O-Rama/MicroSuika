using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.UI
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] [Range(0, 360)] private int _movementDirectionAngle;
        [SerializeField] [Min(0f)] private float _baseMovementSpeed;
        [SerializeField] private float _currentMovementSpeed;
        
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private Renderer _renderer;
        private Vector2 _directionalMovement;

        private void Awake()
        {
            _currentMovementSpeed = _baseMovementSpeed;
            _renderer = GetComponent<Renderer>();
            _directionalMovement = new Vector2(Mathf.Cos(_movementDirectionAngle * Mathf.Deg2Rad), Mathf.Sin(_movementDirectionAngle * Mathf.Deg2Rad));
        }

        private void LateUpdate()
        {
            _directionalMovement =  new Vector2(Mathf.Cos(_movementDirectionAngle * Mathf.Deg2Rad), Mathf.Sin(_movementDirectionAngle * Mathf.Deg2Rad));

            var currentOffset = _renderer.material.GetTextureOffset(MainTex);
            var newOffset = currentOffset + (_currentMovementSpeed / 15f) * Time.deltaTime * _directionalMovement;
            _renderer.material.SetTextureOffset(MainTex, newOffset);
        }

        public void UpdateMovementSpeedByMulti(float value)
        {
            _currentMovementSpeed = _baseMovementSpeed * value;
        }
    }
}