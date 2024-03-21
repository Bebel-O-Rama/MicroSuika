using MultiSuika.Ball;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Handler/Lobby Mode")]
    public class LobbyScoreHandler : ScoreHandlerData
    {
        private FloatReference _currentScore;

        protected override void SetParameters()
        {
            _currentScore = new FloatReference();
        }

        public override void SetActive(bool isActive)
        {
            BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusion, PlayerIndex);
        }
        
        public override void UpdateScore()
        {
        }

        public override FloatReference GetScoreReference() => _currentScore;
        
        public override void ResetScore()
        {
            _currentScore.Variable.SetValue(0f);
        }

        private void OnBallFusion(BallInstance ball)
        {
            _currentScore.Variable.ApplyChange(ball.ScoreValue);
        }
    }
}