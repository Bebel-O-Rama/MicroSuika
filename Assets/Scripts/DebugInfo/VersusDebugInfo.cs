using MultiSuika.Manager;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;

namespace MultiSuika.DebugInfo
{
    public class VersusDebugInfo : MonoBehaviour
    {
        [Header("Visual Debug Parameters")] 
        [SerializeField] private TMP_Text _tmpAverage;
        [SerializeField] private TMP_Text _tmpTimeReq;
        [SerializeField] private TMP_Text _tmpPointsReq;
        
        private FloatReference _averageSpeed;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;

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
