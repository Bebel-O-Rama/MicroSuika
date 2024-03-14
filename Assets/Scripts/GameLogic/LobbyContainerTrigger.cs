using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MultiSuika.GameLogic
{
    public class LobbyContainerTrigger : MonoBehaviour
    {
        [Range(0, 4)][SerializeField] public int playerNumberThreshold;
        [SerializeField] private SignalCollider2D colliderSignal;    
        
        public UnityEvent OnConditionFailed;
        public UnityEvent OnConditionPassed;
        public UnityEvent OnTriggered;

        private IntReference _numberOfActivePlayer;
        private bool _isConditionMet;

        private void Start()
        {
            _isConditionMet = false;
            OnConditionFailed?.Invoke();
        }

        public void Update()
        {
            if (_numberOfActivePlayer >= playerNumberThreshold == _isConditionMet)
                return;
            SetConditionStatus(!_isConditionMet);
        }

        private void SetConditionStatus(bool isActive)
        {
            if (isActive)
            {
                colliderSignal.SubscribeTriggerEnter2D(ColliderTriggered);
                OnConditionPassed?.Invoke();
            }
            else
            {
                colliderSignal.UnsubscribeTriggerEnter2D(ColliderTriggered);
                OnConditionFailed?.Invoke();
            }

            _isConditionMet = isActive;
        }

        private void ColliderTriggered(Collider2D other)
        {
            if (!other.transform.CompareTag("Ball")) 
                return;
            other.transform.parent.GetComponent<BallInstance>().ClearBall(false);
            OnTriggered?.Invoke();
        }

        public void SetNumberOfActivePlayerParameters(IntReference numberOfActivePlayer) =>
            _numberOfActivePlayer = numberOfActivePlayer;
    }
}
