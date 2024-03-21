using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Modifiers/Ball Fusion")]
    public class BallFusionScoreModifier : ScriptableObject, IScoreModifierData
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        private float _scoreValue;
        private bool _isActive;
        private int _playerIndex;
        private FloatReference _targetSpeed;
        
        
        public ScoreModifierStatus ApplyModifier()
        {
            if (!_isActive || _scoreValue < Mathf.Epsilon)
                return ScoreModifierStatus.Continue;

            _targetSpeed.Variable.ApplyChange(_scoreValue);
            _scoreValue = 0;
            return ScoreModifierStatus.Continue;
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            _isActive = isActive;
            // if (_isActive)
            //     ContainerTracker.Instance.OnContainerHit.Subscribe(OnContainerHit, _playerIndex);
            // else
            //     ContainerTracker.Instance.OnContainerHit.Unsubscribe(OnContainerHit, _playerIndex);
                
        }

        public void Init(int playerIndex, FloatReference targetSpeed)
        {
            _playerIndex = playerIndex;
            _targetSpeed = targetSpeed;
        }

        private void OnBallFusion((BallInstance ball, ContainerInstance container) args)
        {
            _scoreValue += args.ball.ScoreValue;
        }
    }
}