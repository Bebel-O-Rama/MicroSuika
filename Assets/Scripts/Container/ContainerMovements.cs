using System.Collections;
using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Container
{
    public class ContainerMovements : MonoBehaviour
    {
        [SerializeField] private float _yMinHeight;
        [SerializeField] private float _yMaxHeight;
        
        private ContainerRacingMode _containerRacingData;

        private Camera _containerCamera;
        
        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _speedSoftCap; // 3000
        
        // Combo parameters
        private FloatReference _comboTimerFull; // 3
        private FloatReference _acceleration; // 3
        private IntReference _combo;
        private FloatReference _comboTimer;
        

        // Update is called once per frame
        void Update()
        {
            var amplitude = _yMaxHeight - _yMinHeight;
            if (amplitude <= 0)
                return;

            var camTf = _containerCamera.transform;
            var newPos = camTf.position;
            newPos.y = -(_yMinHeight + _currentSpeed / _speedSoftCap * amplitude);
            camTf.position = newPos;
        }
        
        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference targetSpeed, FloatReference speedSoftCap)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
            _targetSpeed = targetSpeed;
            _speedSoftCap = speedSoftCap;
        }
        
        public void SetComboParameters(FloatReference comboTimerFull, FloatReference acceleration, IntReference combo, FloatReference comboTimer)
        {
            _comboTimerFull = comboTimerFull;
            _acceleration = acceleration;
            _combo = combo;
            _comboTimer = comboTimer;
        }

        public void SetCamera(Camera containerCamera)
        {
            _containerCamera = containerCamera;
            
            // Just so we don't have camera active for nothing
            _containerCamera.gameObject.SetActive(true);
        }
    }
}
