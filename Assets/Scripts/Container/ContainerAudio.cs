using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    [RequireComponent(typeof(ContainerInstance))]
    public class ContainerAudio : MonoBehaviour
    {
        [Header("Audio Parameters")]
        [SerializeField] private List<SignalCollider2D> _containerColliders;
        [SerializeField] private float _collisionVelocityMinThreshold;
        [SerializeField] private AK.Wwise.RTPC _rtpc_ballImpactVelocity;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT0;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT1;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT2;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT3;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT4;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT5;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT6;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT7;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT8;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT9;
        [SerializeField] private AK.Wwise.Event _wwiseEventBallContainerCollisionT10;

        private void Start()
        {
            foreach (var colliders in _containerColliders)
            {
                // TODO: Fill the events and fix the collision audio feedback. https://cyberturret.atlassian.net/browse/MS-103
                // colliders.SubscribeCollision2DEnter(OnBallCollision);
            }        
        }
        
        private void OnBallCollision(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Ball"))
                return;
            var ball = col.gameObject.GetComponent<BallInstance>();
            var velocity = col.relativeVelocity.magnitude;
            if (ball && velocity < _collisionVelocityMinThreshold)
                return;
            
            
            _rtpc_ballImpactVelocity.SetValue(gameObject, velocity);
            switch (ball.BallTierIndex)
            {
                case 0:
                    _wwiseEventBallContainerCollisionT0.Post(ball.gameObject);
                    break;
                case 1:
                    _wwiseEventBallContainerCollisionT1.Post(ball.gameObject);
                    break;
                case 2:
                    _wwiseEventBallContainerCollisionT2.Post(ball.gameObject);
                    break;
                case 3:
                    _wwiseEventBallContainerCollisionT3.Post(ball.gameObject);
                    break;
                case 4:
                    _wwiseEventBallContainerCollisionT4.Post(ball.gameObject);
                    break;
                case 5:
                    _wwiseEventBallContainerCollisionT5.Post(ball.gameObject);
                    break;
                case 6:
                    _wwiseEventBallContainerCollisionT6.Post(ball.gameObject);
                    break;
                case 7:
                    _wwiseEventBallContainerCollisionT7.Post(ball.gameObject);
                    break;
                case 8:
                    _wwiseEventBallContainerCollisionT8.Post(ball.gameObject);
                    break;
                case 9:
                    _wwiseEventBallContainerCollisionT9.Post(ball.gameObject);
                    break;
                case 10:
                    _wwiseEventBallContainerCollisionT10.Post(ball.gameObject);
                    break;
            }
        }
    }
}