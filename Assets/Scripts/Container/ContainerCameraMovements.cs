using System.Linq;
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
        
        // Speed parameters
        // private FloatReference _currentSpeed;
        // private FloatReference _firstPlayerSpeed;
        // private FloatReference _lastPlayerSpeed;
        

        // Position parameters
        private FloatReference _normalizedVerticalPosition;
        // private FloatReference _minAdaptiveVerticalRange;

        private int _playerIndex;
        
        private void Start()
        {
            var container = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayersByItem(container).FirstOrDefault();
            _normalizedVerticalPosition = ScoreManager.Instance.GetNormalizedSpeedReference(_playerIndex);
            _camera.cullingMask |= (1 << gameObject.layer);
        }

        private void Update()
        {
            // UpdateVerticalPositionRatio();
            // SetMainVerticalPosition(_yMinHeight + _normalizedVerticalPosition * (_yMaxHeight - _yMinHeight));
            var newYPos = _yMinHeight + _normalizedVerticalPosition * (_yMaxHeight - _yMinHeight);
            _mainVerticalTransform.position = new Vector3(0, -newYPos, 0);
        }

        // private void UpdateVerticalPositionRatio()
        // {
        //     _normalizedVerticalPosition.Variable.SetValue(Mathf.Clamp01((_currentSpeed - _lastPlayerSpeed) / Mathf.Max(_minAdaptiveVerticalRange, _firstPlayerSpeed - _lastPlayerSpeed)));
        // }
        
        // private void SetMainVerticalPosition(float yPos) => _mainVerticalTransform.position = new Vector3(0, -yPos, 0);

        // #region Setter
        // // public void SetSpeedParameters(FloatReference currentSpeed, FloatReference firstPlayerSpeed, FloatReference lastPlayerSpeed)
        // // {
        // //     _currentSpeed = currentSpeed;
        // //     _firstPlayerSpeed = firstPlayerSpeed;
        // //     _lastPlayerSpeed = lastPlayerSpeed;
        // // }
        //
        // // public void SetPositionParameters(FloatReference verticalPositionRatio, FloatReference minAdaptiveVerticalRange)
        // // {
        // //     _normalizedVerticalPosition = verticalPositionRatio;
        // //     _minAdaptiveVerticalRange = minAdaptiveVerticalRange;
        // // }
        // #endregion

    }
}
