using System;
using System.Linq;
using Codice.Client.Common.WebApi;
using MultiSuika.Audio;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiSuika.Ball
{
    public class BallInstance : MonoBehaviour
    {
        [SerializeField] public WwiseEventsData ballFusionWwiseEvents;
        
        
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] public Rigidbody2D rb2d;

        public int tier;
        public int scoreValue;
        public IntReference ballScoreRef;
        public BallSetData ballSetData;
        public BallSpriteThemeData ballSpriteThemeData;
        public Container.Container container;
        public IGameModeManager gameModeManager;
        public BallTracker ballTracker;

        public float impulseMultiplier;
        public float impulseExpPower;
        public float impulseRangeMultiplier;

        private bool _isBallCleared = false;

        private void Awake()
        {
            GetComponentInChildren<SignalCollider2D>().SubscribeCollisionEnter2D(FusionCheck);
        }

        public float GetBallArea() => Mathf.PI * Mathf.Pow(transform.localScale.x * 0.5f, 2);
        
        public void DropBallFromCannon()
        {
            ballTracker.RegisterBall(this, container);
            rb2d.simulated = true;
            ApplyRotationForce();
        }

        public void SetBallFreeze(bool isFrozen) => rb2d.simulated = !isFrozen; 
        
        public void ClearBall(bool addToScore = true)
        {
            if (addToScore)
                ballScoreRef?.Variable.ApplyChange(scoreValue);
            ballTracker.UnregisterBall(this, container);
            rb2d.simulated = false;
            _isBallCleared = true;
            Destroy(gameObject);
        }

        private void ApplyRotationForce()
        {
            var zRotationValue = Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
            rb2d.AddTorque(zRotationValue, ForceMode2D.Force);
        }
        
        private void FusionCheck(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Ball"))
                return;
            var otherBall = collision.gameObject.GetComponent<BallInstance>();
            if (otherBall.tier == tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID() && !_isBallCleared && !otherBall.IsBallCleared())
            {
                FuseWithOtherBall(otherBall, collision.GetContact(0).point);
            }
        }

        private void FuseWithOtherBall(BallInstance other, Vector3 contactPosition)
        {
            other.ClearBall();
            ClearBall();
            if (tier < ballSetData.GetMaxTier)
            {
                AddFusionImpulse(tier + 1, contactPosition);
                var newBall = Initializer.InstantiateBall(ballSetData, container,
                    Initializer.WorldToLocalPosition(container.ContainerParent.transform, contactPosition));
                
                // TODO: Check if we can better fit that into the initialization encapsulation (we're setting in two different places)
                newBall.transform.SetLayerRecursively(gameObject.layer);
                
                Initializer.SetBallParameters(newBall, tier + 1, ballScoreRef, ballSetData, ballTracker, ballSpriteThemeData, container, gameModeManager);
                newBall.ballTracker.RegisterBall(newBall, container);
                ballFusionWwiseEvents.PostEventAtIndex(tier, newBall.gameObject);
            }
            gameModeManager.OnBallFusion(this);
        }

        private void AddFusionImpulse(int newBallTier, Vector3 contactPosition)
        {
            float impulseRadius = ballSetData.GetBallData(newBallTier).scale / 2f;
            var ballsInRange =
                from raycast in Physics2D.CircleCastAll(contactPosition, impulseRadius * container.ContainerParent.transform.localScale.x * impulseRangeMultiplier, Vector2.zero, Mathf.Infinity,
                    LayerMask.GetMask("Ball"))
                select raycast.collider;
        

            foreach (var ball in ballsInRange)
            {
                Vector2 pushDirection = ball.transform.position - contactPosition;
                pushDirection.Normalize();

                float pushIntensity = Mathf.Pow(Mathf.Abs(impulseRadius * impulseRangeMultiplier - Vector2.Distance(ball.ClosestPoint(contactPosition), contactPosition)) * impulseMultiplier, impulseExpPower);

                ball.GetComponent<Rigidbody2D>().AddForce(pushIntensity * pushDirection, ForceMode2D.Impulse);
            }
        }
        
        private bool IsBallCleared() => _isBallCleared;
    }
}