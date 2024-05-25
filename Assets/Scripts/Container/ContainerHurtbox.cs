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
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);

            foreach (var hurtbox in _hurtboxes)
            {
                hurtbox.OnTrigger2DEnter.Subscribe(HurtboxTriggered);
            }
        }
        
        private void HurtboxTriggered(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var ball = other.GetComponentInParent<BallInstance>();
            ContainerTracker.Instance.OnContainerHit.CallAction(ball, _playerIndex);
            ball.ClearBall(false, true);
            ClearBallOnDamage();
        }

        // TODO: It should probably be somewhere else, but for now it gets the job now
        private void ClearBallOnDamage()
        {
            var ballList = BallTracker.Instance.GetItemsFromPlayer(_playerIndex);
            foreach (var ball in ballList.Where(ball => Random.Range(0f, 1f) > 0.5f))
            {
                if (ball.Rb2d.simulated)
                    ball.ClearBall(false, true);
            }
        }
    }
}
