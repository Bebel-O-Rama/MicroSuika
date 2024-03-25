using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    [RequireComponent(typeof(ContainerInstance))]
    public class ContainerAudio : MonoBehaviour
    {
        private const string Tag = "Ball"; 

        [Header("Audio Parameters")] // Clean this up
        [SerializeField] private List<SignalCollider2D> _containerColliders;
        [SerializeField] private float _collisionVelocityMinThreshold;
        public AK.Wwise.RTPC rtpc_ballImpactVelocity;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT0;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT1;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT2;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT3;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT4;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT5;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT6;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT7;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT8;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT9;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT10;

        private void Start()
        {
            foreach (var colliders in _containerColliders)
            {
                colliders.SubscribeCollision2DEnter(OnBallCollision);
            }        
        }
        
        private void OnBallCollision(Collision2D col)
        {
            if (!col.gameObject.CompareTag(Tag))
                return;
            var ball = col.gameObject.GetComponent<BallInstance>();
            var velocity = col.relativeVelocity.magnitude;
            if (ball && velocity < _collisionVelocityMinThreshold)
                return;
            
            
            rtpc_ballImpactVelocity.SetValue(gameObject, velocity);
            switch (ball.BallTierIndex)
            {
                case 0:
                    WwiseEventBallContainerCollisionT0.Post(ball.gameObject);
                    break;
                case 1:
                    WwiseEventBallContainerCollisionT1.Post(ball.gameObject);
                    break;
                case 2:
                    WwiseEventBallContainerCollisionT2.Post(ball.gameObject);
                    break;
                case 3:
                    WwiseEventBallContainerCollisionT3.Post(ball.gameObject);
                    break;
                case 4:
                    WwiseEventBallContainerCollisionT4.Post(ball.gameObject);
                    break;
                case 5:
                    WwiseEventBallContainerCollisionT5.Post(ball.gameObject);
                    break;
                case 6:
                    WwiseEventBallContainerCollisionT6.Post(ball.gameObject);
                    break;
                case 7:
                    WwiseEventBallContainerCollisionT7.Post(ball.gameObject);
                    break;
                case 8:
                    WwiseEventBallContainerCollisionT8.Post(ball.gameObject);
                    break;
                case 9:
                    WwiseEventBallContainerCollisionT9.Post(ball.gameObject);
                    break;
                case 10:
                    WwiseEventBallContainerCollisionT10.Post(ball.gameObject);
                    break;
            }
        }
    }
}