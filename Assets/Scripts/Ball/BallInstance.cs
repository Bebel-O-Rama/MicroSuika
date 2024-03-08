using System.Linq;
using MultiSuika.Audio;
using MultiSuika.Container;
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
        public ContainerInstance containerInstance;
        public IGameModeManager gameModeManager;
        public BallTracker ballTracker;

        public float impulseMultiplier;
        public float impulseExpPower;
        public float impulseRangeMultiplier;

        private bool _isBallCleared;

        private void Awake()
        {
            _isBallCleared = false;

            GetComponentInChildren<SignalCollider2D>().SubscribeCollision2DEnter(FusionCheck);
        }

        public float GetBallArea() => Mathf.PI * Mathf.Pow(transform.localScale.x * 0.5f, 2);

        public void DropBallFromCannon()
        {
            ballTracker.RegisterBall(this, containerInstance);
            rb2d.simulated = true;

            var zRotationValue = Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
            rb2d.AddTorque(zRotationValue, ForceMode2D.Force);
        }


        public void ClearBall(bool addToScore = true)
        {
            if (addToScore)
                ballScoreRef?.Variable.ApplyChange(scoreValue);
            ballTracker.UnregisterBall(this, containerInstance);
            rb2d.simulated = false;
            _isBallCleared = true;
            Destroy(gameObject);
        }

        private void FusionCheck(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var otherBall = other.gameObject.GetComponent<BallInstance>();
            if (otherBall.tier == tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID() &&
                !_isBallCleared && !otherBall.IsBallCleared())
            {
                FuseWithOtherBall(otherBall, other.GetContact(0).point);
            }
        }

        private void FuseWithOtherBall(BallInstance other, Vector3 contactPosition)
        {
            other.ClearBall();
            ClearBall();
            if (tier < ballSetData.GetMaxTier)
            {
                FusionImpulse(tier + 1, contactPosition);
                var newBall = Initializer.InstantiateBall(ballSetData, containerInstance,
                    Initializer.WorldToLocalPosition(containerInstance.ContainerParent.transform, contactPosition));

                // TODO: Check if we can better fit that into the initialization encapsulation (we're setting in two different places)
                newBall.transform.SetLayerRecursively(gameObject.layer);

                Initializer.SetBallParameters(newBall, tier + 1, ballScoreRef, ballSetData, ballTracker,
                    ballSpriteThemeData, containerInstance, gameModeManager);
                newBall.ballTracker.RegisterBall(newBall, containerInstance);
                ballFusionWwiseEvents.PostEventAtIndex(tier, newBall.gameObject);
            }

            gameModeManager.OnBallFusion(this);
        }

        private void FusionImpulse(int newBallTier, Vector3 contactPosition)
        {
            float realImpulseRadius = ballSetData.GetBallData(newBallTier).scale * 0.5f * impulseRangeMultiplier *
                                      containerInstance.ContainerParent.transform.localScale.x;

            Physics2DExtensions.ApplyCircularImpulse(realImpulseRadius, contactPosition, "Ball", impulseMultiplier,
                impulseExpPower);
        }

        private bool IsBallCleared() => _isBallCleared;

        #region Setter

        public void SetBallDataParameters(BallData ballData)
        {
        }

        public void SetSimulatedParameters(bool isFrozen) => rb2d.simulated = !isFrozen;

        #endregion
    }
}