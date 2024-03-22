using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    // [CreateAssetMenu(menuName = "Score/Handler/Racing Mode")]
    // public class RacingScoreHandler
    // {
    //     [Header("Base parameters")] 
    //     public FloatReference baseAcceleration;
    //     // [Min(0f)] public FloatReference damageCooldown;
    //     
    //     public ContainerDamageScoreModifier containerDamageScoreModifier;
    //     public BallFusionScoreModifier ballFusionScoreModifier;
    //     public DampingScoreModifier dampingScoreModifier;
    //     public ComboScoreModifier comboScoreModifier;
    //
    //     // Main parameters
    //     private FloatReference _currentScore;
    //     private FloatReference _targetSpeed;
    //     private FloatReference _currentAcceleration;
    //
    //     protected void SetParameters()
    //     {
    //         _currentScore = new FloatReference();
    //         _targetSpeed = new FloatReference();
    //         _currentAcceleration = new FloatReference();
    //
    //         // containerDamageScoreModifier.SetParameters(PlayerIndex, (_currentScore, _targetSpeed));
    //         // ballFusionScoreModifier.SetParameters(PlayerIndex, _targetSpeed);
    //         // dampingScoreModifier.SetParameters(PlayerIndex, (_currentScore, _targetSpeed));
    //         // comboScoreModifier.SetParameters(PlayerIndex,
    //         //     (_currentScore, _targetSpeed, _currentAcceleration, baseAcceleration, OnComboIncrement, OnComboLost));
    //
    //         // SetActive(true);
    //     }
    //
    //     public void SetActive(bool isActive)
    //     {
    //         containerDamageScoreModifier.SetActive(true);
    //         ballFusionScoreModifier.SetActive(true);
    //         dampingScoreModifier.SetActive(true);
    //         comboScoreModifier.SetActive(true);
    //     }
    //     
    //     public void UpdateScore()
    //     {
    //         ApplyScoreModifiers();
    //         _currentScore.Variable.SetValue(Mathf.MoveTowards(_currentScore, _targetSpeed, _currentAcceleration));
    //     }
    //
    //     private void ApplyScoreModifiers()
    //     {
    //         // There are better ways to follow and check the status...
    //         var status = containerDamageScoreModifier.ApplyModifier();
    //         if (status != ScoreModifierStatus.Continue)
    //             return;
    //         ballFusionScoreModifier.ApplyModifier();
    //         dampingScoreModifier.ApplyModifier();
    //         comboScoreModifier.ApplyModifier();
    //     }
    //     
    //     public FloatReference GetScoreReference()
    //     {
    //         return _currentScore;
    //     }
    //
    //     // public override void ResetScore()
    //     // {
    //     //     _currentScore.Variable.SetValue(0);
    //     //     _targetSpeed.Variable.SetValue(0);
    //     //     _currentAcceleration.Variable.SetValue(baseAcceleration);
    //     // }
    //     
    //     public ActionMethodPlayerWrapper<(int, float)> OnComboIncrement { get; } =
    //         new ActionMethodPlayerWrapper<(int, float)>();
    //     public ActionMethodPlayerWrapper<int> OnComboLost { get; } = new ActionMethodPlayerWrapper<int>();
    // }
}