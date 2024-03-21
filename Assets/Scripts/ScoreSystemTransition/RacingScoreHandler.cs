using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    [CreateAssetMenu(menuName = "Score/Handler/Racing Mode")]
    public class RacingScoreHandler : ScoreHandlerData
    {
        public ContainerDamageScoreModifier containerDamageScoreModifier;
        public BallFusionScoreModifier ballFusionScoreModifier;

        [Header("Base parameters")] 
        public FloatReference baseAcceleration;

        // Main parameters
        private FloatReference _currentScore;
        private FloatReference _targetSpeed;
        private FloatReference _currentAcceleration;

        protected override void Init()
        {
            _currentScore = new FloatReference();
            _targetSpeed = new FloatReference();
            _currentAcceleration = new FloatReference();

            containerDamageScoreModifier.Init(PlayerIndex, _targetSpeed, _currentScore);
            ballFusionScoreModifier.Init(PlayerIndex, _targetSpeed);
        }

        public override void UpdateScore()
        {
            // There are better ways to follow and check the status...
            var status = ScoreModifierStatus.Continue;
            while (status == ScoreModifierStatus.Continue)
            {
                status = containerDamageScoreModifier.ApplyModifier();
                status = ballFusionScoreModifier.ApplyModifier();
            }
        }

        public override FloatReference GetScoreReference()
        {
            return _currentScore;
        }

        public override void ResetScore()
        {
            _currentScore.Variable.SetValue(0);
            _targetSpeed.Variable.SetValue(0);
            _currentAcceleration.Variable.SetValue(baseAcceleration);
        }
    }
}