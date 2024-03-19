using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika
{
    public class ScoreHandlerData : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        [SerializeField] private List<ScoreModifierData> _scoreModifiers;
        public List<ScoreModifierData> ScoreModifiers => _scoreModifiers;
        public FloatReference CurrentScore => _currentScore;
        public FloatReference TargetScore => _targetSpeed;
        public FloatReference CurrentAcceleration => _currentAcceleration;

        private FloatReference _currentScore;
        private FloatReference _targetSpeed;
        private FloatReference _currentAcceleration;
        // public FloatReference cooldownTime;


        // public FloatReference baseSpeed;
        public FloatReference baseAcceleration;
        
        private Action<BallInstance> _onBallFused;
        private Action<BallInstance> _onContainerHit;

        public enum ScoreModifierState
        {
            Continue,
            Stop
        }

        private void Awake()
        {
            foreach (var scoreModifier in ScoreModifiers)
            {
                scoreModifier.Init();
            }
        }

        private void UpdateScore()
        {
            foreach (var scoreModifier in ScoreModifiers)
            {
                var state = scoreModifier.ApplyModifier();
                if (state == ScoreModifierState.Stop)
                    return;
            }
        }


        #region Delegate
        /// /// Wait, we should probably connect directly to the ItemTrackers instead
        protected void SubscribeBallFusion(Action<BallInstance> method) => _onBallFused += method;
        protected void UnsubscribeBallFusion(Action<BallInstance> method) => _onBallFused -= method;
        protected void SubscribeContainerHit(Action<BallInstance> method) => _onContainerHit += method;
        protected void UnsubscribeContainerHit(Action<BallInstance> method) => _onContainerHit -= method;

        #endregion
    }

    public abstract class ScoreModifierData : ScoreHandlerData
    {
        public abstract ScoreModifierState ApplyModifier();

        public abstract void SetActive(bool isActive);
    }

    public class RocketDamageScoreModifier : ScoreModifierData
    {
        [SerializeField] public FloatReference damageMultiplier;
        private float _damageValue;
        private bool _isActive;

        /// Missing :
        /// Probably shouldn't be able to mess with the combo???????????????????
        /// Cooldown for the other modifiers if trigger
        /// Cooldown for itself maybe????
        public override ScoreModifierState ApplyModifier()
        {
            if (!_isActive || _damageValue < Mathf.Epsilon)
                return ScoreModifierState.Continue;

            TargetScore.Variable.ApplyChangeClamp(-_damageValue, min: 0f);
            CurrentAcceleration.Variable.SetValue(baseAcceleration);
            _damageValue = 0;            
            return ScoreModifierState.Stop;
        }

        public override void SetActive(bool isActive)
        {
            if (isActive)
                
        }

        private void OnContainerHit(BallInstance ball)
        {
            if (!_isActive)
                return;
            _damageValue += ball.ScoreValue * damageMultiplier;
        }
        
        public override void Init()
        {
            ResetParameters();
            SubscribeContainerHit(OnContainerHit);
        }
        
        
    }
    
    public class BallFusionScoreModifier : ScoreModifierData
    {
        private int _scoreValue;
        private bool _hasBeenTriggered;
        
        public override ScoreModifierState ApplyModifier()
        {
            if (!_hasBeenTriggered)
                return ScoreModifierState.Continue;

            targetSpeed.Variable.ApplyChange(_scoreValue);
            ResetParameters();
            return ScoreModifierState.Continue;
        }

        private void OnBallFusion(BallInstance ball)
        {
            _scoreValue += ball.ScoreValue;
            _hasBeenTriggered = true;
        }
        
        public override void Init()
        {
            ResetParameters();
            SubscribeBallFusion(OnBallFusion);
        }
        
        private void ResetParameters()
        {
            _scoreValue = 0;
            _hasBeenTriggered = true;
        }
    }
}