using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Player;
using MultiSuika.Utilities;
using MultiSuika.zOther;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Cannon
{
    public class CannonInstance : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer spriteRenderer;
        [FormerlySerializedAs("WwiseEventCannonShoot")] [SerializeField] public AK.Wwise.Event WwiseEventCannonShot;

        // PlayerIndex
        private int _playerIndex;

        // Cannon Parameters
        private float _movementSpeed;
        private float _reloadCooldown;
        private float _shootingForce;
        private float _distanceBetweenBallAndCannon;
        private bool _isUsingPeggleMode;
        private PlayerInputHandler _playerInputHandler;

        // Positioning
        private float _horizontalMargin;
        private float _shootingAngle = 0f;
        private Vector2 _shootingDirection = Vector2.down;

        // Ball Parameters
        private BallSetData _ballSetData;
        private BallSpriteThemeData _ballSpriteData;
        private float _currentBallDistanceFromCannon;
        private BallInstance _currentBallInstance;

        public void SetCannonInputEnabled(bool isActive)
        {
            if (_playerInputHandler == null)
                return;
            if (isActive)
            {
                _playerInputHandler.onHorizontalMvtContinuous += MoveCannon;
                _playerInputHandler.onShoot += DropBall;
                if (_currentBallInstance == null)
                    LoadNewBall();
            }
            else
            {
                _playerInputHandler.onHorizontalMvtContinuous -= MoveCannon;
                _playerInputHandler.onShoot -= DropBall;
            }
        }

        public void DisconnectCannonFromPlayer()
        {
            SetCannonInputEnabled(false);
            _playerInputHandler = null;
        }

        private void DropBall()
        {
            if (_currentBallInstance == null)
                return;

            _currentBallInstance.DropBallFromCannon();
            _currentBallInstance.rb2d.AddForce(_shootingDirection.normalized * _shootingForce);
            _currentBallInstance = null;
            Invoke("LoadNewBall", _reloadCooldown);
            WwiseEventCannonShot.Post(gameObject);
        }

        private void MoveCannon(float xAxis)
        {
            if (_isUsingPeggleMode)
            {
                if (xAxis < 0 && _shootingAngle > -Mathf.PI / 2 + 0.1f ||
                    xAxis > 0 && _shootingAngle < Mathf.PI / 2 - 0.1f)
                {
                    _shootingAngle += xAxis * _movementSpeed * Time.deltaTime;
                    _shootingDirection = new Vector2(Mathf.Sin(_shootingAngle), -Mathf.Cos(_shootingAngle));
                }
            }
            else
            {
                if (xAxis < 0 && transform.localPosition.x > -_horizontalMargin ||
                    xAxis > 0 && transform.localPosition.x < _horizontalMargin)
                    transform.Translate(xAxis * Time.deltaTime * _movementSpeed, 0, 0);
            }

            if (_currentBallInstance != null)
                _currentBallInstance.transform.localPosition = (Vector2)transform.localPosition +
                                                               _shootingDirection.normalized *
                                                               _currentBallDistanceFromCannon;
        }

        private void LoadNewBall()
        {
            var newBallIndex = _ballSetData.GetRandomBallTier();
            _currentBallDistanceFromCannon =
                _ballSetData.GetBallData(newBallIndex).Scale / 2f + _distanceBetweenBallAndCannon;

            var containerParentTransform = ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex);

            _currentBallInstance = Instantiate(_ballSetData.BallInstancePrefab, containerParentTransform);
            _currentBallInstance.SetBallPosition((Vector2)transform.localPosition +
                                                 _shootingDirection.normalized * _currentBallDistanceFromCannon);
            _currentBallInstance.SetBallParameters(_playerIndex, newBallIndex, _ballSetData, _ballSpriteData);
            _currentBallInstance.SetSimulatedParameters(false);
        }

        public void DestroyCurrentBall()
        {
            if (_currentBallInstance != null)
                Destroy(_currentBallInstance.gameObject);
        }

        #region Setter

        public void SetCannonParameters(int playerIndex, GameModeData gameModeData)
        {
            _playerIndex = playerIndex;
            
            // Set position
            var tf = transform;
            transform.ResetLocalTransform();

            tf.localPosition = new Vector2(0f, gameModeData.CannonVerticalDistanceFromCenter);
            _horizontalMargin = ContainerTracker.Instance.GetItemsByPlayer(_playerIndex).First()
                .GetContainerHorizontalHalfLength();
            
            if (gameModeData.IsCannonXSpawnPositionRandom)
            {
                var localYPos = transform.localPosition.y;
                transform.localPosition = 
                    new Vector2(Random.Range(-_horizontalMargin, _horizontalMargin), localYPos);
            }
            
            // Set basic cannon parameters
            _movementSpeed = gameModeData.CannonSpeed;
            _reloadCooldown = gameModeData.CannonReloadCooldown;
            _shootingForce = gameModeData.CannonShootingForce;
            _distanceBetweenBallAndCannon = gameModeData.DistanceBetweenBallAndCannon;
            _isUsingPeggleMode = gameModeData.IsCannonUsingPeggleMode;

            _ballSetData = gameModeData.BallSetData;
            _ballSpriteData = gameModeData.SkinData.playersSkinData[_playerIndex].ballTheme;

            // Set sprite
            spriteRenderer.sprite = gameModeData.SkinData.playersSkinData[_playerIndex].cannonSprite;
        }

        public void SetInputParameters(PlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
        }

        #endregion
    }
}