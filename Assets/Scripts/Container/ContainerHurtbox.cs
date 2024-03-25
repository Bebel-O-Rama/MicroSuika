using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    [RequireComponent(typeof(ContainerInstance))]
    public class ContainerHurtbox : MonoBehaviour
    {
        private int _playerIndex;
        private ContainerInstance _containerInstance;
        [SerializeField] private List<SignalCollider2D> _hurtboxes; 
        
        private void Start()
        {
            _containerInstance = GetComponent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);

            foreach (var hurtbox in _hurtboxes)
            {
                hurtbox.SubscribeTriggerEnter2D(HurtboxTriggered);
            }
        }
        
        private void HurtboxTriggered(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var ball = other.GetComponentInParent<BallInstance>();
            ContainerTracker.Instance.OnContainerHit.CallAction((ball, _containerInstance), _playerIndex);
            ball.ClearBall(false);
        }
    }
}
