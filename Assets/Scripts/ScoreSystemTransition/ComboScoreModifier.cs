using MultiSuika.Ball;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using MultiSuika.Container;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Combo")]
    public class ComboScoreModifier : ScriptableObject,
        IScoreModifierData<(FloatReference currentSpeed, FloatReference targetSpeed, FloatReference currentAcceleration,
            FloatReference baseAcceleration, ActionMethodPlayerWrapper<(int, float)> onComboIncrement,
            ActionMethodPlayerWrapper<int> onComboLost)>
    {
#if UNITY_EDITOR
        [Multiline] public string developerDescription = "";
#endif
        public BoolReference isAppliedOnLowerTarget;
        public FloatReference timerFullDuration;
        public BoolReference isDecreasingMaxTimer;
        public FloatReference fullTimerDecrementValue;
        public FloatReference fullTimerMinValue;

        private int _combo = 1;
        private float _timer;
        
        private int _playerIndex;
        private FloatReference _currentSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _currentAcceleration;
        private FloatReference _baseAcceleration;
        private ActionMethodPlayerWrapper<(int, float)> _onComboIncrement;
        private ActionMethodPlayerWrapper<int> _onComboLost;
        private bool _isActive;


        public void SetParameters(int playerIndex,
            (FloatReference currentSpeed, FloatReference targetSpeed, FloatReference currentAcceleration, FloatReference
                baseAcceleration, ActionMethodPlayerWrapper<(int, float)> onComboIncrement,
                ActionMethodPlayerWrapper<int> onComboLost) args)
        {
            _playerIndex = playerIndex;

            _currentSpeed = args.currentSpeed;
            _targetSpeed = args.targetSpeed;
            _currentAcceleration = args.currentAcceleration;
            _baseAcceleration = args.baseAcceleration;

            _onComboIncrement = args.onComboIncrement;
            _onComboLost = args.onComboLost;
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            _isActive = isActive;
            if (_isActive)
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusion, _playerIndex);
            else
            {
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusion, _playerIndex);
                ComboStop();
            }
        }

        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive)
                return ScoreModifierStatus.Continue;
            UpdateComboTimer();
            if (!isAppliedOnLowerTarget && _currentSpeed > _targetSpeed)
            {
                _currentAcceleration.Variable.SetValue(_baseAcceleration);
                return ScoreModifierStatus.Continue;
            }

            _currentAcceleration.Variable.SetValue(_baseAcceleration * _combo);
            return ScoreModifierStatus.Continue;
        }

        private void OnBallFusion(BallInstance ball)
        {
            _combo += 1;
            _timer = isDecreasingMaxTimer
                ? Mathf.Clamp(timerFullDuration - fullTimerDecrementValue * _combo, fullTimerMinValue, Mathf.Infinity)
                : timerFullDuration;
            _onComboIncrement.CallAction((_combo, _timer), _playerIndex);
        }

        private bool IsInCombo() => _combo > 1 && _timer > 0f;

        private void UpdateComboTimer()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                ComboStop();
            }
        }

        private void ComboStop()
        {
            if (IsInCombo())
                _onComboLost.CallAction(_combo, _playerIndex);

            _combo = 1;
            _timer = 0f;
            _currentAcceleration.Variable.SetValue(_baseAcceleration);
        }
    }
}