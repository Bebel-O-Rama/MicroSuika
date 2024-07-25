using System.Collections;
using DG.Tweening;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.GameLogic
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private int _playerScore;
        [SerializeField] private SpeedHandlerData speedHandlerData;

        [Header("Data Reader")]
        [SerializeField] private float _currentSpeedReader;
        [SerializeField] private float _targetSpeedReader;
        [SerializeField] private int _comboReader;
        
        
        // Main score parameters
        private Sequence _damageTimerSequence;
        
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
            _damageTimerSequence = DOTween.Sequence();
            _currentSpeed = new FloatReference();
            _targetSpeed = new FloatReference();
            _combo = new IntReference();
        }

        private void Start()
        {
            if (_playerIndex >= PlayerManager.Instance.GetNumberOfActivePlayer())
                ClearSpeedHandler();
            SetContainerDamageActive(true);
            SetBallFusionActive(true);
            SetDampingActive(true);
            SetComboActive(true);
        }

        private void Update()
        {
            ApplyDamping();
            
            var acceleration = _currentSpeed < _targetSpeed
                ? speedHandlerData.BaseAcceleration * speedHandlerData.ComboImpact.EvaluateClamp(_combo)
                : speedHandlerData.BaseAcceleration;
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
                ContainerTracker.Instance.OnContainerHit.Subscribe(OnContainerHit, _playerIndex);
            else
                ContainerTracker.Instance.OnContainerHit.Unsubscribe(OnContainerHit, _playerIndex);
        }

        private void OnContainerHit(BallInstance ball)
        {
            if (_damageCooldownCoroutine != null)
                StopCoroutine(_damageCooldownCoroutine);
            _damageCooldownCoroutine = StartCoroutine(ContainerDamageCooldown(ball));
        }

        private IEnumerator ContainerDamageCooldown(BallInstance ball)
        {
            var damageValue = ball.ScoreValue * speedHandlerData.DamageMultiplier;
            if (_targetSpeed > _currentSpeed)
                _targetSpeed.Variable.SetValue(_currentSpeed);
            _targetSpeed.Variable.ApplyChangeClamp(-damageValue, min: 0f);
            
            if (_damageTimerSequence.IsPlaying())
                _damageTimerSequence.Kill();
            _damageTimerSequence = DOTween.Sequence();

            _damageTimerSequence.Append(
                DOTween.To(() => _currentSpeed, x => _currentSpeed.Variable.SetValue(x), _targetSpeed, speedHandlerData.DamageCooldownDuration));
            
            SetBallFusionActive(false);
            SetDampingActive(false);
            SetComboActive(false);

            yield return new WaitForSeconds(speedHandlerData.DamageCooldownDuration);

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
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusionPoints, _playerIndex);
            else
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusionPoints, _playerIndex);
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

            var dampingValue = speedHandlerData.DampingMethod switch
            {
                DampingEvaluationMethod.FixedPercent => _currentSpeed * speedHandlerData.DampingFixedPercent,
                DampingEvaluationMethod.Fixed => speedHandlerData.DampingFixedValue,
                DampingEvaluationMethod.AnimCurve => _currentSpeed *
                                                     speedHandlerData.DampingCurvePercent.Evaluate(_currentSpeed /
                                                         speedHandlerData.SpeedSoftCap),
                DampingEvaluationMethod.None => 0f,
                _ => 0f
            };
            
            // Safeguards the player from getting a ridiculously low targetSpeed (the kind that can't be recovered) 
            if (_currentSpeed - _targetSpeed <= 20)
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
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusionCombo, _playerIndex);
            else
            {
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusionCombo, _playerIndex);
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

            var comboTimer = speedHandlerData.IsDecreasingMaxTimer
                ? Mathf.Clamp(speedHandlerData.TimerFullDuration - speedHandlerData.FullTimerDecrementValue * _combo,
                    speedHandlerData.FullTimerMinValue, Mathf.Infinity)
                : speedHandlerData.TimerFullDuration;

            ScoreManager.Instance.OnComboIncrement.CallAction((_combo, comboTimer), _playerIndex);

            yield return new WaitForSeconds(comboTimer);

            StopCombo();
        }

        private void StopCombo()
        {
            if (_comboTimerCoroutine != null)
                StopCoroutine(_comboTimerCoroutine);

            if (_combo > 1)
                ScoreManager.Instance.OnComboLost.CallAction(_combo, _playerIndex);
            _combo.Variable.SetValue(1);
        }

        #endregion

        #region Score

        public int IncrementScore()
        {
            _playerScore += 1;
            return _playerScore;
        }

        public void ResetScore() => _playerScore = 0;

        #endregion 
        
        public void ResetSpeedHandler()
        {
            _currentSpeed.Variable.SetValue(0);
            _targetSpeed.Variable.SetValue(0);
            _combo.Variable.SetValue(1);
        }

        private void ClearSpeedHandler()
        {
            if (_damageCooldownCoroutine != null)
                StopCoroutine(_damageCooldownCoroutine);
            if (_comboTimerCoroutine != null)
                StopCoroutine(_comboTimerCoroutine);

            ScoreManager.Instance.ClearPlayerScoreComponents(_playerIndex);
            Destroy(gameObject);
        }

        private void UpdateReaders()
        {
            _currentSpeedReader = _currentSpeed;
            _targetSpeedReader = _targetSpeed;
            _comboReader = _combo;
        }
        
        #region Getter
        
        public int PlayerIndex { get => _playerIndex; }
        public int PlayerScore { get => _playerScore; }
        public FloatReference CurrentSpeed { get => _currentSpeed; }
        public FloatReference TargetSpeed { get => _targetSpeed; }

        #endregion
    }
}