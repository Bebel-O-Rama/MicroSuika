﻿using System;
using System.Collections;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] public int playerIndex;

        // Main score parameters
        [SerializeField] private FloatReference _currentSpeed;
        [SerializeField] private FloatReference _targetSpeed;
        [SerializeField] private IntReference _combo;
        [SerializeField] private ScoreHandlerData _scoreHandlerData;
        
        private Coroutine _damageCooldownCoroutine;
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
            if (playerIndex >= PlayerManager.Instance.GetNumberOfActivePlayer())
                ClearScoreHandler();
            SetContainerDamageActive(true);
            SetBallFusionActive(true);
            SetDampingActive(true);
            SetComboActive(true);
        }

        private void Update()
        {
            ApplyDamping();

            var acceleration = _currentSpeed < _targetSpeed
                ? _scoreHandlerData.BaseAcceleration * _combo
                : _scoreHandlerData.BaseAcceleration;
            _currentSpeed.Variable.SetValue(Mathf.MoveTowards(_currentSpeed, _targetSpeed,
                acceleration * Time.deltaTime));
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
            var damageValue = ball.ScoreValue * _scoreHandlerData.DamageMultiplier;
            _currentSpeed.Variable.ApplyChangeClamp(_currentSpeed - damageValue * _scoreHandlerData.PercentageInstant,
                min: 0f);
            _targetSpeed.Variable.ApplyChangeClamp(
                _currentSpeed - damageValue * (_scoreHandlerData.PercentageInstant - 1f), min: 0f);

            SetBallFusionActive(false);
            SetDampingActive(false);
            SetComboActive(false);

            yield return new WaitForSeconds(_scoreHandlerData.DamageCooldownDuration);

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

            var dampingValue = _scoreHandlerData.DampingMethod switch
            {
                DampingEvaluationMethod.FixedPercent => _currentSpeed * _scoreHandlerData.DampingFixedPercent,
                DampingEvaluationMethod.Fixed => _scoreHandlerData.DampingFixedValue,
                DampingEvaluationMethod.AnimCurve => _currentSpeed *
                                                     _scoreHandlerData.DampingCurvePercent.Evaluate(_currentSpeed /
                                                         _scoreHandlerData.SpeedSoftCap),
                DampingEvaluationMethod.None => 0f,
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

            var comboTimer = _scoreHandlerData.IsDecreasingMaxTimer
                ? Mathf.Clamp(_scoreHandlerData.TimerFullDuration - _scoreHandlerData.FullTimerDecrementValue * _combo,
                    _scoreHandlerData.FullTimerMinValue, Mathf.Infinity)
                : _scoreHandlerData.TimerFullDuration;

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
            if (_damageCooldownCoroutine != null)
                StopCoroutine(_damageCooldownCoroutine);
            if (_comboTimerCoroutine != null)
                StopCoroutine(_comboTimerCoroutine);

            ScoreManager.Instance.ClearPlayerScoreComponents(playerIndex);
            Destroy(gameObject);
        }

        #region Getter/Setter

        public FloatReference GetCurrentSpeedReference() => _currentSpeed;
        public FloatReference GetTargetSpeedReference() => _targetSpeed;
        public float GetSpeedSoftCap() => _scoreHandlerData.SpeedSoftCap;

        // public void SetScoreHandlerData(ScoreHandlerData scoreHandlerData)
        // {
        //     // _baseAcceleration = scoreHandlerData.baseAcceleration;
        //     //
        //     // _damageMultiplier = scoreHandlerData.damageMultiplier;
        //     // _percentageInstant = scoreHandlerData.percentageInstant;
        //     // _damageCooldownDuration = scoreHandlerData.damageCooldownDuration;
        //     //
        //     // _speedSoftCap = scoreHandlerData.speedSoftCap;
        //     // _dampingEvaluationMethod = scoreHandlerData.dampingMethod;
        //     // _dampingFixedPercent = scoreHandlerData.dampingFixedPercent;
        //     // _dampingFixedValue = scoreHandlerData.dampingFixedValue;
        //     // _dampingCurvePercent = scoreHandlerData.dampingCurvePercent;
        //     //
        //     // _timerFullDuration = scoreHandlerData.timerFullDuration;
        //     // _isDecreasingMaxTimer = scoreHandlerData.isDecreasingMaxTimer;
        //     // _fullTimerDecrementValue = scoreHandlerData.fullTimerDecrementValue;
        //     // _fullTimerMinValue = scoreHandlerData.fullTimerMinValue;
        //     _scoreHandlerData = scoreHandlerData;
        // }

        #endregion
    }
}