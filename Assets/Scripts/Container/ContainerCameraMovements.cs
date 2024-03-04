using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using MultiSuika.DebugInfo;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

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
        private FloatReference _currentSpeed;
        private FloatReference _firstPlayerSpeed;
        private FloatReference _lastPlayerSpeed;
        

        // Position parameters
        private FloatReference _verticalPositionRatio;
        private FloatReference _minAdaptiveVerticalRange;

        private void Start()
        {
            _camera.cullingMask |= (1 << gameObject.layer);
        }

        private void Update()
        {
            UpdateVerticalPositionRatio();
            SetMainVerticalPosition(_yMinHeight + _verticalPositionRatio * (_yMaxHeight - _yMinHeight));
        }

        private void UpdateVerticalPositionRatio()
        {
            _verticalPositionRatio.Variable.SetValue(Mathf.Clamp01((_currentSpeed - _lastPlayerSpeed) / Mathf.Max(_minAdaptiveVerticalRange, _firstPlayerSpeed - _lastPlayerSpeed)));
        }
        
        private void SetMainVerticalPosition(float yPos) => _mainVerticalTransform.position = new Vector3(0, -yPos, 0);

        #region Setter
        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference firstPlayerSpeed, FloatReference lastPlayerSpeed)
        {
            _currentSpeed = currentSpeed;
            _firstPlayerSpeed = firstPlayerSpeed;
            _lastPlayerSpeed = lastPlayerSpeed;
        }
        
        public void SetPositionParameters(FloatReference verticalPositionRatio, FloatReference minAdaptiveVerticalRange)
        {
            _verticalPositionRatio = verticalPositionRatio;
            _minAdaptiveVerticalRange = minAdaptiveVerticalRange;
        }
        #endregion

    }
}
