using DG.Tweening;
using MultiSuika.Ball;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerCameraMovements : MonoBehaviour
    {
        [Header("Camera parameters")] 
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _mainVerticalTransform;
        [SerializeField] private Transform _secondaryTransform;
        [SerializeField] private float _yMinHeight;
        [SerializeField] private float _yMaxHeight;

        [Header("Screen Shake parameters (OnContainerHit)")]
        [SerializeField] private float _hitDuration = 0.5f;
        [SerializeField] private Vector3 _hitStrength;
        [SerializeField] private int _hitVibrato = 20;
        [SerializeField] private float _hitRandomness = 90f;
        [SerializeField] private bool _hitFadeOut = true;
        [SerializeField] private ShakeRandomnessMode _hitMode = ShakeRandomnessMode.Full;
        [SerializeField] private bool _isTestingEnabled = false;

        private int _playerIndex;
        private FloatReference _normalizedVerticalPosition;
        private Tweener _tweener;

        private void Start()
        {
            var container = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(container);
            ContainerTracker.Instance.OnContainerHit.Subscribe(ContainerHitShake, _playerIndex);

            _normalizedVerticalPosition = ScoreManager.Instance.GetNormalizedSpeedReference(_playerIndex);
            _camera.cullingMask |= (1 << gameObject.layer);
        }

        private void Update()
        {
            var newYPos = _yMinHeight + _normalizedVerticalPosition * (_yMaxHeight - _yMinHeight);
            _mainVerticalTransform.position = new Vector3(0, -newYPos, 0);
            
            if (_isTestingEnabled && Input.GetKeyDown(KeyCode.B))
                ContainerHitShakeTest();
        }

        private void ContainerHitShake(BallInstance ball)
        {
            if (_tweener.IsActive())
            {
                _tweener.Restart();
                return;
            }
            _tweener = _secondaryTransform.DOShakePosition(_hitDuration, _hitStrength, _hitVibrato, _hitRandomness, fadeOut: _hitFadeOut, randomnessMode: _hitMode);
        }
        
        private void ContainerHitShakeTest()
        {
            if (_tweener.IsActive())
            {
                _tweener.Restart();
                return;
            }
            _tweener = _secondaryTransform.DOShakePosition(_hitDuration, _hitStrength, _hitVibrato, _hitRandomness);
        }
    }
}