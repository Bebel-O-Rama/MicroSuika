using System;
using MultiSuika.DebugInfo;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Container
{
    public class ContainerRacingMode : MonoBehaviour
    {
        [SerializeField] private bool _isDebugEnabled;
        
        [Header("Speed Parameters")]
        [SerializeField] private FloatReference _maxSpeed; // 3000

        [Header("Ball Collision Parameters")] 
        [SerializeField] [Min(0f)] private float _ballImpactMultiplier; // 2 
        
        [Header("Damping Parameters")]
        [SerializeField] private DampingMethod _dampingMethod; // AnimCurve
        [SerializeField] [Range (0f, 1f)] private float _dampingFixedPercent; // 0.02
        [SerializeField] [Min (0f)] private float _dampingFixedValue; // 1
        [SerializeField] private AnimationCurve _dampingCurvePercent; // 0,0 - 0.5 ; 0.015 - 1.0 ; 0.05
        
        [Header("Container Parameters")]
        [SerializeField] private float _containerMaxArea; // 60

        [Header("Combo Parameters")]
        [SerializeField] private FloatReference _comboTimerFull; // 3
        [SerializeField] private FloatReference _acceleration; // 3
        
        // Score parameters
        private IntReference _playerScore;

        // Area parameters
        private FloatReference _areaFilledPercent;
        private FloatReference _areaFilled;

        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;

        // Combo parameters
        private IntReference _combo;
        private FloatReference _comboTimer;

        // Lead parameters
        private BoolReference _leadStatus;
        private FloatReference _leadTimer;
        
        // Ranking parameters
        private IntReference _ranking;

        private ContainerRacingDebugInfo _containerRacingDebugInfo;

        public enum DampingMethod
        {
            FixedPercent,
            Fixed,
            AnimCurve,
            None
        }

        private void Awake()
        {
            _targetSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _areaFilledPercent = _comboTimer = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _combo = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            _comboTimer = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _ranking = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            
            _combo.Variable.SetValue(1);
        }

        private void Start()
        {
            _playerScore = transform.parent.GetComponentInChildren<Cannon.Cannon>().scoreReference;
            
            if (!_isDebugEnabled)
                return;
            _containerRacingDebugInfo = GetComponentInChildren<ContainerRacingDebugInfo>();
            if (_containerRacingDebugInfo == null)
                return;
            
            _containerRacingDebugInfo.SetScoreParameters(_playerScore);
            _containerRacingDebugInfo.SetBallAreaParameters(_areaFilledPercent);
            _containerRacingDebugInfo.SetSpeedParameters(_currentSpeed, _averageSpeed, _targetSpeed, _maxSpeed);
            _containerRacingDebugInfo.SetComboParameters(_combo, _comboTimer, _comboTimerFull, _acceleration);
            _containerRacingDebugInfo.SetLeadParameters(_leadStatus, _leadTimer);
            _containerRacingDebugInfo.SetRankingParameters(_ranking);
        }
        
        private void Update()
        {
            UpdateData();
            _containerRacingDebugInfo?.SetDebugActive(_isDebugEnabled);
        }

        public void NewBallFused(float scoreValue)
        {
            _combo.Variable.ApplyChange(1);
            _comboTimer.Variable.SetValue(_comboTimerFull);
            _targetSpeed.Variable.ApplyChange(scoreValue * 2f);
        }

        public void BallCollidedWithDeadzone(Ball.Ball ball)
        {
            var impactLevel = ball.scoreValue * _ballImpactMultiplier;
            _currentSpeed.Variable.SetValue(Mathf.Clamp(_currentSpeed.Value - impactLevel, 0f, Mathf.Infinity));
            _targetSpeed.Variable.SetValue(Mathf.Clamp(_targetSpeed - impactLevel, 0f, Mathf.Infinity));
            
            // Add dmg feedback
            
            ball.ClearBall(false);
        }
        
        private void UpdateData()
        {
            // Percentage filled value
            _areaFilledPercent.Variable.SetValue(_areaFilled * 100f / _containerMaxArea);
            
            // Combo value
            if (_combo > 1)
            {
                _comboTimer.Variable.ApplyChange(-Time.deltaTime);
                if (_comboTimer < Mathf.Epsilon)
                    _combo.Variable.SetValue(1);
            }
            
            // Damping value
            _targetSpeed.Variable.ApplyChange(_targetSpeed - GetDampingValue() > 0 ? -GetDampingValue() : 0f);
            
            // Speed value
            _currentSpeed.Variable.SetValue(Mathf.MoveTowards(_currentSpeed, _targetSpeed, _acceleration * _combo * Time.deltaTime));
        }
        
        private float GetDampingValue()
        {
            return _dampingMethod switch
            {
                DampingMethod.FixedPercent => _currentSpeed * _dampingFixedPercent * Time.deltaTime,
                DampingMethod.Fixed => _dampingFixedValue * Time.deltaTime,
                DampingMethod.AnimCurve => _currentSpeed * _dampingCurvePercent.Evaluate(_currentSpeed / _maxSpeed) * Time.deltaTime,
                DampingMethod.None => 0f,
                _ => 0f
            };
        }

        public void SetBallAreaParameters(FloatReference areaFilled) => _areaFilled = areaFilled;

        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
        }

        public void SetRankingParameters(IntReference ranking) => _ranking = ranking;

        public void SetLeadParameters(BoolReference leadStatus, FloatReference leadTimer)
        {
            _leadStatus = leadStatus;
            _leadTimer = leadTimer;
        }
    }
}
