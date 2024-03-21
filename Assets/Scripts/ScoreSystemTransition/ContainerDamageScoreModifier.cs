using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Container Damage")]
    public class ContainerDamageScoreModifier : ScriptableObject, IScoreModifierData
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif    
        public FloatReference damageMultiplier;
        private float _damageValue;
        private bool _isActive;
        private int _playerIndex;
        private FloatReference _targetSpeed;
        private FloatReference _currentSpeed;

        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive || _damageValue < Mathf.Epsilon)
                return ScoreModifierStatus.Continue;

            _targetSpeed.Variable.ApplyChangeClamp(_currentSpeed-_damageValue, min: 0f);
            _damageValue = 0;
            return ScoreModifierStatus.Stop;
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

        public void Init(int playerIndex, FloatReference targetSpeed, FloatReference currentSpeed)
        {
            _playerIndex = playerIndex;
            _targetSpeed = targetSpeed;
            _currentSpeed = currentSpeed;
        }

        private void OnContainerHit((BallInstance ball, ContainerInstance container) args)
        {
            _damageValue += args.ball.ScoreValue * damageMultiplier;
        }
    }
}
