using MultiSuika.Ball;
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
        private Ball.Ball _currentBall;
        private float _currentBallDistanceFromCannon;

        // Wwise Event
        public AK.Wwise.Event WwiseEventCannonShoot;
        
        public void DestroyCurrentBall()
        {
            if (_currentBall != null)
                Destroy(_currentBall.gameObject);
        }
    
        public void SetCannonControlConnexion(PlayerInputHandler playerInputHandler, bool isActive)
        {
            if (isActive)
            {
                playerInputHandler.onHorizontalMvtContinuous += MoveCannon;
                playerInputHandler.onShoot += DropBall;
                if (_currentBall == null)
                    LoadNewBall();
            }
            else
            {
                playerInputHandler.onHorizontalMvtContinuous -= MoveCannon;
                playerInputHandler.onShoot -= DropBall;
            }
        }
    
        private void DropBall()
        {
            if (_currentBall == null)
                return;
        
            _currentBall.DropBallFromCannon();
            _currentBall.rb2d.AddForce(_shootingDirection.normalized * shootingForce);
            _currentBall = null;
            Invoke("LoadNewBall", reloadCooldown);
            Debug.Log("Drop");
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

            if (_currentBall != null)
                _currentBall.transform.localPosition = (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon;
        }
    
        private void LoadNewBall()
        {
            int newBallIndex = ballSetData.GetRandomBallTier();
            _currentBallDistanceFromCannon = ballSetData.GetBallData(newBallIndex).scale / 2f + emptyDistanceBetweenBallAndCannon;
            _currentBall = Initializer.InstantiateBall(ballSetData, container,
                (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon);
            Initializer.SetBallParameters(_currentBall, newBallIndex, scoreReference, ballSetData, ballTracker, ballSpriteData, container, true);
        }
    
    }
}