using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Handler Data")]
    public class ScoreHandlerData : ScriptableObject
    {
        [Header("Base parameters")] 
        public FloatReference baseAcceleration; // 3
        
        [Header("Container damage parameters")]
        public FloatReference damageMultiplier; // 2
        public FloatReference percentageInstant; // 0.6
        public FloatReference damageCooldownDuration; // 1.5
        
        [Header("Damping parameters")]
        public FloatReference speedSoftCap; // 1200
        public DampingMethod dampingMethod; // AnimCurve
        public FloatReference dampingFixedPercent; // 0.02
        public FloatReference dampingFixedValue; // 1
        public AnimationCurve dampingCurvePercent; // 0,0 - 0.5 ; 0.015 - 1.0 ; 0.05
        
        [Header("Combo parameters")]
        public FloatReference timerFullDuration; // 5
        public BoolReference isDecreasingMaxTimer; // true
        public FloatReference fullTimerDecrementValue; // 0.1
        public FloatReference fullTimerMinValue; // 2
        
        public enum DampingMethod
        {
            FixedPercent,
            Fixed,
            AnimCurve,
            None
        }
    }
}