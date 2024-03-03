using System;
using MultiSuika.DebugInfo;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Container
{
    public class ContainerRacingMode : MonoBehaviour
    {
        [SerializeField] private bool _isDebugEnabled;
        // TODO: Clean this up once I'm done testing
        [SerializeField] private float _debugScoreMulti = 1f;
        
        // Score parameters
        private IntReference _playerScore;

        // Area parameters
        private FloatReference _areaFilledPercent;
        private FloatReference _areaFilled;
        private FloatReference _containerMaxArea; // 60
        
        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _speedSoftCap; // 3000

        // Damping parameters
        private DampingMethod _dampingMethod; // AnimCurve
        private IntReference _dampingMethodIndex;
        private float _dampingFixedPercent; // 0.02
        private float _dampingFixedValue; // 1
        private AnimationCurve _dampingCurvePercent; // 0,0 - 0.5 ; 0.015 - 1.0 ; 0.05
        
        // Combo parameters
        private FloatReference _comboTimerFull; // 3
        private FloatReference _acceleration; // 3
        private IntReference _combo;
        private FloatReference _comboTimer;

        // Lead parameters
        private BoolReference _leadStatus;
        private FloatReference _leadTimer;
        
        // Ranking parameters
        private IntReference _ranking;
        
        // Collision parameters
        private float _ballImpactMultiplier; // 2 

        private ContainerRacingDebugInfo _containerRacingDebugInfo;
        private ContainerMovements _containerMovements;
        private Camera _containerCamera;

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

            SetContainerDebugInfoParameters();
            SetContainerMovementParameters();
        }
        
        private void Update()
        {
            _dampingMethod = (DampingMethod)_dampingMethodIndex.Value;
            
            UpdateData();
            _containerRacingDebugInfo?.SetDebugActive(_isDebugEnabled);
        }

        public void NewBallFused(float scoreValue)
        {
            _combo.Variable.ApplyChange(1);
            _comboTimer.Variable.SetValue(_comboTimerFull);
            _targetSpeed.Variable.ApplyChange(scoreValue * 2f * _debugScoreMulti);
        }

        public void DamageReceived(Ball.Ball ball)
        {
            var impactLevel = ball.scoreValue * _ballImpactMultiplier;
            _currentSpeed.Variable.SetValue(Mathf.Clamp(_currentSpeed.Value - impactLevel, 0f, Mathf.Infinity));
            _targetSpeed.Variable.SetValue(Mathf.Clamp(_targetSpeed - impactLevel, 0f, Mathf.Infinity));
            
            // Add dmg feedback
            
            ball.ClearBall(false);
        }
        
        #region Setter
        public void SetAreaParameters(FloatReference areaFilled, FloatReference containerMaxArea)
        {
            _areaFilled = areaFilled;
            _containerMaxArea = containerMaxArea;
        }

        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference speedSoftCap)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
            _speedSoftCap = speedSoftCap;
        }

        public void SetDampingParameters(IntReference dampingMethodIndex, FloatReference dampingFixedPercent,
            FloatReference dampingFixedValue, AnimationCurve dampingCurvePercent)
        {
            _dampingMethodIndex = dampingMethodIndex;
            _dampingFixedPercent = dampingFixedPercent;
            _dampingFixedValue = dampingFixedValue;
            _dampingCurvePercent = dampingCurvePercent;
        }

        public void SetComboParameters(FloatReference comboTimerFull, FloatReference acceleration)
        {
            _comboTimerFull = comboTimerFull;
            _acceleration = acceleration;
        }

        public void SetRankingParameters(IntReference ranking) => _ranking = ranking;

        public void SetLeadParameters(BoolReference leadStatus, FloatReference leadTimer)
        {
            _leadStatus = leadStatus;
            _leadTimer = leadTimer;
        }

        public void SetCollisionParameters(FloatReference ballImpactMultiplier) =>
            _ballImpactMultiplier = ballImpactMultiplier;

        public void SetLayer(string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
                return;
            transform.parent.transform.SetLayerRecursively(layer);
        }
        #endregion

        private void SetContainerDebugInfoParameters()
        {
            if (!_isDebugEnabled)
                return;
            
            _containerRacingDebugInfo = GetComponentInChildren<ContainerRacingDebugInfo>();
            if (_containerRacingDebugInfo == null)
                return;
            
            _containerRacingDebugInfo.SetScoreParameters(_playerScore);
            _containerRacingDebugInfo.SetBallAreaParameters(_areaFilledPercent);
            _containerRacingDebugInfo.SetSpeedParameters(_currentSpeed, _averageSpeed, _targetSpeed, _speedSoftCap);
            _containerRacingDebugInfo.SetComboParameters(_combo, _comboTimer, _comboTimerFull, _acceleration);
            _containerRacingDebugInfo.SetLeadParameters(_leadStatus, _leadTimer);
            _containerRacingDebugInfo.SetRankingParameters(_ranking);

            // NOTE: It's a workaround so that Nova's stuff can be parsed through the cameras
            _containerRacingDebugInfo.gameObject.SetActive(false);
            _containerRacingDebugInfo.gameObject.SetActive(true);
        }

        private void SetContainerMovementParameters()
        {
            _containerMovements = GetComponent<ContainerMovements>();
            
            _containerMovements.SetSpeedParameters(_currentSpeed, _averageSpeed, _targetSpeed, _speedSoftCap);
            _containerMovements.SetComboParameters(_comboTimerFull, _acceleration, _combo, _comboTimer);
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
            // TODO: Confirm if we should use this to make sure the damping doesn't benefit from the combo
            var acceleration = _currentSpeed < _targetSpeed ? _acceleration * (_combo * _debugScoreMulti) * Time.deltaTime : _acceleration;
            _currentSpeed.Variable.SetValue(Mathf.MoveTowards(_currentSpeed, _targetSpeed, acceleration));
        }
        
        private float GetDampingValue()
        {
            return _dampingMethod switch
            {
                DampingMethod.FixedPercent => _currentSpeed * _dampingFixedPercent * Time.deltaTime,
                DampingMethod.Fixed => _dampingFixedValue * Time.deltaTime,
                DampingMethod.AnimCurve => _currentSpeed * _dampingCurvePercent.Evaluate(_currentSpeed / _speedSoftCap) * Time.deltaTime,
                DampingMethod.None => 0f,
                _ => 0f
            };
        }
    }
}
