using System.Collections.Generic;
using System.Linq;
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
            if (!_containerInstance)
            {
                Destroy(gameObject);
                return;
            }
            _playerIndex = ContainerTracker.Instance.GetPlayersByItem(_containerInstance).First();

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
