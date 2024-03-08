using MultiSuika.Utilities;
using TMPro;
using UnityEngine;

namespace MultiSuika.DebugInfo
{
    public class RacingModeDebugInfo : MonoBehaviour
    {
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
        private BoolReference _isDebugActive;
        
        private void Awake()
        {
            _debugHolder = transform.GetChild(0).gameObject;
            if (_debugHolder == null)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            _debugHolder.SetActive(_isDebugActive);
            if (!_isDebugActive)
                return;
            _tmpMean.text = string.Format($"{_averageSpeed.Value:0.00}");
            _tmpSD.text = string.Format($"{_standardDeviationSpeed.Value:0.00}");
            _tmpTimeReq.text = string.Format($"{_currentLeadTimeCondition.Value:0.00}");
            _tmpPointsReq.text = string.Format($"{_currentLeadSpeedCondition.Value:0}");
        }

        public void SetRacingModeParameters(FloatReference averageSpeed, FloatReference stSpeed,
            FloatReference timeCondition, FloatReference speedCondition)
        {
            _averageSpeed = averageSpeed;
            _standardDeviationSpeed = stSpeed;
            _currentLeadTimeCondition = timeCondition;
            _currentLeadSpeedCondition = speedCondition;
        }
        
        public void SetDebugActivationParameters(BoolReference isDebugActive) => _isDebugActive = isDebugActive;
    }
}
