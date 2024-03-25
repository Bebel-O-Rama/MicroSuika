using MultiSuika.Ball;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MultiSuika.GameLogic
{
    public class LobbyContainerTrigger : MonoBehaviour
    {
        [SerializeField] [Range(0, 4)] private int _playerNumberThreshold;
        [SerializeField] private SignalCollider2D _colliderSignal;    
        
        public UnityEvent OnConditionFailed;
        public UnityEvent OnConditionPassed;
        public UnityEvent OnTriggered;

        private IntReference _numberOfActivePlayer;
        private bool _isConditionMet;

        private void Start()
        {
            _numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();
            
            _isConditionMet = false;
            OnConditionFailed?.Invoke();
        }

        public void Update()
        {
            if (_numberOfActivePlayer >= _playerNumberThreshold == _isConditionMet)
                return;
            SetConditionStatus(!_isConditionMet);
        }

        private void SetConditionStatus(bool isActive)
        {
            if (isActive)
            {
                _colliderSignal.SubscribeTriggerEnter2D(ColliderTriggered);
                OnConditionPassed?.Invoke();
            }
            else
            {
                _colliderSignal.UnsubscribeTriggerEnter2D(ColliderTriggered);
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
    }
}
