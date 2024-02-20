using System;
using TMPro;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public class RacingDebugInfo : MonoBehaviour
    {
        [SerializeField] private FloatVariable playerScore;
        [SerializeField] private TMP_Text tmpScore;
        [SerializeField] private TMP_Text tmpScoreVar;
        [SerializeField] private TMP_Text tmpAreaFilled;

        private void Update()
        {
            UpdateScoreValue();
            UpdateScoreVarValue();
            UpdateAreaFilledValue();
        }

        private void UpdateScoreValue()
        {
            tmpScore.text = string.Format($"{playerScore.Value}");

        }
        
        private void UpdateScoreVarValue()
        {
            
        }
        
        private void UpdateAreaFilledValue()
        {
            
        }
    }
}
