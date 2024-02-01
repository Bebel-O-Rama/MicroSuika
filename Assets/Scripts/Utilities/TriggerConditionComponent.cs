using System;
using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TriggerConditionComponent : MonoBehaviour
    {
        private VersusFailCondition _versusFailCondition;
        
        public void SetTriggerCondition(VersusFailCondition cond)
        {
            _versusFailCondition = cond;
            GetComponent<Rigidbody2D>().simulated = true;
        }

        private void OnTriggerEnter2D(Collider2D other) => _versusFailCondition.TriggerConditionEnter(other);
        private void OnTriggerExit2D(Collider2D other) => _versusFailCondition.TriggerConditionExit(other);
    }
}
