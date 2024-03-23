using System.Linq;
using MultiSuika.Audio;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using MultiSuika.zOther;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MultiSuika.Ball
{
    public class BallInstance : MonoBehaviour
    {
        [SerializeField] public WwiseEventsData ballFusionWwiseEvents;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] public Rigidbody2D rb2d;

        public int BallTierIndex { get; private set; }
        public int ScoreValue { get; private set; }
        public ContainerInstance ContainerInstance { get; private set; }

        private int _playerIndex;
        private BallSetData _ballSetData;
        private BallSpriteThemeData _ballSpriteThemeData;
        private Rigidbody2D _rb2d;
        private bool _isBallCleared;

        private void Awake()
        {
            _isBallCleared = false;
            _rb2d = GetComponent<Rigidbody2D>();

            GetComponentInChildren<SignalCollider2D>().SubscribeCollision2DEnter(FusionCheck);
        }

        public void DropBallFromCannon()
        {
            BallTracker.Instance.AddNewItem(this, _playerIndex);
            SetSimulatedParameters(true);

            var zRotationValue = Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
            rb2d.AddTorque(zRotationValue, ForceMode2D.Force);
        }

        public void ClearBall(bool addToScore = true)
        {
            if (addToScore)
                BallTracker.Instance.OnBallFusion.CallAction(this, _playerIndex);
            BallTracker.Instance.ClearItem(this);
            SetSimulatedParameters(false);
            _isBallCleared = true;
            Destroy(gameObject);
        }

        private void FusionCheck(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var otherBall = other.gameObject.GetComponent<BallInstance>();
            if (otherBall.BallTierIndex == BallTierIndex &&
                gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID() &&
                !_isBallCleared && !otherBall._isBallCleared)
            {
                FuseWithOtherBall(otherBall, other.GetContact(0).point);
            }
        }

        private void FuseWithOtherBall(BallInstance other, Vector3 contactPosition)
        {
            other.ClearBall();
            ClearBall();

            if (BallTierIndex >= _ballSetData.GetMaxTier)
                return;

            FusionImpulse(BallTierIndex + 1, contactPosition);
            var newBall = Initializer.InstantiateBall(_ballSetData, ContainerInstance,
                Initializer.WorldToLocalPosition(ContainerInstance.ContainerParent.transform, contactPosition));

            newBall.SetBallParameters(_playerIndex, BallTierIndex + 1, _ballSetData, _ballSpriteThemeData);
            newBall.transform.SetLayerRecursively(gameObject.layer);
            newBall.SetSimulatedParameters(true);

            BallTracker.Instance.AddNewItem(newBall, newBall._playerIndex);

            ballFusionWwiseEvents.PostEventAtIndex(BallTierIndex, newBall.gameObject);
        }

        private void FusionImpulse(int newBallTier, Vector3 contactPosition)
        {
            var realImpulseRadius = _ballSetData.GetBallData(newBallTier).Scale * 0.5f *
                                    _ballSetData.ImpulseRangeMultiplier *
                                    ContainerTracker.Instance.GetItemsByPlayer(_playerIndex).First().ContainerParent
                                        .transform.localScale.x;

            Physics2DExtensions.ApplyCircularImpulse(realImpulseRadius, contactPosition, "Ball",
                _ballSetData.ImpulseForcePerUnit,
                _ballSetData.ImpulseExpPower);
        }

        #region Setter

        public void SetBallParameters(int playerIndex, int ballTierIndex, BallSetData ballSetData,
            BallSpriteThemeData ballSpriteThemeData)
        {
            // PlayerIndex
            _playerIndex = playerIndex;

            // BallData
            BallTierIndex = ballTierIndex;
            _ballSetData = ballSetData;
            var ballData = _ballSetData.GetBallData(ballTierIndex);

            // Score
            ScoreValue = ballData.GetScoreValue();

            // Physics
            _rb2d.mass = ballData.Mass;
            var ballPhysMat = new PhysicsMaterial2D("ballPhysMat")
            {
                bounciness = _ballSetData.Bounciness,
                friction = _ballSetData.Friction
            };
            _rb2d.sharedMaterial = ballPhysMat;
            _rb2d.gravityScale = _ballSetData.GravityScale;

            // Sprite
            _ballSpriteThemeData = ballSpriteThemeData;
            spriteRenderer.sprite = _ballSpriteThemeData.ballSprites[BallTierIndex];

            // Transform
            var tf = transform;
            tf.localScale = Vector3.one * ballData.Scale;
            tf.name = $"Ball T{BallTierIndex} (ID: {transform.GetInstanceID()})";

            // Container
            ContainerInstance = transform.parent.GetComponentInChildren<ContainerInstance>();
        }

        public void SetSimulatedParameters(bool isSimulated) => rb2d.simulated = isSimulated;

        #endregion
    }
}