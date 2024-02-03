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

        private VersusMode _versusMode;

        public void SetCondition(VersusMode gameMode)
        {
            _versusMode = gameMode;
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
            if (other.gameObject.CompareTag("Ball"))
                _versusMode.PlayerFailure(other.GetComponent<Ball.Ball>().container);
        }

        public void TriggerConditionExit(Collider2D other)
        {
        }
    }
}