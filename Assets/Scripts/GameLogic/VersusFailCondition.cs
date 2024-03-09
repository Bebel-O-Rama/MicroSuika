using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class VersusFailCondition : MonoBehaviour
    {
        [SerializeField] public List<TriggerConditionComponent> triggers;
        [SerializeField] public SpriteRenderer indicator;

        private VersusModeManager _versusModeManager;

        public void SetCondition(VersusModeManager gameModeManager)
        {
            _versusModeManager = gameModeManager;
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
                _versusModeManager.PlayerFailure(other.GetComponent<BallInstance>().ContainerInstance);
        }

        public void TriggerConditionExit(Collider2D other)
        {
        }
    }
}