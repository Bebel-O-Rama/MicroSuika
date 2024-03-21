using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Damping")]
    public class DampingScoreModifier : ScriptableObject, IScoreModifierData<(FloatReference currentSpeed, FloatReference targetSpeed)>
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public FloatReference speedSoftCap;
        public DampingMethod dampingMethod; // AnimCurve
        public FloatReference dampingFixedPercent; // 0.02
        public FloatReference dampingFixedValue; // 1
        public AnimationCurve dampingCurvePercent; // 0,0 - 0.5 ; 0.015 - 1.0 ; 0.05
        
        private int _playerIndex;
        private FloatReference _currentSpeed;
        private FloatReference _targetSpeed;
        private bool _isActive;
        
        public enum DampingMethod
        {
            FixedPercent,
            Fixed,
            AnimCurve,
            None
        }
        
        public void SetParameters(int playerIndex, (FloatReference currentSpeed, FloatReference targetSpeed) args)
        {
            _playerIndex = playerIndex;
            _currentSpeed = args.currentSpeed;
            _targetSpeed = args.targetSpeed;
        }
        
        public void SetActive(bool isActive)
        {
            _isActive = isActive;        
        }

        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive)
                return ScoreModifierStatus.Continue;
            _targetSpeed.Variable.ApplyChange(_targetSpeed - GetDampingValue() > 0 ? -GetDampingValue() : 0f);
            return ScoreModifierStatus.Continue;
        }
        
        private float GetDampingValue()
        {
            return dampingMethod switch
            {
                DampingMethod.FixedPercent => _currentSpeed * dampingFixedPercent * Time.deltaTime,
                DampingMethod.Fixed => dampingFixedValue * Time.deltaTime,
                DampingMethod.AnimCurve => _currentSpeed * dampingCurvePercent.Evaluate(_currentSpeed / speedSoftCap) * Time.deltaTime,
                DampingMethod.None => 0f,
                _ => 0f
            };
        }
    }
}