using System;
using UnityEngine;

namespace MultiSuika.Container
{
    [RequireComponent(typeof(Collider2D))]
    public class ContainerCollisionCheck : MonoBehaviour
    {
        private Container _container;
        private float _velocityThreshold;

        public void SetContainerCollisionCheck(Container container, float velThreshold)
        {
            _container = container;
            _velocityThreshold = velThreshold;
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            var otherObj = col.otherCollider.gameObject;
            if (!otherObj.CompareTag("Ball"))
                return;
            
            var velocity = col.relativeVelocity.magnitude;
            if (velocity > _velocityThreshold)
                _container.OnBallCollision(velocity, otherObj.GetComponent<Ball.Ball>());
        }
    }
}
