using System.Collections;
using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerMovements : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float _yMinPos = -1.3f;
        [SerializeField] [Min(0f)] private float _yMaxPos = 1.3f;
        
        private ContainerRacingMode _containerRacingData;
        
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
            var amplitude = _yMaxPos - _yMinPos;
            if (amplitude <= 0)
                return;
            
            var newPos = transform.position;
            newPos.y = _yMinPos + _currentSpeed / _speedSoftCap * amplitude;
            transform.position = newPos;
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
    }
}
