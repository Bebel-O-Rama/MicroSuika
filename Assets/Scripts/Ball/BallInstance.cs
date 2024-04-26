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
        [SerializeField] private Rigidbody2D _rb2d;
        [SerializeField] private SignalCollider2D _signalCollider2D;
        [SerializeField] private BallVisualEffects _ballVisualEffects;

        public int BallTierIndex { get; private set; }
        public int ScoreValue { get; private set; }

        public Rigidbody2D Rb2d
        {
            get => _rb2d;
        }

        private int _playerIndex;
        private BallSetData _ballSetData;
        private BallSkinData _ballSkinData;
        private bool _isBallCleared;

        private void Awake()
        {
            _isBallCleared = false;
            _signalCollider2D.OnCollision2DEnter.Subscribe(FusionCheck);
        }

        public void DropBallFromCannon()
        {
            SetSimulatedParameters(true);

            var zRotationValue = Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
            _rb2d.AddTorque(zRotationValue, ForceMode2D.Force);
        }

        public void ClearBall(bool isFused = true, bool isContainerDmg = false)
        {
            if (isFused)
            {
                BallTracker.Instance.OnBallFusion.CallAction(this, _playerIndex);
                if (_ballVisualEffects)
                    _ballVisualEffects.PlayBallFusedClear();
            }

            if (isContainerDmg)
            {
                if (_ballVisualEffects)
                    _ballVisualEffects.PlayContainerBallDamage();
            }
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
            // TEMPORARY FIX : When two ball fuses, either the combo is incremented twice or the added score is incremented for half the ball
            // It's linked to the other temporary fix in ScoreHandler.cs, OnBallFusionPoints() (the * 2)
            other.ClearBall(false);
            ClearBall();

            _ballVisualEffects.PlayBallFusedContact(contactPosition);
            
            ballFusionWwiseEvents.PostEventAtIndex(BallTierIndex, gameObject);

            if (BallTierIndex >= _ballSetData.GetMaxTier)
                return;

            FusionImpulse(BallTierIndex + 1, contactPosition);
            SpawnBall(contactPosition);
            
        }

        private void FusionImpulse(int newBallTier, Vector3 contactPosition)
        {
            var realImpulseRadius = _ballSetData.GetBallData(newBallTier).Scale * 0.5f *
                                    _ballSetData.ImpulseRangeMultiplier *
                                    ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex).localScale.x;

            Physics2DExtensions.ApplyCircularImpulse(realImpulseRadius, contactPosition, "Ball",
                _ballSetData.ImpulseForcePerUnit,
                _ballSetData.ImpulseExpPower);
        }

        private void SpawnBall(Vector3 contactPosition)
        {
            var containerParentTransform = ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex);
            var ball = Instantiate(_ballSetData.BallInstancePrefab, containerParentTransform);
            BallTracker.Instance.AddNewItem(ball, _playerIndex);

            ball.SetBallPosition(UnityExtension.WorldToLocalPosition(containerParentTransform, contactPosition));
            ball.SetBallParameters(_playerIndex, BallTierIndex + 1, _ballSetData, _ballSkinData);
            ball.transform.SetLayerRecursively(gameObject.layer);
            ball.SetSimulatedParameters(true);
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

            // Layers
            transform.SetLayerRecursively(LayerMask.NameToLayer($"Container{_playerIndex + 1}"));
        }

        public void SetBallScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void ResetBallScale()
        {
            transform.localScale = Vector3.one * _ballSetData.GetBallData(BallTierIndex).Scale;
        }

        public void SetSimulatedParameters(bool isSimulated) => _rb2d.simulated = isSimulated;

        #endregion
    }
}