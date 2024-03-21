using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Container Damage")]
    public class ContainerDamageScoreModifier : ScriptableObject,
        IScoreModifierData<(FloatReference currentSpeed, FloatReference targetSpeed)>
    {
#if UNITY_EDITOR
        [Multiline] public string developerDescription = "";
#endif
        public FloatReference damageMultiplier;
        public FloatReference percentageInstant;
        private float _damageValue;

        private int _playerIndex;
        private FloatReference _currentSpeed;
        private FloatReference _targetSpeed;
        private bool _isActive;

        public void SetParameters(int playerIndex, (FloatReference currentSpeed, FloatReference targetSpeed) args)
        {
            _playerIndex = playerIndex;
            _currentSpeed = args.currentSpeed;
            _targetSpeed = args.targetSpeed;
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            _isActive = isActive;
            if (_isActive)
                ContainerTracker.Instance.OnContainerHit.Subscribe(OnContainerHit, _playerIndex);
            else
                ContainerTracker.Instance.OnContainerHit.Unsubscribe(OnContainerHit, _playerIndex);
        }

        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive || _damageValue < Mathf.Epsilon)
                return ScoreModifierStatus.Continue;
            
            _currentSpeed.Variable.ApplyChangeClamp(_currentSpeed - _damageValue * percentageInstant, min: 0f);
            _targetSpeed.Variable.ApplyChangeClamp(_currentSpeed - _damageValue * (percentageInstant - 1f), min: 0f);
            _damageValue = 0;
            return ScoreModifierStatus.Stop;
        }

        private void OnContainerHit((BallInstance ball, ContainerInstance container) args)
        {
            _damageValue += args.ball.ScoreValue * damageMultiplier;
        }
    }
}