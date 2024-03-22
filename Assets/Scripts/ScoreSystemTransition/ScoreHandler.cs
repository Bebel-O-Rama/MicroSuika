using System;
using System.Collections;
using System.Security.Cryptography;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.ScoreSystemTransition
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] public int playerIndex;

        // Main score parameters
        [SerializeField] private FloatReference _currentSpeed;
        [SerializeField] private FloatReference _targetSpeed;
        [SerializeField] private IntReference _combo;

        // Container damage parameters
        private float _damageMultiplier;
        private float _percentageInstant;

        private bool _isInDamageCooldown;
        private float _damageCooldownDuration;
        private Coroutine _damageCooldownCoroutine;

        // Damping parameters
        private FloatReference _speedSoftCap;
        private ScoreHandlerData.DampingMethod _dampingMethod; // AnimCurve
        private FloatReference _dampingFixedPercent; // 0.02
        private FloatReference _dampingFixedValue; // 1
        private AnimationCurve _dampingCurvePercent; // 0,0 - 0.5 ; 0.015 - 1.0 ; 0.05

        // Combo and Acceleration parameters
        private float _baseAcceleration;
        private FloatReference _timerFullDuration;
        private BoolReference _isDecreasingMaxTimer;
        private FloatReference _fullTimerDecrementValue;
        private FloatReference _fullTimerMinValue;
        private Coroutine _comboTimerCoroutine;

        // Active components parameters
        private bool _isContainerDamageActive;
        private bool _isBallFusionActive;
        private bool _isDampingActive;
        private bool _isComboActive;


        private void Awake()
        {
            _currentSpeed = new FloatReference();
            _targetSpeed = new FloatReference();
            _combo = new IntReference();
        }

        private void Start()
        {
            var test = PlayerManager.Instance.GetNumberOfActivePlayer();
            if (playerIndex >= PlayerManager.Instance.GetNumberOfActivePlayer())
                ClearScoreHandler();
            SetContainerDamageActive(true);
            SetBallFusionActive(true);
            SetDampingActive(true);
            SetComboActive(true);
        }

        private void Update()
        {
            Debug.Log($"Enter Update, _targetSpeed is {_targetSpeed.Variable.Value}");
            ApplyDamping();

            var acceleration = _currentSpeed < _targetSpeed
                ? _baseAcceleration * _combo
                : _baseAcceleration;
            _currentSpeed.Variable.SetValue(Mathf.MoveTowards(_currentSpeed, _targetSpeed,
                acceleration * Time.deltaTime));
            Debug.Log($"Exit Update, _targetSpeed is {_targetSpeed.Variable.Value}");
            Debug.Log("------------------------------------------------------------------------");
        }

        #region ContainerDamage

        private void SetContainerDamageActive(bool isActive)
        {
            if (_isContainerDamageActive == isActive)
                return;
            _isContainerDamageActive = isActive;
            if (_isContainerDamageActive)
                ContainerTracker.Instance.OnContainerHit.Subscribe(OnContainerHit, playerIndex);
            else
                ContainerTracker.Instance.OnContainerHit.Unsubscribe(OnContainerHit, playerIndex);
        }

        private void OnContainerHit((BallInstance ball, ContainerInstance container) args)
        {
            if (_damageCooldownCoroutine != null)
                StopCoroutine(_damageCooldownCoroutine);
            _damageCooldownCoroutine = StartCoroutine(ContainerDamageCooldown(args.ball));
        }

        private IEnumerator ContainerDamageCooldown(BallInstance ball)
        {
            var damageValue = ball.ScoreValue * _damageMultiplier;
            _currentSpeed.Variable.ApplyChangeClamp(_currentSpeed - damageValue * _percentageInstant, min: 0f);
            _targetSpeed.Variable.ApplyChangeClamp(_currentSpeed - damageValue * (_percentageInstant - 1f), min: 0f);

            SetBallFusionActive(false);
            SetDampingActive(false);
            SetComboActive(false);

            yield return new WaitForSeconds(_damageCooldownDuration);

            SetBallFusionActive(true);
            SetDampingActive(true);
            SetComboActive(true);
        }

        #endregion

        #region BallFusion

        private void SetBallFusionActive(bool isActive)
        {
            if (_isBallFusionActive == isActive)
                return;
            _isBallFusionActive = isActive;
            if (_isBallFusionActive)
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusionPoints, playerIndex);
            else
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusionPoints, playerIndex);
        }

        private void OnBallFusionPoints(BallInstance ball)
        {
            _targetSpeed.Variable.ApplyChange(ball.ScoreValue);
            Debug.Log(
                $"Entered OnBallFuionPoints, _targetSpeed after is {_targetSpeed.Variable.Value} and ball.ScoreValue is {ball.ScoreValue}");
        }

        #endregion

        #region Damping

        private void SetDampingActive(bool isActive)
        {
            _isDampingActive = isActive;
        }

        private void ApplyDamping()
        {
            if (!_isDampingActive)
                return;

            var dampingValue = _dampingMethod switch
            {
                ScoreHandlerData.DampingMethod.FixedPercent => _currentSpeed * _dampingFixedPercent,
                ScoreHandlerData.DampingMethod.Fixed => _dampingFixedValue,
                ScoreHandlerData.DampingMethod.AnimCurve => _currentSpeed *
                                                            _dampingCurvePercent.Evaluate(_currentSpeed /
                                                                _speedSoftCap),
                ScoreHandlerData.DampingMethod.None => 0f,
                _ => 0f
            };

            _targetSpeed.Variable.ApplyChangeClamp(-dampingValue * Time.deltaTime, min: 0f);
        }

        #endregion

        #region Combo

        private void SetComboActive(bool isActive)
        {
            if (_isComboActive == isActive)
                return;
            _isComboActive = isActive;
            if (_isComboActive)
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusionCombo, playerIndex);
            else
            {
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusionCombo, playerIndex);
                StopCombo();
            }
        }

        private void OnBallFusionCombo(BallInstance ball)
        {
            if (_comboTimerCoroutine != null)
                StopCoroutine(_comboTimerCoroutine);
            _comboTimerCoroutine = StartCoroutine(ComboTimer());
        }

        private IEnumerator ComboTimer()
        {
            _combo.Variable.ApplyChange(1);

            var comboTimer = _isDecreasingMaxTimer
                ? Mathf.Clamp(_timerFullDuration - _fullTimerDecrementValue * _combo, _fullTimerMinValue,
                    Mathf.Infinity)
                : _timerFullDuration;

            ScoreManager.Instance.OnComboIncrement.CallAction((_combo, comboTimer), playerIndex);

            yield return new WaitForSeconds(comboTimer);

            StopCombo();
        }

        private void StopCombo()
        {
            if (_comboTimerCoroutine != null)
                StopCoroutine(_comboTimerCoroutine);

            if (_combo > 1)
                ScoreManager.Instance.OnComboLost.CallAction(_combo, playerIndex);
            _combo.Variable.SetValue(1);
        }

        #endregion

        public void ResetScore()
        {
            _currentSpeed.Variable.SetValue(0);
            _targetSpeed.Variable.SetValue(0);
            _combo.Variable.SetValue(1);
        }

        private void ClearScoreHandler()
        {
            // Make sure to unsubscribe everyone and from everything
            ScoreManager.Instance.RemoveScoreHandler(playerIndex);
            Destroy(gameObject);
        }

        #region Getter/Setter

        public FloatReference GetCurrentSpeedReference() => _currentSpeed;
        public FloatReference GetTargetSpeedReference() => _targetSpeed;
        public IntReference GetComboReference() => _combo;


        public void SetScoreHandlerData(ScoreHandlerData scoreHandlerData)
        {
            _baseAcceleration = scoreHandlerData.baseAcceleration;

            _damageMultiplier = scoreHandlerData.damageMultiplier;
            _percentageInstant = scoreHandlerData.percentageInstant;
            _damageCooldownDuration = scoreHandlerData.damageCooldownDuration;

            _speedSoftCap = scoreHandlerData.speedSoftCap;
            _dampingMethod = scoreHandlerData.dampingMethod;
            _dampingFixedPercent = scoreHandlerData.dampingFixedPercent;
            _dampingFixedValue = scoreHandlerData.dampingFixedValue;
            _dampingCurvePercent = scoreHandlerData.dampingCurvePercent;

            _timerFullDuration = scoreHandlerData.timerFullDuration;
            _isDecreasingMaxTimer = scoreHandlerData.isDecreasingMaxTimer;
            _fullTimerDecrementValue = scoreHandlerData.fullTimerDecrementValue;
            _fullTimerMinValue = scoreHandlerData.fullTimerMinValue;
        }

        #endregion
    }
}