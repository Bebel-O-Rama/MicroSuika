using System;
using MultiSuika.Ball;
using UnityEngine;

namespace MultiSuika.Container
{
    [RequireComponent(typeof(Collider2D))]
    public class ContainerCollisionCheck : MonoBehaviour
    {
        private ContainerInstance _containerInstance;
        private float _velocityThreshold;

        public void SetContainerCollisionCheck(ContainerInstance containerInstance, float velThreshold)
        {
            _containerInstance = containerInstance;
            _velocityThreshold = velThreshold;
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Ball"))
                return;

            var velocity = col.relativeVelocity.magnitude;
            if (velocity > _velocityThreshold)
                _containerInstance.OnBallCollision(velocity, col.gameObject.GetComponent<BallInstance>());
        }
    }
}
