using System.Linq;
using MultiSuika.Audio;
using MultiSuika.Container;
using MultiSuika.Skin;
using MultiSuika.Utilities;
using UnityEngine;

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
        private BallSkinData _ballSkinData;
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

            var containerParentTransform = ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex);
            var ball = Instantiate(_ballSetData.BallInstancePrefab, containerParentTransform);
            BallTracker.Instance.AddNewItem(ball, _playerIndex);

            ball.SetBallPosition(UnityExtension.WorldToLocalPosition(containerParentTransform, contactPosition));
            ball.SetBallParameters(_playerIndex, BallTierIndex + 1, _ballSetData, _ballSkinData);
            ball.transform.SetLayerRecursively(gameObject.layer);
            ball.SetSimulatedParameters(true);

            BallTracker.Instance.AddNewItem(ball, ball._playerIndex);

            ballFusionWwiseEvents.PostEventAtIndex(BallTierIndex, ball.gameObject);
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

        public void SetBallPosition(Vector3 position, float randomRotationRange = 35f)
        {
            transform.ResetLocalTransform();
            transform.SetLocalPositionAndRotation(position,
                Quaternion.Euler(0f, 0f, Random.Range(-randomRotationRange, randomRotationRange)));
        }

        public void SetBallParameters(int playerIndex, int ballTierIndex, BallSetData ballSetData,
            BallSkinData ballSkinData)
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
            _ballSkinData = ballSkinData;
            spriteRenderer.sprite = _ballSkinData.GetBallSprite(BallTierIndex);

            // Transform
            var tf = transform;
            tf.localScale = Vector3.one * ballData.Scale;
            tf.name = $"Ball T{BallTierIndex} (ID: {transform.GetInstanceID()})";

            // Container
            ContainerInstance = transform.parent.GetComponentInChildren<ContainerInstance>();
            
            // Layers
            transform.SetLayerRecursively(LayerMask.NameToLayer($"Container{_playerIndex+1}"));
        }

        public void SetSimulatedParameters(bool isSimulated) => rb2d.simulated = isSimulated;

        #endregion
    }
}