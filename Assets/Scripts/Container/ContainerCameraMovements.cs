using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerCameraMovements : MonoBehaviour
    {
        [Header("Camera parameters")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _mainVerticalTransform;
        [SerializeField] private Transform _secondaryTransform;
        
        [SerializeField] private float _yMinHeight;
        [SerializeField] private float _yMaxHeight;
        
        private int _playerIndex;
        private FloatReference _normalizedVerticalPosition;
        
        private void Start()
        {
            var container = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(container);
            _normalizedVerticalPosition = ScoreManager.Instance.GetNormalizedSpeedReference(_playerIndex);
            _camera.cullingMask |= (1 << gameObject.layer);
        }

        private void Update()
        {
            var newYPos = _yMinHeight + _normalizedVerticalPosition * (_yMaxHeight - _yMinHeight);
            _mainVerticalTransform.position = new Vector3(0, -newYPos, 0);
        }
    }
}
