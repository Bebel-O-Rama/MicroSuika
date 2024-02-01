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
        private TriggerConditionHandler _triggerConditionHandler;
        
        public void SetTriggerCondition(TriggerConditionHandler cond)
        {
            _triggerConditionHandler = cond;
            GetComponent<Rigidbody2D>().simulated = true;
        }

        private void OnTriggerEnter2D(Collider2D other) => _triggerConditionHandler.TriggerConditionEnter(other);
        private void OnTriggerExit2D(Collider2D other) => _triggerConditionHandler.TriggerConditionExit(other);
    }
}
