using UnityEngine;

namespace MultiSuika.GameLogic
{
    [CreateAssetMenu(menuName = "Versus/Score Handler Data")]
    public class SpeedHandlerData : ScriptableObject
    {
        [Header("Base parameters")] 
        [SerializeField] private float _baseAcceleration; // 3
        
        [Header("Container damage parameters")]
        [SerializeField] private float _damageMultiplier; // 2
        [SerializeField] private float _damageCooldownDuration; // 1
        
        [Header("Damping parameters")]
        [SerializeField] private float _speedSoftCap; // 1200
        [SerializeField] private DampingEvaluationMethod _dampingMethod; // AnimCurve
        [SerializeField] private float _dampingFixedPercent; // 0.02
        [SerializeField] private float _dampingFixedValue; // 1
        [SerializeField] private AnimationCurve _dampingCurvePercent; // (0,0), (0.5 ; 0.015), (1.0 ; 0.05)
        
        [Header("Combo parameters")]
        [SerializeField] private float _timerFullDuration; // 5
        [SerializeField] private bool _isDecreasingMaxTimer; // true
        [SerializeField] private float _fullTimerDecrementValue; // 0.1
        [SerializeField] private float _fullTimerMinValue; // 2
        [SerializeField] private AnimationCurve _comboImpact;
        

        public float BaseAcceleration { get => _baseAcceleration; }
        public float DamageMultiplier { get => _damageMultiplier; }
        public float DamageCooldownDuration { get => _damageCooldownDuration; }
        public float SpeedSoftCap { get => _speedSoftCap; }
        public DampingEvaluationMethod DampingMethod { get => _dampingMethod; }
        public float DampingFixedPercent { get => _dampingFixedPercent; }
        public float DampingFixedValue { get => _dampingFixedValue; }
        public AnimationCurve DampingCurvePercent { get => _dampingCurvePercent; }
        public float TimerFullDuration { get => _timerFullDuration; }
        public bool IsDecreasingMaxTimer { get => _isDecreasingMaxTimer; }
        public float FullTimerDecrementValue { get => _fullTimerDecrementValue; }
        public float FullTimerMinValue { get => _fullTimerMinValue; }
        public AnimationCurve ComboImpact { get => _comboImpact; }
    }
    public enum DampingEvaluationMethod
    {
        FixedPercent,
        Fixed,
        AnimCurve,
        None
    }
}