using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.GameLogic;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public class TriggerConditionHandler : MonoBehaviour
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
            if (other.gameObject.CompareTag("Ball"))
                indicator.color = Color.red;
        }

        public void TriggerConditionExit(Collider2D other)
        {
            
        }
    }
}
