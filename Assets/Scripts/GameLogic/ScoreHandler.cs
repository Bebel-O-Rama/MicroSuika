using System.Collections;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] public int playerIndex;
        [SerializeField] private ScoreHandlerData _scoreHandlerData;

        [Header("Data Reader")]
        [SerializeField] private float _currentSpeedReader;
        [SerializeField] private float _targetSpeedReader;
        [SerializeField] private int _comboReader;
        
        
        // Main score parameters
        private FloatReference _currentSpeed;
        private FloatReference _targetSpeed;
        private IntReference _combo;
        
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
                ? _scoreHandlerData.BaseAcceleration * _scoreHandlerData.ComboImpact.EvaluateClamp(_combo)
                : _scoreHandlerData.BaseAcceleration;
            _currentSpeed.Variable.SetValue(Mathf.MoveTowards(_currentSpeed, _targetSpeed,
                acceleration * Time.deltaTime));

            UpdateReaders();
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

        private void OnContainerHit(BallInstance ball)
        {
            if (_damageCooldownCoroutine != null)
                StopCoroutine(_damageCooldownCoroutine);
            _damageCooldownCoroutine = StartCoroutine(ContainerDamageCooldown(ball));
        }

        private IEnumerator ContainerDamageCooldown(BallInstance ball)
        {
            var damageValue = ball.ScoreValue * _scoreHandlerData.DamageMultiplier;
            _currentSpeed.Variable.ApplyChangeClamp(-damageValue * _scoreHandlerData.PercentageInstant,
                min: 0f);
            _targetSpeed.Variable.SetValue(Mathf.Min(_targetSpeed, _currentSpeed));
            _targetSpeed.Variable.ApplyChangeClamp(-damageValue * (Mathf.Abs(_scoreHandlerData.PercentageInstant - 1f)), min: 0f);

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
            // TEMPORARY FIX : When two ball fuses, either the combo is incremented twice or the added score is incremented for half the ball
            // It's linked to the other temporary fix in BallInstance.cs, FuseWithOtherBall() (the other.ClearBall(false))
            _targetSpeed.Variable.ApplyChange(ball.ScoreValue * 2);
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

        private void UpdateReaders()
        {
            _currentSpeedReader = _currentSpeed;
            _targetSpeedReader = _targetSpeed;
            _comboReader = _combo;
        }
        
        #region Getter/Setter

        public FloatReference GetCurrentSpeedReference() => _currentSpeed;
        public FloatReference GetTargetSpeedReference() => _targetSpeed;

        #endregion
    }
}