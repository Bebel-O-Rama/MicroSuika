using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Ball Fusion")]
    public class BallFusionScoreModifier : ScriptableObject, IScoreModifierData<FloatReference>
    {
#if UNITY_EDITOR
        [Multiline] public string developerDescription = "";
#endif
        private float _scoreIncrementValue;

        private int _playerIndex;
        private FloatReference _targetSpeed;
        private bool _isActive;

        public void SetParameters(int playerIndex, FloatReference targetSpeed)
        {
            _playerIndex = playerIndex;
            _targetSpeed = targetSpeed;
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            _isActive = isActive;
            if (_isActive)
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusion, _playerIndex);
            else
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusion, _playerIndex);
        }

        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive || _scoreIncrementValue < Mathf.Epsilon)
                return ScoreModifierStatus.Continue;

            _targetSpeed.Variable.ApplyChange(_scoreIncrementValue);
            _scoreIncrementValue = 0;
            return ScoreModifierStatus.Continue;
        }

        private void OnBallFusion(BallInstance ball)
        {
            _scoreIncrementValue += ball.ScoreValue;
        }
    }
}