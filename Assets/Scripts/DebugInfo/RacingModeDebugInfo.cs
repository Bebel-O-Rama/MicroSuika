using System;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.DebugInfo
{
    public class RacingModeDebugInfo : MonoBehaviour
    {
        [FormerlySerializedAs("_tmpMean")]
        [Header("Visual Debug Parameters")] 
        [SerializeField] private TMP_Text _tmpAverage;
        [SerializeField] private TMP_Text _tmpTimeReq;
        [SerializeField] private TMP_Text _tmpPointsReq;
        
        private FloatReference _averageSpeed;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;

        // private GameObject _debugHolder;
        
        // private void Awake()
        // {
        //     _debugHolder = transform.GetChild(0).gameObject;
        //     if (_debugHolder == null)
        //         gameObject.SetActive(false);
        // }

        private void Start()
        {
            _averageSpeed = ScoreManager.Instance.GetAverageSpeedReference();
            (_currentLeadTimeCondition, _currentLeadSpeedCondition) =
                VersusManager.Instance.GetLeadRequirementReferences();
        }

        private void Update()
        {
            _tmpAverage.text = string.Format($"{_averageSpeed.Value:0.00}");
            _tmpTimeReq.text = string.Format($"{_currentLeadTimeCondition.Value:0.00}");
            _tmpPointsReq.text = string.Format($"{_currentLeadSpeedCondition.Value:0}");
        }
    }
}
