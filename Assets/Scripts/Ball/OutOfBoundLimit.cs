using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Ball
{
    public class OutOfBoundLimit : MonoBehaviour
    {
        private void Start()
        {
            var colliderSignals = GetComponentsInChildren<SignalCollider2D>();
            foreach (var signal in colliderSignals)
            {
                signal.OnTrigger2DEnter.Subscribe(ObjectOutOfBound);
            }
        }

        private void ObjectOutOfBound(Collider2D other)
        {
            if (other.CompareTag("Ball"))
                other.GetComponentInParent<BallInstance>().ClearBall(false);
        }
    }
}
