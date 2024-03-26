using UnityEngine;

namespace MultiSuika.GameLogic
{
    [CreateAssetMenu(menuName = "Versus/Win Condition Data")]
    public class VersusWinConditionData : ScriptableObject
    {
        [SerializeField] private LeadAdaptiveRequirementMethod _adaptiveRequirementMethod; // AnimCurve
        [SerializeField] private LeadSpeedEvaluationMethod _speedEvaluationMethod; // FromSecond
        [SerializeField] [Min(0f)] private Vector2 _timeRequirementRange; // (3, 7)
        [SerializeField] [Min(0f)] private Vector2 _speedRequirementRange; // (400, 1000)
        [SerializeField] [Min(1f)] private float _timeProgressionLength; // 60
        [SerializeField] [Min(1f)] private float _speedProgressionLength; // 120
        [SerializeField] private AnimationCurve _timeProgressionCurve; // basic sigmoid drop
        [SerializeField] private AnimationCurve _speedProgressionCurve; // basic sigmoid drop
        
        public LeadAdaptiveRequirementMethod AdaptiveRequirementMethod { get => _adaptiveRequirementMethod; }
        public LeadSpeedEvaluationMethod SpeedEvaluationMethod { get => _speedEvaluationMethod; }
        public Vector2 TimeRequirementRange { get => _timeRequirementRange; }
        public Vector2 SpeedRequirementRange { get => _speedRequirementRange; }
        public float TimeProgressionLength { get => _timeProgressionLength; }
        public float SpeedProgressionLength { get => _speedProgressionLength; }
        public AnimationCurve TimeProgressionCurve { get => _timeProgressionCurve; }
        public AnimationCurve SpeedProgressionCurve { get => _speedProgressionCurve; }
    }
    
    public enum LeadAdaptiveRequirementMethod
    {
        Fixed,
        AnimCurve,
        Disabled
    }
    
    public enum LeadSpeedEvaluationMethod
    {
        FromAverage,
        FromSecond,
        FromStart
    }
}