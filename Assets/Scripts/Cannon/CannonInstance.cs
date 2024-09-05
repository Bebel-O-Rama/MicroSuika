using System;
using System.Collections;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Player;
using MultiSuika.Skin;
using MultiSuika.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiSuika.Cannon
{
    public class CannonInstance : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] public AK.Wwise.Event wwiseEventCannonShot;

        // PlayerIndex
        private int _playerIndex;

        // Positioning
        private float _horizontalMargin;
        private float _shootingAngle;
        private Vector2 _shootingDirection = Vector2.down;

        // Cannon Parameters
        private float _movementSpeed;
        private float _reloadCooldown;
        private float _shootingForce;
        private float _distanceBetweenBallAndCannon;
        private bool _isUsingPeggleMode;
        private PlayerInputHandler _playerInputHandler;

        // Ball Parameters
        private BallSetData _ballSetData;
        private BallSkinData _ballSkinData;
        private float _ballHeldDistanceFromCannon;
        private BallInstance _currentBallInstance;

        // Container Parameters
        private ContainerNextBall _containerNextBall;

        private Coroutine _loadBallCoroutine;

        public void Start()
        {
            var container = ContainerTracker.Instance.GetItemFromPlayerOrDefault(_playerIndex);
            _containerNextBall = container.GetComponent<ContainerNextBall>();
        }

        public void SetCannonInputEnabled(bool isActive)
        {
            if (!_playerInputHandler)
                return;
            if (isActive)
            {
                _playerInputHandler.onHorizontalMvtContinuous += MoveCannon;
                _playerInputHandler.onShoot += DropBall;
                if (!_currentBallInstance)
                    LoadBall();
            }
            else
            {
                _playerInputHandler.onHorizontalMvtContinuous -= MoveCannon;
                _playerInputHandler.onShoot -= DropBall;
            }
        }

        public void ClearCannon()
        {
            SetCannonInputEnabled(false);

            if (_currentBallInstance)
                BallTracker.Instance.ClearItem(_currentBallInstance);
        }

        private void DropBall()
        {
            if (!_currentBallInstance)
                return;
            
            _currentBallInstance.DropBallFromCannon();
            _currentBallInstance.Rb2d.AddForce(_shootingDirection.normalized * _shootingForce);
            _currentBallInstance = null;
            wwiseEventCannonShot.Post(gameObject);
            
            _loadBallCoroutine = StartCoroutine(LoadBallWait());
        }

        private IEnumerator LoadBallWait()
        {
            yield return new WaitForSeconds(_reloadCooldown);
            LoadBall();
        }

        // Should refactor that to not be public
        public void OnGameOver(bool hasWon)
        {
            if (hasWon && _loadBallCoroutine != null)
                StopCoroutine(_loadBallCoroutine);
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

            if (_currentBallInstance)
                _currentBallInstance.transform.localPosition = GetBallHeldPosition(_currentBallInstance.BallTierIndex);
        }

        private void LoadBall()
        {
            if (!_containerNextBall)
            {
                SpawnBall();
                return;
            }

            _currentBallInstance = _containerNextBall.GetNextBall();
            _currentBallInstance.SetBallPosition(GetBallHeldPosition(_currentBallInstance.BallTierIndex));
            _currentBallInstance.ResetBallScale();
        }

        private void SpawnBall()
        {
            var ballIndex = _ballSetData.GetRandomBallTier();

            var containerParentTransform = ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex);

            _currentBallInstance = Instantiate(_ballSetData.BallInstancePrefab, containerParentTransform);
            BallTracker.Instance.AddNewItem(_currentBallInstance, _playerIndex);

            _currentBallInstance.SetBallPosition(GetBallHeldPosition(ballIndex));
            _currentBallInstance.SetBallParameters(_playerIndex, ballIndex, _ballSetData, _ballSkinData);
            _currentBallInstance.SetSimulatedParameters(false);
        }

        #region Getter/Setter

        private Vector3 GetBallHeldPosition(int ballIndex) => (Vector2)transform.localPosition +
                                                                  _shootingDirection.normalized *
                                                                  (_distanceBetweenBallAndCannon +
                                                                   _ballSetData.GetBallData(ballIndex).Scale *
                                                                   0.5f);

        public SpriteRenderer GetNextBallSpriteRenderer() => _containerNextBall.GetNextBallSpriteRenderer();
        
        public void SetCannonParameters(int playerIndex, GameModeData gameModeData)
        {
            _playerIndex = playerIndex;

            // Set position and layer
            var tf = transform;
            transform.ResetLocalTransform();
            transform.SetLayerRecursively(LayerMask.NameToLayer($"Container{playerIndex + 1}"));

            tf.localPosition = new Vector2(0f, gameModeData.CannonVerticalDistanceFromCenter);
            _horizontalMargin = ContainerTracker.Instance.GetItemFromPlayerOrDefault(_playerIndex)
                .HorizontalMvtHalfLength;

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
            _ballSkinData = gameModeData.SkinData.GetPlayerSkinData(_playerIndex).BallTheme;

            // Set sprite
            spriteRenderer.sprite = gameModeData.SkinData.GetPlayerSkinData(_playerIndex).CannonSprite;
        }


        public void SetInputParameters(PlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
        }

        #endregion
    }
}