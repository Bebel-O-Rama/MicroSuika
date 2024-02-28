using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;

namespace MultiSuika.DebugInfo
{
    public class RacingModeDebugInfo : MonoBehaviour
    {
        [SerializeField] private bool _isDebugEnabled = false;
        
        [Header("Visual Debug Parameters")] 
        [SerializeField] private TMP_Text _tmpMean;
        [SerializeField] private TMP_Text _tmpSD;
        [SerializeField] private TMP_Text _tmpTimeReq;
        [SerializeField] private TMP_Text _tmpPointsReq;

        private FloatReference _averageSpeed;
        private FloatReference _standardDeviationSpeed;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;

        private GameObject _debugHolder;
        
        private void Awake()
        {
            _debugHolder = transform.GetChild(0).gameObject;
        }

        private void Update()
        {
            if (!_isDebugEnabled)
                return;
            _tmpMean.text = string.Format($"{_averageSpeed.Value:0.00}");
            _tmpSD.text = string.Format($"{_standardDeviationSpeed.Value:0.00}");
            _tmpTimeReq.text = string.Format($"{_currentLeadTimeCondition.Value:0.00}");
            _tmpPointsReq.text = string.Format($"{_currentLeadSpeedCondition.Value:0}");
        }

        public void SetupRacingModeDebugParameters(FloatReference averageSpeed, FloatReference stSpeed,
            FloatReference timeCondition, FloatReference speedCondition)
        {
            _averageSpeed = averageSpeed;
            _standardDeviationSpeed = stSpeed;
            _currentLeadTimeCondition = timeCondition;
            _currentLeadSpeedCondition = speedCondition;

            _isDebugEnabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void SetDebugActive(bool isDebugActive) => _debugHolder.SetActive(isDebugActive);
    }
}
