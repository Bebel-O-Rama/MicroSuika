using MultiSuika.Ball;
using MultiSuika.GameLogic;
using MultiSuika.Player;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Cannon
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer spriteRenderer;
    
        // Cannon Parameters
        public float speed;
        public float reloadCooldown;
        public float shootingForce;
        public float emptyDistanceBetweenBallAndCannon;
        public bool isUsingPeggleMode = false;
        private PlayerInputManager _playerInputManager;
    
        // Positioning
        public float horizontalMargin;
        private float _shootingAngle = 0f;
        private Vector2 _shootingDirection = Vector2.down;

        // Ball Parameters
        public BallSetData ballSetData;
        public BallSpriteThemeData ballSpriteData;
        public IntReference scoreReference;
        public Container.Container container;
        public BallTracker ballTracker;
        private Ball.BallInstance _currentBallInstance;
        private float _currentBallDistanceFromCannon;

        // Wwise Event
        public AK.Wwise.Event WwiseEventCannonShoot;

        public IGameModeManager gameModeManager;
        
        public void DestroyCurrentBall()
        {
            if (_currentBallInstance != null)
                Destroy(_currentBallInstance.gameObject);
        }
    
        public void SetCannonInputConnexion(bool isActive)
        {
            if (_playerInputManager == null)
                return;
            if (isActive)
            {
                _playerInputManager.onHorizontalMvtContinuous += MoveCannon;
                _playerInputManager.onShoot += DropBall;
                if (_currentBallInstance == null)
                    LoadNewBall();
            }
            else
            {
                _playerInputManager.onHorizontalMvtContinuous -= MoveCannon;
                _playerInputManager.onShoot -= DropBall;
            }
        }

        public void ConnectCannonToPlayer(PlayerInputManager playerInputManager)
        {
            _playerInputManager = playerInputManager;
            SetCannonInputConnexion(true);
        }

        public void DisconnectCannonToPlayer()
        {
            SetCannonInputConnexion(false);
            _playerInputManager = null;
        }
        
        private void DropBall()
        {
            if (_currentBallInstance == null)
                return;
        
            _currentBallInstance.DropBallFromCannon();
            _currentBallInstance.rb2d.AddForce(_shootingDirection.normalized * shootingForce);
            _currentBallInstance = null;
            Invoke("LoadNewBall", reloadCooldown);
            WwiseEventCannonShoot.Post(gameObject);
        }
    
        private void MoveCannon(float xAxis)
        {
            if (isUsingPeggleMode)
            {
                if (xAxis < 0 && _shootingAngle > -Mathf.PI / 2 + 0.1f || xAxis > 0 && _shootingAngle < Mathf.PI / 2 - 0.1f)
                {
                    _shootingAngle += xAxis * speed * Time.deltaTime;
                    _shootingDirection = new Vector2(Mathf.Sin(_shootingAngle), -Mathf.Cos(_shootingAngle));
                }
            }
            else
            {
                if (xAxis < 0 && transform.localPosition.x > -horizontalMargin || xAxis > 0 && transform.localPosition.x < horizontalMargin)
                    transform.Translate(xAxis*Time.deltaTime*speed, 0, 0);
            }

            if (_currentBallInstance != null)
                _currentBallInstance.transform.localPosition = (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon;
        }
    
        private void LoadNewBall()
        {
            int newBallIndex = ballSetData.GetRandomBallTier();
            _currentBallDistanceFromCannon = ballSetData.GetBallData(newBallIndex).scale / 2f + emptyDistanceBetweenBallAndCannon;
            _currentBallInstance = Initializer.InstantiateBall(ballSetData, container,
                (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon);
            
            // TODO: Check if we can better fit that into the initialization encapsulation (we're setting in two different places)
            _currentBallInstance.transform.SetLayerRecursively(gameObject.layer);
            
            Initializer.SetBallParameters(_currentBallInstance, newBallIndex, scoreReference, ballSetData, ballTracker, ballSpriteData, container, gameModeManager, true);
        }
    
    }
}