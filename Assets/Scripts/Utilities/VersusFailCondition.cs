using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.GameLogic;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public class VersusFailCondition : MonoBehaviour
    {
        [SerializeField] public List<TriggerConditionComponent> triggers;
        [SerializeField] public SpriteRenderer indicator;

        private void Awake()
        {
            if (triggers == null || !triggers.Any())
            {
                Destroy(this);
                return;
            }

            foreach (var trigger in triggers)
            {
                trigger.SetTriggerCondition(this);
            }
        }

        public void TriggerConditionEnter(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            indicator.enabled = true;
            var ball = other.GetComponent<Ball.Ball>();
            var balls = ball.ballTracker.GetBallsForContainer(ball.container);
            foreach (var b in balls)
            {
                b.SetBallFreeze(true);
            }
        }

        public void TriggerConditionExit(Collider2D other)
        {
        }
    }
}