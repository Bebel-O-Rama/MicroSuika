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
    public class ContainerMovements : MonoBehaviour
    {
        [SerializeField] private float _yMinHeight;
        [SerializeField] private float _yMaxHeight;

        [SerializeField] private ContainerCameraHolder _cameraHolder;
        
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

        
        // TODO: Clean this (it's for testing the ranking system)
        private FloatReference _rankingValue;
        private void Start()
        {
            var racingMode = FindObjectOfType<RacingMode>();
            var container = GetComponent<Container>();
            _rankingValue = racingMode.playersRankingValues[container];
        }
        
        // TODO: End cleanup
        
        private void Update()
        {
            // TODO: Clean this (ranking testing)
            _cameraHolder.SetMainVerticalPosition(_yMinHeight + _rankingValue * (_yMaxHeight - _yMinHeight));
            
            // _cameraHolder.SetMainVerticalPosition(EvaluateYPos());
        }

        private float EvaluateYPos()
        {
            var amplitude = _yMaxHeight - _yMinHeight;
            if (amplitude <= 0)
                return 0f;
            
            return -(_yMinHeight + _currentSpeed / _speedSoftCap * amplitude);
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

        // public void SetCamera(Camera containerCamera)
        // {
        //     _containerCamera = containerCamera;
        //     
        //     // Just so we don't have camera active for nothing
        //     _containerCamera.gameObject.SetActive(true);
        // }
    }
}
