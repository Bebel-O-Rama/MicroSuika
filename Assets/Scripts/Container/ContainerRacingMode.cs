using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.DebugInfo;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerRacingMode : ContainerInstance
    {
        // Score parameters
        private FloatReference _playerScore;

        // // Area parameters
        // private FloatReference _areaFilledPercent;
        // private FloatReference _areaFilled;
        // private FloatReference _containerMaxArea; // 60
        
        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _speedSoftCap;
        private FloatReference _firstPlayerSpeed;
        private FloatReference _lastPlayerSpeed;

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
        
        // Position parameters
        private FloatReference _verticalPositionRatio;
        private FloatReference _minAdaptiveVerticalRange;
        
        // Collision parameters
        private float _ballImpactMultiplier; // 2 

        // Debug parameters
        private BoolReference _isContainerSpeedBarDebugEnabled;
        private BoolReference _isContainerFullDebugTextEnabled;
        private BoolReference _isContainerAbridgedDebugTextEnabled;
        private FloatReference _debugScoreMultiplier;
        
        private ContainerRacingDebugInfo _containerRacingDebugInfo;
        private ContainerCameraMovements _containerCameraMovements;
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
            // _areaFilledPercent = _comboTimer = new FloatReference
            //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _combo = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            _comboTimer = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            
            // TODO: Why do we instantiate the ranking here?????
            _ranking = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            
            _combo.Variable.SetValue(1);
        }

        private void Start()
        {
            _playerScore = transform.parent.GetComponentInChildren<CannonInstance>().scoreReference;

            SetContainerDebugInfoParameters();
            SetContainerMovementParameters();
        }
        
        // private void Update()
        // {
        //     // _dampingMethod = (DampingMethod)_dampingMethodIndex.Value;
        //     
        //     // UpdateData();
        // }

        // TODO: Move that behaviour in its own data type (it's not the job of the container to do that)
        public void NewBallFused(float scoreValue)
        {
            _combo.Variable.ApplyChange(1);
            _comboTimer.Variable.SetValue(_comboTimerFull);
            _targetSpeed.Variable.ApplyChange(scoreValue * 2f * _debugScoreMultiplier);
        }

        public void DamageReceived(BallInstance ballInstance)
        {
            var impactLevel = ballInstance.ScoreValue * _ballImpactMultiplier;
            _currentSpeed.Variable.SetValue(Mathf.Clamp(_currentSpeed.Value - impactLevel, 0f, Mathf.Infinity));
            _targetSpeed.Variable.SetValue(Mathf.Clamp(_targetSpeed - impactLevel, 0f, Mathf.Infinity));
            
            // Add dmg feedback here
            
            ballInstance.ClearBall(false);
        }
        
        #region Setter

        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference speedSoftCap, FloatReference firstPlayerSpeed, FloatReference lastPlayerSpeed)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
            _speedSoftCap = speedSoftCap;
            _firstPlayerSpeed = firstPlayerSpeed;
            _lastPlayerSpeed = lastPlayerSpeed;
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

        public void SetRankingParameters(IntReference ranking)
        {
            _ranking = ranking;
        }

        public void SetLeadParameters(BoolReference leadStatus, FloatReference leadTimer)
        {
            _leadStatus = leadStatus;
            _leadTimer = leadTimer;
        }

        public void SetCollisionParameters(FloatReference ballImpactMultiplier) =>
            _ballImpactMultiplier = ballImpactMultiplier;

        public void SetPositionParameters(FloatReference verticalPositionRatio, FloatReference minAdaptiveVerticalRange)
        {
            _verticalPositionRatio = verticalPositionRatio;
            _minAdaptiveVerticalRange = minAdaptiveVerticalRange;
        }

        public void SetDebugActivationParameters(BoolReference isContainerSpeedBarDebugEnabled,
            BoolReference isContainerFullDebugTextEnabled, BoolReference isContainerAbridgedDebugTextEnabled,
            FloatReference debugScoreMultiplier)
        {
            _isContainerSpeedBarDebugEnabled = isContainerSpeedBarDebugEnabled;
            _isContainerFullDebugTextEnabled = isContainerFullDebugTextEnabled;
            _isContainerAbridgedDebugTextEnabled = isContainerAbridgedDebugTextEnabled;
            _debugScoreMultiplier = debugScoreMultiplier;
        }
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
            _containerRacingDebugInfo = GetComponentInChildren<ContainerRacingDebugInfo>();
            if (_containerRacingDebugInfo == null)
                return;
            
            // _containerRacingDebugInfo.SetScoreParameters(_playerScore);
            // _containerRacingDebugInfo.SetBallAreaParameters(_areaFilledPercent);
            // _containerRacingDebugInfo.SetSpeedParameters(_currentSpeed, _averageSpeed, _targetSpeed, _speedSoftCap);
            // _containerRacingDebugInfo.SetComboParameters(_combo, _comboTimer, _comboTimerFull, _acceleration);
            // _containerRacingDebugInfo.SetLeadParameters(_leadStatus, _leadTimer);
            // _containerRacingDebugInfo.SetRankingParameters(_ranking);
            // _containerRacingDebugInfo.SetPositionParameters(_verticalPositionRatio);
            _containerRacingDebugInfo.SetDebugActivationParameters(_isContainerSpeedBarDebugEnabled, _isContainerFullDebugTextEnabled, _isContainerAbridgedDebugTextEnabled);
            
            // NOTE: It's a workaround so that Nova's stuff can be parsed through the cameras
            _containerRacingDebugInfo.gameObject.SetActive(false);
            _containerRacingDebugInfo.gameObject.SetActive(true);
        }

        private void SetContainerMovementParameters()
        {
            _containerCameraMovements = GetComponentInChildren<ContainerCameraMovements>();
            
            _containerCameraMovements.SetSpeedParameters(_currentSpeed, _firstPlayerSpeed, _lastPlayerSpeed);
            _containerCameraMovements.SetPositionParameters(_verticalPositionRatio, _minAdaptiveVerticalRange);
        }
    }
}
