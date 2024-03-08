using System;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerHurtboxCheck : MonoBehaviour
    {
        private ContainerRacingMode _containerRacing;
        
        private void Awake()
        {
            _containerRacing = GetComponentInParent<ContainerRacingMode>();
        }

        private void Start()
        {
            var hurtboxes = GetComponentsInChildren<SignalCollider2D>();
            foreach (var hurtbox in hurtboxes)
            {
                hurtbox.SubscribeTriggerEnter2D(HurtboxTriggered);
            }
        }

        private void HurtboxTriggered(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var ball = other.GetComponentInParent<BallInstance>();
            _containerRacing.DamageReceived(ball);
        }
    }
}